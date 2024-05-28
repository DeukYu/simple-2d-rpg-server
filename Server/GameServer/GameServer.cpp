#include "pch.h"
#include <thread>
#include <mutex>
#include <CoreMacro.h>
#include <ThreadManager.h>
#include "memory.h"

struct MyObject {
    int x;
    int y;
    int z;
};

void testCustomNewDelete(size_t iterations) {
    std::vector<MyObject*> allocations;
    allocations.reserve(iterations);

    auto start = std::chrono::high_resolution_clock::now();

    for (size_t i = 0; i < iterations; ++i) {
        allocations.push_back(New<MyObject>());
    }

    for (MyObject* ptr : allocations) {
        Delete(ptr);
    }

    auto end = std::chrono::high_resolution_clock::now();
    std::chrono::duration<double> duration = end - start;
    std::cout << "new/delete duration: " << duration.count() << " seconds" << std::endl;
}

void testNewDelete(size_t iterations) {
    std::vector<MyObject*> allocations;
    allocations.reserve(iterations);

    auto start = std::chrono::high_resolution_clock::now();

    for (size_t i = 0; i < iterations; ++i) {
        allocations.push_back(new MyObject);
    }

    for (MyObject* ptr : allocations) {
        delete ptr;
    }

    auto end = std::chrono::high_resolution_clock::now();
    std::chrono::duration<double> duration = end - start;
    std::cout << "new/delete duration: " << duration.count() << " seconds" << std::endl;
}

int main() {
    size_t iterations = 1000000;
    size_t initialPoolSize = 10000;
    size_t expansionSize = 10000;

    std::cout << "Testing custom new/delete:" << std::endl;
    testCustomNewDelete( iterations);

    std::cout << "Testing new/delete:" << std::endl;
    testNewDelete(iterations);

    return 0;
}