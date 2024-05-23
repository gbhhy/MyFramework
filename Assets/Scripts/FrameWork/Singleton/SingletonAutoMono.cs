using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 自动挂载的继承Mono的单例基类
/// </summary>
/// <typeparam name="T"></typeparam>
public class SingletonAutoMono <T>: MonoBehaviour where T : MonoBehaviour
{
    private static T instance;
    public static T Instance
    {
        get
        {
            if(instance == null)
            {
                //在场景上创建空物体
                GameObject obj = new GameObject();
                //为对象改名，以便在编辑器中看到该单例脚本依附的对象
                obj.name =typeof(T).ToString();
                //动态挂载脚本
                instance = obj.AddComponent<T>();
                //过场景时不移除
                DontDestroyOnLoad(obj);
            }
            return instance;
        }
    }
    
}
