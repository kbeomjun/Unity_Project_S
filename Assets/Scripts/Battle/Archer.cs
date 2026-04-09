using UnityEngine;

public class Archer : Unit
{
    private new void Start()
    {
        Init("Archer", 70, 50, 10);
        base.Start();
    }

    public override void PerformAction()
    {
        switch (_nextAction)
        {
            case UnitAction.Attack:
                Attack();
                break;

            case UnitAction.Defense:
                Defense();
                break;

            case UnitAction.Skill:
                UseSkill();
                break;
        }
    }

}
