using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CardManager : MonoBehaviour
{
    private List<Card> _drawPile = new List<Card>();
    private List<Card> _hand = new List<Card>();
    private List<Card> _discardPile = new List<Card>();

    private Unit[] _playerUnits = new Unit[4];
    private Unit[] _enemyUnits = new Unit[4];

    private PlayerInputActions _input;
    private Card _selectedCard = null;
    private Unit _selectedUnit = null;
    private Vector3 _mousePos = Vector3.zero;
    private Vector3 _screenPos = Vector3.zero;
    private Vector3 _worldPos = Vector3.zero;
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
        Debug.Log($"OnClickCard");
        _isDrag = true;
        _selectedCard.State = CardState.Selected;
    }

    private void OnRelease(InputAction.CallbackContext ctx)
    {
        if (_selectedCard == null) return;
        Debug.Log($"OnReleaseCard");

        if (_selectedCard != null && _selectedCard.State == CardState.Selected && !_selectedCard.CardData.NeedTarget)
        {
            Debug.Log($"NonTargeting Card Used");
        }

        if (_selectedCard != null && _selectedCard.State == CardState.Targeting && _selectedCard.CardData.NeedTarget && _selectedUnit != null)
        {
            Debug.Log($"Targeting Card Used");
        }

        _isDrag = false;
        _selectedCard.State = CardState.Idle;
        _selectedCard = null;
        _selectedUnit = null;
    }

    public void MouseProcess()
    {
        _mousePos = _input.Player.Position.ReadValue<Vector2>();
        _screenPos = new Vector3(_mousePos.x, _mousePos.y, 8.0f);
        _worldPos = Camera.main.ScreenToWorldPoint(_screenPos);
        _worldPos.z = 0.0f;

        if (!_isDrag)
        {
            int count = 0;

            foreach (Card card in _hand)
            {
                if (RectTransformUtility.RectangleContainsScreenPoint(card.Rect, _mousePos, null))
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
                _selectedCard.Rect.position = new Vector3(_mousePos.x, _mousePos.y, 0.0f);
            }

            if (_mousePos.y >= _thresholdY)
            {
                if (_selectedCard.CardData.NeedTarget)
                {
                    _selectedCard.State = CardState.Targeting;
                }
            }
        }

        if (_isDrag && _selectedCard != null && _selectedCard.State == CardState.Targeting)
        {
            if (_selectedCard.CardData.TargetType)
            {
                int count = 0;

                foreach (Unit unit in _playerUnits)
                {
                    if (unit == null) continue;

                    if (Vector3.Distance(unit.transform.position, _worldPos) <= 0.6f)
                    {
                        unit.SetHighlight(true);
                        _selectedUnit = unit;
                        count++;
                    }
                    else
                    {
                        unit.SetHighlight(false);
                    }
                }

                if (count == 0) _selectedUnit = null;
            }
            else
            {
                int count = 0;

                foreach (Unit unit in _enemyUnits)
                {
                    if (unit == null) continue;

                    if (Vector3.Distance(unit.transform.position, _worldPos) <= 0.6f)
                    {
                        unit.SetHighlight(true);
                        _selectedUnit = unit;
                        count++;
                    }
                    else
                    {
                        unit.SetHighlight(false);
                    }
                }

                if (count == 0) _selectedUnit = null;
            }
        }
    }

    private void LateUpdate()
    {
        MouseProcess();
    }

}
