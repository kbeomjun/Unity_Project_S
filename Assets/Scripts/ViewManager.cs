using UnityEngine;

public class ViewManager : MonoBehaviour
{
    [SerializeField] private GameObject _battleRewardPopup;
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
        _battleRewardPopup.SetActive(true);
    }

    public void ShowGameOverPopup()
    {
        //_gameOverPopup.SetActive(true);
    }

}
