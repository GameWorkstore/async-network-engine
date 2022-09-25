#include <gtest/gtest.h>
#include <asyncnetworkengine/asyncnetworkengine.hpp>
#include <fstream>

const std::string aws_remote_file = "https://ase-test-bucket.s3.amazonaws.com/file.txt";
const std::string aws_remote_file_content = "testing_receiver_file_1423as7816847813s48asd8s4a5dad48*71263254%%#snl;@as";

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
    const std::vector<std::string> files = {
        aws_remote_file,
        aws_remote_file,
        aws_remote_file
    };

    AsyncNetworkEngine::Download(files,[](bool result, std::vector<std::vector<char>> files)
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