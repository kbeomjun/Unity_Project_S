using System.Collections.Generic;
using UnityEngine;

public class ViewManager : MonoBehaviour
{
    [SerializeField] private RectTransform _mapViewTr;
    [SerializeField] private RectTransform _mapPopupTr;
    [SerializeField] private RectTransform _mapScrollView;

    [SerializeField] private UIView _mapView;
    [SerializeField] private UIView _battleView;
    [SerializeField] private UIView _townView;
    [SerializeField] private UIView _restView;

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
        ShowView(_mapView);
    }

    public void ShowBattleView()
    {
        ShowView(_battleView);
    }

    public void ShowTownView()
    {
        ShowView(_townView);
    }

    public void ShowRestView()
    {
        ShowView(_restView);
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
