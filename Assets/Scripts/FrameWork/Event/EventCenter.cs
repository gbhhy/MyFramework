using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 此基类及其子类用于解决拆装箱问题
/// </summary>
public abstract class EventInfoBase { }
public class EventInfo<T> : EventInfoBase
{
    //监听者的函数信息
    public UnityAction<T> actions;
    public EventInfo(UnityAction<T> action)
    {
        this.actions += action;
    }
}

public class EventInfo:EventInfoBase//无参重载
{
    //监听者的函数信息
    public UnityAction actions;
    public EventInfo(UnityAction action)
    {
        this.actions += action;
    }
}

/// <summary>
/// 事件中心：使用时注意，有加就有减，在Awake中注册事件，务必在OnDestroy中移除
/// </summary>
public class EventCenter : BaseManager<EventCenter>
{
    //用于记录事件，及其观察者
    private Dictionary<E_EventType, EventInfoBase> eventDic = new Dictionary<E_EventType, EventInfoBase>();
    private EventCenter() { }

    /// <summary>
    /// 触发事件
    /// </summary>
    /// <param name="eventName">事件名字</param>
    public void EventTrigger<T>(E_EventType eventName, T info)
    {
        if (eventDic.ContainsKey(eventName))
        {
            (eventDic[eventName] as EventInfo<T>).actions?.Invoke(info);
        }
    }
    /// <summary>
    /// 触发事件无参重载
    /// </summary>
    /// <param name="eventName"></param>
    public void EventTrigger(E_EventType eventName)
    {
        if (eventDic.ContainsKey(eventName))
        {
            (eventDic[eventName] as EventInfo).actions?.Invoke();
        }
    }

    /// <summary>
    /// 添加事件监听
    /// </summary>
    /// <param name="eventName">通知事件名</param>
    /// <param name="func">监听者函数</param>
    public void AddEventListener<T>(E_EventType eventName, UnityAction<T> func)
    {
        if (eventDic.ContainsKey(eventName))
        {
            (eventDic[eventName] as EventInfo<T>).actions += func;
        }
        else
        {
            eventDic.Add(eventName, new EventInfo<T>(func));
        }
    }
    /// <summary>
    /// 添加事件监听无参重载
    /// </summary>
    /// <param name="eventName"></param>
    /// <param name="func"></param>
    public void AddEventListener(E_EventType eventName, UnityAction func)
    {
        if (eventDic.ContainsKey(eventName))
        {
            (eventDic[eventName] as EventInfo).actions += func;
        }
        else
        {
            eventDic.Add(eventName, new EventInfo(func));
        }
    }
    /// <summary>
    /// 移除事件监听者
    /// </summary>
    /// <param name="eventName">通知事件名</param>
    /// <param name="func">监听者函数</param>
    public void RemoveEventListener<T>(E_EventType eventName, UnityAction<T> func)
    {
        if (eventDic.ContainsKey(eventName))
            (eventDic[eventName] as EventInfo<T>).actions -= func;
    }
    /// <summary>
    /// 移除事件监听无参重载
    /// </summary>
    /// <param name="eventName"></param>
    /// <param name="func"></param>
    public void RemoveEventListener(E_EventType eventName, UnityAction func)
    {
        if (eventDic.ContainsKey(eventName))
            (eventDic[eventName] as EventInfo).actions -= func;
    }

    /// <summary>
    /// 清空所有事件监听
    /// </summary>
    public void Clear()
    {
        eventDic.Clear();
    }
    /// <summary>
    /// 重载，清除指定事件监听
    /// </summary>
    /// <param name="eventName"></param>
    public void Clear(E_EventType eventName)
    {
        if (!eventDic.ContainsKey(eventName))
            eventDic.Remove(eventName);
    }
}
