using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
//using static UnityEditor.Progress;
using Object = UnityEngine.Object;

/// <summary>
/// AB包管理器，让外部更方便地进行资源加载
/// </summary>
public class ABManager : SingletonAutoMono<ABManager>
{
    private AssetBundle mainAB = null;
    private AssetBundleManifest mainManifest = null;
    /// <summary>
    /// AB包存放路径，方便修改
    /// </summary>
    private string PathUrl
    {
        get
        {
            return Application.streamingAssetsPath + "/";
        }
    }
    /// <summary>
    /// 主包名，方便修改以及拓展
    /// </summary>
    private string MainABName
    {
        get
        {
#if UNITY_IOS
            return "IOS";
#elif UNITY_ANDROID
            return "Android";
#else 
            return "PC";
#endif
        }
    }
    private Dictionary<string, AssetBundle> abDic = new Dictionary<string, AssetBundle>();

    /// <summary>
    /// 加载主包的方法
    /// </summary>
    public void LoadMainAB()
    {
        if (mainAB == null)
        {
            mainAB = AssetBundle.LoadFromFile(PathUrl + MainABName);
            mainManifest = mainAB.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
        }
    }

    ///// <summary>
    ///// 加载AB包的方法
    ///// </summary>
    ///// <param name="abName">ab包名</param>
    //public void LoadAB(string abName)
    //{
    //    //加载主包
    //    LoadMainAB();
    //    //获取依赖包相关信息
    //    AssetBundle ab = null;
    //    string[] strs = mainManifest.GetAllDependencies(abName);
    //    foreach (var item in strs)
    //    {
    //        if (!abDic.ContainsKey(item))
    //        {
    //            ab = AssetBundle.LoadFromFile(PathUrl + item);
    //            abDic.Add(item, ab);
    //        }
    //    }
    //    //加载资源来源包
    //    ab = AssetBundle.LoadFromFile(PathUrl + abName);
    //    if (!abDic.ContainsKey(abName))
    //    {
    //        ab = AssetBundle.LoadFromFile(PathUrl + abName);
    //        abDic.Add(abName, ab);
    //    }
    //}

    ///// <summary>
    ///// 同步加载资源，不指定类型
    ///// </summary>
    ///// <param name="abName">AB包名</param>
    ///// <param name="resName">资源名</param>
    ///// <returns></returns>
    //public Object LoadRes(string abName, string resName)
    //{
    //    //加载AB包
    //    LoadAB(abName);

    //    //加载资源，若为GameObject，则直接实例化
    //    Object obj = abDic[abName].LoadAsset(resName);
    //    if (obj is GameObject)
    //    {
    //        return Instantiate(obj);
    //    }
    //    else
    //        return obj;
    //}
    ///// <summary>
    ///// 同步加载资源，根据type指定类型
    ///// </summary>
    ///// <param name="abName"></param>
    ///// <param name="resName"></param>
    ///// <param name="type"></param>
    ///// <returns></returns>
    //public Object LoadRes(string abName, string resName, Type type)
    //{
    //    //加载AB包
    //    LoadAB(abName);

    //    //加载资源，若为GameObject，则直接实例化
    //    Object obj = abDic[abName].LoadAsset(resName, type);
    //    if (obj is GameObject)
    //    {
    //        return Instantiate(obj);
    //    }
    //    else
    //        return obj;
    //}
    ///// <summary>
    ///// 同步加载资源，根据泛型指定类型
    ///// </summary>
    ///// <typeparam name="T"></typeparam>
    ///// <param name="abName"></param>
    ///// <param name="resName"></param>
    ///// <returns></returns>
    //public Object LoadRes<T>(string abName, string resName) where T : Object
    //{
    //    //加载AB包
    //    LoadAB(abName);

    //    //加载资源，若为GameObject，则直接实例化
    //    T obj = abDic[abName].LoadAsset<T>(resName);
    //    if (obj is GameObject)
    //    {
    //        return Instantiate(obj);
    //    }
    //    else
    //        return obj;
    //}

