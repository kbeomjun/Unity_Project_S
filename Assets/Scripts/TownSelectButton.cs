using UnityEngine;
using UnityEngine.EventSystems;

public class TownSelectButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private GameObject _cursors;

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

}
