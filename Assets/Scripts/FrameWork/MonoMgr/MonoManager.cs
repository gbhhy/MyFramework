using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

#region Mono����������
//�ò��̳�Mono�Ľű�Ҳ��
//1.����update��֡���º��������߼�
//2.����Э�̴����߼�
//3.����ͳһ���������߼�����������

//ԭ��
//1.ͨ���¼���ί�� ������ظ��º���
//2.�ṩЭ�̿�����رյķ���
#endregion

/// <summary>
/// ����Mono������
/// </summary>
public class MonoManager : SingletonAutoMono<MonoManager>
{

    private event UnityAction updateEvent;
    private event UnityAction fixedUpdateEvent;
    private event UnityAction lateUpdateEvent;

    void Update()
    {
        updateEvent?.Invoke();
    }
    private void FixedUpdate()
    {
        fixedUpdateEvent?.Invoke();
    }
    private void LateUpdate()
    {
        lateUpdateEvent?.Invoke();
    }

    #region ���֡���¼�������
    public void AddUpdateListener(UnityAction action)
    {
        updateEvent += action;
    }
    public void AddFixedUpdateListener(UnityAction action)
    {
        fixedUpdateEvent += action;
    }
    public void AddLateUpdateListener(UnityAction action)
    {
        lateUpdateEvent += action;
    }
    #endregion

    #region �Ƴ�֡���¼�������
    public void RemoveUpdateListener(UnityAction action) { updateEvent -= action; }
    public void RemoveFixedUpdateListener(UnityAction action) { fixedUpdateEvent -= action; }
    public void RemoveLateUpdateListener(UnityAction action) { lateUpdateEvent -= action; }
    #endregion

}
