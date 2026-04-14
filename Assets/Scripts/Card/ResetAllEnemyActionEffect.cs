using UnityEngine;

public class ResetAllEnemyActionEffect : ICardEffect
{
    public ResetAllEnemyActionEffect()
    {
        
    }

    public void Execute(Unit target)
    {
        foreach (Unit unit in BattleManager.Instance.EnemyUnits)
        {
            if (unit == null) continue;
            unit.DecideAction();
        }
    }

}
