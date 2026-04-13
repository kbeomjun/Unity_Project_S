using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CardManager : MonoBehaviour
{
    [SerializeField] private Card _cardPrefab;
    [SerializeField] private Transform _drawPileArea;
    [SerializeField] private Transform _handArea;
    [SerializeField] private Transform _discardPileArea;
    [SerializeField] private TargetArrow _targetArrow;
    [SerializeField] private RectTransform _canvasRect;

    private List<Card> _drawPileCards = new List<Card>();
    private List<Card> _handCards = new List<Card>();
    private List<Card> _discardPileCards = new List<Card>();

    private Vector2 _drawPileOffset = new Vector2(-50.0f, 0.0f);
    private Vector2 _discardPileOffset = new Vector2(50.0f, 0.0f);

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

    public void Init(List<CardData> playerCardDatas)
    {
        foreach (CardData cardData in playerCardDatas)
        {
            Card card = Instantiate(_cardPrefab);
            RectTransform rect = card.GetComponent<RectTransform>();

            rect.SetParent(_drawPileArea, false);
            rect.anchoredPosition = _drawPileOffset;

            card.Init(cardData);
            _drawPileCards.Add(card);
        }
    }

    public void DrawCard(int num)
    {
        for(int i = 0; i < num; i++)
        {
            Card card = _drawPileCards[0];
            RectTransform rect = card.GetComponent<RectTransform>();

            card.State = CardState.Idle;
            rect.SetParent(_handArea, false);
            rect.anchoredPosition = Vector2.zero;
            _handCards.Add(card);

            _drawPileCards.Remove(card);
        }
    }

    private void Arrange()
    {
        if (_handCards.Count <= 0) return;
        
        int count = _handCards.Count;
        float spacing = 150f; 
        float startX = -(count - 1) * 0.5f * spacing;

        for (int i = 0; i < count; i++)
        {
            Card card = _handCards[i];

            float x = startX + i * spacing;
            float y = 0f;

            Vector2 targetPos = new Vector2(x, y);

            card.OriginPosition = targetPos;
        }
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

            for (int i = _handCards.Count - 1; i >= 0; i--)
            {
                Card card = _handCards[i];

                if (RectTransformUtility.RectangleContainsScreenPoint(card.Rect, _screenMousePos, null))
                {
                    _selectedCard = card;
                    card.State = CardState.Hover;
                    count++;

                    for (int j = 0; j < _handCards.Count; j++)
                    {
                        if (j != i) _handCards[j].State = CardState.Idle;
                    }

                    break;
                }
            }

            if (count == 0)
            {
                foreach (var card in _handCards)
                {
                    card.State = CardState.Idle;
                }
            }
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

            Unit[] targetUnits = _selectedCard.CardData.TargetType ? BattleManager.Instance.PlayerUnits : BattleManager.Instance.EnemyUnits;

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
        Arrange();
    }

}