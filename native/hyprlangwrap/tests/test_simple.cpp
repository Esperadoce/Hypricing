#include "hyprlang_wrap.h"
#include <iostream>
#include <cstring>
#include <fstream>
// Helper function to read file into string
std::string read_file(const std::string &filename)
{
    std::ifstream file(filename);
    if (!file.is_open())
    {
        throw std::runtime_error("Cannot open file: " + filename);
    }

    std::string content((std::istreambuf_iterator<char>(file)),
                        std::istreambuf_iterator<char>());
    return content;
}

// Simple callback for tests
void test_callback(const parse_result_t *result, void *user_data)
{
    bool *has_error = static_cast<bool *>(user_data);
    *has_error = result->error;

    if (result->error && result->message)
    {
        std::cout << "  Error message: "
                  << std::string(result->message, result->message_len)
                  << std::endl;
    }
}

// Test 1: Valid config
bool test_valid_config()
{
    std::cout << "Test 1: Valid config... ";

    bool has_error = false;
    std::string config = read_file("tests/config.conf");

    hypr_config_parse_text(config.c_str(), test_callback, &has_error);

    if (!has_error)
    {
        std::cout << "PASS" << std::endl;
        return true;
    }
    else
    {
        std::cout << "FAIL" << std::endl;
        return false;
    }
}

// Test 2: Invalid config
bool test_invalid_config()
{
    std::cout << "Test 2: Invalid config... ";

    bool has_error = false;

    std::string config = read_file("tests/multiline-errors.conf");

    hypr_config_parse_text(config.c_str(), test_callback, &has_error);

    if (has_error)
    {
        std::cout << "PASS (correctly detected error)" << std::endl;
        return true;
    }
    else
    {
        std::cout << "FAIL (should have error)" << std::endl;
        return false;
    }
}

int main()
{
    std::cout << "=== Simple HyprlangWrap Tests ===\n"
              << std::endl;

    int passed = 0;

    if (test_valid_config())
        passed++;
    if (test_invalid_config())
        passed++;

    std::cout << "\n=== Summary ===" << std::endl;
    std::cout << "Passed: " << passed << "/2" << std::endl;

    return (passed == 2) ? 0 : 1;
}