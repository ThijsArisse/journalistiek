using Neocortex.Samples;
using OpenAI.Chat;
using Piper.Samples;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiManager : MonoBehaviour
{
    static public string Endpoint;

    [SerializeField] string endpoint = "127.0.0.1:8080";
    [SerializeField] PiperManager piperAi;
    [SerializeField] [TextArea] public string prompt;
    public bool forceStop = false;
    public LlamaUtil llamaUtil;
    private Coroutine sentMessage;

    private void Start()
    {
        Endpoint = endpoint;
        llamaUtil = new(prompt, "http://" + endpoint + "/v1");
    }

    public void GetRecordedMessage(string text)
    {
        if (forceStop)
        {
            forceStop = false;

            if (sentMessage != null)
            {
                StopCoroutine(sentMessage);
            }

            return;
        }

        sentMessage = StartCoroutine(SentMessage(text));
    }

    private IEnumerator SentMessage(string text)
    {
        var task = llamaUtil.MessageAsync(text);
        // wait until the task is done 
        yield return new WaitUntil(() => task.IsCompleted);
        // now do whatever with the result
        GetGeneratedMessage(task.Result.Text);
    }

    public void GetGeneratedMessage(string text)
    {
        if (forceStop)
        {
            forceStop = false;

            if (sentMessage != null)
            {
                StopCoroutine(sentMessage);
            }

            return;
        }
        piperAi.SayMessage(text);
    }
}
