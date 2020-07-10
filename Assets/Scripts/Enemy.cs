using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int maxHealthPoint;

    private int currentHealthPoint;

    protected Rigidbody2D rb;

    private Animator tempAnimator; // to remove

    void Start()
    {
        tempAnimator = GetComponent<Animator>(); // to remove
        rb = GetComponent<Rigidbody2D>();
        currentHealthPoint = maxHealthPoint;
    }

    public void TakeDamage(int damage, Vector2 directedForce)
    {
        currentHealthPoint -= damage;
        Knockback(directedForce);
        tempAnimator.SetTrigger("Hurt");
        if(currentHealthPoint <= 0)
        {
            Die();
        }
    }

    public void Knockback(Vector2 directedForce)
    {
        rb.velocity += directedForce;
    }

    private void Die()
    {
        Destroy(gameObject, 0.5f);
    }
}
