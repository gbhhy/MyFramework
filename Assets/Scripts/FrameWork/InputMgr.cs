using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputMgr : BaseManager<InputMgr>
{
    private bool isStart;//是否开启输入检测
    private InputInfo nowInputInfo;

    private Dictionary<E_EventType, InputInfo> inputDic;
    private InputMgr()
    {
        MonoManager.Instance.AddUpdateListener(InputUpdate);
    }

    private void InputUpdate()
    {
        if (!isStart)
        {
            return;
        }
        foreach (var eventType in inputDic.Keys)
        {
            nowInputInfo = inputDic[eventType];
            switch (nowInputInfo.type)
            {
                case InputInfo.E_InputType.Keyboard:
                    if (Input.GetKeyDown(nowInputInfo.key))
                        EventCenter.Instance.EventTrigger(eventType);
                    break;
                case InputInfo.E_InputType.Mouse:
                    if (Input.GetMouseButton(nowInputInfo.mouseID))
                        EventCenter.Instance.EventTrigger(eventType);
                    break;
                case InputInfo.E_InputType.Gamepad:
                    if (Input.GetButtonDown(nowInputInfo.buttonName))
                        EventCenter.Instance.EventTrigger(eventType);
                    break;
            }

        }
        EventCenter.Instance.EventTrigger(E_EventType.E_Axis_Horizontal, Input.GetAxis("Horizontal"));
        EventCenter.Instance.EventTrigger(E_EventType.E_Axis_Vertical, Input.GetAxis("Vertical"));
    }
    public void StartOrCloseInputMgr(bool isStart)
    { this.isStart = isStart; }

    public void ChangeKeyboardInfo(E_EventType eventType, KeyCode key)
    {
        if (!inputDic.ContainsKey(eventType))//初始化
        {
            inputDic.Add(eventType, new InputInfo(key));
        }
        else
        {
            inputDic[eventType].key = key;
        }
    }

    public void ChangeMouseInfo(E_EventType eventType, int mouseID)
    {
        if (!inputDic.ContainsKey(eventType))//初始化
        {
            inputDic.Add(eventType, new InputInfo(mouseID));
        }
        else
        {
            inputDic[eventType].mouseID = mouseID;
        }
    }

    public void ChangeGamepadInfo(E_EventType eventType, string buttonName)
    {
        if (!inputDic.ContainsKey(eventType))//初始化
        {
            inputDic.Add(eventType, new InputInfo(buttonName));
        }
        else
        {
            inputDic[eventType].buttonName = buttonName;
        }
    }

    public void RemoveInputInfo(E_EventType eventType)
    {
        if (inputDic.ContainsKey(eventType))
            inputDic.Remove(eventType);
    }
}
