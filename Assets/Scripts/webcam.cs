using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class webcam : MonoBehaviour
{
    // Replace <Subscription Key> with your valid subscription key.              
    const string subscriptionKey = "1ef2a67d893a4e93a8f186e3ef0c7b59";
    // replace <myresourcename> with the string found in your endpoint URL
    //const string uriBase =  "https://<SERVICE_name>.cognitive.microsoft.com/face/v1.0/detect";
    const string uriBase = "https://faceappcloudy.cognitiveservices.azure.com/face/v1.0/detect";

    public static Dictionary<string, float> staticMood = new Dictionary<string, float>(); 

    // Get the path and filename to process from the user.
    async void MakeAnalysisRequest(string imageFilePath)
    {
        HttpClient client = new HttpClient();

        // Request headers.
        client.DefaultRequestHeaders.Add(
            "Ocp-Apim-Subscription-Key", subscriptionKey);

        // Request parameters. A third optional parameter is "details".
        string requestParameters = "returnFaceId=true&returnFaceLandmarks=false" +
            "&returnFaceAttributes=age,gender,headPose,smile,facialHair,glasses," +
            "emotion,hair,makeup,occlusion,accessories,blur,exposure,noise";

        // Assemble the URI for the REST API Call.
        string uri = uriBase + "?" + requestParameters;

        HttpResponseMessage response;

        // Request body. Posts a locally stored JPEG image.
        byte[] byteData = GetImageAsByteArray(imageFilePath);

        using (ByteArrayContent content = new ByteArrayContent(byteData))
        {
            // This example uses content type "application/octet-stream".
            // The other content types you can use are "application/json"
            // and "multipart/form-data".
            content.Headers.ContentType =
                new MediaTypeHeaderValue("application/octet-stream");
            Debug.Log(uri.ToString());
            Debug.Log(content.ToString());
            // Execute the REST API call.
            response = await client.PostAsync(uri, content);

            // Get the JSON response.
            string contentString = await response.Content.ReadAsStringAsync();
            var objects = JArray.Parse(contentString);
            // Display the JSON response.
            print("\nResponse:\n");
            print(contentString);
            staticMood.Clear();
            foreach(JObject obj in objects)
            {
                staticMood.Add("happiness",float.Parse(obj["faceAttributes"]["emotion"]["happiness"].ToString()));
                float val = (obj["faceAttributes"]["gender"].ToString()) == "male" ? 1 : 0;
                float val1 = (obj["faceAttributes"]["glasses"].ToString()) == "ReadingGlasses" ? 1 : 0;
                staticMood.Add("gender", val);
                staticMood.Add("age", float.Parse(obj["faceAttributes"]["age"].ToString()));
                staticMood.Add("glasses", val1);
                staticMood.Add("sadness", float.Parse(obj["faceAttributes"]["emotion"]["sadness"].ToString()));
                staticMood.Add("surprise", float.Parse(obj["faceAttributes"]["emotion"]["surprise"].ToString()));
            }
            foreach(var item in staticMood)
            {
                Debug.Log(item.Key + " " + item.Value);
            }

        }
    }
    static string JsonPrettyPrint(string json)
    {
        if (string.IsNullOrEmpty(json))
            return string.Empty;

        json = json.Replace(Environment.NewLine, "").Replace("\t", "");

        StringBuilder sb = new StringBuilder();
        bool quote = false;
        bool ignore = false;
        int offset = 0;
        int indentLength = 3;

        foreach (char ch in json)
        {
            switch (ch)
            {
                case '"':
                    if (!ignore) quote = !quote;
                    break;
                case '\'':
                    if (quote) ignore = !ignore;
                    break;
            }

            if (quote)
                sb.Append(ch);
            else
            {
                switch (ch)
                {
                    case '{':
                    case '[':
                        sb.Append(ch);
                        sb.Append(Environment.NewLine);
                        sb.Append(new string(' ', ++offset * indentLength));
                        break;
                    case '}':
                    case ']':
                        sb.Append(Environment.NewLine);
                        sb.Append(new string(' ', --offset * indentLength));
                        sb.Append(ch);
                        break;
                    case ',':
                        sb.Append(ch);
                        sb.Append(Environment.NewLine);
                        sb.Append(new string(' ', offset * indentLength));
                        break;
                    case ':':
                        sb.Append(ch);
                        sb.Append(' ');
                        break;
                    default:
                        if (ch != ' ') sb.Append(ch);
                        break;
                }
            }
        }

        return sb.ToString().Trim();
    }
        // Returns the contents of the specified file as a byte array.
        static byte[] GetImageAsByteArray(string imageFilePath)
    {
        using (FileStream fileStream =
            new FileStream(imageFilePath, FileMode.Open, FileAccess.Read))
        {
            BinaryReader binaryReader = new BinaryReader(fileStream);
            return binaryReader.ReadBytes((int)fileStream.Length);
        }
    }

    public string deviceName;
    static WebCamTexture wct;
    public RawImage RawImage;
    public Color32Array colorArray;
    Color32[] data;

    [StructLayout(LayoutKind.Explicit)]
    public struct Color32Array
    {
        [FieldOffset(0)]
        public byte[] byteArray;

        [FieldOffset(0)]
        public Color32[] colors;
    }

    private string _SavePath = "C:/WebcamSnaps/";
    int _CaptureCounter = 0;
    String TakeSnapshot()
    {
        Texture2D snap = new Texture2D(wct.width, wct.height);
        snap.SetPixels(wct.GetPixels());
        snap.Apply();
        String path = _SavePath + _CaptureCounter.ToString() + ".png";
        System.IO.File.WriteAllBytes(_SavePath + _CaptureCounter.ToString() + ".png", snap.EncodeToPNG());
        ++_CaptureCounter;
        return path;
    }

    // Use this for initialization
    void Start()
    {
        WebCamDevice[] devices = WebCamTexture.devices;
        deviceName = devices[0].name;
        wct = new WebCamTexture(deviceName, 640, 480, 60);
        colorArray = new Color32Array();
        colorArray.colors = new Color32[wct.width * wct.height];
        data = new Color32[wct.width * wct.height];
        wct = new WebCamTexture();
        //GetComponent<Renderer>().material.mainTexture = wct;

        //RawImage.material.mainTexture = wct;
        wct.Play();
    }

    void Update()
    {
        if (GameManager.snap)
        {
            String path = TakeSnapshot();
            print(path);

            if (File.Exists(path))
            {
                try
                {
                    MakeAnalysisRequest(path);
                    print("\nWait a moment for the results to appear.\n");
                }
                catch (Exception e)
                {
                    print("\n" + e.Message + "\nPress Enter to exit...\n");
                }
            }
            else
            {
                print("\nInvalid file path.\nPress Enter to exit...\n");
            }
            GameManager.snap = false;
        }
    }
    public byte[] getImageBytes()
    {
        return colorArray.byteArray;
    }
}
