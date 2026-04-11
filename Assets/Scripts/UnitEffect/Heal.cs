using UnityEngine;

public class Heal : MonoBehaviour
{
    [SerializeField] private Animator _animator;

    public void HealEffect()
    {
        _animator.SetTrigger("Heal");
    }

}
