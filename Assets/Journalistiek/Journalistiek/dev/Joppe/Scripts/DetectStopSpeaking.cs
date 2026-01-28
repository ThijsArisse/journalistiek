using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Whisper.Samples
{
    public class DetectStopSpeaking : MonoBehaviour
    {
        [SerializeField] private AiManager aiManager;
        [SerializeField] private MicrophoneDemo microphoneDemo;
        [SerializeField] private AudioSource piperAudioSource;
        [SerializeField] private PiperManager piperManager;
        [SerializeField] private Button stopButton;
        [SerializeField] private float silentWaitTime;
        private Coroutine audioCheck;
        private bool isSpeaking = true;
        private bool aiStartSpeak = false;
        private bool isCalling = false;

        private void Start()
        {
            stopButton.onClick.AddListener(ForceStop);
        }

        private void Update()
        {
            if (isCalling)
            {
                piperManager.stopPlaying = false;

                if (isSpeaking)
                {
                    if (microphoneDemo.microphoneRecord.vadIndicatorImage.color == Color.red)
                    {
                        if (audioCheck == null)
                        {
                            audioCheck = StartCoroutine(AudioCheck());
                        }
                    }
                    else if (audioCheck != null)
                    {
                        StopCoroutine(audioCheck);
                        audioCheck = null;
                    }
                }
                else
                {
                    if (piperManager.isSpeaking)
                    {
                        aiStartSpeak = true;
                    }
                    else if (aiStartSpeak)
                    {
                        isSpeaking = true;
                        aiStartSpeak = false;
                        PressSpeakButton();
                    }
                }
            }
            else if (microphoneDemo.microphoneRecord.vadIndicatorImage.color != Color.white)
            {
                isCalling = true;
                aiStartSpeak = false;
                isSpeaking = true;
            } 
        }

        private IEnumerator AudioCheck()
        {
            yield return new WaitForSeconds(silentWaitTime);

            if (microphoneDemo.microphoneRecord.vadIndicatorImage.color == Color.red)
            {
                isSpeaking = false;
                microphoneDemo.microphoneRecord.vadIndicatorImage.color = Color.blue;
                PressSpeakButton();
            }
        }

        public void PressSpeakButton()
        {
            microphoneDemo.button.onClick.Invoke();
        }

        private void ForceStop()
        {
            if (piperManager.isSpeaking)
            {
                return;
            }
            isCalling = false;
            aiManager.forceStop = true;

            piperManager.stopPlaying = true;
            piperAudioSource.Stop();
            piperAudioSource.clip = null;
            aiManager.llamaUtil.ResetConversation();
            if (audioCheck != null)
            {
                StopCoroutine(audioCheck);
            }
            if (microphoneDemo.microphoneRecord.IsRecording)
            {
                microphoneDemo.button.onClick.Invoke();
            }
            microphoneDemo.microphoneRecord.vadIndicatorImage.color = Color.white;
        }
    }
}
