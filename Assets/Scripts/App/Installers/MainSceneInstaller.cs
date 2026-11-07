using App.Chests;
using App.Dialogs.Merge;
using App.Utils;
using Effects;
using Library.Common.Purchases;
using UnityEngine;
using Zenject;
using Library.Dialogs;


namespace App.Installers
{
    public class MainSceneInstaller : MonoInstaller
    {
        [SerializeField] private DialogsContainer _dialogsContainer;
        [SerializeField] private RectTransform _parentCanvas;
        [SerializeField] private MovePlayerMainScene _movePlayerMainScene;
        [SerializeField] private StatsChanger _statsChanger;
        [SerializeField] private MergeDialog _mergeDialog;
        public override void InstallBindings()
        {
            BindDialogsContainer();
            /*Container.Bind<MovePlayerMainScene>().FromInstance(_movePlayerMainScene).AsSingle();
            Container.Bind<StatsChanger>().FromInstance(_statsChanger).AsSingle();*/
        }

        private void BindDialogsContainer()
        {
            /*DialogsContainer dialogsContainer =    Container.InstantiatePrefabForComponent<DialogsContainer>(_dialogsContainer);
            dialogsContainer.ParentRectTransform = _parentCanvas;*/
            Container.Bind<DialogsContainer>().FromInstance(_dialogsContainer).AsSingle();
            Container.Bind<PurchaseController>().FromNew().AsSingle();

        }

    }
}
