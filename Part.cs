using UnityEngine;
using System;

namespace OASIS;

public sealed class RigidbodyProperties
{
    public float mass;
    public float drag;
    public float angularDrag;
    public bool useGravity;
    public bool isKinematic;
    public RigidbodyInterpolation interpolation;
    public CollisionDetectionMode collisionDetectionMode;
    public RigidbodyConstraints constraints;

    public RigidbodyProperties() { }

    public RigidbodyProperties(Rigidbody rigidbody)
    {
        mass = rigidbody.mass;
        drag = rigidbody.drag;
        angularDrag = rigidbody.angularDrag;
        useGravity = rigidbody.useGravity;
        isKinematic = rigidbody.isKinematic;
        interpolation = rigidbody.interpolation;
        collisionDetectionMode = rigidbody.collisionDetectionMode;
        constraints = rigidbody.constraints;
    }

    public void ApplyTo(Rigidbody rigidbody)
    {
        rigidbody.mass = mass;
        rigidbody.drag = drag;
        rigidbody.angularDrag = angularDrag;
        rigidbody.useGravity = useGravity;
        rigidbody.isKinematic = isKinematic;
        rigidbody.interpolation = interpolation;
        rigidbody.collisionDetectionMode = collisionDetectionMode;
        rigidbody.constraints = constraints;
    }
}

public class Part : Attachable
{
    private Rigidbody rigidbody;

    public RigidbodyProperties RigidbodyProperties { get; private set; }

    public override void Attach(int triggerIndex, bool silent)
    {
        if (rigidbody == null && (rigidbody = GetComponent<Rigidbody>()) == null)
            throw new InvalidOperationException("Part has no rigidbody.");
        base.Attach(triggerIndex, silent);

        RigidbodyProperties = new(rigidbody);
        Destroy(rigidbody);
    }

    public override void Detach(bool silent)
    {
        base.Detach(silent);

        rigidbody = gameObject.AddComponent<Rigidbody>();
        RigidbodyProperties.ApplyTo(rigidbody);
        RigidbodyProperties = null;
    }
}