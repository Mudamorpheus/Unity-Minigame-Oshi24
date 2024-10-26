using UnityEngine;

using System.Linq;

namespace Game.Common.Scripts.DI.Cores
{
    public class BaseEntity : MonoBehaviour
    {
        //======INSPECTOR

        [SerializeField] protected SpriteRenderer entitySprite;
        [SerializeField] protected Animator       entityAnimator;
        [SerializeField] protected AudioSource    entitySource;
        [SerializeField] protected AudioClip[]    entityClips;

        //======PROPERTIES

        public SpriteRenderer Sprite { get { return entitySprite;  } }
        public float          BaseWidth  { get { return entitySprite.sprite.bounds.size.x; } }
        public float          BaseHeight { get { return entitySprite.sprite.bounds.size.y; } }
        public float          Width  { get { return entitySprite.sprite.bounds.size.x * entitySprite.transform.localScale.x; } }
        public float          Height { get { return entitySprite.sprite.bounds.size.y * entitySprite.transform.localScale.y; } }
        public float          ScaleX { get { return entitySprite.transform.localScale.x; } }
        public float          ScaleY { get { return entitySprite.transform.localScale.y; } }

        //======SOUND

        public void PlaySound(string name, bool loop)
        {
            var clip = entityClips.FirstOrDefault(x => x.name == name);
            if (clip)
            {
                entitySource.clip = clip;
                entitySource.loop = loop;
                entitySource.Play();
            }

        }

        public void StopSound()
        {
            entitySource.Stop();
        }
    }
}