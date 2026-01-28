using System;
using System.ClientModel;
using System.Collections.Generic;
using System.Threading.Tasks;
using OpenAI;
using OpenAI.Chat;

public class LlamaUtil
{
    private List<ChatMessage> currentConversation;
    private ChatClient chatClient;
    private string systemPrompt;

    public List<ChatMessage> CurrentConversation
    {
        get => currentConversation;
    }
    public ChatClient ChatClient
    {
        get => chatClient;
    }
    /// <summary>
    /// for use with llama-server
    /// controls chat history on its own
    /// serverUri defaults to localhost
    /// </summary>
    /// <param name="systemPrompt"></param>
    /// <param name="serverUri"></param>
    public LlamaUtil(string systemPrompt, string serverUri = "http://localhost:8080/v1")
    {
        chatClient = new ChatClient(
            "model",
            new ApiKeyCredential("sk-no-key-required"),
            new OpenAIClientOptions() { Endpoint = new(serverUri) }
        );
        this.systemPrompt = systemPrompt;
        ResetConversation(systemPrompt);
    }

    public void ResetConversation(string newSystemPrompt)
    {
        currentConversation = new() { new SystemChatMessage(newSystemPrompt) };
    }

    public void ResetConversation()
    {
        ResetConversation(systemPrompt);
    }

    [Obsolete("use the Async function")]
    public ChatMessageContentPart Message(string message)
    {
        // add the message to the context
        currentConversation.Add(new UserChatMessage(message));
        // get the answer from the server
        ChatCompletion completion = chatClient.CompleteChat(currentConversation);
        ChatMessageContentPart answer = completion.Content[0];
        // add the answer to the context
        currentConversation.Add(new AssistantChatMessage(answer));
        return answer;
    }

    /// <summary>
    /// <example>
    /// do this in a coroutine like this
    /// <code>
    /// var task = MessageChatBotAsync("Do you like patatje met or zonder?");
    /// // wait until the task is done
    /// yield return new WaitUntil(() => task.IsCompleted);
    /// // now do whatever with the result
    /// print(task.Result.Text)
    /// </code>
    /// </example>
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    public Task<ChatMessageContentPart> MessageAsync(string message)
    {
#pragma warning disable CS0618 // Type or member is obsolete
        return Task.Run(() => Message(message));
#pragma warning restore CS0618 // Type or member is obsolete
    }


}
