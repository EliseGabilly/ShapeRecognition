using UnityEngine;
using System.Collections.Generic;
using System.Collections;

/// <summary>
/// Insanely basic audio system which supports 3D sound.
/// Ensure you change the 'Sounds' audio source to use 3D spatial blend if you intend to use 3D sounds.
/// </summary>
public class AudioSystem : StaticInstance<AudioSystem> {

    #region Variable
    private static AudioSource _musicSource;
    private static AudioSource _soundsSource;

    [SerializeField]
    private List<AudioClip> music;
    private int musicIndex = 0;
    #endregion

    private void Start() {
        _soundsSource = GetComponentsInChildren<AudioSource>()[0];
        _musicSource = GetComponentsInChildren<AudioSource>()[1];

        musicIndex = Random.Range(0, music.Count);
        _musicSource.clip = music[musicIndex];
        _musicSource.Play();
    }

    private void Update() {
        if (!_musicSource.isPlaying) {
            PlayNextSong();
        }
    }

    public void SetSourcesVolume(float musicVolume, float soundVolume) {
        if(_soundsSource==null || _musicSource == null) {
            _soundsSource = GetComponentsInChildren<AudioSource>()[0];
            _musicSource = GetComponentsInChildren<AudioSource>()[1];
        }
        _musicSource.volume = musicVolume;
        _soundsSource.volume = soundVolume;
    }

    private void PlayNextSong() {
        musicIndex = (musicIndex + 1) % music.Count;
        _musicSource.clip = music[musicIndex];
        _musicSource.Play();
    }

    public void PlayMusic(AudioClip clip) {
        _musicSource.clip = clip;
        _musicSource.Play();
    }

    public void PlaySound(AudioClip clip, Vector3 pos, float vol = 1) {
        _soundsSource.transform.position = pos;
        PlaySound(clip, vol);
    }

    public void PlaySound(AudioClip clip, float vol = 1) {
        _soundsSource.PlayOneShot(clip, vol);
    }

}