using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseButton : ScreenButton
{
    void Start()
    {
        ButtonDown.AddListener(PauseGame.Instance.ChangeState);
    }
}
