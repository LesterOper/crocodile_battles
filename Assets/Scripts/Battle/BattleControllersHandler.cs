using System;
using System.Collections.Generic;

namespace Battle
{
    public class BattleControllersHandler
    {
        private readonly Dictionary<BattleType, Type> _controllers = new Dictionary<BattleType, Type>
        {
            {BattleType.RANDOM, typeof(RandomBattleController)},
            {BattleType.BOSS, typeof(BossBattleController)},
            {BattleType.ONLINE, typeof(OnlineBattleController)},
        };
    
        public IBattleController GetBattleController(BattleType battleType)
        {
            var type = _controllers[battleType];
            IBattleController controller;
            switch (battleType)
            {
                case BattleType.RANDOM : controller = new RandomBattleController();
                    break;
                case BattleType.BOSS : controller = new BossBattleController();
                    break;
                case BattleType.ONLINE : controller = new OnlineBattleController();
                    break;
                default: controller = new RandomBattleController();
                    break;
            }

            return controller;
        }
    }
}