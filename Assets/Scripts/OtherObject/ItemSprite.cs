using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ItemSprite : MonoBehaviour
{
    private Image _icon;
    private RectTransform _rect;

    private Vector2 _addUnitPosition = new Vector2(750.0f, 500.0f);
    private Vector3 _originScale;
    private float _highliteScale = 1.2f;
    private float _addScale = 0.2f;

    public void Init(int itemType, int unitType)
    {
        _rect = GetComponent<RectTransform>();
        _icon = GetComponent<Image>();

        if (itemType == 1)
        {
            _icon.sprite = DataManager.Instance.UnitSprites[unitType];
        }
        _originScale = _rect.localScale;
    }

    public void PlayRecruitAnimation(int itemType)
    {
        Vector2 addPosition = Vector2.zero;
        switch (itemType)
        {
            case 1:
                addPosition = _addUnitPosition;
                break;
        }

        _rect.DOKill();

        Sequence seq = DOTween.Sequence();

        seq.Append(_rect.DOAnchorPos(_rect.anchoredPosition, 0.5f).SetEase(Ease.OutQuad));
        seq.Join(_rect.DOScale(_originScale * _highliteScale, 0.5f));

        seq.Append(_rect.DOAnchorPos(addPosition, 1.0f).SetEase(Ease.InQuad));
        seq.Join(_rect.DOScale(_originScale * _addScale, 1.0f));
        seq.Join(_rect.DORotate(new Vector3(0, 0, 360), 1.0f));

        seq.OnComplete(() =>
        {
            Destroy(gameObject);
        });
    }

}
