using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Google.Cloud.Speech.V1;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Storage;
public class voice : MonoBehaviour
{
    public GUIText TextBox;
    public static bool isRecording = true;
    struct ClipData
    {
        public int samples;
    }

    const int HEADER_SIZE = 44;

    public GameManager gm;
    private int minFreq;
    private int maxFreq;
    public static string Response = string.Empty;

    private bool micConnected = false;

    private AudioSource goAudioSource;

    public string apiKey = "AIzaSyDLu6MPt50AglztMrDrY65Bq-X2kpKhES4";
    // Start is called before the first frame update
    void Start()
    {
        if (Microphone.devices.Length <= 0)
        {
            Debug.LogWarning("Microphone not connected!");
        }
        else
        {
            micConnected = true;

            Microphone.GetDeviceCaps(null, out minFreq, out maxFreq);
            if (minFreq == 0 && maxFreq == 0)
            {
                maxFreq = 44100;
            }
            goAudioSource = this.GetComponent<AudioSource>();
        }
    }
    void OnGUI()
    {
        if (micConnected)
        {
            isRecording = true;
            if (!Microphone.IsRecording(null))
            {
                if (GUI.Button(new Rect(Screen.width / 2 - 100, (Screen.height / 10)*9, 200, 50), "Record"))
                {
                    goAudioSource.clip = Microphone.Start(null, true, 7, maxFreq); //Currently set for a 7 second clip
                    GameManager.snap = true;
                }
            }
            else //Recording is in progress
            {
                if (GUI.Button(new Rect(Screen.width / 2 - 100, (Screen.height / 10) * 9, 200, 50), "Stop and Play!"))
                {
                    float filenameRand = UnityEngine.Random.Range(0.0f, 10.0f);

                    string filename = "testing" + filenameRand;

                    Microphone.End(null); //Stop the audio recording

                    Debug.Log("Recording Stopped");

                    if (!filename.ToLower().EndsWith(".wav"))
                    {
                        filename += ".wav";
                    }

                    var filePath = Path.Combine("testing/", filename);
                    filePath = Path.Combine(Application.persistentDataPath, filePath);
                    Debug.Log("Created filepath string: " + filePath);
                    Directory.CreateDirectory(Path.GetDirectoryName(filePath));
                    SavWav.Save(filePath, goAudioSource.clip); //Save a temporary Wav File
                    Debug.Log("Saving @ " + filePath);
                    //Insert your API KEY here.
                    string apiURL = "https://speech.googleapis.com/v1/speech:recognize?&key=AIzaSyDyabB9YsvTGdmkYfshAmNDwlOcmQaVUwQ";
                    //Response = 9.ToString();
                    
                    Debug.Log("Uploading " + filePath);
                    Response = HttpUploadFile(apiURL, filePath, "file", "audio/wav; rate=44100");
                    Debug.Log("Response String: " + Response);
                    GameManager.res = Response;
                    var jsonresponse = SimpleJSON.JSON.Parse(Response);

                    if (jsonresponse != null)
                    {
                        string resultString = jsonresponse["result"][0].ToString();
                        var jsonResults = SimpleJSON.JSON.Parse(resultString);

                        string transcripts = jsonResults["alternative"][0]["transcript"].ToString();

                        Debug.Log("transcript string: " + transcripts);
                        TextBox.text = transcripts;

                    }
                    //goAudioSource.Play(); //Playback the recorded audio

                    File.Delete(filePath); //Delete the Temporary Wav file
                    isRecording = false;
                }

                GUI.Label(new Rect(Screen.width / 2 - 100, Screen.height / 2 + 25, 200, 50), "Recording in progress...");
            }
        }
        else // No microphone
        {
            //Print a red "Microphone not connected!" message at the center of the screen
            GUI.contentColor = Color.red;
            GUI.Label(new Rect(Screen.width / 2 - 100, Screen.height / 2 - 25, 200, 50), "Microphone not connected!");
        }
    }

    public string HttpUploadFile(string url, string file, string paramName, string contentType)
    {
        var speech = SpeechClient.Create();
        var response = speech.Recognize(new RecognitionConfig()
        {
            Encoding = RecognitionConfig.Types.AudioEncoding.Linear16,
            SampleRateHertz = 48000,
            LanguageCode = "en",
        }, RecognitionAudio.FromFile(file));
        foreach (var result in response.Results)
        {
            foreach (var alternative in result.Alternatives)
            {
                return (alternative.Transcript);
            }
        }
        return "nothing";
    }
        void Update()
    {
        
    }
}
