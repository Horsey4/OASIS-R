using UnityEngine;

namespace OASIS;

public delegate void JointBrokenCallback(int triggerIndex, float breakForce);

public abstract class JointedPart<TJoint> : Attachable
    where TJoint : Joint
{
    public event JointBrokenCallback OnJointBroken;
    public bool invokeOnDetachWhenBroken = true;

    public TJoint Joint { get; private set; }

    public override void Attach(int triggerIndex, bool silent, bool notify)
    {
        base.Attach(triggerIndex, silent, notify);

        Joint = gameObject.AddComponent<TJoint>();
        InitJoint();
    }

    public override void Detach(bool silent, bool notify)
    {
        base.Detach(silent, notify);

        Destroy(Joint);
    }

    protected virtual void InitJoint() => Joint.connectedBody = transform.parent.GetComponentInParent<Rigidbody>();

    protected virtual void OnJointBreak(float breakForce)
    {
        base.Detach(false, invokeOnDetachWhenBroken);
        OnJointBroken?.Invoke(AttachedToIndex, breakForce);
    }
}