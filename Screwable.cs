using System.Collections;
using UnityEngine;

namespace OASIS;

/// <summary>
/// Provides functionality for fasteners that can be screwed or unscrewed
/// </summary>
public abstract class Screwable : Fastener
{
    public Vector3 positionStep;
    public Vector3 rotationStep;

    /// <summary>
    /// Returns <see langword="true"/> if the screw cooldown is active
    /// </summary>
    public bool IsOnCooldown { get; private set; }

    public override void SetTightness(int value)
    {
        var deltaTightness = value - Tightness;
        base.SetTightness(value);

        transform.localPosition += transform.localRotation * positionStep * deltaTightness;
        transform.localRotation *= Quaternion.Euler(rotationStep * deltaTightness);
    }

    /// <summary>
    /// Increments or decrements the tightness stage if it is valid to do so
    /// </summary>
    /// <param name="tighten">If the tightness stage should be incremented or decremented</param>
    /// <param name="cooldownSeconds">How long to wait before the method can succeed again</param>
    /// <param name="silent">If the screw sound should be played or not</param>
    /// <remarks>Returns <see langword="true"/> If the tightness stage was updated</remarks>
    public bool Screw(bool tighten, float cooldownSeconds, bool silent)
    {
        if (IsOnCooldown) return false;
        var newTightness = Tightness + (tighten ? 1 : -1);
        if (newTightness < 0 || newTightness > maxTightness) return false;

        if (!silent) MasterAudio.PlaySound3DAndForget("CarBuilding", sourceTrans: transform, variationName: "bolt_screw");
        StartCoroutine(ScrewCooldown(cooldownSeconds));
        SetTightness(newTightness);

        return true;
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