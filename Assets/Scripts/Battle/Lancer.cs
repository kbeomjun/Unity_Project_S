using UnityEngine;

public class Lancer : Unit
{
    private void Start()
    {
        Init("Lancer", 100, 1, 30);
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
