#include <gtest/gtest.h>
#include <asyncnetworkengine/asyncnetworkengine.hpp>
#include <fstream>

using namespace std;

const string aws_remote_file = "https://ase-test-bucket.s3.amazonaws.com/file.txt";
const string aws_remote_file_content = "testing_receiver_file_1423as7816847813s48asd8s4a5dad48*71263254%%#snl;@as";

/*bool ReadFile(const string &filename, vector<char>& data)
{
    ifstream file(filename, ios::ate | ios::binary); //ate makes it opens at end of file

    if (!file.is_open()) return false;

    data.resize(static_cast<size_t>(file.tellg()));

    file.seekg(0);
    file.read(data.data(), data.size());
    file.close();

    return true;
}*/

TEST(AseTest, simpleCall){
    AsyncNetworkEngine::Download(aws_remote_file,[](bool result, vector<char> file)
    {
        ASSERT_TRUE(true);
    });
}

TEST(AseTest, downloadFile){
    AsyncNetworkEngine::Download(aws_remote_file,[](bool result, std::vector<char> file)
    {
        std::string fileContent(file.begin(),file.end());
        ASSERT_TRUE(result);
        ASSERT_GE(file.size(),1);
        ASSERT_EQ(aws_remote_file_content,fileContent);
    });
}

TEST(AseTest, downloadFiles){
    const vector<string> files = {
        aws_remote_file,
        aws_remote_file,
        aws_remote_file
    };

    AsyncNetworkEngine::Download(files,[](bool result, vector<vector<char>> files)
    {
        ASSERT_TRUE(result);
        for(auto file : files)
        {
            std::string fileContent(file.begin(),file.end());
            ASSERT_GE(file.size(),1);
            ASSERT_EQ(aws_remote_file_content,fileContent);
        }
    });
}
