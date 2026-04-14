using UnityEngine;

public class AttackEffect : ICardEffect
{
    public int Attack { get; set; }

    public AttackEffect(int attack)
    {
        Attack = attack;
    }

    public void Execute(Unit target)
    {
        target.AddAttack(Attack);
    }

}
