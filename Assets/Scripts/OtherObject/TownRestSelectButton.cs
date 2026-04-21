using UnityEngine;
using UnityEngine.EventSystems;

public enum TownRestSelectButtonType
{
    Barrack,
    Shop,
    Inn,
    Meal,
    Cure
}

public class TownRestSelectButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private GameObject _cursors;
    [SerializeField] private TownRestSelectButtonType _type;

    private void Start()
    {
        Init();
    }

    public void Init()
    {
        _cursors.SetActive(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _cursors.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _cursors.SetActive(false);
    }

    public void onClick()
    {
        switch (_type)
        {
            case TownRestSelectButtonType.Barrack:
                TownRestManager.Instance.ShowBarrack();
                break;

            case TownRestSelectButtonType.Shop:
                TownRestManager.Instance.ShowShop();
                break;

            case TownRestSelectButtonType.Inn:
                break;

            case TownRestSelectButtonType.Meal:
                TownRestManager.Instance.Meal();
                break;

            case TownRestSelectButtonType.Cure:
                TownRestManager.Instance.Cure();
                break;
        }

        _cursors.SetActive(false);
    }

}
