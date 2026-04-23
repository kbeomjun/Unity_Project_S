using System.Collections.Generic;
using UnityEngine;

public class StatusIconGroup : MonoBehaviour
{
    [SerializeField] private StatusIcon _statusIconPrefab;

    private Dictionary<IStatusEffect, StatusIcon> _icons = new Dictionary<IStatusEffect, StatusIcon>();

    public void AddStatus(IStatusEffect status)
    {
        if (_icons.ContainsKey(status)) return;

        StatusIcon icon = Instantiate(_statusIconPrefab, transform);
        icon.Init(status.Icon, status.Duration);

        _icons.Add(status, icon);
    }

    public void UpdateStatus(IStatusEffect status)
    {
        if (_icons.TryGetValue(status, out var icon))
        {
            icon.SetDuration(status.Duration);
        }
    }

    public void RemoveStatus(IStatusEffect status)
    {
        if (_icons.TryGetValue(status, out var icon))
        {
            Destroy(icon.gameObject);
            _icons.Remove(status);
        }
    }

}