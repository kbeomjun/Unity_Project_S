using UnityEngine;

public class Lancer : Unit
{
    public override void DecideAction()
    {
        _currentAttack = _unitData.Attack;
        int random = Random.Range(2, 3);
        _nextAction = (UnitAction)random;

        NextActionScript.ChangeNextActionIcon(random, _currentAttack, _unitData.Defense);
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
