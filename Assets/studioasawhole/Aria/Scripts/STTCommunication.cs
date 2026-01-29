using UnityEngine;
using System;
using OpenAI.Audio;
using System.Collections;
using System.IO;
using UnityEngine.Networking;
using System.Collections.Generic;
using UnityEngine.Events;

public class STTCommunication : MonoBehaviour
{
    public AudioClient client;
    public AudioSource audioSource;

    public UnityEvent StartSST;
    public UnityEvent BusySST;
    public UnityEvent StopSST;

    private bool SSTIsBusy = false;
    private bool micOn = false;

    private void Start()
    {
        // StartCoroutine(Transcribe());
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !SSTIsBusy)
        {
            micOn = true;
            StartCoroutine(Transcribe());
        }
        if (Input.GetKeyUp(KeyCode.Space))
        {
            micOn = false;
        }
    }

    IEnumerator Transcribe()
    {
        SSTIsBusy = true;
        StartSST?.Invoke();

        audioSource.clip = Microphone.Start(null, false, 30, 44100);
        while (micOn)
        {
            yield return null;
        }
        BusySST?.Invoke();
        Microphone.End(null);
        audioSource.Play();

        string filePath = Path.Combine(Application.temporaryCachePath, "test.wav");
        using (var fs_write = new FileStream(filePath, FileMode.Create, FileAccess.Write))
        {
            WriteHeader(fs_write, audioSource.clip);
            ConvertAndWrite(fs_write, audioSource.clip);
        }
        using var fs_read = new FileStream(filePath, FileMode.Open, FileAccess.Read);
        byte[] audio = new byte[fs_read.Length];
        fs_read.Read(audio);

        var formData = new List<IMultipartFormSection>
        {
            new MultipartFormFileSection("file", audio, "test.wav", "application/octet-stream"),
            new MultipartFormDataSection("model", "Systran/faster-whisper-base"),
            new MultipartFormDataSection("temperature", "0.4"),
            new MultipartFormDataSection("response_format", "text"),
            new MultipartFormDataSection("language", "nl"),
        };
        using (var webRequest = UnityWebRequest.Post("http://172.29.250.23:8081/v1/audio/transcriptions", formData))
        {
            webRequest.downloadHandler = new DownloadHandlerBuffer();

            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                print("success");
                print(webRequest.downloadHandler.text);
            }
            else
            {
                print("failure: " + webRequest.error);
                print(webRequest.downloadHandler.text);
            }
        }
        yield return null;
        SSTIsBusy = false;
        StopSST?.Invoke();
    }
    

    static void ConvertAndWrite(FileStream fileStream, AudioClip clip)
    {
        var samples = new float[clip.samples];

        clip.GetData(samples, 0);

        Int16[] intData = new Int16[samples.Length];
        //converting in 2 float[] steps to Int16[], //then Int16[] to Byte[]

        Byte[] bytesData = new Byte[samples.Length * 2];
        //bytesData array is twice the size of
        //dataSource array because a float converted in Int16 is 2 bytes.

        int rescaleFactor = 32767; //to convert float to Int16

        for (int i = 0; i < samples.Length; i++)
        {
            intData[i] = (short)(samples[i] * rescaleFactor);
            Byte[] byteArr = new Byte[2];
            byteArr = BitConverter.GetBytes(intData[i]);
            byteArr.CopyTo(bytesData, i * 2);
        }

        fileStream.Write(bytesData, 0, bytesData.Length);
    }

    static void WriteHeader(FileStream fileStream, AudioClip clip)
    {
        var hz = clip.frequency;
        var channels = clip.channels;
        var samples = clip.samples;

        fileStream.Seek(0, SeekOrigin.Begin);

        Byte[] riff = System.Text.Encoding.UTF8.GetBytes("RIFF");
        fileStream.Write(riff, 0, 4);

        Byte[] chunkSize = BitConverter.GetBytes(fileStream.Length - 8);
        fileStream.Write(chunkSize, 0, 4);

        Byte[] wave = System.Text.Encoding.UTF8.GetBytes("WAVE");
        fileStream.Write(wave, 0, 4);

        Byte[] fmt = System.Text.Encoding.UTF8.GetBytes("fmt ");
        fileStream.Write(fmt, 0, 4);

        Byte[] subChunk1 = BitConverter.GetBytes(16);
        fileStream.Write(subChunk1, 0, 4);

        UInt16 two = 2;
        UInt16 one = 1;

        Byte[] audioFormat = BitConverter.GetBytes(one);
        fileStream.Write(audioFormat, 0, 2);

        Byte[] numChannels = BitConverter.GetBytes(channels);
        fileStream.Write(numChannels, 0, 2);

        Byte[] sampleRate = BitConverter.GetBytes(hz);
        fileStream.Write(sampleRate, 0, 4);

        Byte[] byteRate = BitConverter.GetBytes(hz * channels * 2); // sampleRate * bytesPerSample*number of channels, here 44100*2*2
        fileStream.Write(byteRate, 0, 4);

        UInt16 blockAlign = (ushort)(channels * 2);
        fileStream.Write(BitConverter.GetBytes(blockAlign), 0, 2);

        UInt16 bps = 16;
        Byte[] bitsPerSample = BitConverter.GetBytes(bps);
        fileStream.Write(bitsPerSample, 0, 2);

        Byte[] datastring = System.Text.Encoding.UTF8.GetBytes("data");
        fileStream.Write(datastring, 0, 4);

        Byte[] subChunk2 = BitConverter.GetBytes(samples * channels * 2);
        fileStream.Write(subChunk2, 0, 4);

        //		fileStream.Close();
    }
}
