using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SheepShield : MonoBehaviour
{
    public bool isInLaser;
    public Enemy enemy;
    // Start is called before the
    // first frame update
    void Start()
    {
        enemy.isProtected = true;
        isInLaser = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void Disabling()
    {
        if(isInLaser == true)
        {
            enemy.isProtected = false;
            Destroy(gameObject);
        }
    }
}
