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

    public void ChangeNextActionIcon(int index, int attack, int defense)
    {
        _nextActionImage.sprite = _actionImages[index];
        _nextActionNumberText.gameObject.SetActive(true);

        if (index == 0)
        {
            _nextActionNumberText.text = attack.ToString();
        }
        else if (index == 1)
        {
            _nextActionNumberText.text = defense.ToString();
        }
        else
        {
            _nextActionNumberText.gameObject.SetActive(false);
        }

        _nextActionImage.color = _colors[index];
        _nextActionNumberText.color = _colors[index];
    }

}
