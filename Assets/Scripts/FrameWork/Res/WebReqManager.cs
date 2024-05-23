using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

public class WebReqManager : SingletonAutoMono<WebReqManager>
{
    /// <summary>
    /// 利用UnityWebRequest加载资源
    /// </summary>
    /// <typeparam name="T">类型只能是string，byte数组，texture和assetbundle</typeparam>
    /// <param name="path">资源路径，要自己加上协议</param>
    /// <param name="callBack">加载成功的回调函数</param>
    /// <param name="failCallBack">加载失败的回调函数</param>
    public void LoadRes<T>(string path,UnityAction<T> callBack,UnityAction failCallBack) where T : class
    {
        StartCoroutine(LoadResCoroutine(path, callBack, failCallBack));
    }
    private IEnumerator LoadResCoroutine<T>(string path, UnityAction<T> callBack, UnityAction failCallBack)where T : class
    {
        //string
        //byte[]
        //Texture
        //AssetBundle
        Type type = typeof(T);
        //用于加载的对象
        UnityWebRequest request = null;
        if(type==typeof(string)||
           type == typeof(byte[]))
            request=UnityWebRequest.Get(path);
        else if (type==typeof(Texture))
            request = UnityWebRequestTexture.GetTexture(path);
        else if(type==typeof(AssetBundle))
            request=UnityWebRequestAssetBundle.GetAssetBundle(path);
        else
        {
            failCallBack?.Invoke();
            yield break;
        }
        yield return request.SendWebRequest();

        if(request.result==UnityWebRequest.Result.Success)
        {
            if (type == typeof(string))
                callBack?.Invoke(request.downloadHandler.text as T);
            else if (type == typeof(byte[]))
                callBack?.Invoke(request.downloadHandler.data as T);
            else if (type == typeof(Texture))
                callBack?.Invoke(DownloadHandlerTexture.GetContent(request) as T);
            else if (type == typeof(AssetBundle))
                callBack?.Invoke(DownloadHandlerAssetBundle.GetContent(request) as T);
        }
        else
            failCallBack?.Invoke();
        request.Dispose();
    }
}
