using System;
using UnityEngine;

namespace OASIS;

public delegate void AttachmentCallback(int triggerIndex);

/// <summary>
/// Provides functionality for parent-based attachment
/// </summary>
public abstract class Attachable : Interactable
{
    public event TightnessChangedCallback OnTightnessChanged;
    public event AttachmentCallback OnAttached;
    public event AttachmentCallback OnDetached;
    public Collider[] triggers = [];
    public Fastener[] fasteners = [];
    public bool silent;
    private string cachedTag;
    private int inTriggerIndex = -1;

    /// <summary>
    /// Returns the current parent trigger or <see langword="null"></see> if detached
    /// </summary>
    public Collider AttachedToTrigger => IsAttached ? triggers[AttachedToIndex] : null;

    /// <summary>
    /// Returns <see langword="true"/> while attached to a trigger
    /// </summary>
    public bool IsAttached => AttachedToIndex >= 0;

    /// <summary>
    /// The index of the current parent trigger or -1 if detached
    /// </summary>
    public int AttachedToIndex { get; private set; } = -1;

    /// <summary>
    /// The sum of all fastener tightness stages
    /// </summary>
    public int Tightness { get; private set; }

    /// <summary>
    /// Attaches to the parent trigger at index 0 without playing the assemble sound
    /// </summary>
    /// <exception cref="InvalidOperationException"></exception>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public void Attach() => Attach(0, true);

    /// <summary>
    /// Attaches to the parent trigger at index 0 and plays the assemble sound if <paramref name="silent"/> is <see langword="false"/>
    /// </summary>
    /// <param name="silent">If the assemble sound should be played or not</param>
    /// <exception cref="InvalidOperationException"></exception>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public void Attach(bool silent) => Attach(0, silent);

    /// <summary>
    /// Attaches to the parent trigger at <paramref name="triggerIndex"/> without playing the assemble sound
    /// </summary>
    /// <param name="triggerIndex">The index of the trigger to attach to</param>
    /// <exception cref="InvalidOperationException"></exception>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public void Attach(int triggerIndex) => Attach(triggerIndex, true);

    /// <summary>
    /// Detaches from any parent trigger without playing the disassemble sound
    /// </summary>
    /// <exception cref="InvalidOperationException"></exception>
    public void Detach() => Detach(true);

    /// <summary>
    /// Attaches to the parent trigger at <paramref name="triggerIndex"/> and plays the assemble sound if <paramref name="silent"/> is <see langword="false"/>
    /// </summary>
    /// <param name="triggerIndex">The index of the trigger to attach to</param>
    /// <param name="silent">If the assemble sound should be played or not</param>
    /// <exception cref="InvalidOperationException"></exception>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public virtual void Attach(int triggerIndex, bool silent)
    {
        if (IsAttached) throw new InvalidOperationException($"Object must be detached before {nameof(Attach)} is called.");
        if (triggerIndex < 0 || triggerIndex >= triggers.Length) throw new ArgumentOutOfRangeException(nameof(triggerIndex),
            "Trigger index must be greater than or equal to zero and less than the total number of triggers.");

        if (!silent) MasterAudio.PlaySound3DAndForget("CarBuilding", sourceTrans: transform, variationName: "assemble");
        AttachedToIndex = triggerIndex;
        AttachedToTrigger.enabled = false;
        foreach (var fastener in fasteners) fastener.gameObject.SetActive(true);

        transform.SetParent(AttachedToTrigger.transform);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        cachedTag = (tag == "Untagged") ? "PART" : tag;
        tag = "Untagged";
        OnAttached?.Invoke(triggerIndex);
    }

    /// <summary>
    /// Detaches from any parent trigger and plays the disassemble sound if <paramref name="silent"/> is <see langword="false"/>
    /// </summary>
    /// <param name="silent">If the disassemble sound should be played or not</param>
    /// <exception cref="InvalidOperationException"></exception>
    public virtual void Detach(bool silent)
    {
        if (!IsAttached) throw new InvalidOperationException($"Object must be attached before {nameof(Detach)} is called.");

        var triggerIndex = AttachedToIndex;
        if (!silent) MasterAudio.PlaySound3DAndForget("CarBuilding", sourceTrans: transform, variationName: "disassemble");
        AttachedToTrigger.enabled = true;
        AttachedToIndex = -1;
        foreach (var fastener in fasteners)
        {
            fastener.SetTightness(0);
            fastener.gameObject.SetActive(false);
        }

        transform.SetParent(null);
        tag = cachedTag;
        cachedTag = null;
        OnDetached?.Invoke(triggerIndex);
    }

    /// <summary>
    /// Invoked by any fastener's tightness changed event
    /// </summary>
    protected virtual void FastenerTightnessChanged(int deltaTightness)
    {
        Tightness += deltaTightness;
        if (Tightness == deltaTightness) CursorGUI.Disassemble = false;
        OnTightnessChanged?.Invoke(deltaTightness);
    }

    protected virtual void Reset() => layerMask = 1 << 19;

    protected override void Awake()
    {
        base.Awake();

        foreach (var fastener in fasteners)
        {
            fastener.gameObject.SetActive(false);
            fastener.OnTightnessChanged += FastenerTightnessChanged;
        }
    }

    protected virtual void LateUpdate()
    {
        if (IsAttached || inTriggerIndex < 0) return;

        if (Input.GetMouseButtonDown(0))
        {
            CursorGUI.Assemble = false;
            Attach(inTriggerIndex, silent);
            inTriggerIndex = -1;
        }
        else CursorGUI.Assemble = true;
    }

    protected override void OnCursorOver()
    {
        if (!IsAttached || Tightness > 0) return;

        if (Input.GetMouseButtonDown(1))
        {
            CursorGUI.Disassemble = false;
            Detach(silent);
        }
        else CursorGUI.Disassemble = true;
    }

    protected override void OnCursorExit()
    {
        if (IsAttached) CursorGUI.Disassemble = false;
    }

    protected virtual void OnTriggerEnter(Collider trigger)
    {
        if (!transform.IsChildOf(playerCamera)) return;

        var index = Array.IndexOf(triggers, trigger);
        if (index >= 0) inTriggerIndex = index;
    }

    protected virtual void OnTriggerExit(Collider trigger)
    {
        if (inTriggerIndex < 0 || triggers[inTriggerIndex] != trigger) return;

        inTriggerIndex = -1;
        CursorGUI.Assemble = false;
    }
}