using UnityEngine;

public class UnitAnimationEventReceiver : MonoBehaviour
{
    [SerializeField] private Unit _unit;

    public void TargetHit()
    {
        _unit.TargetHit();
    }

    public void AfterDie()
    {
        _unit.AfterDie();
    }
}
