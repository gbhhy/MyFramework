using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// �˻��༰���������ڽ����װ������
/// </summary>
public abstract class EventInfoBase { }
public class EventInfo<T> : EventInfoBase
{
    //�����ߵĺ�����Ϣ
    public UnityAction<T> actions;
    public EventInfo(UnityAction<T> action)
    {
        this.actions += action;
    }
}

public class EventInfo:EventInfoBase//�޲�����
{
    //�����ߵĺ�����Ϣ
    public UnityAction actions;
    public EventInfo(UnityAction action)
    {
        this.actions += action;
    }
}

/// <summary>
/// �¼����ģ�ʹ��ʱע�⣬�мӾ��м�����Awake��ע���¼��������OnDestroy���Ƴ�
/// </summary>
public class EventCenter : BaseManager<EventCenter>
{
    //���ڼ�¼�¼�������۲���
    private Dictionary<E_EventType, EventInfoBase> eventDic = new Dictionary<E_EventType, EventInfoBase>();
    private EventCenter() { }

    /// <summary>
    /// �����¼�
    /// </summary>
    /// <param name="eventName">�¼�����</param>
    public void EventTrigger<T>(E_EventType eventName, T info)
    {
        if (eventDic.ContainsKey(eventName))
        {
            (eventDic[eventName] as EventInfo<T>).actions?.Invoke(info);
        }
    }
    /// <summary>
    /// �����¼��޲�����
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
    /// ����¼�����
    /// </summary>
    /// <param name="eventName">֪ͨ�¼���</param>
    /// <param name="func">�����ߺ���</param>
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
    /// ����¼������޲�����
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
    /// �Ƴ��¼�������
    /// </summary>
    /// <param name="eventName">֪ͨ�¼���</param>
    /// <param name="func">�����ߺ���</param>
    public void RemoveEventListener<T>(E_EventType eventName, UnityAction<T> func)
    {
        if (eventDic.ContainsKey(eventName))
            (eventDic[eventName] as EventInfo<T>).actions -= func;
    }
    /// <summary>
    /// �Ƴ��¼������޲�����
    /// </summary>
    /// <param name="eventName"></param>
    /// <param name="func"></param>
    public void RemoveEventListener(E_EventType eventName, UnityAction func)
    {
        if (eventDic.ContainsKey(eventName))
            (eventDic[eventName] as EventInfo).actions -= func;
    }

    /// <summary>
    /// ��������¼�����
    /// </summary>
    public void Clear()
    {
        eventDic.Clear();
    }
    /// <summary>
    /// ���أ����ָ���¼�����
    /// </summary>
    /// <param name="eventName"></param>
    public void Clear(E_EventType eventName)
    {
        if (!eventDic.ContainsKey(eventName))
            eventDic.Remove(eventName);
    }
}
