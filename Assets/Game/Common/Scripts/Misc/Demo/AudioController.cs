using UnityEngine;
using UnityEngine.UI;

public enum componentType {
    Sound,
    Music,
    SoundButton,
    MusicButton,
    MusicSlider,
    SoundSlider
}

public class AudioController : MonoBehaviour {
    private AudioManager _manager;
    private AudioSource[] _sources;
    private bool _isMusic;
    public componentType type;

    private void Start() {
        _manager = AudioManager.Instance;
        CheckControllerState();
    }

    private void CheckControllerState() {
        switch (type) {
            case componentType.SoundButton: {
                _manager.soundButton = GetComponent<Button>();
                _manager.UpdateSoundButton();
                _manager.CheckAllSoundsState();
                break;
            }
            case componentType.MusicButton: {
                _manager.musicButton = GetComponent<Button>();
                _manager.UpdateMusicButton();
                _manager.CheckAllSoundsState();
                break;
            }
            case componentType.MusicSlider:
            {
                _manager.musicVolumeSlider = GetComponent<Slider>();
                _manager.musicVolumeSlider.value = UserSettingsStorage.Instance.MusicVolume;
                _manager.UpdateMusicSlider();
                break;
            }
            case componentType.SoundSlider:
            {
                _manager.soundVolumeSlider = GetComponent<Slider>();
                _manager.soundVolumeSlider.value = UserSettingsStorage.Instance.SoundVolume;
                _manager.UpdateSoundSlider();
                break;
            }
            
            case componentType.Sound: {
                _sources = GetComponents<AudioSource>();
                foreach (var src in _sources) {
                    src.mute = !UserSettingsStorage.Instance.SoundState;
                    src.volume = UserSettingsStorage.Instance.SoundVolume;
                }

                _manager.ChangeSoundStateCallback += CheckSoundState;
                break;
            }
            case componentType.Music: {
                _sources = GetComponents<AudioSource>();
                foreach (var src in _sources) {
                    src.mute = !UserSettingsStorage.Instance.MusicState;
                    src.volume = UserSettingsStorage.Instance.MusicVolume;
                }

                _manager.ChangeSoundStateCallback += CheckSoundState;
                break;
            }
            default:
                break;
        }
    }

    private void OnDestroy() {
        if (type is componentType.MusicButton or componentType.SoundButton) return;
       if (_manager == null) _manager = AudioManager.Instance;
       _manager.ChangeSoundStateCallback -= CheckSoundState;
    }

    private void CheckSoundState() {
        switch (type) {
            case componentType.Music: {
                foreach (var src in _sources) {
                    src.mute = !UserSettingsStorage.Instance.MusicState;
                    src.volume = UserSettingsStorage.Instance.MusicVolume;
                }

                break;
            }
            case componentType.Sound: {
                foreach (var src in _sources) {
                    src.mute = !UserSettingsStorage.Instance.SoundState;
                    src.volume = UserSettingsStorage.Instance.SoundVolume;
                }

                break;
            }
            case componentType.SoundButton:
            case componentType.MusicButton:
            case componentType.MusicSlider:
            case componentType.SoundSlider:
            default:
                break;
        }
    }
}