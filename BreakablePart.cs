using UnityEngine;

namespace OASIS;

public class BreakablePart : JointedPart<FixedJoint>
{
    public float breakForce = 4000;
    public float breakForceStep;
    public float minBreakForce;
    public float maxBreakForce = Mathf.Infinity;
    public float breakTorque = 4000;
    public float breakTorqueStep;
    public float minBreakTorque;
    public float maxBreakTorque = Mathf.Infinity;
    public int unbreakableAtTightness;

    protected override void InitJoint()
    {
        base.InitJoint();

        RecalculateBreakForce();
    }

    protected override void FastenerTightnessChanged(int deltaTightness)
    {
        base.FastenerTightnessChanged(deltaTightness);

        RecalculateBreakForce();
    }

    protected virtual void RecalculateBreakForce()
    {
        if (unbreakableAtTightness > 0 && Tightness >= unbreakableAtTightness)
        {
            Joint.breakForce = Mathf.Infinity;
            Joint.breakTorque = Mathf.Infinity;
        }
        else
        {
            Joint.breakForce = Mathf.Clamp(breakForce + breakForceStep * Tightness, minBreakForce, maxBreakForce);
            Joint.breakTorque = Mathf.Clamp(breakTorque + breakTorqueStep * Tightness, minBreakTorque, maxBreakTorque);
        }
    }
}