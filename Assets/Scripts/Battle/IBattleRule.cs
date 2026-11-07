using System;

namespace Battle
{
    public interface IBattleRule
    {
        Type GetNextState();
         void RemoveState(Type state);
    }
}