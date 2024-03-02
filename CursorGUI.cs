﻿using HutongGames.PlayMaker;

namespace OASIS;

public static class CursorGUI
{
#if Editor
    public static bool Assemble { get; set; }

    public static bool Buy { get; set; }

    public static bool Disassemble { get; set; }

    public static bool Use { get; set; }

    public static string Interaction { get; set; }
#else
    private static readonly FsmBool guiAssemble = FsmVariables.GlobalVariables.FindFsmBool("GUIassemble");
    private static readonly FsmBool guiBuy = FsmVariables.GlobalVariables.FindFsmBool("GUIbuy");
    private static readonly FsmBool guiDisassemble = FsmVariables.GlobalVariables.FindFsmBool("GUIdisassemble");
    private static readonly FsmBool guiUse = FsmVariables.GlobalVariables.FindFsmBool("GUIuse");
    private static readonly FsmString guiInteraction = FsmVariables.GlobalVariables.FindFsmString("GUIinteraction");

    public static bool Assemble
    {
        get => guiAssemble.Value;
        set => guiAssemble.Value = value;
    }

    public static bool Buy
    {
        get => guiBuy.Value;
        set => guiBuy.Value = value;
    }

    public static bool Disassemble
    {
        get => guiDisassemble.Value;
        set => guiDisassemble.Value = value;
    }

    public static bool Use
    {
        get => guiUse.Value;
        set => guiUse.Value = value;
    }

    public static string Interaction
    {
        get => guiInteraction.Value;
        set => guiInteraction.Value = value;
    }
#endif
    public static void ClearInteraction() => Interaction = "";
}