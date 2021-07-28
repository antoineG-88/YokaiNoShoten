using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SheepShield : MonoBehaviour
{
    public bool isInLaser;
    // Start is called before the first frame update
    void Start()
    {
        GetComponentInParent<Sheep>().isProtected = true;
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
            GetComponentInParent<Enemy>().isProtected = false;
            Destroy(gameObject);
        }
    }
}
