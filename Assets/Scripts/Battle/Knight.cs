using UnityEngine;

public class Knight : Unit
{
    private void Start()
    {
        Init("Knight", 120, 20, 1, 100);
    }

    public override void DecideAction()
    {
        _currentAttack = _attack;
        int random = Random.Range(1, 2);
        _nextAction = (UnitAction)random;

        NextActionScript.ChangeNextActionIcon(random, _currentAttack, _defense);
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
