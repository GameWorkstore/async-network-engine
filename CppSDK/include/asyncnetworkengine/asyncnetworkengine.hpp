#pragma once
#include <string>
#include <functional>
#include <vector>
#include <mutex>

using namespace std;

class AsyncNetworkEngine {
public:
    static void Download(string url, function<void(bool,vector<char>)> callback);
    static void Download(vector<string> urls, function<void(bool,vector<vector<char>>)> callback);
};