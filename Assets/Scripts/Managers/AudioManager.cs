using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    AudioSource music;

    private void Start()
    {
        music = GetComponent<AudioSource>();
    }

    private void Update()
    {
        music.volume += 0.4f * Time.deltaTime; // fade in
    }

    public void MuteAudioToggle()
    {
        Debug.Log("Mute audio in manager");
        if (AudioListener.volume > 0)
        {
            AudioListener.volume = 0;
        }
        else
        {
            AudioListener.volume = 1;
        }

    }
}
