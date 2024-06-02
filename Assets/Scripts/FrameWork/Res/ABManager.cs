using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
//using static UnityEditor.Progress;
using Object = UnityEngine.Object;

/// <summary>
/// AB�������������ⲿ������ؽ�����Դ����
/// </summary>
public class ABManager : SingletonAutoMono<ABManager>
{
    private AssetBundle mainAB = null;
    private AssetBundleManifest mainManifest = null;
    /// <summary>
    /// AB�����·���������޸�
    /// </summary>
    private string PathUrl
    {
        get
        {
            return Application.streamingAssetsPath + "/";
        }
    }
    /// <summary>
    /// �������������޸��Լ���չ
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
    /// ���������ķ���
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
    ///// ����AB���ķ���
    ///// </summary>
    ///// <param name="abName">ab����</param>
    //public void LoadAB(string abName)
    //{
    //    //��������
    //    LoadMainAB();
    //    //��ȡ�����������Ϣ
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
    //    //������Դ��Դ��
    //    ab = AssetBundle.LoadFromFile(PathUrl + abName);
    //    if (!abDic.ContainsKey(abName))
    //    {
    //        ab = AssetBundle.LoadFromFile(PathUrl + abName);
    //        abDic.Add(abName, ab);
    //    }
    //}

    ///// <summary>
    ///// ͬ��������Դ����ָ������
    ///// </summary>
    ///// <param name="abName">AB����</param>
    ///// <param name="resName">��Դ��</param>
    ///// <returns></returns>
    //public Object LoadRes(string abName, string resName)
    //{
    //    //����AB��
    //    LoadAB(abName);

    //    //������Դ����ΪGameObject����ֱ��ʵ����
    //    Object obj = abDic[abName].LoadAsset(resName);
    //    if (obj is GameObject)
    //    {
    //        return Instantiate(obj);
    //    }
    //    else
    //        return obj;
    //}
    ///// <summary>
    ///// ͬ��������Դ������typeָ������
    ///// </summary>
    ///// <param name="abName"></param>
    ///// <param name="resName"></param>
    ///// <param name="type"></param>
    ///// <returns></returns>
    //public Object LoadRes(string abName, string resName, Type type)
    //{
    //    //����AB��
    //    LoadAB(abName);

    //    //������Դ����ΪGameObject����ֱ��ʵ����
    //    Object obj = abDic[abName].LoadAsset(resName, type);
    //    if (obj is GameObject)
    //    {
    //        return Instantiate(obj);
    //    }
    //    else
    //        return obj;
    //}
    ///// <summary>
    ///// ͬ��������Դ�����ݷ���ָ������
    ///// </summary>
    ///// <typeparam name="T"></typeparam>
    ///// <param name="abName"></param>
    ///// <param name="resName"></param>
    ///// <returns></returns>
    //public Object LoadRes<T>(string abName, string resName) where T : Object
    //{
    //    //����AB��
    //    LoadAB(abName);

    //    //������Դ����ΪGameObject����ֱ��ʵ����
    //    T obj = abDic[abName].LoadAsset<T>(resName);
    //    if (obj is GameObject)
    //    {
    //        return Instantiate(obj);
    //    }
    //    else
    //        return obj;
    //}

