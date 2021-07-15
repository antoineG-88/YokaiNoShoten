using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClueLight : Piercable
{
    public List<Animator> clues;
    public GameObject hitFxPrefab;
    [Tooltip("Set to 0 for permanent switch off")]
    public float maxTimeOff;
    [Header("Temporary")]
    public List<GameObject> objectToDisableOnHit;

    [HideInInspector] public bool isOff;
    private float offTimeElapsed;

    private void Start()
    {
        foreach (Animator clue in clues)
        {
            clue.SetBool("isOff", false);
        }
    }

    private void Update()
    {
        if(isOff && maxTimeOff > 0)
        {
            if (offTimeElapsed > maxTimeOff)
            {
                StartCoroutine(SwitchOn());
            }
            offTimeElapsed += Time.deltaTime;
        }
    }

    public override bool PierceEffect(int damage, Vector2 directedForce)
    {
        StartCoroutine(SwitchOff());
        Debug.Log("pierced");
        return false;
    }

    public IEnumerator SwitchOff()
    {
        isOff = true;
        offTimeElapsed = 0;
        foreach (Animator clue in clues)
        {
            clue.SetBool("isOff", true);
        }
        //effect on light
        foreach (GameObject gameObject in objectToDisableOnHit)
        {
            gameObject.SetActive(false);
        }

        if(hitFxPrefab != null)
            Instantiate(hitFxPrefab, transform.position, Quaternion.identity);

        yield return null;
    }

    public IEnumerator SwitchOn()
    {
        isOff = false;
        foreach (Animator clue in clues)
        {
            clue.SetBool("isOff", false);
        }

        foreach (GameObject gameObject in objectToDisableOnHit)
        {
            gameObject.SetActive(true);
        }
        yield return null;
    }
}
