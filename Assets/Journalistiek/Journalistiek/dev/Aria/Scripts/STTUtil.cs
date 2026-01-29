using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using Whisper.Utils;

public class STTUtil
{
    public async Task<string> TranscribeFromData(AudioChunk recordedData)
    {
        string filePath = Path.Combine(Application.temporaryCachePath, "test.wav");
        using (var fs_write = new FileStream(filePath, FileMode.Create, FileAccess.Write))
        {
            WriteHeader(fs_write, recordedData.Frequency, recordedData.Channels, recordedData.Data.Length);
            ConvertAndWrite(fs_write, recordedData.Data);
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
        // this string shit is stupid but like rn just dont care its over
        using var webRequest = UnityWebRequest.Post($"http://{AiManager.Endpoint[..^5]}:8081/v1/audio/transcriptions", formData);
        webRequest.downloadHandler = new DownloadHandlerBuffer();

        webRequest.SendWebRequest();
        // await Task.Run(()=>webRequest.SendWebRequest());

        while (true)
        {
            if (webRequest.isDone)
            {
                break;
            }
            else
            {
                await Task.Yield();
            }
        }

        if (webRequest.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("success");
            Debug.Log(webRequest.downloadHandler.text);
        }
        else
        {
            Debug.Log("failure: " + webRequest.error);
            Debug.Log(webRequest.downloadHandler.text);
        }
        
        return webRequest.downloadHandler.text;
    }

    void ConvertAndWrite(FileStream fileStream, float[] samples)
    {
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

    void WriteHeader(FileStream fileStream, int hz, int channels, int samples)
    {
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
