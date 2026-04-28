using System.Collections.Generic;
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

        foreach (IEventEffect effect in option.Effects)
        {
            EffectResult result = effect.Evaluate();

            if (!result.CanExecute)
            {
                GetComponent<Button>().interactable = false;
                _eventText.text += $" ({result.Reason})";
                return;
            }
        }

        GetComponent<Button>().interactable = true;
    }

    public void OnClick()
    {
        foreach (IEventEffect effect in _eventOption.Effects)
        {
            effect.Execute();
        }

        StartManager.Instance.EndEvent();
        EventManager.Instance.EndEvent();
    }
}

public class GameEvent
{
    public List<EventOption> Options;

    public GameEvent(List<EventOption> options)
    {
        Options = options;
    }
}

public class EventOption
{
    public string Text;
    public List<IEventEffect> Effects;

    public EventOption(string text, List<IEventEffect> effects)
    {
        Text = text;
        Effects = effects;
    }
}
