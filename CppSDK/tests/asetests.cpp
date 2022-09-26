#include <gtest/gtest.h>
#include <ase/asyncnetworkengine.hpp>
#include <ase/asyncrpc.pb.h>
#include <fstream>

using namespace GameWorkstore::AsyncNetworkEngine;

const std::string aws_remote_file = "https://ase-test-bucket.s3.amazonaws.com/file.txt";
const std::string aws_remote_file_content = "testing_receiver_file_1423as7816847813s48asd8s4a5dad48*71263254%%#snl;@as";
const std::string gcp_notauthorized = "https://us-central1-game-workstore.cloudfunctions.net/gcptest-notauthorized";
const std::string gcp_success = "https://us-central1-game-workstore.cloudfunctions.net/gcptest";

TEST(AseTest, downloadFile)
{
    AsyncNetworkStatic::Download(aws_remote_file,[](bool result, std::vector<char> file)
    {
        std::string fileContent(file.begin(),file.end());
        ASSERT_TRUE(result);
        ASSERT_GE(file.size(),1);
        ASSERT_EQ(aws_remote_file_content,fileContent);
    });
}

TEST(AseTest, downloadFiles)
{
    const std::vector<std::string> files = {
        aws_remote_file,
        aws_remote_file,
        aws_remote_file
    };

    AsyncNetworkStatic::Download(files,[](bool result, std::vector<std::vector<char>> files)
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

TEST(AseTest, GCP_Success)
{
    GenericRequest rqt;
    rqt.set_messege("success");

    AsyncNetworkEngine<GenericRequest,GenericResponse>::Send(gcp_success,rqt,[](Transmission result,GenericResponse resp,GenericErrorResponse error)
    {
        ASSERT_EQ(result, Transmission::Success);
        std::string messege = resp.messege();
        std::string expected = "received-success";
        ASSERT_EQ(messege, expected);
    });
}