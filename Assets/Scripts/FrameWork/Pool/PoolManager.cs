using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 抽屉（对象池）中的数据对象
/// </summary>
public class PoolData
{
    //用来存储抽屉中的具体对象 不在使用中的对象
    private Stack<GameObject> dataStack = new Stack<GameObject>();
    //用来记录使用中的对象
    private List<GameObject> usedDataList = new List<GameObject>();
    //抽屉根对象，用于管理抽屉元素
    private GameObject rootObj;
    //上限
    private int maxNum;

    public bool NeedCreate=>usedDataList.Count <maxNum;
    public int Count => dataStack.Count;
    public int UsedCount => usedDataList.Count;

    /// <summary>
    /// 构造函数，将抽屉根对象设置为柜子
    /// </summary>
    /// <param name="root">对象池根对象</param>
    /// <param name="name">抽屉名字</param>
    /// <param name="usedObj">使用中的对象</param>
    public PoolData(GameObject root, string name, GameObject usedObj)
    {
        if (PoolManager.isOpenLayout)
        {
            rootObj = new GameObject(name);
            rootObj.transform.SetParent(root.transform);
        }
        //将外部创建的对象记录到使用中的对象容器中
        AddToUsedList(usedObj);

        PoolObj poolObj=usedObj.GetComponent<PoolObj>();
        if(poolObj==null)
        {
            Debug.LogError("请为使用缓存池功能的预设体对象挂载PoolObj脚本，用于设置数量上限");
            return;
        }
        maxNum=poolObj.maxNum;
    }


    /// <summary>
    /// 从抽屉中取出数据对象
    /// </summary>
    /// <returns>想要的对象数据</returns>
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
            //从头部移到尾部
            usedDataList.RemoveAt(0);
            usedDataList.Add(obj);
        }
        obj.SetActive(true);
        if (PoolManager.isOpenLayout) { obj.transform.SetParent(null); }

        return obj;
    }
    /// <summary>
    /// 将元素放入抽屉
    /// </summary>
    /// <param name="obj">想放入的对象</param>
    public void Push(GameObject obj)
    {
        obj.SetActive(false);
        if (PoolManager.isOpenLayout) { obj.transform.SetParent(rootObj.transform); }

        dataStack.Push(obj);
        usedDataList.Remove(obj);
    }

    /// <summary>
    /// 将对象放入使用中的对象容器中
    /// </summary>
    /// <param name="obj"></param>
    public void AddToUsedList(GameObject obj)
    {
        usedDataList.Add(obj);
    }
}

/// <summary>
/// 对象池模块管理器
/// </summary>
public class PoolManager : BaseManager<PoolManager>
{
    private PoolManager() { }

    private GameObject poolObj;//对象池根对象

    //柜子当中的抽屉，可以用List，Stack，Queue，此处用stack示例
    private Dictionary<string, PoolData> poolDic = new Dictionary<string, PoolData>();

    public static bool isOpenLayout = true;//是否开启父子关系，发布游戏后可以设置为false以节约性能

    /// <summary>
    /// 拿东西的方法（放入Scene中）
    /// </summary>
    /// <param name="name">抽屉容器的名字</param>
    /// <returns></returns>
    public GameObject PopObj(string name)
    {
        if (poolObj == null && isOpenLayout)//若根物体为空，则创建一个空物体
            poolObj = new GameObject("Pool");
        GameObject obj;

        if (!poolDic.ContainsKey(name) ||
            poolDic[name].Count == 0 && poolDic[name].NeedCreate)//场景中对象数量未到上限
        {
            obj = GameObject.Instantiate(Resources.Load<GameObject>(name));
            obj.name = name;//避免实例化出的对象会加（Clone），方便PushObj调用

            //创建抽屉
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
    /// 往对象池中放入对象(在Scene中移除)
    /// </summary>
    /// <param name="obj">希望放入的对象</param>
    public void PushObj(GameObject obj)
    {


        poolDic[obj.name].Push(obj);
    }

    /// <summary>
    /// 用于清除整个池子的数据，避免造成场景中对象消失了，但对象池中还存在的问题
    /// 场景切换 是主要的应用场景
    /// </summary>
    public void ClearPool()
    {
        poolDic.Clear();
        poolObj = null;//保证切换场景时，场景中会创建新池子
    }
}