    #region �������ּ�����Դ��Ĭ���첽
    public void LoadResAsync(string abName, string resName, UnityAction<Object> callBack,bool isSync=false)
    {
        StartCoroutine(LoadResAsyncCoroutine(abName, resName, callBack, isSync));
    }
    private IEnumerator LoadResAsyncCoroutine(string abName, string resName, UnityAction<Object> callBack, bool isSync)
    {
        //��������
        LoadMainAB();
        //��ȡ�����������Ϣ
        AssetBundleCreateRequest abReq = null;
        string[] strs = mainManifest.GetAllDependencies(abName);
        foreach (var item in strs)
        {
            if (!abDic.ContainsKey(item))
            {
                if (isSync)//ͬ������
                {
                    AssetBundle ab = AssetBundle.LoadFromFile(PathUrl + item);
                    abDic.Add(item, ab);
                }
                else//�첽����
                {
                    //��ʼ�첽���ؾͼ�¼����ֵΪ�գ������ڱ��첽����
                    abDic.Add(item, null);
                    abReq = AssetBundle.LoadFromFileAsync(PathUrl + item);
                    yield return abReq;
                    //���ؽ������滻֮ǰ��null����ʱ��Ϊnull��֤�����ؽ���
                    abDic[item] = abReq.assetBundle;
                }

            }
            else//�ֵ����Ѿ���¼��һ��AB�������Ϣ
            {
                //AB�����ڼ����У�����ֻ��Ҫ�ȴ����ؽ������Ϳ��Լ���ִ�к���Ĵ�����
                while (abDic[item] == null)
                {
                    //�ȴ�һ֡����һ֡�ٽ����жϣ�ֱ��AB���������
                    yield return 0;
                }
            }
        }
        //������Դ��Դ��
        if (!abDic.ContainsKey(abName))

        {
            if (isSync)
            {
                AssetBundle ab = AssetBundle.LoadFromFile(PathUrl + abName);
                abDic.Add(abName, ab);
            }
            else
            {
                abDic.Add(abName, null);//��ʼ�첽���ؾͼ�¼����ֵΪ�գ������ڱ��첽����

                abReq = AssetBundle.LoadFromFileAsync(PathUrl + abName);
                yield return abReq;
                //���ؽ������滻֮ǰ��null����ʱ��Ϊnull��֤�����ؽ���
                abDic[abName] = abReq.assetBundle;
            }

        }
        else
        {
            //AB�����ڼ����У�����ֻ��Ҫ�ȴ����ؽ������Ϳ��Լ���ִ�к���Ĵ�����
            while (abDic[abName] == null)
            {
                //�ȴ�һ֡����һ֡�ٽ����жϣ�ֱ��AB���������
                yield return 0;
            }
        }
        
        //������Դ
        if(isSync)
        {
            Object obj=abDic[abName].LoadAsset(resName);
            callBack(obj);
        }
        else
        {
            AssetBundleRequest abr = abDic[abName].LoadAssetAsync(resName);
            yield return abr;
            //���ؽ�����ͨ��ί�д��ݸ��ⲿ
            callBack(abr.asset);
        }
        
    }
    #endregion

    #region ����type������Դ��Ĭ���첽
    public void LoadResAsync(string abName, string resName, Type type, UnityAction<Object> callBack,bool isSync=false)
    {
        StartCoroutine(LoadResAsyncCoroutine(abName, resName, type, callBack,isSync));
    }
    private IEnumerator LoadResAsyncCoroutine(string abName, string resName, Type type, UnityAction<Object> callBack,bool isSync)
    {
        //��������
        LoadMainAB();
        //��ȡ�����������Ϣ
        AssetBundleCreateRequest abReq = null;
        string[] strs = mainManifest.GetAllDependencies(abName);
        foreach (var item in strs)
        {
            if (!abDic.ContainsKey(item))
            {
                if (isSync)//ͬ������
                {
                    AssetBundle ab = AssetBundle.LoadFromFile(PathUrl + item);
                    abDic.Add(item, ab);
                }
                else//�첽����
                {
                    //��ʼ�첽���ؾͼ�¼����ֵΪ�գ������ڱ��첽����
                    abDic.Add(item, null);
                    abReq = AssetBundle.LoadFromFileAsync(PathUrl + item);
                    yield return abReq;
                    //���ؽ������滻֮ǰ��null����ʱ��Ϊnull��֤�����ؽ���
                    abDic[item] = abReq.assetBundle;
                }

            }
            else//�ֵ����Ѿ���¼��һ��AB�������Ϣ
            {
                //AB�����ڼ����У�����ֻ��Ҫ�ȴ����ؽ������Ϳ��Լ���ִ�к���Ĵ�����
                while (abDic[item] == null)
                {
                    //�ȴ�һ֡����һ֡�ٽ����жϣ�ֱ��AB���������
                    yield return 0;
                }
            }
        }
        //������Դ��Դ��
        if (!abDic.ContainsKey(abName))

        {
            if (isSync)
            {
                AssetBundle ab = AssetBundle.LoadFromFile(PathUrl + abName);
                abDic.Add(abName, ab);
            }
            else
            {
                abDic.Add(abName, null);//��ʼ�첽���ؾͼ�¼����ֵΪ�գ������ڱ��첽����

                abReq = AssetBundle.LoadFromFileAsync(PathUrl + abName);
                yield return abReq;
                //���ؽ������滻֮ǰ��null����ʱ��Ϊnull��֤�����ؽ���
                abDic[abName] = abReq.assetBundle;
            }

        }
        else
        {
            //AB�����ڼ����У�����ֻ��Ҫ�ȴ����ؽ������Ϳ��Լ���ִ�к���Ĵ�����
            while (abDic[abName] == null)
            {
                //�ȴ�һ֡����һ֡�ٽ����жϣ�ֱ��AB���������
                yield return 0;
            }
        }

        //������Դ
        if(isSync)
        {
            Object res = abDic[abName].LoadAsset(resName,type);
            callBack(res);
        }
        else
        {
            AssetBundleRequest abr = abDic[abName].LoadAssetAsync(resName, type);
            yield return abr;
            //���ؽ�����ͨ��ί�д��ݸ��ⲿ
            callBack(abr.asset);
        }
        
    }
    #endregion

