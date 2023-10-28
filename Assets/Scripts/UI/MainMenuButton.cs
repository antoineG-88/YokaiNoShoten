﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MainMenuButton : MonoBehaviour, IPointerEnterHandler, ISelectHandler
{
    public RectTransform rectToMove;
    public Vector2 selectedPos;
    public UISoundManager uISoundManager;
    public AudioClip selectSound;

    private Vector2 basePos;

    private EventSystem eventSystem;
    void Start()
    {
        eventSystem = EventSystem.current;
        basePos = rectToMove.anchoredPosition;
    }

    void Update()
    {
        rectToMove.anchoredPosition = Vector2.Lerp(rectToMove.anchoredPosition, (eventSystem.currentSelectedGameObject == gameObject) ? selectedPos : basePos, 18f * Time.deltaTime);
    }

    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        eventSystem.SetSelectedGameObject(gameObject);
    }

    public void OnSelect(BaseEventData eventData)
    {
        uISoundManager.PlayUISound(selectSound);
    }
}
