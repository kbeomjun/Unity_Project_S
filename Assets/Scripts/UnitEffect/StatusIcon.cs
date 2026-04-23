using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StatusIcon : MonoBehaviour
{
    [SerializeField] private Image _icon;
    [SerializeField] private TMP_Text _durationText;

    public void Init(Sprite sprite, int duration)
    {
        _icon.sprite = sprite;
        SetDuration(duration);
    }

    public void SetDuration(int duration)
    {
        _durationText.text = duration.ToString();
        _durationText.gameObject.SetActive(duration >= 1);
    }

}