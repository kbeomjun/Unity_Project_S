using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Image _fill;
    [SerializeField] private TMP_Text _text;

    private float _currentFill;
    private int _displayHp;

    private Coroutine _hpRoutine;

    public void InitHp(int current, int max)
    {
        _currentFill = (float)current / max;
        _fill.fillAmount = _currentFill;

        _displayHp = current;
        _text.text = current.ToString();

        // 혹시 돌고 있는 코루틴 있으면 정지
        if (_hpRoutine != null) StopCoroutine(_hpRoutine);
    }

    public void SetHp(int current, int max)
    {
        float targetFill = (float)current / max;

        if (_hpRoutine != null) StopCoroutine(_hpRoutine);

        _hpRoutine = StartCoroutine(AnimateHp(targetFill, current));
    }

    private IEnumerator AnimateHp(float targetFill, int targetHp)
    {
        float speed = 1.0f;

        float startFill = _currentFill;
        int startHp = _displayHp;

        float time = 0f;

        while (time < 1f)
        {
            time += Time.deltaTime * speed;

            // 체력바
            _currentFill = Mathf.Lerp(startFill, targetFill, time);
            _fill.fillAmount = _currentFill;

            // 숫자
            _displayHp = Mathf.RoundToInt(Mathf.Lerp(startHp, targetHp, time));
            _text.text = _displayHp.ToString();

            yield return null;
        }

        // 마지막 보정
        _currentFill = targetFill;
        _fill.fillAmount = targetFill;

        _displayHp = targetHp;
        _text.text = targetHp.ToString();
    }

}