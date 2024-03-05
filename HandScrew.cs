using UnityEngine;

namespace OASIS;

public class HandScrew : Screwable
{
    public float screwCooldown = 0.1f;

    protected override void OnCursorOver()
    {
        CursorGUI.Use = true;
        if (Input.mouseScrollDelta.y == 0) return;

        Screw(Input.mouseScrollDelta.y > 0, screwCooldown);
    }

    protected override void OnCursorExit() => CursorGUI.Use = false;
}