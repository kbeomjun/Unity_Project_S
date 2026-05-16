using UnityEngine;

public class Lancer : Unit
{
    //public override void DecideAction()
    //{
    //    _nextActionScript.gameObject.SetActive(true);
    //    _currentAttack = _unitData.Attack;
    //    int random = Random.Range(0, 1);
    //    _nextAction = (UnitAction)random;
    //}

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
