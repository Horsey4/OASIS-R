﻿using UnityEngine;

namespace OASIS;

public class Part : Attachable
{
    public sealed class RigidbodyInfo(Rigidbody rigidbody)
    {
        public float mass = rigidbody.mass;
        public float drag = rigidbody.drag;
        public float angularDrag = rigidbody.angularDrag;
        public bool useGravity = rigidbody.useGravity;
        public bool isKinematic = rigidbody.isKinematic;
        public RigidbodyInterpolation interpolation = rigidbody.interpolation;
        public CollisionDetectionMode collisionDetectionMode = rigidbody.collisionDetectionMode;
        public RigidbodyConstraints constraints = rigidbody.constraints;

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
    private Rigidbody rigidbody;

    public RigidbodyInfo CachedRigidbody { get; private set; }

    public override void Attach(int triggerIndex, bool notify, bool silent)
    {
        base.Attach(triggerIndex, notify, silent);

        rigidbody ??= GetComponent<Rigidbody>();
        CachedRigidbody = new(rigidbody);
        Destroy(rigidbody);
    }

    public override void Detach(bool notify, bool silent)
    {
        base.Detach(notify, silent);

        rigidbody = gameObject.AddComponent<Rigidbody>();
        CachedRigidbody.ApplyTo(rigidbody);
        CachedRigidbody = null;
    }
}