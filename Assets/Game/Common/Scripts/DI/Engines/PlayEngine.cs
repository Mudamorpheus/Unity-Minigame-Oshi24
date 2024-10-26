using UnityEngine;
using UnityEngine.UI;

using Zenject;
using DG.Tweening;
using TMPro;

using Game.Common.Scripts.DI.Cores;
using Game.Common.Scripts.DI.Huds;
using Game.Common.Scripts.DI.Static;

namespace Game.Common.Scripts.DI.Engines
{
    public class PlayEngine : BaseEngine
    {
        //======INSPECTOR

        [SerializeField] private GameObject   terrainBuildings;
        [SerializeField] private GameObject   terrainSticks;
        [SerializeField] private GameObject[] terrainPrefabs;
        //
        [SerializeField] private GameObject   tutorialPopup;
        [SerializeField] private Image        tutorialShadow;
        [SerializeField] private Image        tutorialSpeechBubble1;
        [SerializeField] private TMP_Text     tutorialSpeechText1;
        [SerializeField] private Image        tutorialSpeechBubble2;
        [SerializeField] private TMP_Text     tutorialSpeechText2;
        [SerializeField] private Image        tutorialSpeechPointer2;

        //======INJECTS

        [Inject] private PlayHud      sceneHud;
        [Inject] private PlayerEntity scenePlayer;

        //======FIELDS

        private bool playActive  = false;
        private bool playTutorial = false;
        private int  playTutorialStage = 0;
        private bool playTutorialLock  = true;
        private int  playScore   = 0;
        private int  playSuccess = 0;
        private int  playStreak  = 1;        

        //======PROPERTIES

        public bool Active  { get { return playActive; } }
        public int  Score   { get { return playScore; } }
        public int  Streak  { get { return playStreak; } }

        //======GAME

        public override void Init()
        {
            base.Init();

            //Spawn
            scenePlayer.SpawnStick();

            //States
            playActive = true;

            //Tutorial
            if (sceneHud.Account.Data.Tutorial)
            {
                TutorialStart();
            }
        }

        public void Defeat(bool popup)
        {
            sceneHud.AddBalance(playScore);
            sceneHud.SetBestScore(playScore);
            if (popup) { sceneHud.ShowDefeatPopup(); }
        }

        public override void Pause()
        {
            base.Pause();

            scenePlayer.Pause();
        }

        public override void Resume()
        {
            base.Resume();

            scenePlayer.Resume();
        }

        public void DestroyPlayer()
        {
            Destroy(scenePlayer);
        }

        //======ACCOUNT

        public void Reward()
        {            
            playScore += 10 * playStreak;
            playSuccess++;

            if(playSuccess <= 40)
            {
                playStreak = 1 + playSuccess/10;
            }

            sceneHud.UpdateCurrentScoreTexts();
            sceneHud.UpdateStreakTexts();
        }

        //======TERRAIN

        public void GenerateBuildings(int count)
        {
            for(int i = 0; i < count; i++)
            {
                //New building
                var prefab = terrainPrefabs[Random.Range(0, terrainPrefabs.Length-1)];
                var last = terrainBuildings.transform.GetChild(terrainBuildings.transform.childCount - 1);
                var x = last.localPosition.x + Random.Range(4f, 6f);
                var building = Instantiate(prefab, terrainBuildings.transform);
                building.transform.localPosition = new Vector2(x, 0);

                //Delete old
                if(terrainBuildings.transform.childCount > StaticData.BuildingsLimit)
                {
                    Destroy(terrainBuildings.transform.GetChild(0).gameObject);
                }
            }
            
        }

        //======TAPS

        public void OnTapBegin()
        {
            if(!playTutorial)
            {
                scenePlayer.StartGrow();
            }
            else if(!playTutorialLock)
            {
                playTutorialLock = true;
                if(playTutorialStage == 0)
                {
                    TutorialStage1();
                }                
                else if(playTutorialStage == 1)
                {
                    TutorialStage2();
                }
            }
        }

        public void OnTapEnd()
        {
            if (!playTutorial)
            {
                scenePlayer.EndGrow();
                GenerateBuildings(5);
            }
        }

        //======TUTORIAL
        
        public void TutorialStart()
        {
            playTutorial = true;
            tutorialPopup.SetActive(true);
            Sequence sequence = DOTween.Sequence();
            sequence.Append(tutorialShadow.DOFade(1, 1.00f));
            sequence.Append(tutorialSpeechBubble1.DOFade(1, 0.50f));
            sequence.Append(tutorialSpeechText1.DOFade(1, 0.50f));
            playTutorialLock = false;
        }

        public void TutorialStage1()
        {
            playTutorialStage = 1;
            Sequence sequence = DOTween.Sequence();
            Vector3 rotation = new Vector3(0, 0, -90);
            sequence.Append(scenePlayer.Stick.gameObject.transform.DOScaleY(5f, 1f).SetEase(Ease.Linear));
            sequence.Append(scenePlayer.Stick.gameObject.transform.DORotate(rotation, 1f).SetEase(Ease.Linear));
            sequence.Append(tutorialSpeechBubble2.DOFade(1, 0.5f));
            sequence.Append(tutorialSpeechText2.DOFade(1, 0.5f));
            sequence.Append(tutorialSpeechPointer2.DOFade(1, 0.5f));
            sequence.OnComplete(delegate { playTutorialLock = false; });
        }

        public void TutorialStage2()
        {
            playTutorialStage = 2;
            Sequence sequence = DOTween.Sequence();            
            sequence.Append(tutorialSpeechText2.DOFade(0, 0.50f));
            sequence.Append(tutorialSpeechBubble2.DOFade(0, 0.50f));
            sequence.Append(tutorialSpeechPointer2.DOFade(0, 0.50f));            
            sequence.Append(tutorialSpeechText1.DOFade(0, 0.50f));
            sequence.Append(tutorialSpeechBubble1.DOFade(0, 0.50f));
            sequence.Append(tutorialShadow.DOFade(0, 1.00f));
            sequence.OnComplete(delegate { TutorialEnd(); });
        }

        public void TutorialEnd()
        {
            playTutorialStage = 3;
            scenePlayer.EndGrow();
            GenerateBuildings(5);
            playTutorial = false;
            tutorialPopup.SetActive(false);
            sceneHud.CompleteTutorial();
            playTutorialLock = true;
        }
    }
}

