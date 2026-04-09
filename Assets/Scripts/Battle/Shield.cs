using UnityEngine;

public class Shield : MonoBehaviour
{
    [SerializeField] private Animator _animator;

    public void ShieldEffect()
    {
        _animator.SetTrigger("Shield");
    }

}
