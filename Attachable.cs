using System;
using UnityEngine;

namespace OASIS;

public delegate void AttachmentCallback(int triggerIndex);

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

    public Collider AttachedToCollider => IsAttached ? triggers[AttachedToIndex] : null;

    public bool IsAttached => AttachedToIndex >= 0;

    public int AttachedToIndex { get; private set; } = -1;

    public int Tightness { get; private set; }

    public void Attach() => Attach(0, true, true);

    public void Attach(int triggerIndex) => Attach(triggerIndex, true, true);

    public void Attach(int triggerIndex, bool silent) => Attach(triggerIndex, silent, true);

    public void Detach() => Detach(true, true);

    public void Detach(bool silent) => Detach(silent, true);

    public virtual void Attach(int triggerIndex, bool silent, bool notify)
    {
        if (IsAttached) throw new InvalidOperationException($"Object must be detached before {nameof(Attach)} is called.");
        if (triggerIndex < 0 || triggerIndex >= triggers.Length) throw new ArgumentOutOfRangeException(nameof(triggerIndex),
            "Trigger index must be greater than or equal to zero and less than the total number of triggers.");

        if (!silent) MasterAudio.PlaySound3DAndForget("CarBuilding", sourceTrans: transform, variationName: "assemble");
        AttachedToIndex = triggerIndex;
        AttachedToCollider.enabled = false;
        foreach (var fastener in fasteners) fastener.gameObject.SetActive(true);

        transform.SetParent(AttachedToCollider.transform);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        cachedTag = (tag == "Untagged") ? "PART" : tag;
        tag = "Untagged";
        if (notify) OnAttached?.Invoke(triggerIndex);
    }

    public virtual void Detach(bool silent, bool notify)
    {
        if (!IsAttached) throw new InvalidOperationException($"Object must be attached before {nameof(Detach)} is called.");

        var triggerIndex = AttachedToIndex;
        if (!silent) MasterAudio.PlaySound3DAndForget("CarBuilding", sourceTrans: transform, variationName: "disassemble");
        AttachedToCollider.enabled = true;
        AttachedToIndex = -1;
        foreach (var fastener in fasteners)
        {
            fastener.SetTightness(0);
            fastener.gameObject.SetActive(false);
        }

        transform.SetParent(null);
        tag = cachedTag;
        cachedTag = null;
        if (notify) OnDetached?.Invoke(triggerIndex);
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

    protected virtual void FastenerTightnessChanged(int deltaTightness)
    {
        Tightness += deltaTightness;
        if (Tightness == deltaTightness) CursorGUI.Disassemble = false;
        OnTightnessChanged?.Invoke(deltaTightness);
    }
}