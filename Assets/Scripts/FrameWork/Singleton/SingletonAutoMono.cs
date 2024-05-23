using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// �Զ����صļ̳�Mono�ĵ�������
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
                //�ڳ����ϴ���������
                GameObject obj = new GameObject();
                //Ϊ����������Ա��ڱ༭���п����õ����ű������Ķ���
                obj.name =typeof(T).ToString();
                //��̬���ؽű�
                instance = obj.AddComponent<T>();
                //������ʱ���Ƴ�
                DontDestroyOnLoad(obj);
            }
            return instance;
        }
    }
    
}
