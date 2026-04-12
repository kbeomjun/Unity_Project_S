using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum CardState
{
    Idle,
    Hover,
    Selected,
    Dragging,
    Targeting
}

public class Card : MonoBehaviour
{
    [SerializeField] private TMP_Text _nameText;
    [SerializeField] private TMP_Text _costText;
    [SerializeField] private TMP_Text _descriptionText;
    [SerializeField] private Image _iconImage;

    private CardData _cardData;
    public CardData CardData => _cardData;

    private RectTransform _rect;
    public RectTransform Rect => _rect;

    private Vector2 _originPos;
    private Vector3 _originScale;

    public Vector2 OriginPos => _originPos;

    private float _hoverY = 50f;
    private float _hoverScale = 1.2f;
    private float _speed = 10f;

    private CardState _state = CardState.Idle;

    private void Awake()
    {
        _rect = GetComponent<RectTransform>();
    }

    public void Init(CardData cardData)
    {
        _cardData = new CardData(cardData);

        _nameText.text = _cardData.Name;
        _costText.text = _cardData.Cost.ToString();
        _descriptionText.text = _cardData.Description;
        
        _originPos = _rect.anchoredPosition;
        _originScale = _rect.localScale;
    }

    public void SetHighlight(bool onOff)
    {
        if (_state == CardState.Dragging) return;

        if (onOff)
        {
            if (_state == CardState.Idle)
                _state = CardState.Hover;
        }
        else
        {
            if (_state == CardState.Hover)
                _state = CardState.Idle;
        }
    }

    private void Update()
    {
        Vector2 targetPos = _originPos;
        Vector3 targetScale = _originScale;

        switch (_state)
        {
            case CardState.Idle:
                break;

            case CardState.Hover:
                targetPos += new Vector2(0, _hoverY);
                targetScale = _originScale * _hoverScale;
                break;

            case CardState.Selected:
                targetPos += new Vector2(0, _hoverY);
                targetScale = _originScale * _hoverScale;
                break;

            case CardState.Dragging:
                break;
        }

        _rect.anchoredPosition = Vector2.Lerp(_rect.anchoredPosition, targetPos, Time.deltaTime * _speed);
        _rect.localScale = Vector3.Lerp(_rect.localScale, targetScale, Time.deltaTime * _speed);
    }

}
