using System;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using OpenAI;
using OpenAI.Assistants;
using OpenAI.Chat;
using UnityEngine;

public class LLMCommunication : MonoBehaviour
{
    private List<ChatMessage> currentConv = new();
    private ChatClient chatClient;

    [SerializeField] private string endpoint = "localhost:8080";

    void Start()
    {
        // Some Testing stuff ignore
        // StartCoroutine(WaitForTask());
        // List<ChatMessage> chatMessages = new()
        // {
        //     new SystemChatMessage(
        //         "You are an unhelpful assistant that sometimes uses dutch word throughout the sentence."
        //     ),
        //     new UserChatMessage("Can you tell me about stroopwafels?"),
        // };
        // ChatCompletion completion = chatClient.CompleteChat(chatMessages);
        // chatMessages.Add(new AssistantChatMessage(completion.Content[0]));
        // chatMessages.Add(new UserChatMessage("What did I ask about?"));

        // ----- The System -----
        // setup the chat client
        chatClient = new ChatClient(
            "model",
            new ApiKeyCredential("sk-no-key-required"),
            new OpenAIClientOptions() { Endpoint = new($"http://{endpoint}/v1") }
        );
        // create system message
        currentConv.Add(
            new SystemChatMessage("You are an angry person on the phone constantly annoyed.")
        );
        // launch coroutine
        StartCoroutine(Chatting());
    }

    IEnumerator Chatting()
    {
        print("coroutine started");
        // create the Task which should run the function on another thread
        Task<ChatMessageContentPart> task = Task.Run(() =>
            MessageChatBot("Hello would you like to buy my cookies?")
        );
        // wait until the task is done
        yield return new WaitUntil(() => task.IsCompleted);
        // now do whatever with the result
        print(task.Result.Text); //Text isnt necessary here but just to show how to convert it to a string if needed
        print("coroutine stopped");
    }

    public ChatMessageContentPart MessageChatBot(string message)
    {
        // add the message to the context
        currentConv.Add(new UserChatMessage(message));
        // get the answer from the server
        ChatCompletion completion = chatClient.CompleteChat(currentConv);
        ChatMessageContentPart answer = completion.Content[0];
        // add the answer to the context
        currentConv.Add(new AssistantChatMessage(answer));
        return answer;
    }




    IEnumerator WaitForTask()
    {
        Task<string> task = Task.Run(() => LongTask());
        yield return new WaitUntil(() => task.IsCompleted);
        print(task.Result);
    }

    string LongTask()
    {
        print("Starting task..");
        System.Threading.Thread.Sleep(10000);
        print("TASK COMPLETE!");
        return "345678gg";
    }
}
