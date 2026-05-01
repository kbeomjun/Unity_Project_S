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

public class RewardItemButton : MonoBehaviour
{
    [SerializeField] private Image _icon;
    [SerializeField] private RectTransform _iconRect;
    [SerializeField] private ItemSprite _iconScript;
    [SerializeField] private TMP_Text _text;

    public RectTransform Rect { get; set; }
    public RewardItemType Type { get; set; }
    public int Value { get; set; }
    public int Index { get; set; }

    private void Awake()
    {
        Rect = GetComponent<RectTransform>();
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
                _text.text = Value.ToString();
                break;

            case RewardItemType.Unit:
                _icon.sprite = DataManager.Instance.UnitSprites[Value];
                _text.text = $"Add Unit: {DataManager.Instance.UnitDatas[Value].Name}";
                break;

            case RewardItemType.Card:
                _icon.sprite = DataManager.Instance.RewardItemSprites[(int)Type];
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
                GameManager.Instance.CurrentCoin += Value;
                BattleManager.Instance.RewardUI.RemoveItem(this);
                break;

            case RewardItemType.Unit:
                if(GameManager.Instance.PlayerUnitDatas.Count < 4)
                {
                    GameManager.Instance.AddUnit(Value);
                    _iconRect.SetParent(DataManager.Instance.CanvasRect, true);
                    _iconScript.Init((int)Type, Value);
                    _iconScript.PlayRecruitAnimation((int)Type);
                    BattleManager.Instance.RewardUI.RemoveItem(this);
                }
                else
                {
                    Debug.Log("PlayerUnit is Full");
                }
                break;

            case RewardItemType.Card:
                BattleManager.Instance.RewardUI.ShowCardRewards(this);
                break;

            case RewardItemType.Item:
                BattleManager.Instance.RewardUI.RemoveItem(this);
                break;
        }
    }

}
