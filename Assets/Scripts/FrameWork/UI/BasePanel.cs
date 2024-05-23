using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public abstract class BasePanel : MonoBehaviour
{
    /// <summary>
    /// ���ڴ洢��������е�UI�ؼ�
    /// </summary>
    protected Dictionary<string, UIBehaviour> controlDic = new Dictionary<string, UIBehaviour>();
    /// <summary>
    /// �ؼ�Ĭ�����֣����ؼ��������ڴ���������ÿؼ�����ͨ������ʹ�ã�ֻ����ʾ����
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
        //Ϊ����ĳһ�������ϴ��������������ͬ������button�ϵ�image
        //Ӧ���Ȳ�����Ҫ��������ҵ����ֵ���ڴ˼�ֵ�ԣ��㲻���ظ�����
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
    /// �����ʾʱ���õ��߼�
    /// </summary>
    public abstract void ShowMe();
    /// <summary>
    /// ����ʱ���õ��߼�
    /// </summary>
    public abstract void HideMe();

    /// <summary>
    /// ��ȡָ������ָ�����͵����
    /// </summary>
    /// <typeparam name="T">�������</typeparam>
    /// <param name="name">�������</param>
    /// <returns></returns>
    public T GetControl<T>(string name)where T : UIBehaviour
    {
        if(controlDic.ContainsKey(name))
        {
            T control = controlDic[name] as T;
            if(control == null)
                Debug.LogError($"�����ڶ�Ӧ����{name}����Ϊ{typeof(T)}�����");
            return control;
        }
        else
        {
            Debug.LogError($"�����ڶ�Ӧ���ֵ����{name}");
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

                    //�жϿؼ����ͣ������Ƿ������¼�����
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