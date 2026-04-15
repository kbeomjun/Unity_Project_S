using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class CardManager : MonoBehaviour
{
    [SerializeField] private Transform _handArea;
    [SerializeField] private TMP_Text _drawPileText;
    [SerializeField] private TMP_Text _discardPileText;
    [SerializeField] private RectTransform _canvasRect;
    [SerializeField] private TargetArrow _targetArrow;

    private List<Card> _drawPileCards = new List<Card>();
    private List<Card> _handCards = new List<Card>();
    private List<Card> _discardPileCards = new List<Card>();

    private Vector2 _drawPileOffset = Vector2.zero;
    private Vector2 _discardPileOffset = Vector2.zero;

    private float _spacing = 150.0f;
    private float _startX = 0.0f;

    private PlayerInputActions _input;
    private Card _selectedCard = null;
    private Unit _selectedUnit = null;
    public Unit SelectedUnit => _selectedUnit;

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

        _drawPileOffset = new Vector2(-Screen.width, 0.0f);
        _discardPileOffset = new Vector2(Screen.width, 0.0f);
    }

    public void Init(List<CardData> playerCardDatas)
    {
        foreach (CardData cardData in playerCardDatas)
        {
            Card card = Instantiate(DataManager.Instance.CardPrefab);
            RectTransform rect = card.GetComponent<RectTransform>();

            rect.SetParent(_handArea, false);
            rect.anchoredPosition = _drawPileOffset;

            card.Init(cardData);
            _discardPileCards.Add(card);
        }

        StartCoroutine(Shuffle());
        SetPileText();
    }

    public IEnumerator DrawCardRoutine()
    {
        if (_drawPileCards.Count <= 0)
        {
            if (_discardPileCards.Count <= 0)
                yield break;

            yield return StartCoroutine(Shuffle());
        }

        Card card = _drawPileCards[0];
        card.State = CardState.Idle;
        card.transform.SetAsLastSibling();
        _handCards.Add(card);
        _drawPileCards.Remove(card);

        SetPileText();
    }

    private IEnumerator Shuffle()
    {
        while(_discardPileCards.Count > 0)
        {
            int index = Random.Range(0, _discardPileCards.Count);
            Card card = _discardPileCards[index];
            card.State = CardState.Draw;
            card.OriginPosition = _drawPileOffset;
            _drawPileCards.Add(card);
            _discardPileCards.Remove(card);

            yield return new WaitForSeconds(0.1f);
        }

        SetPileText();
    }

    public void DiscardHandCards()
    {
        foreach(Card card in _handCards)
        {
            card.State = CardState.Discard;
            card.OriginPosition = _discardPileOffset;
            _discardPileCards.Add(card);
        }

        _handCards.Clear();

        SetPileText();
    }

    private void Arrange()
    {
        int count = _handCards.Count;
        if (count <= 0) return;
        
        _startX = -(count - 1) * 0.5f * _spacing;

        for (int i = 0; i < count; i++)
        {
            Card card = _handCards[i];
            card.OriginIndex = i;
            float x = _startX + i * _spacing;
            card.OriginPosition = new Vector2(x, 0.0f);
        }
    }

    private void SetPileText()
    {
        _drawPileText.text = _drawPileCards.Count.ToString();
        _discardPileText.text = _discardPileCards.Count.ToString();
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

        if (_selectedCard.State == CardState.Selected && _screenMousePos.y >= _thresholdY && _selectedCard.CardData.TargetType == TargetType.None)
        {
            bool flag = BattleManager.Instance.UseCard(_selectedCard.CardData.Cost);
            
            if (flag)
            {
                Debug.Log("NonTargeting Card Used");
                _selectedCard.Use();
                _selectedCard.State = CardState.Discard;
                _selectedCard.OriginPosition = _discardPileOffset;
                _discardPileCards.Add(_selectedCard);
                _handCards.Remove(_selectedCard);
                SetPileText();
            }
            else
            {
                _selectedCard.State = CardState.Idle;
                _selectedCard.transform.SetSiblingIndex(_selectedCard.OriginIndex);
            }
        }
        else if (_selectedCard.State == CardState.Targeting && _selectedUnit != null && _screenMousePos.y >= _thresholdY)
        {
            bool flag = BattleManager.Instance.UseCard(_selectedCard.CardData.Cost);

            if (flag)
            {
                Debug.Log("Targeting Card Used");
                _selectedCard.Use();
                _selectedCard.State = CardState.Discard;
                _selectedCard.OriginPosition = _discardPileOffset;
                _discardPileCards.Add(_selectedCard);
                _handCards.Remove(_selectedCard);
                SetPileText();
            }
            else
            {
                _selectedCard.State = CardState.Idle;
                _selectedCard.transform.SetSiblingIndex(_selectedCard.OriginIndex);
            }
        }
        else
        {
            _selectedCard.State = CardState.Idle;
            _selectedCard.transform.SetSiblingIndex(_selectedCard.OriginIndex);
        }

        _isDrag = false;
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
                    card.transform.SetAsLastSibling();
                    count++;

                    for (int j = 0; j < _handCards.Count; j++)
                    {
                        if (j != i) 
                        {
                            _handCards[j].State = CardState.Idle;
                            _handCards[j].transform.SetSiblingIndex(_handCards[j].OriginIndex);
                        } 
                    }

                    break;
                }
            }

            if (count == 0)
            {
                _selectedCard = null;

                foreach (Card card in _handCards)
                {
                    card.State = CardState.Idle;
                    card.transform.SetSiblingIndex(card.OriginIndex);
                }
            }
        }

        if (_isDrag && _selectedCard != null)
        {
            if (_selectedCard.State != CardState.Targeting)
            {
                _selectedCard.State = CardState.Selected;
                _selectedCard.transform.SetAsLastSibling();
                _selectedCard.Rect.anchoredPosition = _uiMousePos + new Vector2(0.0f, _selectedCard.Height * 1.5f);
            }

            if (_screenMousePos.y >= _thresholdY)
            {
                if (_selectedCard.CardData.TargetType != TargetType.None)
                {
                    _selectedCard.State = CardState.Targeting;
                    _selectedCard.transform.SetAsLastSibling();
                }
            }
        }

        if (_isDrag && _selectedCard != null && _selectedCard.State == CardState.Targeting)
        {
            _targetArrow.Show();
            _targetArrow.UpdateArrow(_selectedCard.Rect.anchoredPosition + new Vector2(0.0f, _selectedCard.Height * 0.4f), 
                                        _uiMousePos + new Vector2(0.0f, _selectedCard.Height * 1.5f));

            Unit[] targetUnits = null;

            if (_selectedCard.CardData.TargetType == TargetType.Ally)
            {
                targetUnits = BattleManager.Instance.PlayerUnits;
            }
            else if(_selectedCard.CardData.TargetType == TargetType.Enemy)
            {
                targetUnits = BattleManager.Instance.EnemyUnits;
            }

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
