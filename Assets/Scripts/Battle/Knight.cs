using UnityEngine;

public class Knight : Unit
{
    private void Start()
    {
        Init("Knight", 120, 1, 100);
    }

    public override void DecideAction()
    {
        _currentAttack = _attack;
        _nextAction = (UnitAction)Random.Range(1, 2);
    }

    public override void ResetAction()
    {
        base.ResetAction();
        _animator.SetBool("IsSkillUsing", false);
    }

    public override void UseSkill()
    {
        base.UseSkill();
        _animator.SetBool("IsSkillUsing", true);
    }

}
