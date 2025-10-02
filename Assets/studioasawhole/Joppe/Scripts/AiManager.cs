using Neocortex.Samples;
using OpenAI.Chat;
using Piper.Samples;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiManager : MonoBehaviour
{
    [SerializeField] LLMCommunication chatAi;
    [SerializeField] PiperSample piperAi;
    public bool forceStop = false;

    public void GetRecordedMessage(string text)
    {
        if (forceStop)
        {
            forceStop = false;
            Debug.Log(1);
            return;
        }

        ChatMessageContentPart response = chatAi.MessageChatBot(text);
        GetGeneratedMessage(response.Text);
    }

    public void GetGeneratedMessage(string text)
    {
        if (forceStop)
        {
            forceStop = false;
            Debug.Log(1);
            return;
        }

        piperAi.SayMessage(text);
    }
}