    #region 根据名字加载资源，默认异步
    public void LoadResAsync(string abName, string resName, UnityAction<Object> callBack,bool isSync=false)
    {
        StartCoroutine(LoadResAsyncCoroutine(abName, resName, callBack, isSync));
    }
    private IEnumerator LoadResAsyncCoroutine(string abName, string resName, UnityAction<Object> callBack, bool isSync)
    {
        //加载主包
        LoadMainAB();
        //获取依赖包相关信息
        AssetBundleCreateRequest abReq = null;
        string[] strs = mainManifest.GetAllDependencies(abName);
        foreach (var item in strs)
        {
            if (!abDic.ContainsKey(item))
            {
                if (isSync)//同步加载
                {
                    AssetBundle ab = AssetBundle.LoadFromFile(PathUrl + item);
                    abDic.Add(item, ab);
                }
                else//异步加载
                {
                    //开始异步加载就记录，若值为空，则正在被异步加载
                    abDic.Add(item, null);
                    abReq = AssetBundle.LoadFromFileAsync(PathUrl + item);
                    yield return abReq;
                    //加载结束后，替换之前的null，此时不为null，证明加载结束
                    abDic[item] = abReq.assetBundle;
                }

            }
            else//字典中已经记录了一个AB包相关信息
            {
                //AB包正在加载中，我们只需要等待加载结束，就可以继续执行后面的代码了
                while (abDic[item] == null)
                {
                    //等待一帧，下一帧再进行判断，直到AB包加载完成
                    yield return 0;
                }
            }
        }
        //加载资源来源包
        if (!abDic.ContainsKey(abName))

        {
            if (isSync)
            {
                AssetBundle ab = AssetBundle.LoadFromFile(PathUrl + abName);
                abDic.Add(abName, ab);
            }
            else
            {
                abDic.Add(abName, null);//开始异步加载就记录，若值为空，则正在被异步加载

                abReq = AssetBundle.LoadFromFileAsync(PathUrl + abName);
                yield return abReq;
                //加载结束后，替换之前的null，此时不为null，证明加载结束
                abDic[abName] = abReq.assetBundle;
            }

        }
        else
        {
            //AB包正在加载中，我们只需要等待加载结束，就可以继续执行后面的代码了
            while (abDic[abName] == null)
            {
                //等待一帧，下一帧再进行判断，直到AB包加载完成
                yield return 0;
            }
        }
        
        //加载资源
        if(isSync)
        {
            Object obj=abDic[abName].LoadAsset(resName);
            callBack(obj);
        }
        else
        {
            AssetBundleRequest abr = abDic[abName].LoadAssetAsync(resName);
            yield return abr;
            //加载结束后，通过委托传递给外部
            callBack(abr.asset);
        }
        
    }
    #endregion

    #region 根据type加载资源，默认异步
    public void LoadResAsync(string abName, string resName, Type type, UnityAction<Object> callBack,bool isSync=false)
    {
        StartCoroutine(LoadResAsyncCoroutine(abName, resName, type, callBack,isSync));
    }
    private IEnumerator LoadResAsyncCoroutine(string abName, string resName, Type type, UnityAction<Object> callBack,bool isSync)
    {
        //加载主包
        LoadMainAB();
        //获取依赖包相关信息
        AssetBundleCreateRequest abReq = null;
        string[] strs = mainManifest.GetAllDependencies(abName);
        foreach (var item in strs)
        {
            if (!abDic.ContainsKey(item))
            {
                if (isSync)//同步加载
                {
                    AssetBundle ab = AssetBundle.LoadFromFile(PathUrl + item);
                    abDic.Add(item, ab);
                }
                else//异步加载
                {
                    //开始异步加载就记录，若值为空，则正在被异步加载
                    abDic.Add(item, null);
                    abReq = AssetBundle.LoadFromFileAsync(PathUrl + item);
                    yield return abReq;
                    //加载结束后，替换之前的null，此时不为null，证明加载结束
                    abDic[item] = abReq.assetBundle;
                }

            }
            else//字典中已经记录了一个AB包相关信息
            {
                //AB包正在加载中，我们只需要等待加载结束，就可以继续执行后面的代码了
                while (abDic[item] == null)
                {
                    //等待一帧，下一帧再进行判断，直到AB包加载完成
                    yield return 0;
                }
            }
        }
        //加载资源来源包
        if (!abDic.ContainsKey(abName))

        {
            if (isSync)
            {
                AssetBundle ab = AssetBundle.LoadFromFile(PathUrl + abName);
                abDic.Add(abName, ab);
            }
            else
            {
                abDic.Add(abName, null);//开始异步加载就记录，若值为空，则正在被异步加载

                abReq = AssetBundle.LoadFromFileAsync(PathUrl + abName);
                yield return abReq;
                //加载结束后，替换之前的null，此时不为null，证明加载结束
                abDic[abName] = abReq.assetBundle;
            }

        }
        else
        {
            //AB包正在加载中，我们只需要等待加载结束，就可以继续执行后面的代码了
            while (abDic[abName] == null)
            {
                //等待一帧，下一帧再进行判断，直到AB包加载完成
                yield return 0;
            }
        }

        //加载资源
        if(isSync)
        {
            Object res = abDic[abName].LoadAsset(resName,type);
            callBack(res);
        }
        else
        {
            AssetBundleRequest abr = abDic[abName].LoadAssetAsync(resName, type);
            yield return abr;
            //加载结束后，通过委托传递给外部
            callBack(abr.asset);
        }
        
    }
    #endregion

