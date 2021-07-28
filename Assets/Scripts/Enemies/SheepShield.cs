using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SheepShield : MonoBehaviour
{
    [HideInInspector]
    public Enemy enemy;
    [HideInInspector]
    public bool isActive;
    private SpriteRenderer sr;
    // Start is called before the
    // first frame update
    void Start()
    {
        enemy.isProtected = true;
        sr = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void Disabling()
    {
        enemy.isProtected = false;
        isActive = false;
        sr.enabled = false;
    }
}
