using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputInfo
{
    public enum E_InputType
    {
        Keyboard,
        Gamepad,
        Mouse,
    }

    public E_InputType type;
    public KeyCode key;
    public int mouseID;
    public string buttonName;
    public InputInfo( KeyCode key)
    {
        this.type=E_InputType.Keyboard;
        this.key = key;
    }
    public InputInfo(int mouseID)
    {
        this.type = E_InputType.Mouse;
        this.mouseID = mouseID;
    }
    public InputInfo(string buttonName)
    {
        this.type = E_InputType.Gamepad;
        this.buttonName = buttonName;
    }
}
