using System.Collections;
using UnityEngine;

namespace OASIS;

public abstract class Threadable : Fastener
{
    public Vector3 positionStep;
    public Vector3 rotationStep;

    public bool IsOnCooldown { get; private set; }

    public override int Tightness
    {
        set
        {
            var deltaTightness = value - Tightness;
            base.Tightness = value;
            transform.localPosition += transform.localRotation * positionStep * deltaTightness;
            transform.localRotation *= Quaternion.Euler(rotationStep * deltaTightness);
        }
    }

    protected void StartCooldown(float seconds) => StartCoroutine(Cooldown(seconds));

    protected virtual void Reset() => maxTightness = 8;

    protected override void Awake()
    {
        base.Awake();

        transform.localPosition -= transform.localRotation * positionStep * maxTightness;
        transform.localRotation *= Quaternion.Euler(rotationStep * -maxTightness);
    }

    private IEnumerator Cooldown(float seconds)
    {
        IsOnCooldown = true;
        yield return new WaitForSeconds(seconds);
        IsOnCooldown = false;
    }
}