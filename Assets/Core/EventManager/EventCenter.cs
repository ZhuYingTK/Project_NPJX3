using System;
using System.Collections.Generic;

public class EventCenter : Singleton<EventCenter>
{
    private static Dictionary<string, IEventData> eventDictionary = new Dictionary<string, IEventData>();

    public static void AddSingleEventListener<T>(string eventKey, Action<T> listener)
    {
        eventDictionary[eventKey] = new EventData<T>(listener);
    }
    
    public static void AddSingleEventListener(string eventKey, Action listener)
    {
        eventDictionary[eventKey] = new EventData(listener);
    }
    // 添加事件监听器
    public static void AddEventListener<T>(string eventKey, Action<T> listener)
    {
        if (eventDictionary.TryGetValue(eventKey,out var preciousAction))
        {
            if (preciousAction is EventData<T> eventData)
            {
                eventData.Listeners += listener;
            }
        }
        else
        {
            eventDictionary.Add(eventKey,new EventData<T>(listener));
        }
    }

    public static void AddEventListener(string eventKey, Action listener)
    {
        if (eventDictionary.TryGetValue(eventKey,out var preciousAction))
        {
            if (preciousAction is EventData eventData)
            {
                eventData.Listeners += listener;
            }
        }
        else
        {
            eventDictionary.Add(eventKey,new EventData(listener));
        }
    }

    // 移除事件监听器
    public static void RemoveEventListener<T>(string eventKey, Action<T> listener)
    {
        if (eventDictionary.TryGetValue(eventKey, out var previousAction))
        {
            if (previousAction is EventData<T> eventData)
            {
                eventData.Listeners -= listener;
            }
        }
    }
    public static void RemoveEventListener(string eventKey, Action listener)
    {
        if (eventDictionary.TryGetValue(eventKey, out var previousAction))
        {
            if (previousAction is EventData eventData)
            {
                eventData.Listeners -= listener;
            }
        }
    }

    // 触发事件
    public static void TriggerEvent<T>(string eventKey, T eventData)
    {
        if (eventDictionary.TryGetValue(eventKey, out var previousAction))
        {
            (previousAction as EventData<T>)?.Listeners?.Invoke(eventData);
        }
    }
    public static void TriggerEvent(string eventKey)
    {
        if (eventDictionary.TryGetValue(eventKey, out var previousAction))
        {
            (previousAction as EventData)?.Listeners?.Invoke();
        }
    }
}

public interface IEventData{}

public class EventData<T> : IEventData
{
    public Action<T> Listeners;

    public EventData(Action<T> action)
    {
        Listeners += action;
    }
}

public class EventData : IEventData
{
    public Action Listeners;

    public EventData(Action action)
    {
        Listeners += action;
    }
}

public class EventKey
{
    public static readonly string GAME_START = "GAME_START";//游戏开始
}