using System.Collections.Generic;
using UnityEngine;

public class UITooltipPanel : MonoBehaviour
{
    private RectTransform _rect;
    private RectTransform _followTarget;

    private void Awake()
    {
        _rect = GetComponent<RectTransform>();
        gameObject.SetActive(false);
    }

    public void Init(List<TooltipData> datas)
    {
        foreach (TooltipData data in datas)
        {
            Tooltip tooltip = Instantiate(DataManager.Instance.UITooltipPrefab, _rect);
            tooltip.Init(data);
        }
    }

    public void Show(RectTransform followTarget)
    {
        _followTarget = followTarget;
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        _followTarget = null;
        gameObject.SetActive(false);
    }

    private void LateUpdate()
    {
        if (_followTarget == null) return;

        // 위치/회전 복사
        _rect.position = _followTarget.position;
        _rect.rotation = _followTarget.rotation;

        // scale 복사 (부모 scale 보정)
        Vector3 scale = _followTarget.lossyScale;
        Vector3 parentScale = transform.parent.lossyScale;

        _rect.localScale = new Vector3(scale.x / parentScale.x, scale.y / parentScale.y, 1.0f);
    }

}
