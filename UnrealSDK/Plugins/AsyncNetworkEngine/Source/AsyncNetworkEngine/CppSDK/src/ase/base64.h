#pragma once
#include <vector>
#include <string>

std::string base64_encode(const uint8_t* buffer, uint32_t bufferLength);
std::vector<uint8_t> base64_decode(const std::vector<char>& encoded_string);
