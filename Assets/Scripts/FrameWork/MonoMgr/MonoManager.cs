using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

#region Mono管理器介绍
//让不继承Mono的脚本也能
//1.利用update等帧更新函数处理逻辑
//2.利用协程处理逻辑
//3.可以统一管理以上逻辑，减少消耗

//原理：
//1.通过事件或委托 管理相关更新函数
//2.提供协程开启或关闭的方法
#endregion

/// <summary>
/// 公共Mono管理器
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

    #region 添加帧更新监听函数
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

    #region 移除帧更新监听函数
    public void RemoveUpdateListener(UnityAction action) { updateEvent -= action; }
    public void RemoveFixedUpdateListener(UnityAction action) { fixedUpdateEvent -= action; }
    public void RemoveLateUpdateListener(UnityAction action) { lateUpdateEvent -= action; }
    #endregion

}
