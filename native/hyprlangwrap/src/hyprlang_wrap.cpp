#include <cstring>
#include <cstdio>
#include <string>
#include <hyprlang.hpp>
#include "hyprlang_wrap.h"

using namespace Hyprlang;

static std::string json_escape(const std::string &in)
{
    std::string out;
    out.reserve(in.size() + 8);
    for (unsigned char c : in)
    {
        switch (c)
        {
        case '\\':
            out += "\\\\";
            break;
        case '"':
            out += "\\\"";
            break;
        case '\n':
            out += "\\n";
            break;
        case '\r':
            out += "\\r";
            break;
        case '\t':
            out += "\\t";
            break;
        default:
            if (c < 0x20)
            {
                char buf[7];
                std::snprintf(buf, sizeof(buf), "\\u%04x", c);
                out += buf;
            }
            else
            {
                out += (char)c;
            }
        }
    }
    return out;
}

extern "C" void hypr_config_parse_text(
    const char *utf8_text,
    parse_cb_t cb,
    void *user_data)
{
    if (!cb)
        return;

    parse_result_t r{};
    r.error = 0;
    r.message = nullptr;
    r.message_len = 0;

    try
    {
        Hyprlang::SConfigOptions opts{};
        opts.pathIsStream = true;
        opts.verifyOnly = true;

        CConfig cfg(utf8_text ? utf8_text : "", opts);
        cfg.commence();

        const auto res = cfg.parse();
        if (!res.error)
        {
            // Pass user_data back in success case too
            cb(&r, user_data);
            return;
        }

        r.error = 1;
        res.getError();
        const auto json = std::string{"[{\"message\":\""} + json_escape(res.getError()) + "\"}]";
        r.message = json.c_str();
        r.message_len = static_cast<uint32_t>(json.size());
        cb(&r, user_data);
        return;
    }
    catch (const std::exception &e)
    {
        r.error = 1;
        const auto json = std::string{"[{\"message\":\""} + json_escape(e.what()) + "\"}]";
        r.message = json.c_str();
        r.message_len = static_cast<uint32_t>(json.size());
        cb(&r, user_data);
        return;
    }
    catch (...)
    {
        r.error = 1;
        const char *msg = "[{\"message\":\"unknown native error\"}]";
        r.message = msg;
        r.message_len = static_cast<uint32_t>(std::strlen(msg));
        cb(&r, user_data);
        return;
    }
}