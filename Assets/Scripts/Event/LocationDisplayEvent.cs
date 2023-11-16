using System.Collections;
using UnityEngine.UI;
using UnityEngine;

public class LocationDisplayEvent : EventPart
{
    public string textToDisplay;
    private Text chapterNameDisplay;

    public override void StartEventPart()
    {
        base.StartEventPart();
        StartCoroutine(DisplaySpecificLocationName(textToDisplay));
        EndEventPart();
    }

    private IEnumerator DisplaySpecificLocationName(string locationName)
    {
        chapterNameDisplay = GameObject.Find("ZoneNameDisplay").GetComponent<Text>();

        chapterNameDisplay.CrossFadeAlpha(0f, 0f, false);
        chapterNameDisplay.text = locationName;
        yield return new WaitForSeconds(0.7f);

        chapterNameDisplay.CrossFadeAlpha(1f, 0.5f, false);

        yield return new WaitForSeconds(4.0f);

        chapterNameDisplay.CrossFadeAlpha(0f, 0.5f, false);
    }
}
