using UnityEngine;
using UnityEngine.InputSystem;
using Balpreet.AudioSystem;

public class PlaySoundOnClick : MonoBehaviour
{
    public SoundManager soundManager;
    public SoundData soundData;
    public float delay = 0.1f;
    private float mouseDownTime = 0f;
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            //PlaySound();
            mouseDownTime = Time.unscaledTime;
        }

        if (Input.GetMouseButton(0))
        {
            if ((Time.unscaledTime - mouseDownTime) >= delay)
            {
                //PlaySound();
                mouseDownTime = Time.unscaledTime;
            }
        }
    }

    private void PlaySound()
    {
        soundManager.CreateSound()
                   .WithSoundData(soundData)
                   .WithPosition(Mouse.current.position.value)
                   .Play();
    }
}
