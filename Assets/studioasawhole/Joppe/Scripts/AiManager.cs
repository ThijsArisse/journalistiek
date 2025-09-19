using Neocortex.Samples;
using Piper.Samples;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiManager : MonoBehaviour
{
    [SerializeField] ChatSample chatAi;
    [SerializeField] PiperSample piperAi;

    private string playerTextInput;

    public void GetRecordedMessage(string text)
    {
        Debug.Log(1);
        chatAi.Respond(text);
    }

    public void GetGeneratedMessage(string text)
    {
        Debug.Log(5);
        piperAi.SayMessage(text);
    }
}
