using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NextAction : MonoBehaviour
{
    [SerializeField] private Image _nextActionImage;
    [SerializeField] private TMP_Text _nextActionNumberText;
    [SerializeField] private Sprite[] _actionImages;

    private Color[] _colors =
    {
        new Color32(210, 40, 40, 255),
        new Color32(255, 255, 255, 255),
        new Color32(40, 55, 210, 255),
    };

    public void ChangeNextActionIcon(int random, int attack, int defense)
    {
        _nextActionImage.sprite = _actionImages[random];
        _nextActionNumberText.gameObject.SetActive(true);

        if (random == 0)
        {
            _nextActionNumberText.text = attack.ToString();
        }
        else if (random == 1)
        {
            _nextActionNumberText.text = defense.ToString();
        }
        else
        {
            _nextActionNumberText.gameObject.SetActive(false);
        }

        _nextActionImage.color = _colors[random];
        _nextActionNumberText.color = _colors[random];
    }

    public void SetNextActionNumberText(int num)
    {
        _nextActionNumberText.text = num.ToString();
    }

}
