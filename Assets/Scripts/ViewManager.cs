using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ViewManager : MonoBehaviour
{
    [SerializeField] private Image _fadeImage;
    [SerializeField] private RectTransform _mapViewTr;
    [SerializeField] private RectTransform _mapPopupTr;
    [SerializeField] private RectTransform _mapScrollView;

    [SerializeField] private UIView _mapView;
    [SerializeField] private UIView _startView;
    [SerializeField] private UIView _battleView;
    [SerializeField] private UIView _townView;
    [SerializeField] private UIView _restView;
    [SerializeField] private UIView _eventView;

    [SerializeField] private GameObject _dimBackGround;
    [SerializeField] private UIPopup _mapPopup;
    [SerializeField] private UIPopup _rewardPopup;
    [SerializeField] private UIPopup _rewardCardPopup;
    [SerializeField] private UIPopup _gameOverPopup;
    [SerializeField] private UIPopup _shopPopup;
    [SerializeField] private UIPopup _unitCollectionPopup;
    [SerializeField] private UIPopup _cardCollectionPopup;
    [SerializeField] private UIPopup _cardHighlitedPopup;

    private UIView _currentView = null;
    private Stack<UIPopup> _popupStack = new Stack<UIPopup>();

    public static ViewManager Instance { get; private set; }
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        Color color = _fadeImage.color;
        color.a = 0.0f;
        _fadeImage.color = color;
        _fadeImage.gameObject.SetActive(false);
    }

    public IEnumerator ChangeView(UIView nextView)
    {
        _fadeImage.gameObject.SetActive(true);
        yield return _fadeImage.DOFade(1f, 0.25f).WaitForCompletion();
        ShowView(nextView);
        yield return _fadeImage.DOFade(0f, 0.25f).WaitForCompletion();
        _fadeImage.gameObject.SetActive(false);
    }

    public void ShowView(UIView view)
    {
        if (_currentView != null)
        {
            _currentView.Hide();
        }

        _currentView = view;
        _currentView.Show();
    }

    public void ShowMapView()
    {
        _mapScrollView.SetParent(_mapViewTr, false);
        StartCoroutine(ChangeView(_mapView));

        if (_popupStack.Count > 0)
        {
            if (_popupStack.Peek() == _mapPopup)
                Pop();
        }
    }
    
    public void ShowStartView()
    {
        StartCoroutine(ChangeView(_startView));
    }

    public void ShowBattleView()
    {
        StartCoroutine(ChangeView(_battleView));
    }

    public void ShowTownView()
    {
        StartCoroutine(ChangeView(_townView));
    }

    public void ShowRestView()
    {
        StartCoroutine(ChangeView(_restView));
    }

    public void ShowEventView()
    {
        StartCoroutine(ChangeView(_eventView));
    }

    public void Push(UIPopup popup)
    {
        if (_popupStack.Count > 0)
        {
            _popupStack.Peek().Hide(); 
        }

        _popupStack.Push(popup);
        popup.Show();

        _dimBackGround.SetActive(true);
    }

    public void Pop()
    {
        if (_popupStack.Count == 0) return;

        UIPopup top = _popupStack.Pop();
        top.Hide();

        if (_popupStack.Count > 0)
        {
            _popupStack.Peek().Show();
        }
        else
        {
            _dimBackGround.SetActive(false);
        }
    }

    public bool ShowMapPopup()
    {
        if (_currentView == _mapView) return false;

        if (_popupStack.Count > 0)
        {
            if (_popupStack.Peek() == _mapPopup)
                return false;
        }

        _mapScrollView.SetParent(_mapPopupTr, false);
        Push(_mapPopup);
        return true;
    }

    public void ShowRewardPopup()
    {
        Push(_rewardPopup);
    }

    public void ShowRewardCardPopup()
    {
        Push(_rewardCardPopup);
    }

    public void ShowGameOverPopup() 
    {
        Push(_gameOverPopup);
    }

    public void ShowShopPopup()
    {
        Push(_shopPopup);
    }

    public bool ShowUnitCollectionPopup()
    {
        if (_popupStack.Count > 0)
        {
            if (_popupStack.Peek() == _unitCollectionPopup)
                return false;
        }

        Push(_unitCollectionPopup);
        return true;
    }

    public bool ShowCardCollectionPopup()
    {
        if (_popupStack.Count > 0)
        {
            if (_popupStack.Peek() == _cardCollectionPopup ||
                _popupStack.Peek() == _cardHighlitedPopup)
                return false;
        }

        Push(_cardCollectionPopup);
        return true;
    }

    public void ShowCardHighlitedPopup()
    {
        Push(_cardHighlitedPopup);
    }

}
