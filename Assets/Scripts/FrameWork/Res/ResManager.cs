using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Resources资源加载模块管理器
/// </summary>
public class ResManager :BaseManager<ResManager>
{
    private ResManager() { }

    /// <summary>
    /// 异步加载资源的方法
    /// </summary>
    /// <typeparam name="T">资源类型</typeparam>
    /// <param name="path">资源路径</param>
    /// <param name="callBack">加载结束后的回调函数</param>
    public void LoadAsync<T>(string path,UnityAction<T> callBack)where T : Object
    {
        //通过协程异步加载资源
        MonoManager.Instance.StartCoroutine(LoadAsyncCoroutine<T>(path, callBack));
    }

    private IEnumerator LoadAsyncCoroutine<T>(string path, UnityAction<T> callBack) where T : Object
    {
        ResourceRequest request=Resources.LoadAsync<T>(path);
        yield return request;
        //资源加载结束，将资源传到外部委托函数进行使用
        callBack(request.asset as T);
    }
}
