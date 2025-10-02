using System.ClientModel;
using System.ClientModel.Primitives;
using System.Collections;
using System.Collections.Generic;
using OpenAI;
using OpenAI.Chat;
using UnityEngine;

public class LLMCommunication : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        var client = new ChatClient(
            "model",
            new ApiKeyCredential("sk-no-key-required"),
            new OpenAIClientOptions() { Endpoint = new("http://localhost:8080/v1") }
        );
        ChatCompletion completion = client.CompleteChat("Say 'this is a test.'");
        print(completion.Content[0].Text);
    }

    // Update is called once per frame
    void Update() { }
}
