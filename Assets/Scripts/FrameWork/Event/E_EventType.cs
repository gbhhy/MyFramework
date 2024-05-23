using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 事件类型枚举，需要用什么类型就现场添加
/// </summary>
public enum E_EventType
{
    //示例：
    ///// <summary>
    ///// 怪物死亡事件，参数Monster
    ///// </summary>
    //E_Monster_Dead,
    /// <summary>
    /// 获取场景加载进度
    /// </summary>
    E_SceneLoadProgress,

    E_Keyboard_Down,
    E_Keyboard_Up,
    E_Keyboard_Hold,

    E_Mouse_Down,
    E_Mouse_Up,
    E_Mouse_Hold,

    E_Axis_Horizontal,
    E_Axis_Vertical,
}
