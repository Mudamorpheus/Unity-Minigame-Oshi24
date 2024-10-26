using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;


public class AudioManager : MonoBehaviour {
    public static AudioManager Instance;
    public Sprite onSound, onMusic, offSound, offMusic;
    [HideInInspector] public Button soundButton;
    [HideInInspector] public Button musicButton;
    [HideInInspector] public Slider musicVolumeSlider;
    [HideInInspector] public Slider soundVolumeSlider;
    private Image _soundImage, _musicImage;
    public event Action ChangeSoundStateCallback;
    private UserSettingsStorage _data;
    public AudioClip[] clips;
    public AudioSource sfx;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
            return;
        }

        _data = UserSettingsStorage.Instance;
    }


    public void PlayClipByIndex(int clipIndex) {
        if (!UserSettingsStorage.Instance.SoundState) return;

        if (clipIndex > clips.Length - 1 || clips[clipIndex] == null) {
            Debug.LogError(
                $"DemoAudioManager.PlayClipByIndex(): index [{clipIndex}] more than clips array lenght or clip with index [{clipIndex}] is null");
            return;
        }

        sfx.PlayOneShot(clips[clipIndex]);
    }

    public void PlayClipByName(string clipName) {
        if (!UserSettingsStorage.Instance.SoundState) return;

        var clip = clips.FirstOrDefault(x => x.name == clipName);

        if (clip) {
            sfx.PlayOneShot(clip);
        } else {
            Debug.LogError($"DemoAudioManager.PlayClipByIndex(): Clip with name [{clipName}] doesn't exist");
        }
    }

    public void UpdateSoundButton() {
        _soundImage = soundButton.GetComponent<Image>();
        soundButton.onClick.AddListener(ChangeSoundState);
    }

    public void UpdateMusicButton() {
        _musicImage = musicButton.GetComponent<Image>();
        musicButton.onClick.AddListener(ChangeMusicState);
    }

    public void UpdateMusicSlider() {
        musicVolumeSlider.onValueChanged.AddListener(delegate { CheckAllSoundsState(); });
    }

    public void UpdateSoundSlider() {
        soundVolumeSlider.onValueChanged.AddListener(delegate { CheckAllSoundsState(); });
    }


    private void ChangeSoundState() {
        _data.SoundState = !_data.SoundState;
        CheckAllSoundsState();
    }

    private void ChangeMusicState() {
        _data.MusicState = !_data.MusicState;
        CheckAllSoundsState();
    }

    public void CheckAllSoundsState() {
        if (musicVolumeSlider != null) {
            UserSettingsStorage.Instance.MusicVolume = musicVolumeSlider.value;
        }

        if (soundVolumeSlider != null) {
            UserSettingsStorage.Instance.SoundVolume = soundVolumeSlider.value;
        }

        if (_soundImage != null) _soundImage.sprite = _data.SoundState ? onSound : offSound;
        if (_musicImage != null) _musicImage.sprite = _data.MusicState ? onMusic : offMusic;
        ChangeSoundStateCallback?.Invoke();
    }
}
