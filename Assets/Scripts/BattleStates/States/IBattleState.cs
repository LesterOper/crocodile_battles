using System;
using CodeBase.Infrastructure.States;

namespace BattleStates.States
{
    public interface IBattleState : IState
    {
        event Action Complete;
    }
}