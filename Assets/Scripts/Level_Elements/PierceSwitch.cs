using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PierceSwitch : Switch
{
    public Color activatedColor;

    private SpriteRenderer spriteRenderer;
    private Color baseColor;

    public override bool PierceEffect(int damage, Vector2 directedForce)
    {
        SwitchOnOff();
        return false;
    }

    public override void Start()
    {
        base.Start();
        spriteRenderer = GetComponent<SpriteRenderer>();
        baseColor = spriteRenderer.color;
    }

    private void Update()
    {
        spriteRenderer.color = isOn ? activatedColor : baseColor;
    }
}
