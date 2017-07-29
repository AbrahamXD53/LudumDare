using GamepadInput;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PlayerInput
{
    public bool useKeyboard = false;
    public GamePad.Index controlIndex;
    public int realIndex = 0;
    public bool ready = false;
}
