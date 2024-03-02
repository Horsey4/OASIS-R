using UnityEngine;

namespace OASIS;

[RequireComponent(typeof(Collider))]
public abstract class Interactable : MonoBehaviour
{
    public LayerMask layerMask = ~(1 << 2);
    public float maxInteractionDistance = 1;
    private protected static Transform playerCamera;

    public bool IsCursorOver { get; private set; }

    protected virtual void OnCursorEnter() { }

    protected virtual void OnCursorOver() { }

    protected virtual void OnCursorExit() { }

    protected virtual void Awake() => playerCamera ??= GameObject.Find("PLAYER/Pivot/AnimPivot/Camera/FPSCamera").transform;

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