using UnityEngine;

public class DefenseEffect : ICardEffect
{
    public int Defense { get; set; }

    public DefenseEffect(int defense)
    {
        Defense = defense;
    }

    public void Execute(Unit target)
    {
        target.Defense(Defense);
    }

}
