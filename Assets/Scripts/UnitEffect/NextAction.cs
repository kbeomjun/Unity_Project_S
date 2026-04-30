using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NextAction : MonoBehaviour
{
    [SerializeField] private Image _nextActionIcon;
    [SerializeField] private TMP_Text _nextActionNumberText;
    
    public void UpdateNextActionIcon(int index, int attack, int defense)
    {
        _nextActionIcon.sprite = DataManager.Instance.NextActionSprites[index];
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
    }

    public Sprite GetNextActionIcon()
    {
        return _nextActionIcon.sprite;
    }

}
