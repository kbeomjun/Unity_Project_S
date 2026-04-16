using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;

public class RewardManager : MonoBehaviour
{
    [SerializeField] private RectTransform _rewardContent;
    [SerializeField] private VerticalLayoutGroup _layoutGroup;
    [SerializeField] private RewardItem _rewardItem;

    private List<RewardItem> _rewardItems = new List<RewardItem>();

    public static RewardManager Instance { get; private set; }
    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ShowRewards(NodeType nodeType)
    {
        ClearRewardItems();
        CreateRewards(nodeType);
        ViewManager.Instance.ShowRewardPopup();
    }

    private void CreateRewards(NodeType nodeType)
    {
        RewardItem coin = null;
        RewardItem unit = null;
        RewardItem card = null;
        RewardItem item = null;

        switch (nodeType)
        {
            case NodeType.Battle:
                coin = Instantiate(_rewardItem, _rewardContent);
                coin.Init(RewardItemType.Coin, 100);

                unit = Instantiate(_rewardItem, _rewardContent);
                unit.Init(RewardItemType.Unit, 1);

                card = Instantiate(_rewardItem, _rewardContent);
                card.Init(RewardItemType.Card, 3);
                break;

            case NodeType.Elite:
                break;

            case NodeType.Boss:
                break;
        }

        _rewardItems.Add(coin);
        _rewardItems.Add(unit);
        _rewardItems.Add(card);
        if (item != null) _rewardItems.Add(item);
    }

    private void ClearRewardItems()
    {
        foreach (RewardItem item in _rewardItems)
            Destroy(item);

        _rewardItems.Clear();
    }

    public void RemoveItem(RewardItem item)
    {
        StartCoroutine(SmoothRemove(item));
    }

    private IEnumerator SmoothRemove(RewardItem item)
    {
        // 현재 아이템들 가져오기
        List<RectTransform> rects = new List<RectTransform>();
        foreach (var it in _rewardItems)
        {
            rects.Add(it.GetComponent<RectTransform>());
        }

        // 기존 위치 저장
        Vector2[] oldPos = new Vector2[rects.Count];
        for (int i = 0; i < rects.Count; i++)
        {
            oldPos[i] = rects[i].anchoredPosition;
        }

        // 삭제
        int removeIndex = _rewardItems.IndexOf(item);
        _rewardItems.RemoveAt(removeIndex);
        rects.RemoveAt(removeIndex);

        Destroy(item.gameObject);

        yield return null;

        // Layout 강제 갱신 → 목표 위치 계산
        LayoutRebuilder.ForceRebuildLayoutImmediate(_rewardContent);

        Vector2[] targetPos = new Vector2[rects.Count];
        for (int i = 0; i < rects.Count; i++)
        {
            targetPos[i] = rects[i].anchoredPosition;
        }

        // Layout 끄기
        _layoutGroup.enabled = false;

        // 위치를 다시 원래 위치로 되돌림
        for (int i = 0; i < rects.Count; i++)
        {
            rects[i].anchoredPosition = oldPos[i < removeIndex ? i : i + 1];
        }

        // 부드럽게 이동
        float duration = 0.25f;
        float time = 0;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = Mathf.SmoothStep(0, 1, time / duration);

            for (int i = 0; i < rects.Count; i++)
            {
                rects[i].anchoredPosition = Vector2.Lerp(oldPos[i < removeIndex ? i : i + 1], targetPos[i], t);
            }

            yield return null;
        }

        // Layout 다시 켜기
        _layoutGroup.enabled = true;
    }

}
