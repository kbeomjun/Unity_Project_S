using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum CardState
{
    Idle,
    Hover,
    Selected,
    Targeting,
    Draw,
    Discard
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

    private Vector2 _originPosition;
    private Vector3 _originScale;
    private int _originIndex = -1;

    public Vector2 OriginPosition
    {
        get => _originPosition;
        set => _originPosition = value;
    }
    public int OriginIndex
    {
        get => _originIndex;
        set => _originIndex = value;
    }

    private float _height = 0.0f;
    public float Height => _height;

    private float _hoverY = 80.0f;
    private float _hoverScale = 1.3f;
    private float _selectedScale = 1.6f;
    private float _discardScale = 0.2f;
    private float _currentZAngle = 0.0f;
    private float _targetZAngle = 0.0f;
    private float _newZAngle = 0.0f;
    private float _speed = 5.0f;

    private CardState _state = CardState.Draw;
    public CardState State
    {
        get => _state;
        set => _state = value;
    }

    private void Awake()
    {
        _rect = GetComponent<RectTransform>();
        _height = _rect.rect.height;
    }

    public void Init(CardData cardData)
    {
        _cardData = new CardData(cardData);

        _nameText.text = _cardData.Name;
        _costText.text = _cardData.Cost.ToString();
        _descriptionText.text = _cardData.Description;
        _cardData.Image = DataManager.Instance.CardImages[(int)_cardData.Type];
        _iconImage.sprite = _cardData.Image;

        _originPosition = _rect.anchoredPosition;
        _originScale = _rect.localScale;
        _state = CardState.Draw;
    }

    private void Update()
    {
        Vector2 hoverOffset = _originPosition + new Vector2(0.0f, _hoverY);

        switch (_state)
        {
            case CardState.Idle:
                _rect.anchoredPosition = Vector2.Lerp(_rect.anchoredPosition, _originPosition, _speed * Time.deltaTime);
                _rect.localScale = Vector3.Lerp(_rect.localScale, _originScale, _speed * Time.deltaTime);
                _currentZAngle = _rect.eulerAngles.z;
                _targetZAngle = 0.0f;
                _newZAngle = Mathf.LerpAngle(_currentZAngle, _targetZAngle, _speed * Time.deltaTime);
                _rect.rotation = Quaternion.Euler(0, 0, _newZAngle);
                break;

            case CardState.Hover:
                _rect.anchoredPosition = Vector2.Lerp(_rect.anchoredPosition, hoverOffset, _speed * Time.deltaTime);
                _rect.localScale = Vector3.Lerp(_rect.localScale, _originScale * _hoverScale, _speed * Time.deltaTime);
                break;

            case CardState.Selected:
                _rect.localScale = Vector3.Lerp(_rect.localScale, _originScale * _selectedScale, _speed * Time.deltaTime);
                break;

            case CardState.Targeting:
                _rect.anchoredPosition = Vector2.Lerp(_rect.anchoredPosition, hoverOffset, _speed * Time.deltaTime);
                _rect.localScale = Vector3.Lerp(_rect.localScale, _originScale * _selectedScale, _speed * Time.deltaTime);
                break;

            case CardState.Draw:
                _rect.anchoredPosition = Vector2.Lerp(_rect.anchoredPosition, _originPosition, _speed * Time.deltaTime);
                _rect.localScale = Vector3.Lerp(_rect.localScale, _originScale * _discardScale, _speed * 2.0f * Time.deltaTime);
                _currentZAngle = _rect.eulerAngles.z;
                _targetZAngle = 90.0f;
                _newZAngle = Mathf.LerpAngle(_currentZAngle, _targetZAngle, _speed * Time.deltaTime);
                _rect.rotation = Quaternion.Euler(0, 0, _newZAngle);
                break;

            case CardState.Discard:
                _rect.anchoredPosition = Vector2.Lerp(_rect.anchoredPosition, _originPosition, _speed * Time.deltaTime);
                _rect.localScale = Vector3.Lerp(_rect.localScale, _originScale * _discardScale, _speed * 2.0f * Time.deltaTime);
                _currentZAngle = _rect.eulerAngles.z;
                _targetZAngle = -90.0f;
                _newZAngle = Mathf.LerpAngle(_currentZAngle, _targetZAngle, _speed * Time.deltaTime);
                _rect.rotation = Quaternion.Euler(0, 0, _newZAngle);
                break;
        }
    }

}
