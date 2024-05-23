using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
/// <summary>
/// 层级枚举
/// </summary>
public enum E_UILayer
{
    Bottom,
    Middle,
    Top,
    System
}

/// <summary>
/// 初始化及层级相关
/// </summary>
public partial class UIManager : BaseManager<UIManager>
{

    private Canvas uiCanvas;
    private EventSystem uiEventSystem;

    private Transform bottomLayer;
    private Transform middleLayer;
    private Transform topLayer;
    private Transform systemLayer;

    private UIManager()
    {
        //动态创建唯一的Canvas和EventSystem，并保证过场景不移除


        uiCanvas = GameObject.Instantiate(Resources.Load<GameObject>("UI/Canvas").GetComponent<Canvas>());
        GameObject.DontDestroyOnLoad(uiCanvas.gameObject);

        uiEventSystem = GameObject.Instantiate(Resources.Load<GameObject>("UI/EventSystem").GetComponent<EventSystem>());
        GameObject.DontDestroyOnLoad(uiEventSystem.gameObject);

        bottomLayer = uiCanvas.transform.Find("Bottom");
        middleLayer = uiCanvas.transform.Find("Middle");
        topLayer = uiCanvas.transform.Find("Top");
        systemLayer = uiCanvas.transform.Find("System");

    }

    /// <summary>
    /// 获取对应层级
    /// </summary>
    /// <param name="layer">层级枚举</param>
    /// <returns></returns>
    public Transform GetLayer(E_UILayer layer)
    {
        switch (layer)
        {
            case E_UILayer.Bottom:
                return bottomLayer;
            case E_UILayer.Middle:
                return middleLayer;
            case E_UILayer.Top:
                return topLayer;
            case E_UILayer.System:
                return systemLayer;
            default:
                return null;
        }
    }
}

/// <summary>
/// 面板管理相关
/// 面板预设体名字要和面板类名一致方可使用！
/// </summary>
public partial class UIManager : BaseManager<UIManager>
{
    /// <summary>
    /// 用里氏替换原则解决Dictionary不能储存不确定的泛型类的问题
    /// </summary>
    private abstract class BasePanelInfo { }
    /// <summary>
    /// 用于存储面板信息和加载回调函数
    /// </summary>
    /// <typeparam name="T">面板类型</typeparam>
    private class PanelInfo<T> : BasePanelInfo where T : BasePanel
    {
        public T panel;
        public UnityAction<T> callBack;
        public bool isHide;

        public PanelInfo(UnityAction<T> callBack)
        {
            this.callBack += callBack;
        }
    }

    //存储所有面板
    private Dictionary<string, BasePanelInfo> panelDic = new Dictionary<string, BasePanelInfo>();

    /// <summary>
    /// 显示面板
    /// </summary>
    /// <typeparam name="T">面板类型</typeparam>
    /// <param name="layer">显示层级，默认为中层</param>
    /// <param name="callBack">回调，由于可能异步加载，所以用委托传递面板</param>
    /// <param name="isSync">是否同步加载，默认异步</param>
    public void ShowPanel<T>(E_UILayer layer = E_UILayer.Middle, UnityAction<T> callBack = null, bool isSync = false) where T : BasePanel
    {
        string panelName = typeof(T).Name;
        if (panelDic.ContainsKey(panelName))//存在面板
        {
            //取出字典中的数据
            PanelInfo<T> panelInfo = panelDic[panelName] as PanelInfo<T>;

            //正在异步加载中
            if (panelInfo.panel == null)
            {
                panelInfo.isHide = false;
                //等待加载完毕，只需要记录回调函数，加载完毕后调用
                if (callBack != null)
                    panelInfo.callBack += callBack;
            }
            else//已经加载结束
            {
                if(!panelInfo.panel.gameObject.activeSelf)//如果是失活状态，直接激活
                {
                    panelInfo.panel.gameObject.SetActive(true);
                }
                panelInfo.panel.ShowMe();
                callBack?.Invoke(panelInfo.panel);

            }

            return;
        }
        //不存在面板，先存入字典中，占个位置，显示时才能得到字典中信息进行判断
        panelDic.Add(panelName, new PanelInfo<T>(callBack));
        ABResManager.Instance.LoadResAsync<GameObject>("ui", panelName, (panelPrefab) =>
        {
            //取出字典中的数据
            PanelInfo<T> panelInfo = panelDic[panelName] as PanelInfo<T>;
            if (panelInfo.isHide)//异步加载结束前，就想移除该面板了
            {
                panelDic.Remove(panelName);
                return;
            }
            Transform parent = GetLayer(layer);
            //将面板预设体创建到对应父对象下，并保持原本缩放大小
            GameObject panelObj = GameObject.Instantiate(panelPrefab, parent, false);



            T panel = panelObj.GetComponent<T>();
            panel.ShowMe();
            panelInfo.callBack?.Invoke(panel);
            //回调执行完，将其清空，避免内存泄漏
            panelInfo.callBack = null;
            //存储panel
            panelInfo.panel = panel;
        }, isSync);
    }

/// <summary>
/// 隐藏面板
/// </summary>
/// <typeparam name="T">面板类型</typeparam>
/// <param name="isDestroy">决定隐藏面板时是否销毁，内存压力大时销毁避免程序崩溃；内存压力小时失活，避免频繁GC造成卡顿</param>
    public void HidePanel<T>(bool isDestroy = false) where T : BasePanel
    {
        string panelName = typeof(T).Name;
        if (panelDic.ContainsKey(panelName))
        {
            //取出字典中的数据
            PanelInfo<T> panelInfo = panelDic[panelName] as PanelInfo<T>;
            //正在加载中
            if (panelInfo.panel == null)
            {
                panelInfo.isHide = true;
                panelInfo.callBack = null;
            }
            else
            {
                //已经加载结束
                panelInfo.panel.HideMe();
                if (isDestroy)
                {
                    GameObject.Destroy(panelInfo.panel.gameObject);
                    panelDic.Remove(panelName);
                }
                else//若不销毁，就只是失活，下次显示时直接复用
                    panelInfo.panel.gameObject.SetActive(false);
            }

        }
    }

    /// <summary>
    /// 得到面板
    /// </summary>
    /// <typeparam name="T">面板类型</typeparam>
    public void GetPanel<T>(UnityAction<T> callBack) where T : BasePanel
    {
        string panelName = typeof(T).Name;
        if (panelDic.ContainsKey(panelName))
        {
            //取出字典中的数据
            PanelInfo<T> panelInfo = panelDic[panelName] as PanelInfo<T>;
            //正在加载中
            if (panelInfo.panel == null)
            {
                panelInfo.callBack += callBack;
            }
            else if (!panelInfo.isHide)//加载结束，且没有隐藏
            {
                callBack?.Invoke(panelInfo.panel);
            }
        }
    }
}

/// <summary>
/// 事件监听相关
/// </summary>
public partial class UIManager
{
    /// <summary>
    /// 为控件添加自定义事件
    /// </summary>
    /// <param name="control">控件</param>
    /// <param name="type">事件类型</param>
    /// <param name="callBack">响应函数</param>
    public static void AddCustomEventListener(UIBehaviour control,EventTriggerType type,UnityAction<BaseEventData> callBack)
    {
        EventTrigger trigger=control.GetComponent<EventTrigger>();
        if (trigger == null)
            trigger=control.gameObject.AddComponent<EventTrigger>();

        EventTrigger.Entry entry=new EventTrigger.Entry();
        entry.eventID = type;
        entry.callback.AddListener(callBack);

        trigger.triggers.Add(entry);
    }
}
