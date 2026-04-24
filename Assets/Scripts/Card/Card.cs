using System.Collections.Generic;
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
    Discard,
    Add,
    Stop
}

public class Card : MonoBehaviour
{
    [SerializeField] private TMP_Text _nameText;
    [SerializeField] private TMP_Text _costText;
    [SerializeField] private TMP_Text _descriptionText;
    [SerializeField] private Image _iconImage;

    private RectTransform _rect;
    public RectTransform Rect => _rect;
    
    private CardData _cardData;
    public CardData CardData => _cardData;

    private Vector2 _drawPilePosition = new Vector2(907.0f, -47.0f);
    private Vector2 _discardPilePosition = new Vector2(-907.0f, -47.0f);
    private Vector2 _addPosition = new Vector2(811.0f, 1037.0f);
    private Vector2 _originPosition;
    private Vector3 _originScale;
    private int _originIndex = -1;

    public Vector2 AddPosition => _addPosition;
    public Vector2 OriginPosition
    {
        get => _originPosition;
        set => _originPosition = value;
    }
    public Vector3 OriginScale
    {
        get => _originScale;
        set => _originScale = value;
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
    private float _addScale = 0.1f;
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
        _cardData.Effects = DataManager.Instance.CardEffects[_cardData.Key];
        _cardData.Image = DataManager.Instance.CardImages[_cardData.Key];

        _nameText.text = _cardData.Name;
        _costText.text = _cardData.Cost.ToString();
        _descriptionText.text = _cardData.Description;
        _iconImage.sprite = _cardData.Image;

        _originPosition = _rect.anchoredPosition;
        _originScale = _rect.localScale;
        _state = CardState.Draw;
    }

    public void Use()
    {
        foreach (var factory in _cardData.Effects)
        {
            IEffect effect = factory(); 
            List<Unit> targets = effect.TargetSelector.SelectTargets(null);
            effect.Execute(null, targets);
        }
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

            case CardState.Add:
                _rect.anchoredPosition = Vector2.Lerp(_rect.anchoredPosition, _originPosition, _speed * Time.deltaTime);
                _rect.localScale = Vector3.Lerp(_rect.localScale, _originScale * _addScale, _speed * 2.0f * Time.deltaTime);
                if (Vector3.Distance(_rect.anchoredPosition, _originPosition) <= 2.0f) 
                    Destroy(gameObject);
                break;
        }
    }

}
