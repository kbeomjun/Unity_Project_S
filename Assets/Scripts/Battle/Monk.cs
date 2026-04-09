using UnityEngine;

public class Monk : Unit
{
    private void Start()
    {
        Init("Monk", 50, 50, 5, 5);
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
                UseSkill(_target);
                break;
        }
    }

}
