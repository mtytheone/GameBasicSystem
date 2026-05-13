using System;
using UnityEngine;
using UnityEngine.Assertions;

namespace HatzeLaboratory.GameBasicSystem.Runtime.Sound
{
    public sealed class SeController
    {
        private AudioSource[] _seAudioSourceList;

        public SeController(AudioSource[] seAudioSourceList)
        {
            foreach (AudioSource audioSource in seAudioSourceList)
            {
                Assert.IsNotNull(audioSource);
            }

            _seAudioSourceList = new AudioSource[seAudioSourceList.Length];
            Array.Copy(seAudioSourceList, _seAudioSourceList, seAudioSourceList.Length);
        }

        internal void PlaySe(AudioClip audioClip)
        {
            if (!audioClip)
            {
                Debug.LogError($"AudioClip is null.");
                return;
            }

            for (int i = 0; i < _seAudioSourceList.Length; i++)
            {
                AudioSource audioSource = _seAudioSourceList[i];
                if (!audioSource)
                {
                    continue;
                }

                if (audioSource.isPlaying)
                {
                    continue;
                }

                audioSource.clip = audioClip;
                audioSource.Play();
                return;
            }

            Debug.LogError($"All SE AudioSources are busy. Cannot play {audioClip.name}.");
        }
    }
}
