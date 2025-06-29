using System;
using System.Collections;

using UnityEngine;
namespace Balpreet.AudioSystem
{
    [RequireComponent(typeof(AudioSource))]
    public class SoundEmitter : MonoBehaviour
    {
        public SoundData Data { get; private set; }
        AudioSource audioSource;
        Coroutine playingCoroutine;
        SoundManager soundManager;

        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
        }
        public void Initialize(SoundData data, SoundManager soundManager)
        {
            this.Data = data;
            this.soundManager = soundManager;
            audioSource.clip = data.clip;
            audioSource.outputAudioMixerGroup = data.mmixerGroup;
            audioSource.loop = data.loop;
            audioSource.playOnAwake = data.playOnAwake;
        }

        public void Play()
        {
            if (playingCoroutine != null)
            {
                StopCoroutine(playingCoroutine);
            }
            audioSource.Play();
            playingCoroutine = StartCoroutine(WaitForSoundToEnd());
        }

        public void Stop()
        {
            if ((playingCoroutine != null))
            {
                StopCoroutine(playingCoroutine);
                playingCoroutine = null;
            }
        }
        private IEnumerator WaitForSoundToEnd()
        {
            yield return new WaitWhile(() => audioSource.isPlaying);
            soundManager.ReturnToPool(this);
        }

        internal void WithRandomPitch(float min = 0.05f, float max = 0.05f)
        {
            audioSource.pitch += UnityEngine.Random.Range(min, max);
        }
    }
}
