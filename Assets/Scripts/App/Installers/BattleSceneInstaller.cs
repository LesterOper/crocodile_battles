using App.Utils;
using Battle;
using BattleStates.States;
using Library.Dialogs;
using UnityEngine;
using Zenject;

namespace App.Installers
{
    public class BattleSceneInstaller : MonoInstaller
    {
        [SerializeField] private DialogsContainer _dialogsContainer;
        [SerializeField] private RectTransform _parentCanvas;
        [SerializeField] private BattleEndView _battleEndView;
        [SerializeField] private BattleViewController _battleViewController;
        [SerializeField] private StatsChanger _statsChanger;
        public override void InstallBindings()
        {
            BindDialogsContainer();

        }

        private void BindDialogsContainer()
        {
            /*DialogsContainer dialogsContainer =    Container.InstantiatePrefabForComponent<DialogsContainer>(_dialogsContainer);
            dialogsContainer.ParentRectTransform = _parentCanvas;*/
            Container.Bind<DialogsContainer>().FromInstance(_dialogsContainer).AsSingle();
            /*Container.Bind<BattleEndView>().FromInstance(_battleEndView).AsSingle();
            Container.Bind<BattleViewController>().FromInstance(_battleViewController).AsSingle();
            Container.Bind<StatsChanger>().FromInstance(_statsChanger).AsSingle();*/
        }
    }
}

