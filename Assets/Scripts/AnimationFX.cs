using UnityEngine;

public class AnimationFX : MonoBehaviour
{
    public AnimationClip fxAnimation;

    void Start()
    {
        Invoke("AutoDestroy", fxAnimation.length - 0.01f);
    }

    void AutoDestroy()
    {
        Destroy(gameObject);
    }

}