    #region ���ݷ��ͼ�����Դ��Ĭ���첽
    public void LoadResAsync<T>(string abName, string resName, UnityAction<T> callBack,bool isSync=false) where T : Object
    {
        StartCoroutine(LoadResAsyncCoroutine<T>(abName, resName, callBack,isSync));
    }
    private IEnumerator LoadResAsyncCoroutine<T>(string abName, string resName, UnityAction<T> callBack, bool isSync) where T : Object
    {
        //��������
        LoadMainAB();
        //��ȡ�����������Ϣ
        AssetBundleCreateRequest abReq = null;
        string[] strs = mainManifest.GetAllDependencies(abName);
        foreach (var item in strs)
        {
            if (!abDic.ContainsKey(item))
            {
                if(isSync)//ͬ������
                {
                    AssetBundle ab = AssetBundle.LoadFromFile(PathUrl + item);
                    abDic.Add(item, ab);
                }
                else//�첽����
                {
                    //��ʼ�첽���ؾͼ�¼����ֵΪ�գ������ڱ��첽����
                    abDic.Add(item, null);
                    abReq = AssetBundle.LoadFromFileAsync(PathUrl + item);
                    yield return abReq;
                    //���ؽ������滻֮ǰ��null����ʱ��Ϊnull��֤�����ؽ���
                    abDic[item] = abReq.assetBundle;
                }
                
            }
            else//�ֵ����Ѿ���¼��һ��AB�������Ϣ
            {
                //AB�����ڼ����У�����ֻ��Ҫ�ȴ����ؽ������Ϳ��Լ���ִ�к���Ĵ�����
                while (abDic[item] == null)
                {
                    //�ȴ�һ֡����һ֡�ٽ����жϣ�ֱ��AB���������
                    yield return 0;
                }
            }
        }
        //������Դ��Դ��
        if (!abDic.ContainsKey(abName))
        
        {
            if(isSync)
            {
                AssetBundle ab = AssetBundle.LoadFromFile(PathUrl + abName);
                abDic.Add(abName, ab);
            }
            else
            {
                abDic.Add(abName, null);//��ʼ�첽���ؾͼ�¼����ֵΪ�գ������ڱ��첽����

                abReq = AssetBundle.LoadFromFileAsync(PathUrl + abName);
                yield return abReq;
                //���ؽ������滻֮ǰ��null����ʱ��Ϊnull��֤�����ؽ���
                abDic[abName] = abReq.assetBundle;
            }
            
        }
        else
        {
            //AB�����ڼ����У�����ֻ��Ҫ�ȴ����ؽ������Ϳ��Լ���ִ�к���Ĵ�����
            while (abDic[abName] == null)
            {
                //�ȴ�һ֡����һ֡�ٽ����жϣ�ֱ��AB���������
                yield return 0;
            }
        }

        //������Դ
        if(isSync)
        {
            T res=abDic[abName].LoadAsset<T>(resName);
            callBack(res);
        }
        else
        {
            AssetBundleRequest abr = abDic[abName].LoadAssetAsync<T>(resName);
            yield return abr;
            //���ؽ�����ͨ��ί�д��ݸ��ⲿ
            callBack(abr.asset as T);
        }
        
    }
    #endregion

    /// <summary>
    /// ж�ص�����
    /// </summary>
    /// <param name="abName"></param>
    public void UnloadAB(string abName,UnityAction<bool> callBack)
    {
        if (abDic.ContainsKey(abName))
        {
            if (abDic[abName] == null)
            {
                //ж��ʧ��
                callBack(false);
                return;

            }
            abDic[abName].Unload(false);
            abDic.Remove(abName);
            //ж�سɹ�
            callBack(true);
        }
    }


    /// <summary>
    /// ж�����а�
    /// </summary>
    public void clearAB()
    {
        //����֮ǰֹͣЭ��
        StopAllCoroutines();
        AssetBundle.UnloadAllAssetBundles(false);
        abDic.Clear();
        mainAB = null;
        mainManifest = null;
    }
}
