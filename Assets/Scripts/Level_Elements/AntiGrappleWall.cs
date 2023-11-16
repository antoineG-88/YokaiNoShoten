using System.Collections;
using UnityEngine;

public class AntiGrappleWall : MonoBehaviour
{
    public Transform firstPos;
    public Transform secondPos;
    [ColorUsage(true, true)]
    public Color effectColor;

    private LineRenderer line;
    private Vector3[] linePos;
    private Material lineMat;
    private Color baseLineColor;

    private void Start()
    {
        line = GetComponent<LineRenderer>();
        linePos = new Vector3[2];
        linePos[0] = firstPos.position;
        linePos[1] = secondPos.position;
        line.SetPositions(linePos);
        lineMat = Instantiate(line.sharedMaterial);
        baseLineColor = lineMat.GetColor("_color");
        line.sharedMaterial = lineMat;
    }

    public void PlayFeedBack()
    {
        StartCoroutine(BlockGrappleEffect());
    }

    private IEnumerator BlockGrappleEffect()
    {
        lineMat.SetColor("_color", effectColor);
        float timer = 0;
        while(timer < 0.5f)
        {
            yield return new WaitForEndOfFrame();
            lineMat.SetColor("_color", Color.Lerp(effectColor, baseLineColor, timer / 0.5f));
            timer += Time.deltaTime;
        }
        lineMat.SetColor("_color", baseLineColor);
    }

    public void OnDrawGizmosSelected()
    {
        line = GetComponent<LineRenderer>();
        linePos = new Vector3[2];
        linePos[0] = firstPos.position;
        linePos[1] = secondPos.position;
        line.SetPositions(linePos);
        lineMat = Instantiate(line.sharedMaterial);
    }
}
