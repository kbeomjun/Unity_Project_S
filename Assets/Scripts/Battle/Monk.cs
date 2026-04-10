using UnityEngine;

public class Monk : Unit
{
    public override void DecideAction()
    {
        _currentAttack = _unitData.Attack;
        int random = Random.Range(0, 1);
        _nextAction = (UnitAction)random;

        NextActionScript.ChangeNextActionIcon(random, _currentAttack, _unitData.Defense);
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
