using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioController : MonoBehaviour
{
    public Slider _musicSlider, _sfxSlider;
    [SerializeField] private Canvas _soundCanvas;

    private void Start()
    {
        if (_soundCanvas != null)
        {
            _soundCanvas.enabled = false;
        }
        
    }

    public void ShowMusicConfig()
    {
        if (_soundCanvas != null)
        {
            _soundCanvas.enabled = true;
        }
    }

    public void HideMusicConfig()
    {
        if (_soundCanvas != null)
        {
            _soundCanvas.enabled = false;
        }
    }

    public void ToggleMusic()
    {
        AudioManager.instance.ToggleMusic();
    }

    public void ToggleSFX()
    {
        AudioManager.instance.ToggleSFX();
    }

    public void MusicVolume()
    {
        if(_musicSlider != null)
        {
            AudioManager.instance.MusicVolume(_musicSlider.value);
        }
    }

    public void SFXVolume()
    {
        if (_sfxSlider != null)
        {
            AudioManager.instance.SFXVolume(_sfxSlider.value);
        }
    }

}
