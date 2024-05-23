using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
/// <summary>
/// 此为懒汉单例模式，此外还有饿汉单例模式如下
/// public abstract class BaseManager <T>where T: class,new()
///  {private static T instance=new T();
///  public static T Instance=>instance;}
///  懒汉第一次使用时才创建实例，饿汉天生线程安全
///  
/// 
/// 不继承Mono的单例模式基类，抽象类保证外部不可new
/// 继承该类的脚本应将构造函数私有化
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class BaseManager <T>where T: class//,new()
{
    private static T instance;
    //用于加锁的对象
    protected static readonly object lockObj =new object();
    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                lock (lockObj)
                {
                    if (instance == null)
                    {
                        //利用反射得到无参私有的构造函数，用于实例化
                        Type type = typeof(T);
                        ConstructorInfo info = type.GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic,//表示成员私有方法
                                                                                                    null,//表示无绑定对象
                                                                                                    Type.EmptyTypes,//表示无参
                                                                                                    null);//表示没有参数修饰符
                        if (info != null)
                            instance = info.Invoke(null) as T;
                        else
                            Debug.LogError("没有得到对应的无参构造函数");
                    }
                }
            }
            return instance;
        }
    }

    
}
