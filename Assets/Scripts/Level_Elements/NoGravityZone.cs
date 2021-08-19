using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoGravityZone : MonoBehaviour
{
    public float minSpeedInNGZone;
    public float momentumSlowingForce;
    public float maxSpeedInNGZone;
    public float aboveMaxMomentumSlowingForce;
    [Header("Display settinngs")]
    public LineRenderer edgeLine;
    public List<Transform> corners;

    private Serpent potentialSerpent;
    private Material material;

    private void Start()
    {
        material = Instantiate(GetComponent<SpriteRenderer>().sharedMaterial);
        material.SetVector("_tiling", new Vector2(transform.localScale.x, transform.localScale.y));
        GetComponent<SpriteRenderer>().sharedMaterial = material;
    }

    public void SetEdgeLines()
    {
        edgeLine.SetPosition(0, corners[0].position);
        edgeLine.SetPosition(1, corners[1].position);
        edgeLine.SetPosition(2, corners[2].position);
        edgeLine.SetPosition(3, corners[3].position);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            GameData.movementHandler.currentGravityZone = this;
        }

        potentialSerpent = collision.gameObject.GetComponent<Serpent>();
        if (potentialSerpent != null)
        {
            potentialSerpent.isInNoGravityZone = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            GameData.movementHandler.currentGravityZone = null;
        }


        potentialSerpent = collision.gameObject.GetComponent<Serpent>();
        if (potentialSerpent != null)
        {
            potentialSerpent.isInNoGravityZone = false;
        }
    }
}
