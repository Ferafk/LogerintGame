using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    public Sound[] musicSounds, sfxSounds;
    public AudioSource musicSource, musicSource2, sfxSource;

    private Coroutine musicTransitionCoroutine;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        PlayMusic("Theme");
        musicSource.volume = 1f;
        musicSource2.volume = 0f;
    }

    public void PlayMusic(string name)
    {
        Sound s = Array.Find(musicSounds, x => x.name == name);

        if (s == null)
        {
            Debug.Log("Sonido no encontrado");
        }
        else
        {
            musicSource.clip = s.clip;
            musicSource.Play();
        }
    }

    public void PlayMusic2(string name)
    {
        Sound s = Array.Find(musicSounds, x => x.name == name);

        if (s == null)
        {
            Debug.Log("Sonido no encontrado");
        }
        else
        {
            musicSource2.clip = s.clip;
            musicSource2.Play();
        }
    }

    public void PlaySFX(string name)
    {
        Sound s = Array.Find(sfxSounds, x => x.name == name);

        if (s == null)
        {
            Debug.Log("Sonido no encontrado");
        }
        else
        {
            sfxSource.PlayOneShot(s.clip);
        }
    }

    public void SetCombat(bool combatState)
    {
        if (musicTransitionCoroutine != null)
        {
            StopCoroutine(musicTransitionCoroutine);
        }
        musicTransitionCoroutine = StartCoroutine(HandleMusicTransition(combatState));
    }

    private IEnumerator HandleMusicTransition(bool toCombat)
    {
        float transitionSpeed = 1f; // Controla la velocidad de la transición

        if (toCombat)
        {
            PlayMusic2("Danger");
            while (musicSource.volume > 0f || musicSource2.volume < 1f)
            {
                musicSource.volume = Mathf.MoveTowards(musicSource.volume, 0f, transitionSpeed * Time.deltaTime);
                musicSource2.volume = Mathf.MoveTowards(musicSource2.volume, 1f, transitionSpeed * Time.deltaTime);
                yield return null;
            }
        }
        else
        {
            while (musicSource.volume < 1f || musicSource2.volume > 0f)
            {
                musicSource.volume = Mathf.MoveTowards(musicSource.volume, 1f, transitionSpeed * Time.deltaTime);
                musicSource2.volume = Mathf.MoveTowards(musicSource2.volume, 0f, transitionSpeed * Time.deltaTime);
                yield return null;
            }
        }
    }

    public void ToggleMusic()
    {
        musicSource.mute = !musicSource.mute;
    }

    public void ToggleSFX()
    {
        sfxSource.mute = !sfxSource.mute;
    }

    public void MusicVolume(float volume)
    {
        musicSource.volume = volume;
        musicSource2.volume = volume;
    }

    public void SFXVolume(float volume)
    {
        sfxSource.volume = volume;
    }

    public void StopMusic()
    {
        musicSource.Stop();
    }

    public void StopMusic2()
    {
        musicSource2.Stop();
    }

    public void StopSFX()
    {
        sfxSource.Stop();
    }

}
