using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
/// <summary>
/// �㼶ö��
/// </summary>
public enum E_UILayer
{
    Bottom,
    Middle,
    Top,
    System
}

/// <summary>
/// ��ʼ�����㼶���
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
        //��̬����Ψһ��Canvas��EventSystem������֤���������Ƴ�


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
    /// ��ȡ��Ӧ�㼶
    /// </summary>
    /// <param name="layer">�㼶ö��</param>
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
/// ���������
/// ���Ԥ��������Ҫ���������һ�·���ʹ�ã�
/// </summary>
public partial class UIManager : BaseManager<UIManager>
{
    /// <summary>
    /// �������滻ԭ����Dictionary���ܴ��治ȷ���ķ����������
    /// </summary>
    private abstract class BasePanelInfo { }
    /// <summary>
    /// ���ڴ洢�����Ϣ�ͼ��ػص�����
    /// </summary>
    /// <typeparam name="T">�������</typeparam>
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

    //�洢�������
    private Dictionary<string, BasePanelInfo> panelDic = new Dictionary<string, BasePanelInfo>();

    /// <summary>
    /// ��ʾ���
    /// </summary>
    /// <typeparam name="T">�������</typeparam>
    /// <param name="layer">��ʾ�㼶��Ĭ��Ϊ�в�</param>
    /// <param name="callBack">�ص������ڿ����첽���أ�������ί�д������</param>
    /// <param name="isSync">�Ƿ�ͬ�����أ�Ĭ���첽</param>
    public void ShowPanel<T>(E_UILayer layer = E_UILayer.Middle, UnityAction<T> callBack = null, bool isSync = false) where T : BasePanel
    {
        string panelName = typeof(T).Name;
        if (panelDic.ContainsKey(panelName))//�������
        {
            //ȡ���ֵ��е�����
            PanelInfo<T> panelInfo = panelDic[panelName] as PanelInfo<T>;

            //�����첽������
            if (panelInfo.panel == null)
            {
                panelInfo.isHide = false;
                //�ȴ�������ϣ�ֻ��Ҫ��¼�ص�������������Ϻ����
                if (callBack != null)
                    panelInfo.callBack += callBack;
            }
            else//�Ѿ����ؽ���
            {
                if(!panelInfo.panel.gameObject.activeSelf)//�����ʧ��״̬��ֱ�Ӽ���
                {
                    panelInfo.panel.gameObject.SetActive(true);
                }
                panelInfo.panel.ShowMe();
                callBack?.Invoke(panelInfo.panel);

            }

            return;
        }
        //��������壬�ȴ����ֵ��У�ռ��λ�ã���ʾʱ���ܵõ��ֵ�����Ϣ�����ж�
        panelDic.Add(panelName, new PanelInfo<T>(callBack));
        ABResManager.Instance.LoadResAsync<GameObject>("ui", panelName, (panelPrefab) =>
        {
            //ȡ���ֵ��е�����
            PanelInfo<T> panelInfo = panelDic[panelName] as PanelInfo<T>;
            if (panelInfo.isHide)//�첽���ؽ���ǰ�������Ƴ��������
            {
                panelDic.Remove(panelName);
                return;
            }
            Transform parent = GetLayer(layer);
            //�����Ԥ���崴������Ӧ�������£�������ԭ�����Ŵ�С
            GameObject panelObj = GameObject.Instantiate(panelPrefab, parent, false);



            T panel = panelObj.GetComponent<T>();
            panel.ShowMe();
            panelInfo.callBack?.Invoke(panel);
            //�ص�ִ���꣬������գ������ڴ�й©
            panelInfo.callBack = null;
            //�洢panel
            panelInfo.panel = panel;
        }, isSync);
    }

/// <summary>
/// �������
/// </summary>
/// <typeparam name="T">�������</typeparam>
/// <param name="isDestroy">�����������ʱ�Ƿ����٣��ڴ�ѹ����ʱ���ٱ������������ڴ�ѹ��Сʱʧ�����Ƶ��GC��ɿ���</param>
    public void HidePanel<T>(bool isDestroy = false) where T : BasePanel
    {
        string panelName = typeof(T).Name;
        if (panelDic.ContainsKey(panelName))
        {
            //ȡ���ֵ��е�����
            PanelInfo<T> panelInfo = panelDic[panelName] as PanelInfo<T>;
            //���ڼ�����
            if (panelInfo.panel == null)
            {
                panelInfo.isHide = true;
                panelInfo.callBack = null;
            }
            else
            {
                //�Ѿ����ؽ���
                panelInfo.panel.HideMe();
                if (isDestroy)
                {
                    GameObject.Destroy(panelInfo.panel.gameObject);
                    panelDic.Remove(panelName);
                }
                else//�������٣���ֻ��ʧ��´���ʾʱֱ�Ӹ���
                    panelInfo.panel.gameObject.SetActive(false);
            }

        }
    }

    /// <summary>
    /// �õ����
    /// </summary>
    /// <typeparam name="T">�������</typeparam>
    public void GetPanel<T>(UnityAction<T> callBack) where T : BasePanel
    {
        string panelName = typeof(T).Name;
        if (panelDic.ContainsKey(panelName))
        {
            //ȡ���ֵ��е�����
            PanelInfo<T> panelInfo = panelDic[panelName] as PanelInfo<T>;
            //���ڼ�����
            if (panelInfo.panel == null)
            {
                panelInfo.callBack += callBack;
            }
            else if (!panelInfo.isHide)//���ؽ�������û������
            {
                callBack?.Invoke(panelInfo.panel);
            }
        }
    }
}

/// <summary>
/// �¼��������
/// </summary>
public partial class UIManager
{
    /// <summary>
    /// Ϊ�ؼ�����Զ����¼�
    /// </summary>
    /// <param name="control">�ؼ�</param>
    /// <param name="type">�¼�����</param>
    /// <param name="callBack">��Ӧ����</param>
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
