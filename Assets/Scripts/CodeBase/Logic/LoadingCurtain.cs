using System;
using System.Collections;
using System.Collections.Generic;
using App;
using App.Signals;
using DG.Tweening;
using Library;
using UnityEngine;
using UnityEngine.UI;
using VoxelBusters.CoreLibrary;
using Zenject;

namespace CodeBase.Logic
{
  public class LoadingCurtain : MonoBehaviour
  {
    public event Action OnCurtainShowed;
    public event Action OnCurtainHide;

    public CanvasGroup Curtain;
    [SerializeField] private List<Letters> letters;
    [SerializeField] private Ease ease;
    [SerializeField] private Transform eye;
    [SerializeField] private Image logo;
    [SerializeField] private Image blick;
    [SerializeField] private RectTransform blickTr;
    [SerializeField] private RectTransform endObjectForBlick;
    private Coroutine _fadeCoroutine;
    private Coroutine _lettersCoroutine;
    private Coroutine _HideCoroutine;
    private WaitForSeconds _waitForFade = new WaitForSeconds(0.001f);
    private Vector2[] _lettersPos = new Vector2[5];
    private Sequence _sequence;
    private SignalBus _signalBus;

    [Inject]
    public void Construct(SignalBus signalBus)
    {
      _signalBus = signalBus;
    }
    public LoadingCurtain SubscribeCurtainShowListener(Action Handler)
    {
      OnCurtainShowed += Handler;
      return this;
    }

    public LoadingCurtain UnsubscribeCurtainShowListener(Action Handler)
    {
      OnCurtainShowed -= Handler;
      return this;
    }
    
    public void Show(Callback callback = null)
    {
      StopPreviousFade();
      gameObject.SetActive(true);
      _fadeCoroutine = StartCoroutine(DoFadeOut());
      logo.transform.DOScale(1, 0.8f).SetLoops(-1, LoopType.Yoyo);
      _sequence = DOTween.Sequence();
      _sequence.Append(blick.DOFade(1, 0.6f));
      _sequence.Join(blick.transform.DOScale(0.68f, 2f));
      _sequence.Join(blickTr.DOAnchorPos(endObjectForBlick.anchoredPosition, 2).SetEase(Ease.Linear));
      _sequence.SetLoops(-1, LoopType.Restart);
    }

    public void HideLetters()
    {
      StopPreviousFade();
      _fadeCoroutine = StartCoroutine(DoFadeIn());
    }
    
    public void Hide(Action callback = null)
    {
      if (Curtain.isActiveAndEnabled)
      {
        StopPreviousFade();
        _fadeCoroutine = StartCoroutine(DoFadeIn(callback));
      }
    }

    private void Awake()
    {
      for (int i = 0; i < letters.Count; i++)
      {
        _lettersPos[i] = letters[i].letter.localPosition;
      }
      DontDestroyOnLoad(this);
    }

    private void OnEnable()
    {
      
    }

    private void OnDisable()
    {
      //_signalBus.TryUnsubscribe<OpenCurtainSignal>(HideLetters);
    }

    private void Start()
    {
      _signalBus.Subscribe<OpenCurtainSignal>(HideLetters);
      Curtain.alpha = 1;
    }

    private IEnumerator AnimationLetters(Callback callback)
    {
      yield return new WaitForSeconds(0.5f);
      for (int i = 0; i < letters.Count; i++)
      {
        letters[i].letter.DOScale(letters[i].startScale, 0.25f);
        letters[i].letter.DOLocalMove(letters[i].targetPos, 0.25f).SetEase(ease);
        yield return new WaitForSeconds(0.07f);
      }

      yield return new WaitForSeconds(0.1f);
      eye.DOLocalJump(new Vector3(0, 0, 0), 0.3f, 1, 1f).SetEase(Ease.InSine);
      callback?.Invoke();
    }

    private void StopPreviousFade()
    {
      if (_fadeCoroutine != null)
        StopCoroutine(_fadeCoroutine);
      if (_lettersCoroutine != null)
        StopCoroutine(_lettersCoroutine);
    }

    private IEnumerator DoFadeIn(Action callback = null)
    {
      while (Curtain.alpha > 0)
      {
        Curtain.alpha -= 0.025f;
        yield return _waitForFade;
      }

      gameObject.SetActive(false);
      callback?.Invoke();
    }

    private IEnumerator DoFadeOut()
    {
      gameObject.SetActive(true);

      while (Curtain.alpha < 1)
      {
        Curtain.alpha += 0.035f;
        yield return _waitForFade;
      }
      OnCurtainShowed?.Invoke();
    }

    [Serializable]
    public struct Letters
    {
      public RectTransform letter;
      public Vector2 targetPos;
      public float startScale;
    }
  }
}