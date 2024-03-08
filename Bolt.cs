using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using System;
using UnityEngine;

namespace OASIS;

public class Bolt : Screwable
{
    public float size = 1;
    public bool canUseRatchet = true;
    public bool silent;
#if !Editor
    private static readonly FsmFloat wrenchSize = FsmVariables.GlobalVariables.FindFsmFloat("ToolWrenchSize");
    private static readonly FsmBool usingRatchet = FsmVariables.GlobalVariables.FindFsmBool("PlayerHasRatchet");
    private static GameObject spanner;
    private static Material highlightMaterial;
    private static FsmBool ratchetSwitch;
    private bool isHighlighted;
    private Renderer renderer;
#endif
    public Material MaterialCache { get; set; }

    protected override void Reset()
    {
        base.Reset();

        layerMask = 1 << 12;
        positionStep = new(0, 0, -0.0025f);
        rotationStep = new(0, 0, 45);
    }
#if !Editor
    protected override void Awake()
    {
        base.Awake();

        if (spanner == null)
        {
            spanner = playerCamera.Find("2Spanner").gameObject;
            var boltCheckFsm = spanner.transform.Find("Raycast").GetComponents<PlayMakerFSM>()[1].Fsm;
            var ratchetSwitchFsm = spanner.transform.Find("Pivot/Ratchet").GetComponent<PlayMakerFSM>().Fsm;
            boltCheckFsm.InitData();

            highlightMaterial = ((SetMaterial)boltCheckFsm.States[2].Actions[1]).material.Value;
            ratchetSwitch = ratchetSwitchFsm.Variables.FindFsmBool("Switch");
        }
    }

    protected override void OnCursorOver()
    {
        if (wrenchSize.Value == size && (!usingRatchet.Value || canUseRatchet) && spanner.activeSelf)
        {
            if (!isHighlighted)
            {
                isHighlighted = true;
                if (renderer == null && (renderer = GetComponent<Renderer>()) == null)
                    throw new InvalidOperationException("Bolt has no renderer.");

                MaterialCache = renderer.material;
                renderer.material = highlightMaterial;
            }
            if (Input.mouseScrollDelta.y == 0) return;

            if (usingRatchet.Value) Screw(ratchetSwitch.Value, 0.08f, silent);
            else Screw(Input.mouseScrollDelta.y > 0, 0.28f, silent);
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
#else
    protected override void OnCursorOver() { }

    protected override void OnCursorExit() { }

    protected virtual void OnDisable() { }
#endif
}