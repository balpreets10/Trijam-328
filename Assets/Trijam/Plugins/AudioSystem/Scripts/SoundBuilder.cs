using UnityEngine;
namespace Balpreet.AudioSystem
{
    public class SoundBuilder
    {
        readonly SoundManager soundManager;
        SoundData soundData;
        Vector3 position = Vector3.zero;
        Transform parent = null;
        bool randomPitch;

        public SoundBuilder(SoundManager soundManager)
        {
            this.soundManager = soundManager;
        }

        public SoundBuilder WithSoundData(SoundData soundData)
        {
            this.soundData = soundData;
            return this;
        }

        public SoundBuilder WithPosition(Vector3 position)
        {
            this.position = position;
            return this;
        }
        public SoundBuilder WithParent(Transform parent)
        {
            this.parent = parent;
            return this;
        }

        public void Play()
        {
            if (!soundManager.CanPlaySound(soundData)) return;
            SoundEmitter soundEmitter = soundManager.Get();
            soundEmitter.Initialize(soundData, soundManager);
            soundEmitter.transform.position = position;
            if (parent != null)
                soundEmitter.transform.parent = parent;

            if (randomPitch)
            {
                soundEmitter.WithRandomPitch();
            }
            if (soundData.frequentSound)
            {
                soundManager.FrequentSoundEmitters.Enqueue(soundEmitter);
            }
            soundEmitter.Play();
        }
    }
}