using System.Collections.Generic;
using UnityEngine;

public class CardTooltipPanel : MonoBehaviour
{
    private RectTransform _rect;

    private void Awake()
    {
        _rect = GetComponent<RectTransform>();
        gameObject.SetActive(false);
    }

    public void Init(List<TooltipData> datas)
    {
        foreach (TooltipData data in datas)
        {
            Tooltip tooltip = Instantiate(DataManager.Instance.TooltipPrefab, _rect);
            tooltip.Init(data);
        }
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

}
