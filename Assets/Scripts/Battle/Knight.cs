using UnityEngine;

public class Knight : Unit
{
    private new void Start()
    {
        Init("Knight", 120, 20, 20);
        base.Start();
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
