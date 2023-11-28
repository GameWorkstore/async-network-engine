#if !defined(__UNREAL__)
#include <gtest/gtest.h>
#include <ase/asyncnetworkengine.h>
#include <ase/asyncrpc.pb.h>
#include <fstream>

using namespace GameWorkstore::AsyncNetworkEngine;

const std::string aws_remote_file = "https://ase-test-bucket.s3.amazonaws.com/file.txt";
const std::string aws_remote_file_content = "testing_receiver_file_1423as7816847813s48asd8s4a5dad48*71263254%%#snl;@as";

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
        "https://phyengine.com/content/base/logomark.png",
        "https://phyengine.com/content/favicon/android-chrome-192x192.png",
        "https://phyengine.com/content/favicon/android-chrome-512x512.png",
        "https://phyengine.com/content/favicon/apple-touch-icon.png",
        "https://phyengine.com/content/favicon/favicon-16x16.png"
    };

    AsyncNetworkStatic::Download(files,[](bool result, std::vector<std::vector<char>> files)
    {
        ASSERT_TRUE(result);
        ASSERT_EQ(files.size(),5);
        for(auto file : files)
        {
            std::cout << "File Size: " << file.size() << std::endl;
        }
    });
}

//const std::string gcp_notauthorized = "https://us-central1-game-workstore.cloudfunctions.net/gcptest-notauthorized";
//const std::string gcp_success = "https://us-central1-game-workstore.cloudfunctions.net/gcptest";
const std::string phyengine_dev_gettest = "https://phy-dev-api.phyengine.com/phy-dev-gettest";

/*TEST(AseTest, GCP_NotAuthorized)
{
    GenericRequest rqt;
    rqt.set_messege("anything");

    AsyncNetworkEngine<GenericRequest,GenericResponse>::Send(gcp_notauthorized,rqt,[](Transmission result,GenericResponse resp,GenericErrorResponse error)
    {
        ASSERT_EQ(result, Transmission::ErrorParser);
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
}*/

TEST(AseTest, PhyEngine_Success)
{
    GenericRequest rqt;
    rqt.set_messege("message test");

    std::vector<uint8_t> buffer;
    GenericResponse rp;
    rp.set_messege("test");
    buffer.resize(rp.ByteSizeLong());
    rp.SerializeToArray(buffer.data(), rp.ByteSizeLong());

    AsyncNetworkEngine<GenericRequest, GenericResponse>::Send(phyengine_dev_gettest, rqt,
        [](Transmission result, GenericResponse resp, GenericErrorResponse error)
        {
            ASSERT_EQ(result, Transmission::Success);
            std::string messege = resp.messege();
            std::string expected = "test";
            ASSERT_EQ(messege, expected);
        }
    );
}
#endif