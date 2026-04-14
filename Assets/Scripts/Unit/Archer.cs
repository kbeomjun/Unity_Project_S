using UnityEngine;

public class Archer : Unit
{
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
        _target = BattleManager.Instance.GetRandomEnemyTarget(this);
        _currentAttack *= 2;
        Debug.Log($"{gameObject.name} UseSkill {_currentAttack} {_target.gameObject.name}");
        _animator.SetTrigger("Skill");
    }

}
