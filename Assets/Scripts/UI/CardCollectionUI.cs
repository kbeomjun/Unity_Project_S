using System.Collections.Generic;
using UnityEngine;

public class CardCollectionUI : MonoBehaviour
{
    [SerializeField] private RectTransform _canvasRect;
    [SerializeField] private RectTransform _cardCollectionContent;
    [SerializeField] private RectTransform _cardHighlitedTr;
    [SerializeField] private GameObject _cardHighlitedRemoveButton;

    private List<Card> _cards = new List<Card>();
    private bool _isRemove = false;
    private Vector3 _originScale = new Vector3(1.0f, 1.0f, 0.0f);
    private float _hoverScale = 1.2f;
    private float _highlitedScale = 2.5f;
    private float _speed = 10.0f;

    private Card _selectedCard;
    private Vector2 _uiMousePos = Vector2.zero;

    public void Init(List<CardData> _datas, bool isRemove)
    {
        ClearCards();
        _isRemove = isRemove;

        int i = 0;
        foreach(CardData data in _datas)
        {
            Card card = Instantiate(DataManager.Instance.CardPrefab);
            RectTransform rect = card.Rect;

            rect.SetParent(_cardCollectionContent, false);
            card.Init(data);
            card.State = CardState.Stop;
            card.OriginIndex = i;

            _cards.Add(card);
            i++;
        }
    }

    public void OnClickCardCollectionPrevButton()
    {
        InputManager.Instance.Pop();
        ViewManager.Instance.Pop();
    }

    public void OnClickCardHighlitedPrevButton()
    {
        _selectedCard.Rect.SetParent(_cardCollectionContent);
        _selectedCard.Rect.SetSiblingIndex(_selectedCard.OriginIndex);
        _selectedCard.Rect.localScale = _originScale;

        InputManager.Instance.Pop();
        ViewManager.Instance.Pop();
    }

    public void OnClickCardHighlitedRemoveButton()
    {
        _cardHighlitedRemoveButton.SetActive(false);

        GameManager.Instance.RemoveCard(_selectedCard.OriginIndex);
        TownRestManager.Instance.OnRemoveCard();
        InputManager.Instance.Pop();
        InputManager.Instance.Pop();
        ViewManager.Instance.Pop();
        ViewManager.Instance.Pop();
    }

    private void ClearCards()
    {
        foreach(Card card in _cards)
            Destroy(card.gameObject);
        _cards.Clear();
    }

    public void OnClick()
    {
        if (_selectedCard == null) return;

        _selectedCard.Rect.SetParent(_cardHighlitedTr);
        _selectedCard.Rect.anchoredPosition = new Vector2(0.0f, -30.0f);
        _selectedCard.Rect.localScale = _originScale * _highlitedScale;
        if (_isRemove) _cardHighlitedRemoveButton.SetActive(true);

        InputManager.Instance.Push(InputState.None);
        ViewManager.Instance.ShowCardHighlitedPopup();
    }

    public void OnRelease()
    {
        if (_selectedCard == null) return;
    }

    public void MouseProcess(Vector2 mousePos)
    {
        // Screen ¡æ UI ÁÂÇ¥ º¯È¯ (ÇÙ½É)
        RectTransformUtility.ScreenPointToLocalPointInRectangle(_canvasRect, mousePos, null, out _uiMousePos);

        int count = 0;

        foreach (Card card in _cards)
        {
            if (RectTransformUtility.RectangleContainsScreenPoint(card.Rect, mousePos, null))
            {
                card.Rect.localScale = Vector3.Lerp(card.Rect.localScale, _originScale * _hoverScale, _speed * Time.deltaTime);
                _selectedCard = card;
                count++;
            }
            else
            {
                card.Rect.localScale = Vector3.Lerp(card.Rect.localScale, _originScale, _speed * Time.deltaTime);
            }
        }

        if (count == 0) _selectedCard = null;
    }

}
