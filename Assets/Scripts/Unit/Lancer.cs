using UnityEngine;

public class Lancer : Unit
{
    public override void ResetAction()
    {
        base.ResetAction();
        _animator.SetBool("IsSkillUsing", false);
    }

    public override void Die()
    {
        _animator.SetBool("IsSkillUsing", false);
        _animator.SetTrigger("Die");
    }

    public override void UseSkill()
    {
        _animator.SetBool("IsSkillUsing", true);
    }

}
