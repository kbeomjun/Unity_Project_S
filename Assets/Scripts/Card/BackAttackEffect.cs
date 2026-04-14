using UnityEngine;

public class BackAttackEffect : ICardEffect
{
    public int Attack { get; set; }

    public BackAttackEffect(int attack)
    {
        Attack = attack;
    }

    public void Execute(Unit target)
    {
        if (BattleManager.Instance.PlayerUnits[2] != null)
        {
            BattleManager.Instance.PlayerUnits[2].AddAttack(Attack);
        }

        if (BattleManager.Instance.PlayerUnits[3] != null)
        {
            BattleManager.Instance.PlayerUnits[3].AddAttack(Attack);
        }
    }

}
