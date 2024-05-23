using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Resources��Դ����ģ�������
/// </summary>
public class ResManager :BaseManager<ResManager>
{
    private ResManager() { }

    /// <summary>
    /// �첽������Դ�ķ���
    /// </summary>
    /// <typeparam name="T">��Դ����</typeparam>
    /// <param name="path">��Դ·��</param>
    /// <param name="callBack">���ؽ�����Ļص�����</param>
    public void LoadAsync<T>(string path,UnityAction<T> callBack)where T : Object
    {
        //ͨ��Э���첽������Դ
        MonoManager.Instance.StartCoroutine(LoadAsyncCoroutine<T>(path, callBack));
    }

    private IEnumerator LoadAsyncCoroutine<T>(string path, UnityAction<T> callBack) where T : Object
    {
        ResourceRequest request=Resources.LoadAsync<T>(path);
        yield return request;
        //��Դ���ؽ���������Դ�����ⲿί�к�������ʹ��
        callBack(request.asset as T);
    }
}
