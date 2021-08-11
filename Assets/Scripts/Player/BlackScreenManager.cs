using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlackScreenManager : MonoBehaviour
{
    public Image blackScreenImage;

    public static Image blackScreen;

    private void Start()
    {
        blackScreen = blackScreenImage;
    }
}
