using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//������ͬһ������ض�ű�
[DisallowMultipleComponent]
/// <summary>
/// ���ڹ��أ��̳�Mono�ĵ���ģʽ���࣬������ʹ��
/// �����ƻ�������Ψһ�ԣ�
/// 1�����ض���ű�
/// 2���л�����ʱ�����ɶ������
/// 3��ͨ�����붯̬��Ӷ���ű�
/// </summary>
/// <typeparam name="T"></typeparam>
public class SingletonMono<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance;
    public static T Instance => instance;
    protected virtual void Awake()
    {
        //ȷ��Ψһ��
        if (instance != null)
        {
            Destroy(this);
            return;
        }
        instance = this as T;
        DontDestroyOnLoad(this.gameObject);
    }
}
