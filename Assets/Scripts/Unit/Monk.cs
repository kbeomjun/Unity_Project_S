using UnityEngine;

public class Monk : Unit
{
    public override void DecideAction()
    {
        _nextActionScript.gameObject.SetActive(true);
        _currentAttack = _unitData.Attack;
        int random = Random.Range(0, 1);
        _nextAction = (UnitAction)random;
    }
}
