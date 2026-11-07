using App.Crocodile.Interfaces;
using App.PrototypeBattle;
using UnityEngine;

namespace BattleStates
{
    public class BattleStateInfoHandler
    {
        private  ICrocodileController _player;
        private  ICrocodileController _enemy;
        private  bool Is_Auto;
        private  bool isAutoByButton;
        private  bool IsAllCardsWasUsed;
        private bool IsEnemyAllCardsWasUsed;
        private  bool IsCrocodilesOnPositionOfBattlePoint;
        private bool _playerFreeFastener;
        private bool _enemyFreeFastener;
        
        public void SetCardsHandlers(ICrocodileController player, ICrocodileController enemy)
        {
            _player = player;
            _enemy = enemy;
            PlayerPrefs.SetInt(AppConstants.IS_AUTO_MODE_ON, 0);
        }

        public  void CheckIsCrocodilesOnBattlePoints() =>
            IsCrocodilesOnPositionOfBattlePoint = _player.CanMove() && _enemy.CanMove();

        public void ResetEnemy(ICrocodileController enemy)
        {
            PlayerPrefs.SetInt(AppConstants.IS_AUTO_MODE_ON, 0);
            _enemy = enemy;
        }
        
        public  bool IsAutoBattleMode ()
        {
            return Is_Auto || isAutoByButton;
        }

        public  bool IsAllCards => IsAllCardsWasUsed;
        public bool IsEnemyAllCards => IsEnemyAllCardsWasUsed;
        public  bool IsBattleFinishedProperty { get; set; }

        public  bool IsPlayerWin { get; set; }
        public bool IsEnemyWin { get; set; }
        
    }
}
