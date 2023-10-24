using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonSelectFeedback : MonoBehaviour, IPointerEnterHandler
{
    public RectTransform rectToMove;
    public RectTransform rectToMove2;
    public Vector2 selectedPos;
    public float lerpSpeed;
    public bool isScaleModification;

    private Vector2 basePos;
    private Vector2 baseScale;

    private EventSystem eventSystem;
    void Start()
    {
        eventSystem = EventSystem.current;
        basePos = rectToMove.anchoredPosition;
        baseScale = rectToMove.localScale;
    }

    void Update()
    {
        if(isScaleModification)
        {
            rectToMove.localScale = Vector2.Lerp(rectToMove.localScale, (eventSystem.currentSelectedGameObject == gameObject) ? selectedPos : baseScale, lerpSpeed * Time.unscaledDeltaTime);
        }
        else
        {
            rectToMove.anchoredPosition = Vector2.Lerp(rectToMove.anchoredPosition, (eventSystem.currentSelectedGameObject == gameObject) ? selectedPos : basePos, lerpSpeed * Time.unscaledDeltaTime);
            rectToMove2.anchoredPosition = Vector2.Lerp(rectToMove2.anchoredPosition, (eventSystem.currentSelectedGameObject == gameObject) ? -selectedPos : -basePos, lerpSpeed * Time.unscaledDeltaTime);
        }
    }

    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        eventSystem.SetSelectedGameObject(gameObject);
    }
}
