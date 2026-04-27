using System.Collections.Generic;
using UnityEngine;

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
