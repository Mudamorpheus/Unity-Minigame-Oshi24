using System.Linq;

using UnityEngine;

namespace Game.Common.Scripts.Systems
{	
	public class SoundSystem : MonoBehaviour
	{
        //======SINGLETON

        public static SoundSystem Instance;

        //======INSPECTOR

        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip[] audioClips;

        //======FIELDS

        private bool audioSoundActive = true;

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

        //======SOUND

        public void PlaySound(string name)
        {
            if (!audioSoundActive) return;

            var clip = audioClips.FirstOrDefault(x => x.name == name);
            if (clip)
            {
                audioSource.PlayOneShot(clip);
            }
            else
            {
                Debug.LogError($"SoundSystem.PlaySound(): Clip with name [{name}] doesn't exist.");
            }
        }

        public void SwitchSound(bool state)
        {
            audioSoundActive = state;
            audioSource.enabled = state;
        }
    }
}
