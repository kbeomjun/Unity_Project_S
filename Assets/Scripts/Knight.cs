using UnityEngine;

public class Knight : Unit
{
    private void Start()
    {
        Init("Knight", 120, 20, 20);
    }

    public override void ResetAction()
    {
        base.ResetAction();
        _animator.SetBool("IsSkillUsing", false);
    }

    public override void DecideAction()
    {
        base.DecideAction();
        _nextAction = (UnitAction)Random.Range(0, 3);
    }

    public override void PerformAction()
    {
        base.PerformAction();
        
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

    public override void UseSkill()
    {
        base.UseSkill();
        _animator.SetBool("IsSkillUsing", true);
    }

}
