using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class SceneMgr : BaseManager<SceneMgr>
{
    private SceneMgr() { }

    /// <summary>
    /// ͬ���л�����
    /// </summary>
    /// <param name="name"></param>
    /// <param name="callBack"></param>
    public void LoadScene(string name,UnityAction callBack=null)
    {
        SceneManager.LoadScene(name);

        callBack?.Invoke();
    }

    public void LoadSceneAsync(string name,UnityAction callBack=null)
    {
        MonoManager.Instance.StartCoroutine(LoadSceneAsyncCoroutine(name, callBack));
    }

    private IEnumerator LoadSceneAsyncCoroutine(string name,UnityAction callBack)
    {
        AsyncOperation ao= SceneManager.LoadSceneAsync(name);
        while(!ao.isDone)//ÿ֡����Ƿ���ؽ���
        {
            //�����¼����ģ����ͽ���
            EventCenter.Instance.EventTrigger(E_EventType.E_SceneLoadProgress,ao.progress);
            yield return 0;
        }
        EventCenter.Instance.EventTrigger(E_EventType.E_SceneLoadProgress, 1f);//�����ٶ�̫��û�м��ص�1

        callBack?.Invoke();
    }
}
