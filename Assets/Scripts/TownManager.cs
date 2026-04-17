using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;

public class TownManager : MonoBehaviour
{
    [SerializeField] private RectTransform[] _cardPositions;
    [SerializeField] private TMP_Text[] _cardCosts;

    private Card[] _cards;
    private int _cardCount = 5;

    public static TownManager Instance { get; private set; }
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(this);

        _cards = new Card[_cardCount];
    }

    public void StartTown()
    {
        ClearCards();
        CreateCards();
    }

    private void CreateCards()
    {
        HashSet<int> set = new HashSet<int>();
        while (set.Count < _cardCount)
        {
            int index = Random.Range(0, DataManager.Instance.CardDatas.Length);
            set.Add(index);
        }

        int i = 0;
        foreach(int index in set)
        {
            Card card = Instantiate(DataManager.Instance.CardPrefab);
            RectTransform rect = card.Rect;

            rect.anchoredPosition = Vector2.zero;
            rect.SetParent(_cardPositions[i], false);

            card.Init(DataManager.Instance.CardDatas[i]);
            card.State = CardState.Idle;



            _cards[i] = card;
            i++;
        }
    }

    private void ClearCards()
    {
        foreach(Card card in _cards)
        {
            if (card == null) continue;
            Destroy(card.gameObject);
        }
    }

}
