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
    public bool isCustomShape;
    public List<Transform> corners;
    public GameObject enterEffectPrefab;
    public GameObject exitEffectPrefab;
    public float shaderTilingScale;
    [Header("Sounds")]
    public AudioSource ambientSource;
    public Sound enterSound;
    public Sound exitSound;

    private Serpent potentialSerpent;
    private Material material;
    private Collider2D col;
    private PolygonCollider2D shapeCol;

    private void FixedUpdate()
    {
        ambientSource.pitch = Time.timeScale;
    }

    private void Start()
    {
        material = Instantiate(GetComponent<SpriteRenderer>().sharedMaterial);
        material.SetVector("_tiling", new Vector2(transform.localScale.x * shaderTilingScale, transform.localScale.y * shaderTilingScale));
        GetComponent<SpriteRenderer>().sharedMaterial = material;
        col = GetComponent<Collider2D>();
        if (isCustomShape)
            shapeCol = col as PolygonCollider2D;
    }

    public void SetEdgeLines()
    {
        if(!isCustomShape)
        {
            edgeLine.positionCount = 4;
            edgeLine.SetPosition(0, corners[0].position);
            edgeLine.SetPosition(1, corners[1].position);
            edgeLine.SetPosition(2, corners[2].position);
            edgeLine.SetPosition(3, corners[3].position);
        }
        else
        {
            col = GetComponent<Collider2D>();
            shapeCol = col as PolygonCollider2D;
            edgeLine.positionCount = shapeCol.points.Length;
            edgeLine.useWorldSpace = false;
            for (int i = 0; i < shapeCol.points.Length; i++)
            {
                edgeLine.SetPosition(i, new Vector2(shapeCol.points[i].x, shapeCol.points[i].y));
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            GameData.movementHandler.currentGravityZone = this;
            GameData.grappleHandler.noGravityMaxTractionSpeed = maxTractingSpeedInNGZone;
            GameData.audioManager.EnableNoGravityMixerEffects();
            ambientSource.Play();
            GameData.playerSource.PlayOneShot(enterSound.clip, enterSound.volumeScale);
            PlayEffect(collision.transform.position, enterEffectPrefab);
        }

        potentialSerpent = collision.gameObject.GetComponent<Serpent>();
        if (potentialSerpent != null)
        {
            potentialSerpent.isInNoGravityZone = true;
            PlayEffect(collision.transform.position, enterEffectPrefab);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            GameData.movementHandler.currentGravityZone = null;
            PlayEffect(collision.transform.position, exitEffectPrefab);
            GameData.audioManager.DisableNoGravityMixerEffects();
            GameData.playerSource.PlayOneShot(exitSound.clip, exitSound.volumeScale);
            ambientSource.Stop();
        }


        potentialSerpent = collision.gameObject.GetComponent<Serpent>();
        if (potentialSerpent != null)
        {
            potentialSerpent.isInNoGravityZone = false;
            PlayEffect(collision.transform.position, exitEffectPrefab);
        }
    }

    private void PlayEffect(Vector2 enterPos, GameObject effectPrefab)
    {
        RaycastHit2D hit = Physics2D.Raycast(enterPos, col.ClosestPoint(enterPos) - enterPos, 100f, LayerMask.GetMask("PlayerInteract"));
        if (hit)
        {
            Instantiate(effectPrefab, col.ClosestPoint(enterPos), Quaternion.Euler(0f, 0f, -90f + Vector2.SignedAngle(Vector2.right, hit.normal)));
        }
    }
}
