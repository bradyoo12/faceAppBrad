using System;
using System.IO;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.CognitiveServices.Vision.Face;
using Microsoft.Azure.CognitiveServices.Vision.Face.Models;

namespace Company.Function
{
    public static class BlobTriggerFaceApp
    {
        [FunctionName("BlobTriggerFaceApp")]
        public static void Run([BlobTrigger("samples-workitems/{name}", Connection = "storageaccountmyfacb275_STORAGE")]Stream myBlob, 
            string name, ILogger log)
        {
            log.LogInformation($"C# Blob trigger function Processed blob\n Name:{name} \n Size: {myBlob.Length} Bytes!!!!");
            var client = Authenticate("https://myfaceapibrad.cognitiveservices.azure.com/", "06d050b2574d4ece9c7c36f609b02b97");
            var url = //"https://csdx.blob.core.windows.net/resources/Face/Images/detection6.jpg";
                        "https://storageaccountmyfacb275.blob.core.windows.net/samples-workitems/" + name;
            log.LogInformation("url: " + url);
            DetectFaceExtract(client, url, RecognitionModel.Recognition01, log, myBlob).Wait();
        }

        public static IFaceClient Authenticate(string endpoint, string key)
        {
            string SUBSCRIPTION_KEY = key;//Environment.GetEnvironmentVariable(key);
            string ENDPOINT = endpoint;//Environment.GetEnvironmentVariable(endpoint);
            return new FaceClient(new ApiKeyServiceClientCredentials(SUBSCRIPTION_KEY)) { Endpoint = ENDPOINT };
        }

        public static async Task DetectFaceExtract(IFaceClient client, string url, string recognitionModel, ILogger log, Stream myBlob)
        {
            log.LogInformation("========DETECT FACES========");

            // Create a list of images
            // List<string> imageFileNames = new List<string>
            //                 {
            //                     "detection1.jpg",    // single female with glasses
            //                     // "detection2.jpg", // (optional: single man)
            //                     // "detection3.jpg", // (optional: single male construction worker)
            //                     // "detection4.jpg", // (optional: 3 people at cafe, 1 is blurred)
            //                     "detection5.jpg",    // family, woman child man
            //                     "detection6.jpg"     // elderly couple, male female
            //                 };

            // foreach (var imageFileName in imageFileNames)
            // {
                IList<DetectedFace> detectedFaces;

                // Detect faces with all attributes from image url.
                detectedFaces = await client.Face.DetectWithStreamAsync(myBlob,
                        returnFaceAttributes: new List<FaceAttributeType> { FaceAttributeType.Accessories, FaceAttributeType.Age,
                        FaceAttributeType.Blur, FaceAttributeType.Emotion, FaceAttributeType.Exposure, FaceAttributeType.FacialHair,
                        FaceAttributeType.Gender, FaceAttributeType.Glasses, FaceAttributeType.Hair, FaceAttributeType.HeadPose,
                        FaceAttributeType.Makeup, FaceAttributeType.Noise, FaceAttributeType.Occlusion, FaceAttributeType.Smile },
                        recognitionModel: recognitionModel);

                log.LogInformation($"{detectedFaces.Count} face(s) detected from image `{url}`.");
            // }
        }
    }
}
