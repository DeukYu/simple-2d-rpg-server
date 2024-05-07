#pragma once

inline void Crash(std::string_view message) {
    std::cerr << "Crash: " << message << std::endl;
    std::abort();
}

template <typename Expr>
inline void AssertCrash(Expr expression, std::string_view expr_str, std::string_view file, int line) {
    if (!expression) {
        std::cerr << "Assertion failed: " << expr_str << " in file " << file << ", line " << line << std::endl;
        Crash("ASSERT_CRASH");
    }
}

#define ASSERT_CRASH(expr) AssertCrash((expr), #expr, __FILE__, __LINE__)