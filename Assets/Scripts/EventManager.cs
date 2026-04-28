using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    [SerializeField] private RectTransform _canvasRect;
    [SerializeField] private RectTransform _eventButttonTr;

    private List<GameEvent> _events = new List<GameEvent>();
    private List<EventButton> _eventButtons = new List<EventButton>();

    public static EventManager Instance { get; private set; }
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        InitEvents();
    }

    private void InitEvents()
    {
        _events = new List<GameEvent>()
        {
            new GameEvent
            (
                new List<EventOption>
                {
                    new EventOption("Gain 50 Coin", new List<IEventEffect> { new GainGold(50) }),
                    new EventOption("Gain 100 Coin, Lose 10 Health(All Party)", new List<IEventEffect> { new GainGold(100), new HurtUnit(10, true) })
                }
            ),
            new GameEvent
            (
                new List<EventOption>
                {
                    new EventOption("Gain 100 Coin", new List<IEventEffect> { new GainGold(50) }),
                    new EventOption("Add Unit: Knight", new List<IEventEffect> { new AddUnit(0) }),
                    new EventOption("Gain 200 Coin, Lose 20 Health(All Party)", new List<IEventEffect> { new GainGold(200), new HurtUnit(20, true) })
                }
            ),
        };
    }

    public void StartEvent(int chapter)
    {
        GameEvent gameEvent = CreateRandomEvent(chapter);

        foreach (EventOption option in gameEvent.Options)
        {
            EventButton eventButton = Instantiate(DataManager.Instance.EventButtonPrefab, _eventButttonTr, false);
            eventButton.Init(option);
            _eventButtons.Add(eventButton);
        }
    }

    private GameEvent CreateRandomEvent(int chapter)
    {
        int index = Random.Range(0, _events.Count);
        return _events[index];
    }

    public void EndEvent()
    {
        Clear();
        GameManager.Instance.OnClearNode();
    }

    private void Clear()
    {
        foreach (EventButton button in _eventButtons)
            Destroy(button.gameObject);
        _eventButtons.Clear();
    }

}
