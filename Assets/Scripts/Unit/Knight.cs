using UnityEngine;

public class Knight : Unit
{
    public override void ResetAction()
    {
        base.ResetAction();
        _animator.SetBool("IsSkillUsing", false);
        _isSkillUsing = false;
    }

    public override void UseSkill()
    {
        _animator.SetBool("IsSkillUsing", true);
        _isSkillUsing = true;
    }

    public override void DecideAction()
    {
        _currentAttack = _unitData.Attack;
        int random = Random.Range(0, 1);
        _nextAction = (UnitAction)random;
    }

}
