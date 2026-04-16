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
    [SerializeField] private Image _icon;
    [SerializeField] private RectTransform _iconRect;
    [SerializeField] private TMP_Text _text;

    private Button _button;
    private RectTransform _rect;
    public RectTransform Rect => _rect;

    public RewardItemType Type { get; set; }
    public int Value { get; set; }
    public int Index { get; set; }

    private void Awake()
    {
        _button = GetComponent<Button>();
        _rect = GetComponent<RectTransform>();
    }

    public void Init(RewardItemType type, int value, int index)
    {
        Type = type;
        Value = value;
        Index = index;

        switch (Type) 
        {
            case RewardItemType.Coin:
                _icon.sprite = DataManager.Instance.RewardItemSprites[(int)Type];
                _iconRect.sizeDelta = new Vector2(64.0f, 64.0f);
                _text.text = Value.ToString();
                break;

            case RewardItemType.Unit:
                _icon.sprite = DataManager.Instance.UnitSprites[Value];
                _iconRect.sizeDelta = new Vector2(100.0f, 100.0f);
                _text.text = $"Add Unit: {DataManager.Instance.UnitData[Value].Name}";
                break;

            case RewardItemType.Card:
                _icon.sprite = DataManager.Instance.RewardItemSprites[(int)Type];
                _iconRect.sizeDelta = new Vector2(40.0f, 55.0f);
                _text.text = $"Select Card";
                break;

            case RewardItemType.Item:
                break;
        }
    }

    public void OnClick()
    {
        switch (Type)
        {
            case RewardItemType.Coin:
                GameManager.Instance.AddCoin(Value);
                RewardManager.Instance.RemoveItem(this);
                break;

            case RewardItemType.Unit:
                if(GameManager.Instance.PlayerUnitDatas.Count < 4)
                {
                    GameManager.Instance.AddUnit(Value);
                    RewardManager.Instance.RemoveItem(this);
                }
                else
                {
                    Debug.Log("PlayerUnit is Full");
                }
                break;

            case RewardItemType.Card:
                RewardManager.Instance.ShowCardRewards(this);
                break;

            case RewardItemType.Item:
                RewardManager.Instance.RemoveItem(this);
                break;
        }
    }

}
