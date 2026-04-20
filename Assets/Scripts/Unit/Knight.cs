using UnityEngine;

public class Knight : Unit
{
    public override void ResetAction()
    {
        base.ResetAction();
        _animator.SetBool("IsSkillUsing", false);
    }

    public override void Die()
    {
        _animator.SetBool("IsSkillUsing", false);
        base.Die();
    }

    public override void UseSkill()
    {
        _animator.SetBool("IsSkillUsing", true);
    }

}
