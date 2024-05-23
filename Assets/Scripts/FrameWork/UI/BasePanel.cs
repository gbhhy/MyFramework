using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public abstract class BasePanel : MonoBehaviour
{
    /// <summary>
    /// 用于存储面板上所有的UI控件
    /// </summary>
    protected Dictionary<string, UIBehaviour> controlDic = new Dictionary<string, UIBehaviour>();
    /// <summary>
    /// 控件默认名字，若控件名存在于此容器，则该控件不会通过代码使用，只起到显示作用
    /// </summary>
    private static List<string> controlDefaultNames = new List<string>() { "Image",
                                                                                                                                "Text (TMP)",
                                                                                                                                "RawImage",
                                                                                                                                "Background",
                                                                                                                                "Checkmark",
                                                                                                                                "Label",
                                                                                                                                "Text (Legacy)",
                                                                                                                                "Arrow",
                                                                                                                                "Placeholder",
                                                                                                                                "Fill",
                                                                                                                                "Handle",
                                                                                                                                "Viewport",
                                                                                                                                "Scrollbar Horizontal",
                                                                                                                                "Scrollbar Vertical"};
    protected virtual void Awake()
    {
        //为避免某一个对象上存在两种组件导致同名：如button上的image
        //应优先查找重要的组件，找到后字典存在此键值对，便不会重复查找
        FindChildrenControl<Button>();
        FindChildrenControl<Toggle>();
        FindChildrenControl<Slider>();
        FindChildrenControl<InputField>();
        FindChildrenControl<ScrollRect>();
        FindChildrenControl<Dropdown>();

        FindChildrenControl<Text>();
        FindChildrenControl<TextMeshPro>();
        FindChildrenControl<Image>();
    }

    /// <summary>
    /// 面板显示时调用的逻辑
    /// </summary>
    public abstract void ShowMe();
    /// <summary>
    /// 隐藏时调用的逻辑
    /// </summary>
    public abstract void HideMe();

    /// <summary>
    /// 获取指定名字指定类型的组件
    /// </summary>
    /// <typeparam name="T">组件类型</typeparam>
    /// <param name="name">组件名字</param>
    /// <returns></returns>
    public T GetControl<T>(string name)where T : UIBehaviour
    {
        if(controlDic.ContainsKey(name))
        {
            T control = controlDic[name] as T;
            if(control == null)
                Debug.LogError($"不存在对应名字{name}类型为{typeof(T)}的组件");
            return control;
        }
        else
        {
            Debug.LogError($"不存在对应名字的组件{name}");
            return null;
        }
        
    }

    protected virtual void ClickBtn(string btnName)
    {

    }
    protected virtual void SliderValueChange(string sliderName,float value)
    {

    }
    protected virtual void ToggleValueChange(string toggleName, bool value)
    {

    }

    private void FindChildrenControl<T>() where T : UIBehaviour
    {
        T[] controls = this.GetComponentsInChildren<T>(true);
        foreach (T control in controls)
        {
            string controlName = control.gameObject.name;
            if (!controlDic.ContainsKey(controlName))
            {
                if (!controlDefaultNames.Contains(controlName))
                {
                    controlDic.Add(controlName, control);

                    //判断控件类型，决定是否添加事件监听
                    if(control is Button)
                    {
                        (control as Button).onClick.AddListener(() =>
                        {
                            ClickBtn(controlName);
                        });
                    }
                    else if (control is Slider)
                    {
                        (control as Slider).onValueChanged.AddListener((value) =>
                        {
                            SliderValueChange(controlName, value);
                        });
                    }
                    else if (control is Toggle)
                    {
                        (control as Toggle).onValueChanged.AddListener((value) =>
                        {
                            ToggleValueChange(controlName, value);
                        });
                    }
                }

            }
        }
    }
}
