#pragma once
#include <string>
#include <functional>
#include <vector>
#include <mutex>

class AsyncNetworkEngine {
public:
    static void Download(std::string url, std::function<void(bool,std::vector<char>)> callback);
    static void Download(std::vector<std::string> urls, std::function<void(bool,std::vector<std::vector<char>>)> callback);
};