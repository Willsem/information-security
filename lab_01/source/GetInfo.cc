#include "GetInfo.h"

std::string GetInfo::serial()
{
    char buffer[128];
    std::string result = "";

    FILE* pipe = popen("system_profiler SPHardwareDataType | awk '/Serial/ {print $4}'", "r");
    if (!pipe) {
        return "popen failed!";
    }

    while (!feof(pipe)) {
        if (fgets(buffer, 128, pipe) != NULL)
            result += buffer;
    }

    pclose(pipe);
    return result;
}
