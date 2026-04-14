using UnityEngine;

public class FrontDefenseEffect : ICardEffect
{
    public int Defense { get; set; }

    public FrontDefenseEffect(int defense)
    {
        Defense = defense;
    }
    
    public void Execute(Unit target)
    {
        if (BattleManager.Instance.PlayerUnits[0] != null)
        {
            BattleManager.Instance.PlayerUnits[0].Defense(Defense);
        }

        if (BattleManager.Instance.PlayerUnits[1] != null)
        {
            BattleManager.Instance.PlayerUnits[1].Defense(Defense);
        }
    }

}
