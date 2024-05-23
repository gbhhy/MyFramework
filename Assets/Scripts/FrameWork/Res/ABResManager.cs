using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
/// <summary>
/// 用于进行加载AB相关资源
/// </summary>
public class ABResManager : BaseManager<ABResManager>
{
    private bool isDebug = true;//true会在编辑模式中用EditorResManager进行加载
    private ABResManager() { }

    public void LoadResAsync<T>(string abName, string resName, UnityAction<T> callBack, bool isSync = false) where T:Object
    {
#if UNITY_EDITOR
        if(isDebug)
        {
            T res = EditorResManager.Instance.LoadEditorRes<T>($"{abName}/{resName}");
            callBack?.Invoke( res as T);
        }
        else
        {
            ABManager.Instance.LoadResAsync<T>(abName, resName, callBack, isSync);
        }
#else
            ABManager.Instance.LoadResAsync<T>(abName, resName, callBack, isSync);
#endif
    }
}
