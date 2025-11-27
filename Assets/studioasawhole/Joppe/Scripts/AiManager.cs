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
    [SerializeField] private string prompt;
    public bool forceStop = false;
    public LlamaUtil llamaUtil;
    private Coroutine sentMessage;

    private void Start()
    {
         llamaUtil = new(prompt, "http://" + chatAi.endpoint + "/v1");
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
