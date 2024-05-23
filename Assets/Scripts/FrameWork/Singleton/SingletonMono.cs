using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//不允许同一对象挂载多脚本
[DisallowMultipleComponent]
/// <summary>
/// 用于挂载，继承Mono的单例模式基类，不建议使用
/// 容易破坏单例的唯一性：
/// 1，挂载多个脚本
/// 2，切换场景时，生成多个单例
/// 3，通过代码动态添加多个脚本
/// </summary>
/// <typeparam name="T"></typeparam>
public class SingletonMono<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance;
    public static T Instance => instance;
    protected virtual void Awake()
    {
        //确保唯一性
        if (instance != null)
        {
            Destroy(this);
            return;
        }
        instance = this as T;
        DontDestroyOnLoad(this.gameObject);
    }
}
