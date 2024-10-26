using System.Linq;

using UnityEngine;

namespace Game.Common.Scripts.Systems
{
    public class MusicSystem : MonoBehaviour
    {
        //======SINGLETON

        public static MusicSystem Instance;

        //======INSPECTOR

        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip[] audioClips;

        //======FIELDS
        
        private bool   audioMusicActive = true;
        private string audioMusicSaved;

        //======MONOBEHAVIOUR

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
                return;
            }
        }

        //======MUSIC

        public void Init(string name)
        {
            audioMusicSaved = name;
        }

        public void PlayMusic(string name)
        {
            if (!audioMusicActive) return;

            var clip = audioClips.FirstOrDefault(x => x.name == name);
            if (clip)
            {
                audioSource.clip = clip;
                audioSource.loop = true;
                audioSource.Play();
            }
            else
            {
                Debug.LogError($"MusicSystem.PlayMusic(): Clip with name [{name}] doesn't exist.");
            }
        }

        public void SwitchMusic(bool state)
        {
            audioMusicActive = state;
            audioSource.enabled = state;
            
            if(audioMusicActive)
            {
                PlayMusic(audioMusicSaved);
            }
        }
    }
}
