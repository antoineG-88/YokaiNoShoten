using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sheep : Enemy
{
    public Collider2D shieldingRange;
    public Enemy[] protectedEnemies;
    public GameObject shield;
    // Start is called before the first frame update
    new void Start()
    {
        Shield();
    }

    // Update is called once per frame
    new void Update()
    {
        
    }
    new void FixedUpdate()
    {

    }
    public override void UpdateMovement()
    {
        throw new System.NotImplementedException();
    }
    public override void DamageEffect()
    {
        throw new System.NotImplementedException();
    }
    protected override void UpdateBehavior()
    {

    }
    void Shield()
    {
        for (int i = 0; i < protectedEnemies.Length; i++)
        {
            Instantiate(shield, protectedEnemies[i].transform);
        }
    }
}
