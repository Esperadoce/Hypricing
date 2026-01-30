#pragma once
#include <stdint.h>

#define HYPRWRAP_API __attribute__((visibility("default")))

#ifdef __cplusplus
extern "C"
{
#endif

    typedef struct parse_result_t
    {
        uint8_t error;
        const char *message;
        uint32_t message_len;
    } parse_result_t;

    typedef void (*parse_cb_t)(const parse_result_t *r, void *user_data);

    HYPRWRAP_API void hypr_config_parse_text(
        const char *utf8_text,
        parse_cb_t cb,
        void *user_data);

#ifdef __cplusplus
}
#endif
