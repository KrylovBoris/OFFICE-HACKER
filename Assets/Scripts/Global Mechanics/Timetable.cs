using System;
using UnityEngine;

public enum EventType
{
    Break,
    Lunch,
    WorkTime
}

[System.Serializable]
public class TimetableEntry
{
    public EventType @event;
    [SerializeField]
    private int startHours;
    [SerializeField]
    private int startingMinutes;

    [SerializeField] private int endingHours;
    [SerializeField] private int endingMinutes;
    public (int hours, int minutes) startingTime => (startHours, startingMinutes);
    public float timeInSecondsFromStartBegin => startHours * 60 * GameManager.gm.MinuteDuration + startingMinutes * GameManager.gm.MinuteDuration;
    public float timeInSecondsFromStartEnd => endingHours * 60 * GameManager.gm.MinuteDuration + endingMinutes * GameManager.gm.MinuteDuration;
}
public class Timetable : MonoBehaviour
{
    
    private float _seconds;
    private float _minuteDuration;

    public TimetableEntry[] timeTable;
    private (int hours, int minutes) _currentTime;
    private EventType _currentEvent;
    
    // Start is called before the first frame update
    void Start()
    {
        _currentTime = GameManager.gm.StartingTime;
        _minuteDuration = GameManager.gm.MinuteDuration;
        _seconds = 0;
    }

    // Update is called once per frame
    void Update()
    {
        foreach (var timeTableEvent in timeTable)
        {
            if (GameManager.gm.SecondsFromStart > timeTableEvent.timeInSecondsFromStartBegin &&
                timeTableEvent.timeInSecondsFromStartEnd >= GameManager.gm.SecondsFromStart)
            {
                _currentEvent = timeTableEvent.@event;
            }
        }
    }

    public string GetCurrentEvent()
    {
        switch (_currentEvent)
        {
            case EventType.Break:
                return "Break";
            case EventType.Lunch:
                return "Lunch";
            case EventType.WorkTime:
                return "Work";
            default: throw new NotImplementedException();
        }
    }
}
