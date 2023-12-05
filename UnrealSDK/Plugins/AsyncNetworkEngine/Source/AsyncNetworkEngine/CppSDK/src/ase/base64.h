#pragma once
#include <vector>
#include <string>
#if defined(__UNREAL__)
#include "CoreMinimal.h"
#endif

std::string base64_encode(const uint8_t* buffer, uint32_t bufferLength);

#if defined(__UNREAL__)
const TArray<uint8_t> base64_decode(const TArray<uint8_t>& encoded_string);
#else
std::vector<uint8_t> base64_decode(const std::vector<char>& encoded_string);
#endif
