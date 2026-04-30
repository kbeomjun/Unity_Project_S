using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum TooltipType
{
    NextAction,
    Status,
    Card
}

public class Tooltip : MonoBehaviour
{
    [SerializeField] private TMP_Text _titleText;
    [SerializeField] private Image _icon;
    [SerializeField] private TMP_Text _descriptionText;

    public void Init(TooltipData data)
    {
        _titleText.text = data.Title;
        _descriptionText.text = data.Description;

        if (data.Icon != null)
        {
            _icon.gameObject.SetActive(true);
            _icon.sprite = data.Icon;
        }
        else
        {
            _icon.gameObject.SetActive(false);
        }
    }
}

public class TooltipData
{
    public TooltipType Type;
    public string Title;
    public string Description;
    public Sprite Icon;

    public TooltipData(TooltipType type, string title, string description, Sprite icon = null)
    {
        Type = type;
        Title = title;
        Description = description;
        Icon = icon;
    }
}

public static class TooltipUtility
{
    public static Vector2 GetCanvasPosition(RectTransform canvasRect, Vector3 worldPosition, Camera camera = null)
    {
        Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(camera, worldPosition);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPoint, null, out Vector2 localPoint);
        return localPoint;
    }
}
