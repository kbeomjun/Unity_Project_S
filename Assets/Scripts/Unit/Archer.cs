using UnityEngine;

public class Archer : Unit
{
    public override void UseSkill()
    {
        _animator.SetTrigger("Skill");
    }

}
