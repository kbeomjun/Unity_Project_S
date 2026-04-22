using UnityEngine;

public class UnitEffect : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    
    float _time = 0.0f;
    float _duration = 0.3f;
    bool _isStart = false;

    public void DefenseEffect()
    {
        _animator.SetTrigger("Defense");
    }

    public void HealEffect()
    {
        _animator.SetTrigger("Heal");
    }

    public void Init()
    {
        _isStart = true;
    }

    private void Update()
    {
        if (_isStart)
        {
            _time += Time.deltaTime;
            if (_time >= _duration) Destroy(gameObject);
        }
    }

}
