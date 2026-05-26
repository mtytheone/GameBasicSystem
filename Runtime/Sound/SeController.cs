using System;
using UnityEngine;
using UnityEngine.Assertions;

namespace HatzeLaboratory.GameBasicSystem.Runtime.Sound
{
    /// <summary>
    /// SE の再生を管理するクラス。
    /// <see cref="SoundManager"/> から内部的に使用されます。直接インスタンス化は不要です。
    /// </summary>
    public sealed class SeController
    {
        private AudioSource[] _seAudioSourceList;

        /// <summary>
        /// 指定したAudioSource配列でSeControllerを初期化します。
        /// </summary>
        /// <param name="seAudioSourceList">SE 再生用のAudioSource配列</param>
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
