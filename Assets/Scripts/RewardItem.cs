using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum RewardItemType
{
    Coin,
    Unit,
    Card,
    Item,
}

public class RewardItem : MonoBehaviour
{
    [SerializeField] private Button _button;
    [SerializeField] private RectTransform _rect;
    [SerializeField] private Image _icon;
    [SerializeField] private TMP_Text _text;

    public RewardItemType Type { get; set; }
    public int Value { get; set; }

    public void Init(RewardItemType type, int value)
    {
        Type = type;
        Value = value;

        switch (Type) 
        {
            case RewardItemType.Coin:
                break;

            case RewardItemType.Unit:
                break;

            case RewardItemType.Card:
                break;

            case RewardItemType.Item:
                break;
        }
    }

    public void OnClick()
    {
        RewardManager.Instance.RemoveItem(this);
    }

}
