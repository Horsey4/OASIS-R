using System.Collections;
using UnityEngine;

namespace OASIS;

public abstract class Screwable : Fastener
{
    public Vector3 positionStep;
    public Vector3 rotationStep;

    public bool IsOnCooldown { get; private set; }

    public override void SetTightness(int value)
    {
        var deltaTightness = value - Tightness;
        base.SetTightness(value);

        transform.localPosition += transform.localRotation * positionStep * deltaTightness;
        transform.localRotation *= Quaternion.Euler(rotationStep * deltaTightness);
    }

    protected void Screw(bool tighten, float cooldownSeconds, bool silent)
    {
        if (IsOnCooldown) return;
        var newTightness = Tightness + (tighten ? 1 : -1);
        if (newTightness < 0 || newTightness > maxTightness) return;

        SetTightness(newTightness);
        if (!silent) MasterAudio.PlaySound3DAndForget("CarBuilding", sourceTrans: transform, variationName: "bolt_screw");
        StartCoroutine(ScrewCooldown(cooldownSeconds));
    }

    protected virtual void Reset() => maxTightness = 8;

    protected override void Awake()
    {
        base.Awake();

        transform.localPosition -= transform.localRotation * positionStep * maxTightness;
        transform.localRotation *= Quaternion.Euler(rotationStep * -maxTightness);
    }

    private IEnumerator ScrewCooldown(float seconds)
    {
        IsOnCooldown = true;
        yield return new WaitForSeconds(seconds);
        IsOnCooldown = false;
    }
}