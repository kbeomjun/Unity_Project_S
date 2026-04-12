using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CardManager : MonoBehaviour
{
    private List<Card> _drawPile = new List<Card>();
    private List<Card> _hand = new List<Card>();
    private List<Card> _discardPile = new List<Card>();

    private PlayerInputActions _input;
    private Card _selectedCard = null;
    private Unit _selectedUnit = null;
    private Vector3 _mousePos = Vector3.zero;
    private bool _isDrag = false;
    private float _thresholdX = 0.0f;
    private float _thresholdY = 0.0f;

    public static CardManager Instance { get; private set; }
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        _input = new PlayerInputActions();
        _thresholdX = Screen.width * 0.5f;
        _thresholdY = Screen.height * 0.4f;
    }

    public void Init(List<Card> cards)
    {
        _hand = cards;
    }

    private void OnEnable()
    {
        _input.Enable();
        _input.Player.Click.started += OnClick;
        _input.Player.Click.canceled += OnRelease;
    }

    private void OnDisable()
    {
        _input.Player.Click.started -= OnClick;
        _input.Player.Click.canceled -= OnRelease;
        _input.Disable();
    }

    private void OnClick(InputAction.CallbackContext ctx)
    {
        if (_selectedCard == null) return;
        Debug.Log($"OnClickCard");
        _isDrag = true;
    }

    private void OnRelease(InputAction.CallbackContext ctx)
    {
        if (_selectedCard == null) return;
        Debug.Log($"OnReleaseCard");

        if(_selectedCard != null)
        {
            _selectedCard.Rect.anchoredPosition = _selectedCard.OriginPos;
        }

        _isDrag = false;
        _selectedCard = null;
    }

    public void MouseProcess()
    {
        _mousePos = _input.Player.Position.ReadValue<Vector2>();

        if (!_isDrag)
        {
            int count = 0;

            foreach (Card card in _hand)
            {
                if (RectTransformUtility.RectangleContainsScreenPoint(card.Rect, _mousePos, null))
                {
                    card.SetHighlight(true);
                    _selectedCard = card;
                    count++;
                }
                else
                {
                    card.SetHighlight(false);
                }
            }

            if (count == 0) _selectedCard = null;
        }

        if (_isDrag && _selectedCard != null)
        {
            _selectedCard.Rect.position = new Vector3(_mousePos.x, _mousePos.y, 0.0f);

            if(_selectedCard.Rect.position.y >= _thresholdY)
            {
                Debug.Log("Can Use Card");
                if (_selectedCard.CardData.NeedTarget)
                {
                    _selectedCard.Rect.position = new Vector3(_thresholdX, _thresholdY, 0.0f);
                }
                else
                {

                }
            }
        }
    }

    private void LateUpdate()
    {
        MouseProcess();
    }

}
