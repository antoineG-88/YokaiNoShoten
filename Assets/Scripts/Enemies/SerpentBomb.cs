using System.Collections;
using UnityEngine;

public class SerpentBomb : MonoBehaviour
{
    public float triggerDistance;
    public float maxLifeTime;
    public float explosionRadius;
    public int damage;
    public float knockbackForce;
    public float explodeDelay;
    public GameObject explosionFxPrefab;

    private float lifeTimeElapsed;
    private float distToPlayer;
    private bool exploding;

    private void Start()
    {
        lifeTimeElapsed = 0;
    }

    void Update()
    {
        distToPlayer = Vector2.Distance(GameData.player.transform.position, transform.position);

        if (distToPlayer < triggerDistance && !exploding)
        {
            StartCoroutine(Explode());
        }

        if(lifeTimeElapsed > maxLifeTime)
        {
            StartCoroutine(Disappear());
        }

        lifeTimeElapsed += Time.deltaTime;
    }

    private IEnumerator Disappear()
    {
        //Disappear anim
        yield return new WaitForSeconds(0.1f);
        Destroy(gameObject);
    }

    private IEnumerator Explode()
    {
        exploding = true;
        Instantiate(explosionFxPrefab, transform.position, Quaternion.identity).transform.localScale = new Vector2(explosionRadius / 2, explosionRadius / 2);

        yield return new WaitForSeconds(explodeDelay);

        if(Physics2D.OverlapCircle(transform.position, explosionRadius, LayerMask.GetMask("Player")))
        {
            Vector2 playerDirection = GameData.player.transform.position - transform.position;
            playerDirection.Normalize();
            GameData.playerManager.TakeDamage(damage, playerDirection * knockbackForce);
        }
        Destroy(gameObject);
    }
}
