using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CardManager : MonoBehaviour
{
    [SerializeField] TargetArrow _targetArrow;
    [SerializeField] RectTransform _canvasRect;

    private List<Card> _drawPile = new List<Card>();
    private List<Card> _hand = new List<Card>();
    private List<Card> _discardPile = new List<Card>();

    private Unit[] _playerUnits = new Unit[4];
    private Unit[] _enemyUnits = new Unit[4];

    private PlayerInputActions _input;
    private Card _selectedCard = null;
    private Unit _selectedUnit = null;

    private Vector2 _uiMousePos = Vector2.zero;
    private Vector2 _screenMousePos = Vector2.zero;
    private Vector2 _worldMousePos = Vector2.zero;

    private bool _isDrag = false;
    private float _thresholdY = 0.0f;

    public static CardManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;

        _input = new PlayerInputActions();
        _thresholdY = Screen.height * 0.4f;
    }

    public void Init(List<Card> cards, Unit[] playerUnits, Unit[] enemyUnits)
    {
        _hand = cards;
        _playerUnits = playerUnits;
        _enemyUnits = enemyUnits;
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

        _isDrag = true;
        _selectedCard.State = CardState.Selected;
    }

    private void OnRelease(InputAction.CallbackContext ctx)
    {
        if (_selectedCard == null) return;

        if (_selectedCard.State == CardState.Selected && !_selectedCard.CardData.NeedTarget && _screenMousePos.y >= _thresholdY)
        {
            Debug.Log("NonTargeting Card Used");
        }

        if (_selectedCard.State == CardState.Targeting && _selectedUnit != null && _screenMousePos.y >= _thresholdY)
        {
            Debug.Log("Targeting Card Used");
        }

        _isDrag = false;

        _selectedCard.State = CardState.Idle;
        _selectedCard = null;

        if (_selectedUnit != null)
            _selectedUnit.SetHighlight(false);

        _selectedUnit = null;

        _targetArrow.Hide();
    }

    public void MouseProcess()
    {
        _screenMousePos = _input.Player.Position.ReadValue<Vector2>();

        // Screen ¡æ UI ÁÂÇ¥ º¯È¯ (ÇÙ½É)
        RectTransformUtility.ScreenPointToLocalPointInRectangle(_canvasRect, _screenMousePos, null, out _uiMousePos);

        Vector3 screen = new Vector3(_screenMousePos.x, _screenMousePos.y, 8.0f);
        Vector3 world = Camera.main.ScreenToWorldPoint(screen);
        world.z = 0.0f;

        _worldMousePos = world;

        if (!_isDrag)
        {
            int count = 0;

            foreach (Card card in _hand)
            {
                if (RectTransformUtility.RectangleContainsScreenPoint(card.Rect, _screenMousePos, null))
                {
                    _selectedCard = card;
                    _selectedCard.State = CardState.Hover;
                    count++;
                }
                else
                {
                    card.State = CardState.Idle;
                }
            }

            if (count == 0) _selectedCard = null;
        }

        if (_isDrag && _selectedCard != null)
        {
            if (_selectedCard.State != CardState.Targeting)
            {
                _selectedCard.State = CardState.Selected;
                _selectedCard.Rect.anchoredPosition = _uiMousePos + new Vector2(0.0f, _selectedCard.Height * 1.5f);
            }

            if (_screenMousePos.y >= _thresholdY)
            {
                if (_selectedCard.CardData.NeedTarget)
                    _selectedCard.State = CardState.Targeting;
            }
        }

        if (_isDrag && _selectedCard != null && _selectedCard.State == CardState.Targeting)
        {
            _targetArrow.Show();
            _targetArrow.UpdateArrow(_selectedCard.Rect.anchoredPosition + new Vector2(0.0f, _selectedCard.Height * 0.4f), 
                                        _uiMousePos + new Vector2(0.0f, _selectedCard.Height * 1.5f));

            Unit[] targetUnits = _selectedCard.CardData.TargetType ? _playerUnits : _enemyUnits;

            int count = 0;

            foreach (Unit unit in targetUnits)
            {
                if (unit == null) continue;

                if (Vector3.Distance(unit.transform.position, _worldMousePos) <= 0.6f)
                {
                    unit.SetHighlight(true);
                    _selectedUnit = unit;
                    count++;
                    _targetArrow.SetArrowColor(true);
                }
                else
                {
                    unit.SetHighlight(false);
                }
            }

            if (count == 0) 
            { 
                _selectedUnit = null;
                _targetArrow.SetArrowColor(false);
            }
        }
    }

    private void LateUpdate()
    {
        MouseProcess();
    }

}