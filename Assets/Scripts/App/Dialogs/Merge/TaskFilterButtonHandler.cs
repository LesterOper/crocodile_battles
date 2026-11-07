using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace App.Dialogs.Merge
{
    public class TaskFilterButtonHandler : MonoBehaviour
    {
        [SerializeField] private Button filterButton;
        [SerializeField] private Image icon;
        [SerializeField] private Sprite active, nonActive;
        [SerializeField] private TaskFilterType _filterType;
        [SerializeField] private Image completedTaskIcon;
        private TaskFilterType currentActive;
        private IGameInfoService _gameInfoService;
        private Tween _tween;

        public void AddListener(Action action) =>
            filterButton.onClick.AddListener(()=>
            {
                action.Invoke();
                ChangeSprite(true);
            });

        public void ChangeSprite(bool isActive) => icon.sprite = isActive ? active : nonActive;

        public void Interactable(bool interact) => filterButton.interactable = interact;

    }
}