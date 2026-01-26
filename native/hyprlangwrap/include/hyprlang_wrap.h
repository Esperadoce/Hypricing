#pragma once
#include <stddef.h>

#ifdef __cplusplus
extern "C" {
#endif

    typedef void* hypr_handle_t;

    typedef struct hypr_result_t {
        int ok;            // 1 success, 0 fail
        const char* error; // nullable; free with hypr_free_string
    } hypr_result_t;

    hypr_handle_t hypr_config_parse_text(const char* utf8_text, hypr_result_t* out);
    hypr_handle_t hypr_config_parse_file(const char* utf8_path, hypr_result_t* out);

    const char* hypr_config_get_diagnostics_json(hypr_handle_t h);

    void hypr_config_destroy(hypr_handle_t h);
    void hypr_free_string(const char* s);

#ifdef __cplusplus
}
#endif
