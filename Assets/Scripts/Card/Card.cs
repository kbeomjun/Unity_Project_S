using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum CardState
{
    Idle,
    Hover,
    Selected,
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

    private Vector3 _originPosition;
    private Vector3 _originScale;

    public Vector3 OriginPosition => _originPosition;

    private float _hoverY = 40f;
    private float _hoverScale = 1.2f;
    private float _selectedScale = 1.4f;
    private float _speed = 10f;

    private CardState _state = CardState.Idle;
    public CardState State
    {
        get => _state;
        set => _state = value;
    }

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
        
        _originPosition = _rect.position;
        _originPosition.z = 0.0f;
        _originScale = _rect.localScale;
    }

    private void Update()
    {
        Vector3 hoverOffset = _originPosition + new Vector3(0.0f, _hoverY, 0.0f);

        switch (_state)
        {
            case CardState.Idle:
                _rect.position = Vector3.Lerp(_rect.position, _originPosition, _speed * Time.deltaTime);
                _rect.localScale = Vector3.Lerp(_rect.localScale, _originScale, _speed * Time.deltaTime);
                break;

            case CardState.Hover:
                _rect.position = Vector3.Lerp(_rect.position, hoverOffset, _speed * Time.deltaTime);
                _rect.localScale = Vector3.Lerp(_rect.localScale, _originScale * _hoverScale, _speed * Time.deltaTime);
                break;

            case CardState.Selected:
                _rect.localScale = Vector3.Lerp(_rect.localScale, _originScale * _selectedScale, _speed * Time.deltaTime);
                break;

            case CardState.Targeting:
                _rect.position = Vector3.Lerp(_rect.position, hoverOffset, _speed * Time.deltaTime);
                _rect.localScale = Vector3.Lerp(_rect.localScale, _originScale * _selectedScale, _speed * Time.deltaTime);
                break;
        }
    }

}
