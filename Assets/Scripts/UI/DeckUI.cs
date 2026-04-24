using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DeckUI : MonoBehaviour
{
    [SerializeField] private RectTransform _canvasRect;
    [SerializeField] private Transform _handArea;
    [SerializeField] private TMP_Text _drawPileText;
    [SerializeField] private TMP_Text _discardPileText;
    [SerializeField] private TargetArrow _targetArrow;

    private List<Card> _drawPileCards = new List<Card>();
    private List<Card> _handCards = new List<Card>();
    private List<Card> _discardPileCards = new List<Card>();

    private Vector2 _drawPileOffset = Vector2.zero;
    private Vector2 _discardPileOffset = Vector2.zero;

    private float _spacing = 150.0f;
    private float _startX = 0.0f;

    private bool _isDrawing = false;
    private bool _endTurnRequested = false;

    public bool IsDrawing => _isDrawing;
    public bool EndTurnRequested => _endTurnRequested;

    private Card _selectedCard = null;
    private Unit _selectedUnit = null;
    public Unit SelectedUnit => _selectedUnit;
    private Vector2 _uiMousePos = Vector2.zero;
    private bool _isDrag = false;
    private float _thresholdY = 0.0f;

    private void Awake()
    {
        _thresholdY = Screen.height * 0.4f;
        _drawPileOffset = new Vector2(-Screen.width, 0.0f);
        _discardPileOffset = new Vector2(Screen.width, 0.0f);
    }

    public void Init(List<CardData> playerCardDatas)
    {
        ClearCards();

        foreach (CardData cardData in playerCardDatas)
        {
            Card card = Instantiate(DataManager.Instance.CardPrefab);
            RectTransform rect = card.Rect;

            rect.SetParent(_handArea, false);
            rect.anchoredPosition = _drawPileOffset;

            card.Init(cardData);
            _discardPileCards.Add(card);
        }

        StartCoroutine(Shuffle());
    }

    public IEnumerator DrawCards(int num)
    {
        _isDrawing = true;

        for (int i = 0; i < num; i++)
        {
            yield return StartCoroutine(DrawCardRoutine());
            yield return new WaitForSeconds(0.1f);
        }

        _isDrawing = false;

        if (_endTurnRequested)
        {
            _endTurnRequested = false;
            BattleManager.Instance.EndPlayerTurn();
        }
    }

    public void RequestEndTurn()
    {
        _endTurnRequested = true;
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
            yield return new WaitForSeconds(0.05f);
        }
        yield return new WaitForSeconds(0.1f);
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

    public void OnClickDrawPileCardButton()
    {
        if (ViewManager.Instance.ShowCardCollectionPopup())
        {
            List<CardData> list = new List<CardData>();
            foreach (Card card in _drawPileCards)
                list.Add(card.CardData);
            GameManager.Instance.CardCollectionUI.Init(list, false);
            InputManager.Instance.Push(InputState.CardCollection);
        }
    }

    public void OnClickDiscardPileCardButton()
    {
        if (ViewManager.Instance.ShowCardCollectionPopup())
        {
            List<CardData> list = new List<CardData>();
            foreach (Card card in _discardPileCards)
                list.Add(card.CardData);
            GameManager.Instance.CardCollectionUI.Init(list, false);
            InputManager.Instance.Push(InputState.CardCollection);
        }
    }

    private void ClearCards()
    {
        foreach (Card card in _drawPileCards)
            Destroy(card.gameObject);
        foreach (Card card in _handCards)
            Destroy(card.gameObject);
        foreach (Card card in _discardPileCards)
            Destroy(card.gameObject);

        _drawPileCards.Clear();
        _handCards.Clear();
        _discardPileCards.Clear();
    }

    public void OnClick()
    {
        if (_selectedCard == null) return;

        _isDrag = true;
        _selectedCard.State = CardState.Selected;
    }

    public void OnRelease(Vector2 mousePos)
    {
        if (_selectedCard == null) return;

        if (_selectedCard.State == CardState.Selected && mousePos.y >= _thresholdY && _selectedCard.CardData.TargetType == TargetType.None)
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
            }
            else
            {
                _selectedCard.State = CardState.Idle;
                _selectedCard.transform.SetSiblingIndex(_selectedCard.OriginIndex);
            }
        }
        else if (_selectedCard.State == CardState.Targeting && _selectedUnit != null && mousePos.y >= _thresholdY)
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

    public void MouseProcess(Vector2 mousePos, Vector3 worldPos)
    {
        // Screen ¡æ UI ÁÂÇ¥ º¯È¯ (ÇÙ½É)
        RectTransformUtility.ScreenPointToLocalPointInRectangle(_canvasRect, mousePos, null, out _uiMousePos);

        if (!_isDrag)
        {
            int count = 0;

            for (int i = _handCards.Count - 1; i >= 0; i--)
            {
                Card card = _handCards[i];

                if (RectTransformUtility.RectangleContainsScreenPoint(card.Rect, mousePos, null))
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

            if (mousePos.y >= _thresholdY)
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

                if (Vector3.Distance(unit.transform.position, worldPos) <= 0.6f)
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

    private void UpdatePileText()
    {
        _drawPileText.text = _drawPileCards.Count.ToString();
        _discardPileText.text = _discardPileCards.Count.ToString();
    }

    private void LateUpdate()
    {
        Arrange();
        UpdatePileText();
    }

}
