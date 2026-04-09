using UnityEngine;

public class Archer : Unit
{
    private void Start()
    {
        Init("Archer", 70, 50, 50, 10);
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
