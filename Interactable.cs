using UnityEngine;

namespace OASIS;

[RequireComponent(typeof(Collider))]
public abstract class Interactable : MonoBehaviour
{
    public LayerMask layerMask = ~(1 << 2);
    public float maxInteractionDistance = 1.0f;
    private protected static Transform playerCamera;

    public bool IsMouseOver { get; private set; }

    protected virtual void OnCursorEnter() { }

    protected virtual void OnCursorOver() { }

    protected virtual void OnCursorExit() { }

    protected virtual void Awake() => playerCamera ??= GameObject.Find("PLAYER/Pivot/AnimPivot/Camera/FPSCamera").transform;

    protected virtual void Update()
    {
        if (Physics.Raycast(playerCamera.position, playerCamera.forward, out var hit, maxInteractionDistance, layerMask) && hit.collider.gameObject == gameObject)
        {
            if (!IsMouseOver)
            {
                IsMouseOver = true;
                OnCursorEnter();
            }
            OnCursorOver();
        }
        else if (IsMouseOver)
        {
            IsMouseOver = false;
            OnCursorExit();
        }
    }
}