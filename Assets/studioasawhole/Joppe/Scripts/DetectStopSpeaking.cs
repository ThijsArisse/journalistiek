using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Whisper.Samples
{
    public class DetectStopSpeaking : MonoBehaviour
    {
        [SerializeField] private MicrophoneDemo microphoneDemo;
        [SerializeField] private AudioSource piperAudioSource;
        [SerializeField] private float silentWaitTime;
        private Coroutine audioCheck;
        private bool isSpeaking = true;
        private bool aiStartSpeak = false;


        private void Update()
        {
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
                if(piperAudioSource.isPlaying)
                {
                    aiStartSpeak = true;
                } 
                else if(aiStartSpeak)
                {
                    isSpeaking = true;
                    aiStartSpeak = false;
                    PressSpeakButton();
                }
            }
        }

        private IEnumerator AudioCheck()
        {
            yield return new WaitForSeconds(silentWaitTime);

            if (microphoneDemo.microphoneRecord.vadIndicatorImage.color == Color.red)
            {
                isSpeaking = false;
                PressSpeakButton();
            }
        }

        public void PressSpeakButton()
        {
            Debug.Log(8);
            microphoneDemo.button.onClick.Invoke();
        }
    }
}
