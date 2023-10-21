using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MainMenuParallaxHandler : MonoBehaviour
{
    public List<ParallaxPlan> parallaxPlans;
    public float parallaxLerpSpeed;
    public float parallaxMovementSpeed;

    private Vector2 parallaxInput;
    private Vector2 parallaxTargetOffset;

    void Start()
    {
        
    }

    void Update()
    {
        if(GameManager.isUsingController)
        {
            if(EventSystem.current.currentSelectedGameObject != null)
            {
                parallaxInput = EventSystem.current.currentSelectedGameObject.GetComponent<RectTransform>().position;
            }
            else
            {
                parallaxInput = Vector2.zero;
            }
        }
        else
        {
            parallaxInput = new Vector2(Input.mousePosition.x - (Screen.width / 2f), Input.mousePosition.y - (Screen.height / 2f));
        }

        parallaxTargetOffset = Vector2.Lerp(parallaxTargetOffset, parallaxInput, parallaxLerpSpeed * Time.deltaTime);

        for (int i = 0; i < parallaxPlans.Count; i++)
        {
            parallaxPlans[i].planImage.rectTransform.anchoredPosition = parallaxTargetOffset * parallaxPlans[i].planMovementRatio * parallaxMovementSpeed;
        }
    }

    [System.Serializable]
    public class ParallaxPlan
    {
        public Image planImage;
        public float planMovementRatio;
    }
}
