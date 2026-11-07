using System;
using System.Collections;
using System.Collections.Generic;
using App;
using App.Audio;
using App.Signals;
using DG.Tweening;
using Library;
using Library.Common;
using Library.Dialogs;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Zenject;

namespace Effects
{
    public class LogoScreen : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer textRect;
    
        [SerializeField] private Transform cameras;

        [SerializeField] private MovePlayerMainScene _movePlayerMainScene;
        [SerializeField] private CanvasGroup mainCanvas;
        [SerializeField] private List<Letters> letters;
        [SerializeField] private GameObject chests;
        [SerializeField] private SpriteRenderer reflection;
        [SerializeField] private Ease ease;
        [SerializeField] private Transform eye;
        [SerializeField] private Image backOnBattleScene;
        [SerializeField] private CanvasGroup tutorialCanvas;
        [SerializeField] private RectTransform cardBlocker;
        private Vector3 _eyeStartPos;
        private SoundPlayer _soundPlayer;
    
        public static bool isCurrentSession = false;
        private DialogsContainer _container;
        private SignalBus _signalBus;
        [Inject]
        public void Construct(DialogsContainer dialogsContainer, SignalBus signalBus,SoundPlayer soundPlayer)
        {
            _container = dialogsContainer;
            _signalBus = signalBus;
            _soundPlayer = soundPlayer;
        } 
    
        [Serializable]
        public struct Letters
        {
            public Transform letter;
            public Vector2 targetPos;
            public float startScale;
        }
        void Start()
        {
            _eyeStartPos = eye.position;
            if (SceneManager.GetActiveScene().name == "BattleScene" /*tutorialEnd?*/)
            {
                StartCoroutine(AnimationOnBattleScene());
                return;
            }
            if(!isCurrentSession)
                StartCoroutine(Animation());
            else if(SceneManager.GetActiveScene().name != "BattleScene")
            {
                WithoutMove(-7f);
            }
        }

        private IEnumerator Animation()
        {
            eye.position = _eyeStartPos;
            yield return new WaitForSeconds(0.5f);
            _soundPlayer.PlaySound("logo_start_croco_01" ,AudioMixerController.MasterMixerGroup.SFX, 0.4f);
            for (int i = 0; i < letters.Count; i++)
            {
                letters[i].letter.DOScale(letters[i].startScale, 0.35f );
                letters[i].letter.DOLocalMove(letters[i].targetPos, 0.35f).SetEase(ease);
                yield return new WaitForSeconds(0.3f);
            }
            yield return new WaitForSeconds(0.3f);
            textRect.DOFade(1, 0.4f);
            eye.DOLocalJump(new Vector3(0, 0, 0), 0.3f, 1, 1f).SetEase(Ease.InSine);
            yield return new WaitForSeconds(0.8f);
        
            _movePlayerMainScene.Move();
            chests.SetActive(true);
            cameras.DOMoveX(-7f, 1.6f).SetEase(Ease.InOutSine);
            mainCanvas.DOFade(1, 0.8f).SetDelay(0.8f).OnComplete(()=>
            {
            
                tutorialCanvas.DOFade(1, 0.2f);
                Destroy(gameObject);
                reflection.DOFade(0.5f, 1.2f);
            });
        }

        private IEnumerator AnimationOnBattleScene()
        {
            mainCanvas.DOFade(0, 0);
            cameras.transform.position = new Vector3(-18, 0, -10f);
            yield return new WaitForSeconds(1f);
            _soundPlayer.PlaySound("logo_start_croco_01" ,AudioMixerController.MasterMixerGroup.SFX, 0.4f);
            for (int i = 0; i < letters.Count; i++)
            {
                letters[i].letter.DOScale(letters[i].startScale, 0.35f );
                letters[i].letter.DOLocalMove(letters[i].targetPos, 0.35f).SetEase(ease);
                yield return new WaitForSeconds(0.15f);
            }
            yield return new WaitForSeconds(0.8f);
            cameras.DOMoveX(0, 1.6f).SetEase(Ease.InOutSine).OnComplete(() =>
            {
                //_signalBus.Fire<ShowTutorialSignal>();
                cardBlocker.gameObject.SetActive(false);
            });
            backOnBattleScene.DOFade(0, 0.8f)
                .OnComplete(() => backOnBattleScene.gameObject.SetActive(false));
            mainCanvas.DOFade(1, 0.8f)
                .SetDelay(0.8f)
                .OnComplete(() =>
                {
                    tutorialCanvas.DOFade(1, 0.2f);
                    Destroy(gameObject);
                });
        }

        public void WithoutMove(float cameraPosX)
        {
            cameras.position = new Vector3(cameraPosX, cameras.position.y, cameras.position.z);
            mainCanvas.alpha = 1;
            if(reflection != null)
                reflection.DOFade(0.65f, 0.6f);
            if(backOnBattleScene != null)
                backOnBattleScene.gameObject.SetActive(false);
            tutorialCanvas.DOFade(1, 0.2f);
            if(cardBlocker!=null)
                cardBlocker.gameObject.SetActive(false);
            Destroy(gameObject);
        }
    }
}
