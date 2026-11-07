using App.Crocodile.Interfaces;
using App.DAO;
using App.SetupsScript;
using CodeBase.Infrastructure.Services;
using Spine.Unity;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

namespace App.Crocodile
{
    public class CrocodileSkin : MonoBehaviour
    {
        [SerializeField] private BasicCharacterSkinSetup _skinSetup;
        [SerializeField] private SkeletonAnimation _animation;
        private ICrocodileController _controller;
        [Inject]private IDataService _dataService;

        private void Start()
        {
            _controller = GetComponentInParent<ICrocodileController>();
            if (_controller != null)
            {
                if (!_controller.GetCrocodileInfo().Crocodile.IsPlayer())
                    SetSkin((CharacterType) GenerateRandomSkinExcludingPlayerSkin());
            }
            else SetSkin(_dataService.GetCrocodileData().CharacterType);
        }

        private void SetSkin(CharacterType type)
        {
            if (type == CharacterType.NONE)
                type = CharacterType.BASIC_1;
            _animation.skeletonDataAsset = _skinSetup.GetCharacterByCharacterType(type).skin;
            _animation.ClearState();
            _animation.Initialize(true);
        }

        private int GenerateRandomSkinExcludingPlayerSkin()
        {
            int index = 1;
            while (true)
            {
                index = Random.Range(1, 10);
                if (index != (int) _dataService.GetCrocodileData().CharacterType)
                    break;
            }

            return index;
        }
    }
}
