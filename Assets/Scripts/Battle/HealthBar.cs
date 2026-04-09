using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Image _fillBackGround;
    [SerializeField] private Image _fill;
    [SerializeField] private TMP_Text _healthText;
    [SerializeField] private Image _shieldMaskImage;
    [SerializeField] private Image _shieldIcon;
    [SerializeField] private TMP_Text _shieldText;

    private float _currentFill;
    private int _displayHp;

    private Coroutine _hpRoutine;

    private int _currentDefense = 0;

    public void InitHp(int current, int max)
    {
        _currentFill = (float)current / max;
        _fill.fillAmount = _currentFill;
        _fillBackGround.fillAmount = _currentFill;

        _displayHp = current;
        _healthText.text = current.ToString();

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

        // _fill는 바로 적용
        _fill.fillAmount = targetFill;

        float startFill = _fillBackGround.fillAmount; // 백그라운드 현재값
        float time = 0f;

        while (time < 1f)
        {
            time += Time.deltaTime * speed;

            // 백그라운드 서서히 줄이기
            _fillBackGround.fillAmount = Mathf.Lerp(startFill, targetFill, time);

            yield return null;
        }

        // 마지막 보정
        _fillBackGround.fillAmount = targetFill;

        // 숫자 바로 변경
        _displayHp = targetHp;
        _healthText.text = targetHp.ToString();
    }

    public void SetDefense(int defense)
    {
        _currentDefense = defense;

        if(_currentDefense > 0)
        {
            _shieldMaskImage.gameObject.SetActive(true);
            _shieldIcon.gameObject.SetActive(true);
            _shieldText.text = _currentDefense.ToString();
        }
        else
        {
            _currentDefense = 0;
            _shieldMaskImage.gameObject.SetActive(false);
            _shieldIcon.gameObject.SetActive(false);
            _shieldText.text = _currentDefense.ToString();
        }
    }

}