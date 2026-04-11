using UnityEngine;

public class Monk : Unit
{
    public override void DecideAction()
    {
        _currentAttack = _unitData.Attack;
        int random = Random.Range(2, 3);
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

    public override void UseSkill(Unit target)
    {
        _target = BattleManager.Instance.GetRandomTeamTarget(this);
        Debug.Log($"{gameObject.name} UseSkill {_target.gameObject.name}");
        _animator.SetTrigger("Skill");
    }

}
