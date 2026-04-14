using UnityEngine;

public class ChangeActionEffect : ICardEffect
{
    public UnitAction Action { get; set; }

    public ChangeActionEffect(UnitAction action)
    {
        Action = action;
    }

    public void Execute(Unit target)
    {
        target.SetNextAction(Action);
    }

}
