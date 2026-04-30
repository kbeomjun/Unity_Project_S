using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TooltipPanel : MonoBehaviour
{
    [SerializeField] private RectTransform _canvasRect;
    public RectTransform CanvasRect => _canvasRect;

    private RectTransform _rect;
    private List<Tooltip> _tooltips = new List<Tooltip>();

    public static TooltipPanel Instance { get; private set; }
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        _rect = GetComponent<RectTransform>();
        gameObject.SetActive(false);
    }

    public void Show(Vector2 localPosition, List<TooltipData> datas)
    {
        Clear();

        foreach (TooltipData data in datas)
        {
            Tooltip tooltip = Instantiate(DataManager.Instance.TooltipPrefab, _rect);
            tooltip.Init(data);
            _tooltips.Add(tooltip);
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(_rect);

        _rect.anchoredPosition = localPosition;

        ClampToScreen();

        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    private void ClampToScreen()
    {
        Vector2 pos = _rect.anchoredPosition;
        Vector2 size = _rect.rect.size;

        float canvasWidth = _canvasRect.rect.width;
        float canvasHeight = _canvasRect.rect.height;

        float halfWidth = size.x * 0.5f;
        float halfHeight = size.y * 0.5f;

        pos.x = Mathf.Clamp(pos.x, -canvasWidth / 2 + halfWidth, canvasWidth / 2 - halfWidth);
        pos.y = Mathf.Clamp(pos.y, -canvasHeight / 2 + halfHeight, canvasHeight / 2 - halfHeight);

        _rect.anchoredPosition = pos;
    }

    private void Clear()
    {
        foreach (Tooltip tooltip in _tooltips)
            Destroy(tooltip.gameObject);
        _tooltips.Clear();
    }

}
