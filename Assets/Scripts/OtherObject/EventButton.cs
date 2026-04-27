using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EventButton : MonoBehaviour
{
    [SerializeField] private TMP_Text _eventText;

    private EventOption _eventOption;

    public void Init(EventOption option)
    {
        _eventOption = option;
        _eventText.text = option.Text;
        
        if(option.Effects[0] is AddUnit && GameManager.Instance.PlayerUnitDatas.Count >= 4)
        {
            GetComponent<Button>().interactable = false;
            _eventText.text += " (Party is Full)";
        }
    }

    public void OnClick()
    {
        foreach (IEventEffect effect in _eventOption.Effects)
        {
            effect.Execute();
        }

        StartManager.Instance.EndEvent();
    }

}
