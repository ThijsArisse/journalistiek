using UnityEngine;
using Neocortex.Data;

namespace Neocortex.Samples
{
    public class ChatSample : MonoBehaviour
    {
        [SerializeField] private NeocortexChatPanel chatPanel;
        [SerializeField] private NeocortexTextChatInput chatInput;
        [SerializeField] private OllamaModelDropdown modelDropdown;
        [SerializeField, TextArea] private string systemPrompt;
        [SerializeField] private AiManager aiManager;

        private OllamaRequest request;

        void Start()
        {
            request = new OllamaRequest();
            request.OnChatResponseReceived += OnChatResponseReceived;
            request.ModelName = modelDropdown.options[0].text;
            chatInput.OnSendButtonClicked.AddListener(OnUserMessageSent);
            modelDropdown.onValueChanged.AddListener(OnDropdownValueChanged);

            request.AddSystemMessage(systemPrompt);
        }

        private void OnDropdownValueChanged(int index)
        {
            request.ModelName = modelDropdown.options[index].text;
        }

        private void OnChatResponseReceived(ChatResponse response)
        {
            Debug.Log(4);
            aiManager.GetGeneratedMessage(response.message);
/*            chatPanel.AddMessage(response.message, false);
*/        }

        private void OnUserMessageSent(string message)
        {
            Debug.Log(2);
            request.Send(message);
            /*chatPanel.AddMessage(message, true);*/
        }

        public void Respond(string text)
        {
            chatInput.OnSendButtonClicked.Invoke(text);
        }
    }
}