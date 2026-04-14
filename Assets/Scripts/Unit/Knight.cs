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
        base.UseSkill();
        _animator.SetBool("IsSkillUsing", true);
        _isSkillUsing = true;
        AddDefense(30);
    }

}
