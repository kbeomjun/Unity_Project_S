using UnityEngine;
using UnityEngine.EventSystems;

public enum TownSelectButtonType
{
    Barrack,
    Shop,
    Inn
}

public class TownSelectButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private GameObject _cursors;
    [SerializeField] private TownSelectButtonType _type;

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
            case TownSelectButtonType.Barrack:
                TownManager.Instance.ShowBarrack();
                break;

            case TownSelectButtonType.Shop:
                TownManager.Instance.ShowShop();
                break;

            case TownSelectButtonType.Inn:
                break;
        }
    }

}
