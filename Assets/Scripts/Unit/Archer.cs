using UnityEngine;

public class Archer : Unit
{
    //public override void DecideAction()
    //{
    //    _nextActionScript.gameObject.SetActive(true);
    //    _currentAttack = _unitData.Attack;
    //    int random = Random.Range(1, 2);
    //    _nextAction = (UnitAction)random;
    //}

    public override void UseSkill()
    {
        _animator.SetTrigger("Skill");
    }

}
