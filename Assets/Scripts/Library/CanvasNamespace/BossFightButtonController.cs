using App;
using App.Analytic;
using App.DAO;
using App.Signals;
using App.Tutorial;
using App.Utils;
using CodeBase.Infrastructure.Services;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Library.CanvasNamespace
{
    [RequireComponent(typeof(Image) , typeof(Button))]
    public class BossFightButtonController : MonoBehaviour
    {
    
        [SerializeField] private Sprite openSprite;
        [SerializeField] private Sprite closedSprite;
        [SerializeField] private Image progressBar;
        [SerializeField] private TextMeshProUGUI toOpenCountText;
        private Image image;
        private Button button;
        private IDataService _dataService;
        private IAnalytics _analytics;
        private SignalBus _signalBus;

        [Inject]
        public void Construct(SignalBus signalBus, IDataService dataService, IAnalytics analytics)
        {
            _signalBus = signalBus;
            _dataService = dataService;
            _analytics = analytics;
        }

        void Start()
        {
            image = GetComponent<Image>();
            button = GetComponent<Button>();
            SetupButton();
        }

        private void SetupButton()
        {
            if (FightUtils.BossBattleIsOpen(_dataService))
            {
                if (!_dataService.GetTutorialStepComplete(TutorialStepType.BOSS_OPEN))
                {
                    _signalBus.Fire(new SetTutorialStepSignal(){StepType = TutorialStepType.BOSS_OPEN});
                    _signalBus.Fire<ShowTutorialSignal>();
                }
                image.sprite = openSprite;
                progressBar.fillAmount = 0;
                button.interactable = true;
                image.DOFade(0.6f, 0.6f).SetDelay(0.25f).SetLoops(-1, LoopType.Yoyo);
            }
            else
            {
                toOpenCountText.text = FightUtils.BattlesCount(_dataService) + "/" + FightUtils.GetNeedFightsToOpen(_dataService);
                image.sprite = closedSprite;
                progressBar.fillAmount = FightUtils.ProgressToOpen(_dataService);
                button.interactable = false;
            }
        }
    }
}