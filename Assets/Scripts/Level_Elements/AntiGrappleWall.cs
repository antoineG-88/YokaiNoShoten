using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AntiGrappleWall : MonoBehaviour
{
    public Transform firstPos;
    public Transform secondPos;

    private LineRenderer line;
    private Vector3[] linePos;
    private Material lineMat;

    private void Start()
    {
        line = GetComponent<LineRenderer>();
        linePos = new Vector3[2];
        linePos[0] = firstPos.position;
        linePos[1] = secondPos.position;
        line.SetPositions(linePos);
        lineMat = Instantiate(line.sharedMaterial);

    }

    public void PlayFeedBack()
    {
        //VK à ton tour !!!
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
