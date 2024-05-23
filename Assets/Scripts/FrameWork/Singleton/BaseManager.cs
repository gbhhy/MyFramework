using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
/// <summary>
/// ��Ϊ��������ģʽ�����⻹�ж�������ģʽ����
/// public abstract class BaseManager <T>where T: class,new()
///  {private static T instance=new T();
///  public static T Instance=>instance;}
///  ������һ��ʹ��ʱ�Ŵ���ʵ�������������̰߳�ȫ
///  
/// 
/// ���̳�Mono�ĵ���ģʽ���࣬�����ౣ֤�ⲿ����new
/// �̳и���Ľű�Ӧ�����캯��˽�л�
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class BaseManager <T>where T: class//,new()
{
    private static T instance;
    //���ڼ����Ķ���
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
                        //���÷���õ��޲�˽�еĹ��캯��������ʵ����
                        Type type = typeof(T);
                        ConstructorInfo info = type.GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic,//��ʾ��Ա˽�з���
                                                                                                    null,//��ʾ�ް󶨶���
                                                                                                    Type.EmptyTypes,//��ʾ�޲�
                                                                                                    null);//��ʾû�в������η�
                        if (info != null)
                            instance = info.Invoke(null) as T;
                        else
                            Debug.LogError("û�еõ���Ӧ���޲ι��캯��");
                    }
                }
            }
            return instance;
        }
    }

    
}
