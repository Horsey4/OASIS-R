using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using System;
using UnityEngine;

namespace OASIS;

public class Bolt : Screwable
{
    public float size = 1.0f;
    public bool canUseRatchet = true;
#if Editor
    protected override void Reset()
    {
        base.Reset();

        layerMask = 1 << 12;
        positionStep = new(0, 0, -0.0025f);
        rotationStep = new(0, 0, 45);
    }
#else
    private static readonly FsmFloat wrenchSize = FsmVariables.GlobalVariables.FindFsmFloat("ToolWrenchSize");
    private static readonly FsmBool usingRatchet = FsmVariables.GlobalVariables.FindFsmBool("PlayerHasRatchet");
    private static Material highlightMaterial;
    private static FsmBool ratchetSwitch;
    private bool isHighlighted;
    private Renderer renderer;

    public Material MaterialCache { get; set; }

    protected override void Awake()
    {
        base.Awake();

        if (highlightMaterial == null)
        {
            var spanner = playerCamera.Find("2Spanner");
            var boltCheckFsm = spanner.Find("Raycast").GetComponents<PlayMakerFSM>()[1];
            var ratchetSwitchFsm = spanner.Find("Pivot/Ratchet").GetComponent<PlayMakerFSM>();
            boltCheckFsm.Fsm.InitData();

            highlightMaterial = ((SetMaterial)boltCheckFsm.FsmStates[2].Actions[1]).material.Value;
            ratchetSwitch = ratchetSwitchFsm.FsmVariables.FindFsmBool("Switch");
        }
    }

    protected override void OnCursorOver()
    {
        if (wrenchSize.Value == size && (!usingRatchet.Value || canUseRatchet))
        {
            if (!isHighlighted)
            {
                isHighlighted = true;
                renderer ??= GetComponent<Renderer>();
                if (renderer == null) throw new InvalidOperationException("Bolt has no renderer.");

                MaterialCache = renderer.material;
                renderer.material = highlightMaterial;
            }
            if (Input.mouseScrollDelta.y == 0) return;

            if (usingRatchet.Value) Screw(ratchetSwitch.Value ? 1 : -1, 0.08f);
            else Screw(Math.Sign(Input.mouseScrollDelta.y), 0.28f);
        }
        else UnHighlight();
    }

    protected override void OnCursorExit() => UnHighlight();

    protected virtual void OnDisable() => UnHighlight();

    private void UnHighlight()
    {
        if (isHighlighted)
        {
            isHighlighted = false;
            renderer.material = MaterialCache;
            MaterialCache = null;
        }
    }
#endif
}