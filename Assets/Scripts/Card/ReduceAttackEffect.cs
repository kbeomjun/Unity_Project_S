using UnityEngine;

public class ReduceAttackEffect : ICardEffect
{
    public int Percentage { get; set; }

    public ReduceAttackEffect(int percentage)
    {
        Percentage = percentage;
    }

    public void Execute(Unit target)
    {
        target.ReduceAttack(Percentage);
    }

}
