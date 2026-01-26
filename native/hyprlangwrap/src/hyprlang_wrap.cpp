#include "hyprlang_wrap.h"

#include <cstdlib>
#include <cstring>
#include <mutex>
#include <string>
#include <vector>

#include <hyprlang.hpp>

using namespace Hyprlang;

struct Handle {
    std::string diagnosticsJson; // JSON array of diagnostics
};

// Hyprlang may not be safe to call concurrently depending on usage.
// Keep it simple.
static std::mutex g_mutex;

static const char* dup_cstr(const std::string& s) {
    char* p = (char*)std::malloc(s.size() + 1);
    if (!p) return nullptr;
    std::memcpy(p, s.data(), s.size());
    p[s.size()] = '\0';
    return p;
}

static std::string json_escape(const std::string& in) {
    std::string out;
    out.reserve(in.size() + 8);
    for (unsigned char c : in) {
        switch (c) {
            case '\\': out += "\\\\"; break;
            case '"':  out += "\\\""; break;
            case '\n': out += "\\n";  break;
            case '\r': out += "\\r";  break;
            case '\t': out += "\\t";  break;
            default:
                if (c < 0x20) {
                    // control chars -> \u00XX
                    char buf[7];
                    std::snprintf(buf, sizeof(buf), "\\u%04x", c);
                    out += buf;
                } else {
                    out += (char)c;
                }
        }
    }
    return out;
}

static std::string make_diag_json_from_parse_result(const CParseResult& r) {
    if (!r.error)
        return "[]";

    const char* msgC = r.getError();
    if (!msgC || !*msgC) msgC = "unknown error";

    return std::string{"[{\"message\":\""} + json_escape(msgC) + "\"}]";
}


extern "C" {

hypr_handle_t hypr_config_parse_text(const char* utf8_text, hypr_result_t* out) {
    if (out) { out->ok = 0; out->error = nullptr; }

    auto* h = new Handle();
    try {
        std::lock_guard<std::mutex> lock(g_mutex);

        Hyprlang::SConfigOptions opts{};
        opts.pathIsStream = true;
        opts.verifyOnly   = true; // we only want validation/diagnostics
        // opts.throwAllErrors = true; // optionally aggregate all errors

        CConfig cfg(utf8_text ? utf8_text : "", opts);

        // IMPORTANT: If you want more than syntax validation, you'd register keys/handlers here.
        // For verifyOnly syntax validation, commence is still required.
        cfg.commence();

        const auto res = cfg.parse();
        h->diagnosticsJson = make_diag_json_from_parse_result(res);

        if (out) {
            out->ok = res.error ? 0 : 1;
            out->error = res.error ? dup_cstr(res.getError() ? res.getError() : "error") : nullptr;

        }
    } catch (const std::exception& e) {
        h->diagnosticsJson = std::string{"[{\"message\":\""} + json_escape(e.what()) + "\"}]";
        if (out) { out->ok = 0; out->error = dup_cstr(e.what()); }
    } catch (...) {
        h->diagnosticsJson = "[{\"message\":\"unknown native error\"}]";
        if (out) { out->ok = 0; out->error = dup_cstr("unknown native error"); }
    }

    return (hypr_handle_t)h;
}

hypr_handle_t hypr_config_parse_file(const char* utf8_path, hypr_result_t* out) {
    if (out) { out->ok = 0; out->error = nullptr; }

    auto* h = new Handle();
    try {
        std::lock_guard<std::mutex> lock(g_mutex);

        Hyprlang::SConfigOptions opts{};
        opts.pathIsStream = false;
        opts.verifyOnly   = true;

        CConfig cfg(utf8_path ? utf8_path : "", opts);
        cfg.commence();

        const auto res = cfg.parse();
        h->diagnosticsJson = make_diag_json_from_parse_result(res);

        if (out) {
            out->ok = res.error ? 0 : 1;
            out->error = res.error ? dup_cstr(res.getError() ? res.getError() : "error") : nullptr;
        }
    } catch (const std::exception& e) {
        h->diagnosticsJson = std::string{"[{\"message\":\""} + json_escape(e.what()) + "\"}]";
        if (out) { out->ok = 0; out->error = dup_cstr(e.what()); }
    } catch (...) {
        h->diagnosticsJson = "[{\"message\":\"unknown native error\"}]";
        if (out) { out->ok = 0; out->error = dup_cstr("unknown native error"); }
    }

    return (hypr_handle_t)h;
}

const char* hypr_config_get_diagnostics_json(hypr_handle_t handle) {
    if (!handle)
        return dup_cstr("[]");

    auto* h = (Handle*)handle;
    return dup_cstr(h->diagnosticsJson.empty() ? "[]" : h->diagnosticsJson);
}

void hypr_config_destroy(hypr_handle_t handle) {
    if (!handle) return;
    delete (Handle*)handle;
}

void hypr_free_string(const char* s) {
    if (!s) return;
    std::free((void*)s);
}

} // extern "C"
