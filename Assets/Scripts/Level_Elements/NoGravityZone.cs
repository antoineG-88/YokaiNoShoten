using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoGravityZone : MonoBehaviour
{
    public float minSpeedInNGZone;
    public float momentumSlowingForce;
    public float maxSpeedInNGZone;
    public float aboveMaxMomentumSlowingForce;
    public float maxTractingSpeedInNGZone;
    [Header("Display settings")]
    public LineRenderer edgeLine;
    public List<Transform> corners;
    public GameObject enterEffectPrefab;
    public GameObject exitEffectPrefab;

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
            GameData.grappleHandler.noGravityMaxTractionSpeed = maxTractingSpeedInNGZone;
            PlayEnterEffect(collision.transform.position);
        }

        potentialSerpent = collision.gameObject.GetComponent<Serpent>();
        if (potentialSerpent != null)
        {
            potentialSerpent.isInNoGravityZone = true;
            PlayEnterEffect(collision.transform.position);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            GameData.movementHandler.currentGravityZone = null;
            PlayExitEffect(collision.transform.position);
        }


        potentialSerpent = collision.gameObject.GetComponent<Serpent>();
        if (potentialSerpent != null)
        {
            potentialSerpent.isInNoGravityZone = false;
            PlayExitEffect(collision.transform.position);
        }
    }

    private void PlayEnterEffect(Vector2 enterPos)
    {
        Vector2 direction = enterPos - (Vector2)transform.position;
        float angle = Vector2.SignedAngle(Vector2.right, direction) - transform.rotation.eulerAngles.z;
        direction = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad)) * direction.magnitude;
        Vector2 newDirection;
        float newAngle;

        if (direction.x > transform.localScale.x / 2f)
        {
            newDirection = new Vector2(transform.localScale.x / 2f, direction.y);
            newAngle = Vector2.SignedAngle(Vector2.right, newDirection) + transform.rotation.eulerAngles.z;
            newDirection = new Vector2(Mathf.Cos(newAngle * Mathf.Deg2Rad), Mathf.Sin(newAngle * Mathf.Deg2Rad)) * newDirection.magnitude;

            Instantiate(enterEffectPrefab, new Vector3(transform.position.x + newDirection.x, transform.position.y + newDirection.y, 0.0f), Quaternion.Euler(0f, 0f, -90f + transform.rotation.eulerAngles.z));
        }
        else if(direction.x < -transform.localScale.x / 2f)
        {
            newDirection = new Vector2(-transform.localScale.x / 2f, direction.y);
            newAngle = Vector2.SignedAngle(Vector2.right, newDirection) + transform.rotation.eulerAngles.z;
            newDirection = new Vector2(Mathf.Cos(newAngle * Mathf.Deg2Rad), Mathf.Sin(newAngle * Mathf.Deg2Rad)) * newDirection.magnitude;

            Instantiate(enterEffectPrefab, new Vector3(transform.position.x + newDirection.x, transform.position.y + newDirection.y, 0.0f), Quaternion.Euler(0f, 0f, 90f + transform.rotation.eulerAngles.z));
        }
        else if (direction.y > transform.localScale.y / 2f)
        {
            newDirection = new Vector2(direction.x, transform.localScale.y / 2f);
            newAngle = Vector2.SignedAngle(Vector2.right, newDirection) + transform.rotation.eulerAngles.z;
            newDirection = new Vector2(Mathf.Cos(newAngle * Mathf.Deg2Rad), Mathf.Sin(newAngle * Mathf.Deg2Rad)) * newDirection.magnitude;

            Instantiate(enterEffectPrefab, new Vector3(transform.position.x + newDirection.x, transform.position.y + newDirection.y, 0.0f), Quaternion.Euler(0f, 0f, 0f + transform.rotation.eulerAngles.z));
        }
        else if (direction.y < -transform.localScale.y / 2f)
        {
            newDirection = new Vector2(direction.x, -transform.localScale.y / 2f);
            newAngle = Vector2.SignedAngle(Vector2.right, newDirection) + transform.rotation.eulerAngles.z;
            newDirection = new Vector2(Mathf.Cos(newAngle * Mathf.Deg2Rad), Mathf.Sin(newAngle * Mathf.Deg2Rad)) * newDirection.magnitude;

            Instantiate(enterEffectPrefab, new Vector3(transform.position.x + newDirection.x, transform.position.y + newDirection.y, 0.0f), Quaternion.Euler(0f, 0f, 180f + transform.rotation.eulerAngles.z));
        }
    }

    private void PlayExitEffect(Vector2 enterPos)
    {
        Vector2 direction = enterPos - (Vector2)transform.position;
        float angle = Vector2.SignedAngle(Vector2.right, direction) - transform.rotation.eulerAngles.z;
        direction = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad)) * direction.magnitude;
        Vector2 newDirection;
        float newAngle;

        if (direction.x > transform.localScale.x / 2f)
        {
            newDirection = new Vector2(transform.localScale.x / 2f, direction.y);
            newAngle = Vector2.SignedAngle(Vector2.right, newDirection) + transform.rotation.eulerAngles.z;
            newDirection = new Vector2(Mathf.Cos(newAngle * Mathf.Deg2Rad), Mathf.Sin(newAngle * Mathf.Deg2Rad)) * newDirection.magnitude;

            Instantiate(exitEffectPrefab, new Vector3(transform.position.x + newDirection.x, transform.position.y + newDirection.y, 0.0f), Quaternion.Euler(0f, 0f, -90f + transform.rotation.eulerAngles.z));
        }
        else if (direction.x < -transform.localScale.x / 2f)
        {
            newDirection = new Vector2(-transform.localScale.x / 2f, direction.y);
            newAngle = Vector2.SignedAngle(Vector2.right, newDirection) + transform.rotation.eulerAngles.z;
            newDirection = new Vector2(Mathf.Cos(newAngle * Mathf.Deg2Rad), Mathf.Sin(newAngle * Mathf.Deg2Rad)) * newDirection.magnitude;

            Instantiate(exitEffectPrefab, new Vector3(transform.position.x + newDirection.x, transform.position.y + newDirection.y, 0.0f), Quaternion.Euler(0f, 0f, 90f + transform.rotation.eulerAngles.z));
        }
        else if (direction.y > transform.localScale.y / 2f)
        {
            newDirection = new Vector2(direction.x, transform.localScale.y / 2f);
            newAngle = Vector2.SignedAngle(Vector2.right, newDirection) + transform.rotation.eulerAngles.z;
            newDirection = new Vector2(Mathf.Cos(newAngle * Mathf.Deg2Rad), Mathf.Sin(newAngle * Mathf.Deg2Rad)) * newDirection.magnitude;

            Instantiate(exitEffectPrefab, new Vector3(transform.position.x + newDirection.x, transform.position.y + newDirection.y, 0.0f), Quaternion.Euler(0f, 0f, 0f + transform.rotation.eulerAngles.z));
        }
        else if (direction.y < -transform.localScale.y / 2f)
        {
            newDirection = new Vector2(direction.x, -transform.localScale.y / 2f);
            newAngle = Vector2.SignedAngle(Vector2.right, newDirection) + transform.rotation.eulerAngles.z;
            newDirection = new Vector2(Mathf.Cos(newAngle * Mathf.Deg2Rad), Mathf.Sin(newAngle * Mathf.Deg2Rad)) * newDirection.magnitude;

            Instantiate(exitEffectPrefab, new Vector3(transform.position.x + newDirection.x, transform.position.y + newDirection.y, 0.0f), Quaternion.Euler(0f, 0f, 180f + transform.rotation.eulerAngles.z));
        }
    }
}
