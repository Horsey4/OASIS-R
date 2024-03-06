using UnityEngine;

namespace OASIS;

public delegate void JointBrokenCallback(int triggerIndex, float breakForce);

public abstract class JointedPart<TJoint> : Attachable
    where TJoint : Joint
{
    public event JointBrokenCallback OnJointBroken;

    public TJoint Joint { get; private set; }

    public override void Attach(int triggerIndex, bool silent)
    {
        base.Attach(triggerIndex, silent);

        Joint = gameObject.AddComponent<TJoint>();
        InitJoint();
    }

    public override void Detach(bool silent)
    {
        base.Detach(silent);

        if (Joint != null) Destroy(Joint);
    }

    protected virtual void InitJoint() => Joint.connectedBody = transform.parent.GetComponentInParent<Rigidbody>();

    protected virtual void OnJointBreak(float breakForce)
    {
        var triggerIndex = AttachedToIndex;
        Detach(false);
        OnJointBroken?.Invoke(triggerIndex, breakForce);
    }
}