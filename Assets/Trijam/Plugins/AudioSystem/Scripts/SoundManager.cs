using System;
using System.Collections.Generic;

using Balpreet.AudioSystem;

using UnityEngine;
using UnityEngine.Pool;
namespace Balpreet.AudioSystem
{
    [Serializable]
    public class SoundManager : ISoundManager
    {
        IObjectPool<SoundEmitter> soundEmitterPool;

        readonly List<SoundEmitter> activeSoundEmitters = new();
        public readonly Queue<SoundEmitter> FrequentSoundEmitters = new();

        [SerializeField] SoundEmitter soundEmitterPrefab;
        [SerializeField] bool collectionCheck = true;
        [SerializeField] int defaultCapacity = 10;
        [SerializeField] int maxPoolSize = 100;
        [SerializeField] int maxSoundInstances = 30;


        private void Start()
        {
            InitializePool();
        }

        public SoundBuilder CreateSound() => new SoundBuilder(this);

        private void InitializePool()
        {
            soundEmitterPool = new ObjectPool<SoundEmitter>(
                CreateSoundEmitter,
                OnTakeFromPool,
                OnReturnedToPool,
                OnDestroyPoolObject,
                collectionCheck,
                defaultCapacity,
                maxPoolSize

                );
        }

        public bool CanPlaySound(SoundData data)
        {
            if (!data.frequentSound) return true;
            if (FrequentSoundEmitters.Count >= maxSoundInstances && FrequentSoundEmitters.TryDequeue(out var soundEmitter))
            {
                try
                {
                    soundEmitter.Stop();
                    //ReturnToPool(soundEmitter);
                    return true;
                }
                catch
                {
                    Debug.Log("Sound Emitter is already released");
                }
                return false;
            }
            return true;
        }

        public SoundEmitter Get()
        {
            return soundEmitterPool.Get();
        }

        public void ReturnToPool(SoundEmitter soundEmitter)
        {
            Debug.Log("Returning to Pool", soundEmitter.gameObject);
            soundEmitterPool.Release(soundEmitter);
        }

        private void OnReturnedToPool(SoundEmitter soundEmitter)
        {
            Debug.Log("Returned to Pool", soundEmitter.gameObject);
            soundEmitter.gameObject.SetActive(false);
            activeSoundEmitters.Remove(soundEmitter);
        }

        private void OnDestroyPoolObject(SoundEmitter soundEmitter)
        {
            GameObject.Destroy(soundEmitter.gameObject);
        }

        private void OnTakeFromPool(SoundEmitter soundEmitter)
        {
            soundEmitter.gameObject.SetActive(true);
            activeSoundEmitters.Add(soundEmitter);
        }

        SoundEmitter CreateSoundEmitter()
        {
            SoundEmitter soundEmitter = GameObject.Instantiate(soundEmitterPrefab);
            soundEmitter.gameObject.SetActive(false);
            return soundEmitter;
        }
    }


}

public interface ISoundManager
{
    SoundBuilder CreateSound();

    SoundEmitter Get();

    void ReturnToPool(SoundEmitter soundEmitter);
}