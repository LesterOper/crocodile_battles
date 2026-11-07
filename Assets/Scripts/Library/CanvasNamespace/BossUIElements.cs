using CodeBase.Infrastructure.Services;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Library.CanvasNamespace
{
    public class BossUIElements : MonoBehaviour
    {
        [SerializeField] private Sprite bossUISprite;
        private IGameInfoService _gameInfoService;
        [Inject]
        private void Construct(IGameInfoService gameInfoService)
        {
            _gameInfoService = gameInfoService;
        }
        
        void Start()
        {
            Image imageComponent = GetComponent<Image>();
            Sprite defaultSprite = imageComponent.sprite;
            if (_gameInfoService.IsBossFight())
            {
                imageComponent.sprite = bossUISprite;
                if (GetComponentInParent<PlayerCardsHandler>() != null)
                    imageComponent.sprite = defaultSprite;
            }
            
        }
    }
}
