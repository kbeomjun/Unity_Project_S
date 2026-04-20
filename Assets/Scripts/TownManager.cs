using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TownManager : MonoBehaviour
{
    [SerializeField] private RectTransform _canvasRect;
    [SerializeField] private RectTransform[] _cardPositions;
    [SerializeField] private GameObject[] _cardCoins;
    [SerializeField] private TMP_Text[] _cardCoinTexts;
    [SerializeField] private Button _cardRemoveButton;
    [SerializeField] private GameObject _cardRemoveCoin;
    [SerializeField] private TMP_Text _cardRemoveCoinText;

    private Card[] _cards;
    private int _cardCount = 5;
    private Vector3 _originScale = new Vector3(1.0f, 1.0f, 0.0f);
    private float _hoverScale = 1.2f;
    private float _speed = 10.0f;
    public int CardDeleteCoin { get; set; }

    private Card _selectedCard = null;
    private Vector2 _uiMousePos = Vector2.zero;

    public static TownManager Instance { get; private set; }
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(this);

        _cards = new Card[_cardCount];
    }

    public void StartTown()
    {
        CreateCards();
    }

    private void CreateCards()
    {
        ClearCards();
        _cardRemoveButton.enabled = true;

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
            card.Init(DataManager.Instance.CardDatas[index]);
            card.State = CardState.Idle;

            _cards[i] = card;
            _cardCoins[i].SetActive(true);
            _cardCoinTexts[i].text = card.CardData.Coin.ToString();
            i++;
        }

        _cardRemoveCoin.SetActive(true);
        _cardRemoveCoinText.text = CardDeleteCoin.ToString();
    }

    public void ShowBarrack()
    {
        GameManager.Instance.OnClickUnitCollectionButton(true);
    }

    public void ShowShop()
    {
        InputManager.Instance.Push(InputState.Shop);
        ViewManager.Instance.ShowShopPopup();
    }

    public void OnRemoveCard()
    {
        _cardRemoveButton.enabled = false;
        _cardRemoveCoin.SetActive(false);
        CardDeleteCoin += 50;
    }

    public void OnClickTownNextButton()
    {
        GameManager.Instance.StartBattle();
    }

    public void OnClickCardRemoveButton()
    {
        if (CardDeleteCoin <= GameManager.Instance.CurrentCoin)
            GameManager.Instance.OnClickCardCollectionButton(true);
    }

    public void OnClickShopPrevButton()
    {
        InputManager.Instance.Pop();
        ViewManager.Instance.Pop();
    }

    private void ClearCards()
    {
        for(int i = 0; i < _cards.Length; i++)
        {
            if (_cards[i] != null) Destroy(_cards[i].gameObject);
            _cardCoins[i].SetActive(false);
        }
    }

    public void OnClick()
    {
        if (_selectedCard == null) return;

        if (_selectedCard.CardData.Coin <= GameManager.Instance.CurrentCoin)
        {
            _selectedCard.State = CardState.Add;
            _selectedCard.OriginPosition = _selectedCard.AddPosition;
            _selectedCard.Rect.SetParent(_canvasRect);

            int index = -1;
            for (int i = 0; i < _cards.Length; i++)
            {
                if (_cards[i] == null) continue;

                if (_cards[i] == _selectedCard)
                {
                    index = i;
                    break;
                }
            }
            _cardCoins[index].SetActive(false);

            GameManager.Instance.CurrentCoin -= _selectedCard.CardData.Coin;
            GameManager.Instance.AddCard(_selectedCard.CardData);
        }

        _selectedCard = null;
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

        for (int i = 0; i < _cardCount; i++)
        {
            Card card = _cards[i];

            if (card == null) continue;

            if (RectTransformUtility.RectangleContainsScreenPoint(card.Rect, mousePos, null))
            {
                _selectedCard = card;
                _cardPositions[i].localScale = Vector3.Lerp(_cardPositions[i].localScale, _originScale * _hoverScale, _speed * Time.deltaTime);
                count++;
            }
            else
            {
                _cardPositions[i].localScale = Vector3.Lerp(_cardPositions[i].localScale, _originScale, _speed * Time.deltaTime);
            }
        }

        if (count == 0) _selectedCard = null;
    }

}
