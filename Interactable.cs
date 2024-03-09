using UnityEngine;

namespace OASIS;

/// <summary>
/// Provides functionality for raycast-based interaction
/// </summary>
[RequireComponent(typeof(Collider))]
public abstract class Interactable : MonoBehaviour
{
    public LayerMask layerMask = ~(1 << 2);
    public float maxInteractionDistance = 1;
    private protected static Transform playerCamera;

    /// <summary>
    /// Returns <see langword="true"></see> every frame <see cref="OnCursorOver"/> is called
    /// </summary>
    public bool IsCursorOver { get; private set; }

    /// <summary>
    /// Called on the first frame the raycast hits an attached collider
    /// </summary>
    protected virtual void OnCursorEnter() { }

    /// <summary>
    /// Called every frame the raycast hits an attached collider
    /// </summary>
    protected virtual void OnCursorOver() { }

    /// <summary>
    /// Called on the first frame the raycast no longer hits an attached collider
    /// </summary>
    protected virtual void OnCursorExit() { }

    protected virtual void Awake()
    {
        if (playerCamera == null) playerCamera = GameObject.Find("PLAYER/Pivot/AnimPivot/Camera/FPSCamera").transform;
    }

    protected virtual void Update()
    {
        if (Physics.Raycast(playerCamera.position, playerCamera.forward, out var hit, maxInteractionDistance, layerMask) && hit.collider.gameObject == gameObject)
        {
            if (!IsCursorOver)
            {
                IsCursorOver = true;
                OnCursorEnter();
            }
            OnCursorOver();
        }
        else if (IsCursorOver)
        {
            IsCursorOver = false;
            OnCursorExit();
        }
    }
}