using UnityEngine;

public class ViewManager : MonoBehaviour
{
    [SerializeField] private GameObject _rewardPopup;
    [SerializeField] private GameObject _rewardCardPopup;
    [SerializeField] private GameObject _gameOverPopup;

    public static ViewManager Instance { get; private set; }
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
    }

    public void ShowRewardPopup()
    {
        _rewardPopup.SetActive(true);
    }

    public void UnShowRewardPopup()
    {
        _rewardPopup.SetActive(false);
    }

    public void ShowRewardCardPopup()
    {
        _rewardCardPopup.SetActive(true);
    }

    public void UnShowRewardCardPopup()
    {
        _rewardCardPopup.SetActive(false);
    }

    public void ShowGameOverPopup()
    {
        //_gameOverPopup.SetActive(true);
    }

}
