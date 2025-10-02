using UnityEngine;
using Neocortex.Data;

namespace Neocortex.Samples
{
    public class ChatSample : MonoBehaviour
    {
        [SerializeField] private NeocortexChatPanel chatPanel;
        [SerializeField] private NeocortexTextChatInput chatInput;
        [SerializeField] private OllamaModelDropdown modelDropdown;
        [SerializeField, TextArea] private string systemPrompt1;
        [SerializeField, TextArea] private string systemPrompt2;
        [SerializeField, TextArea] private string systemPrompt3;
        [SerializeField] private AiManager aiManager;
        [SerializeField] private bool sayMessage;
        [SerializeField] private bool typeMessage;

        private OllamaRequest request;

        void Start()
        {
            request = new OllamaRequest();
            request.OnChatResponseReceived += OnChatResponseReceived;
            request.ModelName = modelDropdown.options[0].text;
            chatInput.OnSendButtonClicked.AddListener(OnUserMessageSent);
            modelDropdown.onValueChanged.AddListener(OnDropdownValueChanged);

            string systemPrompt = systemPrompt1 + systemPrompt2 + systemPrompt3;
            request.AddSystemMessage(systemPrompt);
        }

        private void OnDropdownValueChanged(int index)
        {
            request.ModelName = modelDropdown.options[index].text;
        }

        private void OnChatResponseReceived(ChatResponse response)
        {
            if (sayMessage)
            {
                aiManager.GetGeneratedMessage(response.message);
            }
            if (typeMessage)
            {
                chatPanel.AddMessage(response.message, false);
            }
        }

        private void OnUserMessageSent(string message)
        {
            request.Send(message);
            /*chatPanel.AddMessage(message, true);*/
        }

        public void Respond(string text)
        {
            chatInput.OnSendButtonClicked.Invoke(text);
        }
    }
}