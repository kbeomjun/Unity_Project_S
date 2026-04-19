using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RewardManager : MonoBehaviour
{
    [SerializeField] private RectTransform _canvasRect;
    [SerializeField] private RectTransform _rewardContentTr;
    [SerializeField] private VerticalLayoutGroup _layoutGroup;
    [SerializeField] private RewardItem _rewardItemPrefab;
    [SerializeField] private RectTransform _cardRewardsTr;

    private List<RewardItem> _rewardItems = new List<RewardItem>();
    private List<List<CardData>> _rewardCardDatasList = new List<List<CardData>>();
    private List<Card> _rewardCards = new List<Card>();

    private Card _selectedCard = null;
    private RewardItem _selectedRewardCard = null;
    Vector2 _uiMousePos = Vector2.zero;

    public static RewardManager Instance { get; private set; }
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void ShowRewards(NodeType nodeType)
    {
        ClearRewardItems();
        CreateRewards(nodeType);
        ViewManager.Instance.ShowRewardPopup();
    }

    private void CreateRewards(NodeType nodeType)
    {
        RewardItem coin = null;
        RewardItem unit = null;
        RewardItem card = null;
        RewardItem item = null;
        int coinValue = 0;
        int unitValue = 0;
        int cardValue = 3;
        int cardCount = 0;

        switch (nodeType)
        {
            case NodeType.Battle:
                coinValue = Random.Range(30, 34);
                coin = Instantiate(_rewardItemPrefab, _rewardContentTr);
                coin.Init(RewardItemType.Coin, coinValue, cardCount);

                unitValue = Random.Range(0, DataManager.Instance.UnitData.Length);
                unit = Instantiate(_rewardItemPrefab, _rewardContentTr);
                unit.Init(RewardItemType.Unit, unitValue, cardCount);

                card = Instantiate(_rewardItemPrefab, _rewardContentTr);
                card.Init(RewardItemType.Card, cardValue, cardCount++);
                break;

            case NodeType.Elite:
                break;

            case NodeType.Boss:
                break;
        }

        _rewardItems.Add(coin);
        _rewardItems.Add(unit);
        _rewardItems.Add(card);
        if (item != null) _rewardItems.Add(item);

        for(int i = 0; i < cardCount; i++)
        {
            _rewardCardDatasList.Add(new List<CardData>());
        }
    }

    public void ShowCardRewards(RewardItem rewardCard)
    {
        ClearRewardCards();
        _selectedRewardCard = rewardCard;
        int cardNum = rewardCard.Value;

        if (_rewardCardDatasList[rewardCard.Index].Count == 0)
        {
            HashSet<int> set = new HashSet<int>();
            while (set.Count < cardNum)
            {
                int index = Random.Range(0, DataManager.Instance.CardDatas.Length);
                set.Add(index);
            }

            List<CardData> cardDatas = new List<CardData>();
            foreach (int index in set)
            {
                cardDatas.Add(DataManager.Instance.CardDatas[index]);
            }

            _rewardCardDatasList[rewardCard.Index] = cardDatas;
        }

        float spacing = 500.0f; // 카드 간격
        float startX = -(cardNum - 1) * spacing * 0.5f;
        int i = 0;

        List<Card> rewardCards = new List<Card>();

        foreach (CardData data in _rewardCardDatasList[rewardCard.Index])
        {
            Card card = Instantiate(DataManager.Instance.CardPrefab);
            RectTransform rect = card.Rect;

            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = new Vector2(startX + i * spacing, 0);
            rect.SetParent(_cardRewardsTr, false);

            card.Init(data);
            card.State = CardState.Idle;
            card.OriginScale = new Vector3(1.5f, 1.5f, 1.5f);

            rewardCards.Add(card);
            i++;
        }

        _rewardCards = rewardCards;

        InputManager.Instance.Push(InputState.RewardCard);
        ViewManager.Instance.ShowRewardCardPopup();
    }

    public void RemoveItem(RewardItem item)
    {
        StartCoroutine(SmoothRemove(item));
    }

    private IEnumerator SmoothRemove(RewardItem item)
    {
        // 현재 아이템들 가져오기
        List<RectTransform> rects = new List<RectTransform>();
        foreach (RewardItem it in _rewardItems)
        {
            rects.Add(it.Rect);
        }

        // 기존 위치 저장
        Vector2[] oldPos = new Vector2[rects.Count];
        for (int i = 0; i < rects.Count; i++)
        {
            oldPos[i] = rects[i].anchoredPosition;
        }

        // 삭제
        int removeIndex = _rewardItems.IndexOf(item);

        if (removeIndex < 0) yield break;

        _rewardItems.RemoveAt(removeIndex);
        rects.RemoveAt(removeIndex);

        Destroy(item.gameObject);

        yield return null;

        // Layout 강제 갱신 → 목표 위치 계산
        LayoutRebuilder.ForceRebuildLayoutImmediate(_rewardContentTr);

        Vector2[] targetPos = new Vector2[rects.Count];
        for (int i = 0; i < rects.Count; i++)
        {
            targetPos[i] = rects[i].anchoredPosition;
        }

        // Layout 끄기
        _layoutGroup.enabled = false;

        // 위치를 다시 원래 위치로 되돌림
        for (int i = 0; i < rects.Count; i++)
        {
            rects[i].anchoredPosition = oldPos[i < removeIndex ? i : i + 1];
        }

        // 부드럽게 이동
        float duration = 0.25f;
        float time = 0;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = Mathf.SmoothStep(0, 1, time / duration);

            for (int i = 0; i < rects.Count; i++)
            {
                rects[i].anchoredPosition = Vector2.Lerp(oldPos[i < removeIndex ? i : i + 1], targetPos[i], t);
            }

            yield return null;
        }

        // Layout 다시 켜기
        _layoutGroup.enabled = true;
    }

    public void OnClickRewardNextButton()
    {
        _selectedCard = null;
        _selectedRewardCard = null;

        foreach (RewardItem item in _rewardItems)
            Destroy(item.gameObject);
        _rewardItems.Clear();
        _rewardCardDatasList.Clear();
        ClearRewardCards();

        InputManager.Instance.Pop();
        ViewManager.Instance.Pop();
    }

    public void OnClickRewardCardPrevButton()
    {
        _selectedRewardCard = null;
        ClearRewardCards();

        InputManager.Instance.Pop();
        ViewManager.Instance.Pop();
    }

    private void ClearRewardItems()
    {
        foreach (RewardItem item in _rewardItems)
            Destroy(item.gameObject);
        _rewardItems.Clear();
    }

    private void ClearRewardCards()
    {
        foreach (Card card in _rewardCards)
            Destroy(card.gameObject);
        _rewardCards.Clear();
    }

    public void OnClick()
    {
        if (_selectedCard == null) return;

        _selectedCard.State = CardState.Add;
        _selectedCard.OriginPosition = _selectedCard.AddPosition;
        _selectedCard.Rect.SetParent(_canvasRect);
        GameManager.Instance.AddCard(_selectedCard.CardData);
        RemoveItem(_selectedRewardCard);
        _selectedRewardCard = null;
        ClearRewardCards();

        InputManager.Instance.Pop();
        ViewManager.Instance.Pop();
    }

    public void OnRelease()
    {
        if (_selectedCard == null) return;
    }

    public void MouseProcess(Vector2 mousePos)
    {
        // Screen → UI 좌표 변환 (핵심)
        RectTransformUtility.ScreenPointToLocalPointInRectangle(_canvasRect, mousePos, null, out _uiMousePos);

        int count = 0;

        for (int i = 0; i < _rewardCards.Count; i++)
        {
            Card card = _rewardCards[i];

            if (RectTransformUtility.RectangleContainsScreenPoint(card.Rect, mousePos, null))
            {
                _selectedCard = card;
                card.State = CardState.Hover;
                count++;
            }
            else
            {
                card.State = CardState.Idle;
            }
        }

        if(count == 0) _selectedCard = null;
    }

}
