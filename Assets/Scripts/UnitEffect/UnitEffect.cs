using UnityEngine;

public class UnitEffect : MonoBehaviour
{
    [SerializeField] private Animator _animator;

    public void ShieldEffect()
    {
        _animator.SetTrigger("Shield");
    }

    public void HealEffect()
    {
        _animator.SetTrigger("Heal");
    }

}
