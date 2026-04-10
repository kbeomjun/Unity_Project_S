using UnityEngine;

public class UnitAnimationEventReceiver : MonoBehaviour
{
    [SerializeField] private Unit _unit;

    public void HitTarget()
    {
        _unit.HitTarget();
    }

    public void AfterDie()
    {
        _unit.AfterDie();
    }

    public void HealTarget()
    {
        _unit.HealTarget();
    }

}
