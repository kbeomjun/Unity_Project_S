using UnityEngine;

public class Lancer : Unit
{
    private new void Start()
    {
        Init("Lancer", 100, 30, 15);
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
