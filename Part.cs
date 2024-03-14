using UnityEngine;
using System;

namespace OASIS;

/// <summary>
/// Saves and applies the properties of a <see cref="Rigidbody"/>
/// </summary>
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

    /// <summary>
    /// Creates an instance with unset values
    /// </summary>
    public RigidbodyProperties() { }

    /// <summary>
    /// Creates an instance with the same properties as <paramref name="rigidbody"/>
    /// </summary>
    /// <param name="rigidbody">The rigidbody to copy properties from</param>
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

    /// <summary>
    /// Applies saved properties to <paramref name="rigidbody"/>
    /// </summary>
    /// <param name="rigidbody">The rigidbody to modify</param>
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

/// <summary>
/// Emulates basic vanilla attachment by destroying and re-creating the <see cref="Rigidbody"/> component
/// </summary>
public class Part : Attachable
{
    private Rigidbody rigidbody;

    /// <summary>
    /// Returns the properties to apply to the new rigidbody when detached or <see langword="null"/> if already detached
    /// </summary>
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