    #region 根据泛型加载资源，默认异步
    public void LoadResAsync<T>(string abName, string resName, UnityAction<T> callBack,bool isSync=false) where T : Object
    {
        StartCoroutine(LoadResAsyncCoroutine<T>(abName, resName, callBack,isSync));
    }
    private IEnumerator LoadResAsyncCoroutine<T>(string abName, string resName, UnityAction<T> callBack, bool isSync) where T : Object
    {
        //加载主包
        LoadMainAB();
        //获取依赖包相关信息
        AssetBundleCreateRequest abReq = null;
        string[] strs = mainManifest.GetAllDependencies(abName);
        foreach (var item in strs)
        {
            if (!abDic.ContainsKey(item))
            {
                if(isSync)//同步加载
                {
                    AssetBundle ab = AssetBundle.LoadFromFile(PathUrl + item);
                    abDic.Add(item, ab);
                }
                else//异步加载
                {
                    //开始异步加载就记录，若值为空，则正在被异步加载
                    abDic.Add(item, null);
                    abReq = AssetBundle.LoadFromFileAsync(PathUrl + item);
                    yield return abReq;
                    //加载结束后，替换之前的null，此时不为null，证明加载结束
                    abDic[item] = abReq.assetBundle;
                }
                
            }
            else//字典中已经记录了一个AB包相关信息
            {
                //AB包正在加载中，我们只需要等待加载结束，就可以继续执行后面的代码了
                while (abDic[item] == null)
                {
                    //等待一帧，下一帧再进行判断，直到AB包加载完成
                    yield return 0;
                }
            }
        }
        //加载资源来源包
        if (!abDic.ContainsKey(abName))
        
        {
            if(isSync)
            {
                AssetBundle ab = AssetBundle.LoadFromFile(PathUrl + abName);
                abDic.Add(abName, ab);
            }
            else
            {
                abDic.Add(abName, null);//开始异步加载就记录，若值为空，则正在被异步加载

                abReq = AssetBundle.LoadFromFileAsync(PathUrl + abName);
                yield return abReq;
                //加载结束后，替换之前的null，此时不为null，证明加载结束
                abDic[abName] = abReq.assetBundle;
            }
            
        }
        else
        {
            //AB包正在加载中，我们只需要等待加载结束，就可以继续执行后面的代码了
            while (abDic[abName] == null)
            {
                //等待一帧，下一帧再进行判断，直到AB包加载完成
                yield return 0;
            }
        }

        //加载资源
        if(isSync)
        {
            T res=abDic[abName].LoadAsset<T>(resName);
            callBack(res);
        }
        else
        {
            AssetBundleRequest abr = abDic[abName].LoadAssetAsync<T>(resName);
            yield return abr;
            //加载结束后，通过委托传递给外部
            callBack(abr.asset as T);
        }
        
    }
    #endregion

    /// <summary>
    /// 卸载单个包
    /// </summary>
    /// <param name="abName"></param>
    public void UnloadAB(string abName,UnityAction<bool> callBack)
    {
        if (abDic.ContainsKey(abName))
        {
            if (abDic[abName] == null)
            {
                //卸载失败
                callBack(false);
                return;

            }
            abDic[abName].Unload(false);
            abDic.Remove(abName);
            //卸载成功
            callBack(true);
        }
    }


    /// <summary>
    /// 卸载所有包
    /// </summary>
    public void clearAB()
    {
        //清理之前停止协程
        StopAllCoroutines();
        AssetBundle.UnloadAllAssetBundles(false);
        abDic.Clear();
        mainAB = null;
        mainManifest = null;
    }
}
