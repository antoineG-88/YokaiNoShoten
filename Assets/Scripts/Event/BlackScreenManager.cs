using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlackScreenManager : MonoBehaviour
{
    public Image blackScreenImage;
    public Image seikiLoadImage;
    public CanvasGroup blackScreenCanvasGroup;

    public static Image blackScreen;
    public static Image seikiLoad;
    public static CanvasGroup blackScreenGroup;

    private void Awake()
    {
        blackScreen = blackScreenImage;
        seikiLoad = seikiLoadImage;
        blackScreenGroup = blackScreenCanvasGroup;
    }

    public static void SetAlpha(float alpha)
    {
        blackScreenGroup.alpha = alpha;
    }
    public static void SetLoadActive(bool active)
    {
        seikiLoad.enabled = active;
    }
}
