using System;

using UnityEngine;
using UnityEngine.Audio;
namespace Balpreet.AudioSystem
{
    [Serializable]
    public class SoundData
    {
        public AudioClip clip;
        public AudioMixerGroup mmixerGroup;
        public bool loop;
        public bool playOnAwake;
        public bool frequentSound;
    }
}
