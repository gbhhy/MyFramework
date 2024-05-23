using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// ���루����أ��е����ݶ���
/// </summary>
public class PoolData
{
    //�����洢�����еľ������ ����ʹ���еĶ���
    private Stack<GameObject> dataStack = new Stack<GameObject>();
    //������¼ʹ���еĶ���
    private List<GameObject> usedDataList = new List<GameObject>();
    //������������ڹ������Ԫ��
    private GameObject rootObj;
    //����
    private int maxNum;

    public bool NeedCreate=>usedDataList.Count <maxNum;
    public int Count => dataStack.Count;
    public int UsedCount => usedDataList.Count;

    /// <summary>
    /// ���캯�������������������Ϊ����
    /// </summary>
    /// <param name="root">����ظ�����</param>
    /// <param name="name">��������</param>
    /// <param name="usedObj">ʹ���еĶ���</param>
    public PoolData(GameObject root, string name, GameObject usedObj)
    {
        if (PoolManager.isOpenLayout)
        {
            rootObj = new GameObject(name);
            rootObj.transform.SetParent(root.transform);
        }
        //���ⲿ�����Ķ����¼��ʹ���еĶ���������
        AddToUsedList(usedObj);

        PoolObj poolObj=usedObj.GetComponent<PoolObj>();
        if(poolObj==null)
        {
            Debug.LogError("��Ϊʹ�û���ع��ܵ�Ԥ����������PoolObj�ű�������������������");
            return;
        }
        maxNum=poolObj.maxNum;
    }


    /// <summary>
    /// �ӳ�����ȡ�����ݶ���
    /// </summary>
    /// <returns>��Ҫ�Ķ�������</returns>
    public GameObject Pop()
    {
        GameObject obj;
        if (Count > 0)
        {
            obj = dataStack.Pop();
            usedDataList.Add(obj);
        }
        else
        {
            obj = usedDataList[0];
            //��ͷ���Ƶ�β��
            usedDataList.RemoveAt(0);
            usedDataList.Add(obj);
        }
        obj.SetActive(true);
        if (PoolManager.isOpenLayout) { obj.transform.SetParent(null); }

        return obj;
    }
    /// <summary>
    /// ��Ԫ�ط������
    /// </summary>
    /// <param name="obj">�����Ķ���</param>
    public void Push(GameObject obj)
    {
        obj.SetActive(false);
        if (PoolManager.isOpenLayout) { obj.transform.SetParent(rootObj.transform); }

        dataStack.Push(obj);
        usedDataList.Remove(obj);
    }

    /// <summary>
    /// ���������ʹ���еĶ���������
    /// </summary>
    /// <param name="obj"></param>
    public void AddToUsedList(GameObject obj)
    {
        usedDataList.Add(obj);
    }
}

/// <summary>
/// �����ģ�������
/// </summary>
public class PoolManager : BaseManager<PoolManager>
{
    private PoolManager() { }

    private GameObject poolObj;//����ظ�����

    //���ӵ��еĳ��룬������List��Stack��Queue���˴���stackʾ��
    private Dictionary<string, PoolData> poolDic = new Dictionary<string, PoolData>();

    public static bool isOpenLayout = true;//�Ƿ������ӹ�ϵ��������Ϸ���������Ϊfalse�Խ�Լ����

    /// <summary>
    /// �ö����ķ���������Scene�У�
    /// </summary>
    /// <param name="name">��������������</param>
    /// <returns></returns>
    public GameObject PopObj(string name)
    {
        if (poolObj == null && isOpenLayout)//��������Ϊ�գ��򴴽�һ��������
            poolObj = new GameObject("Pool");
        GameObject obj;

        if (!poolDic.ContainsKey(name) ||
            poolDic[name].Count == 0 && poolDic[name].NeedCreate)//�����ж�������δ������
        {
            obj = GameObject.Instantiate(Resources.Load<GameObject>(name));
            obj.name = name;//����ʵ�������Ķ����ӣ�Clone��������PushObj����

            //��������
            if (!poolDic.ContainsKey(name))
                poolDic.Add(name, new PoolData(poolObj, name, obj));

            else
                poolDic[name].AddToUsedList(obj);
        }
        else
        {
            obj = poolDic[name].Pop();
        }
        return obj;
    }

    /// <summary>
    /// ��������з������(��Scene���Ƴ�)
    /// </summary>
    /// <param name="obj">ϣ������Ķ���</param>
    public void PushObj(GameObject obj)
    {


        poolDic[obj.name].Push(obj);
    }

    /// <summary>
    /// ��������������ӵ����ݣ�������ɳ����ж�����ʧ�ˣ���������л����ڵ�����
    /// �����л� ����Ҫ��Ӧ�ó���
    /// </summary>
    public void ClearPool()
    {
        poolDic.Clear();
        poolObj = null;//��֤�л�����ʱ�������лᴴ���³���
    }
}
