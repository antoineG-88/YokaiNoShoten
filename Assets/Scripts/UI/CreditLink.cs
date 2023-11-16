using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreditLink : MonoBehaviour
{
    public string memberLink;

    public void OpenLink()
    {
        Application.OpenURL(memberLink);
    }
}
