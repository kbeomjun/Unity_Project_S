using UnityEngine;

public class HealAllEffect : ICardEffect
{
    public int Percentage { get; set; }

    public HealAllEffect(int percentage)
    {
        Percentage = percentage;
    }

    public void Execute(Unit target)
    {
        foreach(Unit unit in BattleManager.Instance.PlayerUnits)
        {
            if (unit == null) continue;
            unit.Heal(Percentage);
        }
    }

}
