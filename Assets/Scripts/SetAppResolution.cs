using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetAppResolution : MonoBehaviour
{
    private void Awake()
    {
        Application.targetFrameRate = 60;
        //var x = Screen.currentResolution;
        //Screen.SetResolution(1080, 1920, true, 60);
    }
}
