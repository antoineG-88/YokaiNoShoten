using UnityEngine;

public class BodyPart : MonoBehaviour
{
    [HideInInspector] public Enemy owner;

    private ProtectionHandler protectionHandler;

    private void Start()
    {
        protectionHandler = GetComponent<ProtectionHandler>();
    }

    public void ReceiveDamage(int damage, Vector2 knockback, float noControlTime)
    {
        if(!owner.recentlyHit)
        {
            if (protectionHandler != null)
            {
                if (!protectionHandler.IsProtected(knockback))
                {
                    owner.TakeDamage(damage, knockback, noControlTime);
                }
            }
            else
            {
                owner.TakeDamage(damage, knockback, noControlTime);
            }

            owner.recentlyHit = true;
        }
    }
}
