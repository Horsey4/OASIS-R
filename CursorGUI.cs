using HutongGames.PlayMaker;

namespace OASIS;

/// <summary>
/// Wrapper class for cursor-related GUIxxx global <see cref="FsmVariables"/>
/// </summary>
/// <remarks>
/// All properties are reset by the game at varying intervals
/// </remarks>
public static class CursorGUI
{
#if !Editor
    private static readonly FsmBool guiAssemble = FsmVariables.GlobalVariables.FindFsmBool("GUIassemble");
    private static readonly FsmBool guiBuy = FsmVariables.GlobalVariables.FindFsmBool("GUIbuy");
    private static readonly FsmBool guiDisassemble = FsmVariables.GlobalVariables.FindFsmBool("GUIdisassemble");
    private static readonly FsmBool guiDrive = FsmVariables.GlobalVariables.FindFsmBool("GUIdrive");
    private static readonly FsmBool guiPassenger = FsmVariables.GlobalVariables.FindFsmBool("GUIpassenger");
    private static readonly FsmBool guiUse = FsmVariables.GlobalVariables.FindFsmBool("GUIuse");
    private static readonly FsmString guiInteraction = FsmVariables.GlobalVariables.FindFsmString("GUIinteraction");

    /// <summary>
    /// The checkmark cursor icon
    /// </summary>
    public static bool Assemble
    {
        get => guiAssemble.Value;
        set => guiAssemble.Value = value;
    }

    /// <summary>
    /// The shopping cart cursor icon
    /// </summary>
    public static bool Buy
    {
        get => guiBuy.Value;
        set => guiBuy.Value = value;
    }

    /// <summary>
    /// The no symbol cursor icon
    /// </summary>
    public static bool Disassemble
    {
        get => guiDisassemble.Value;
        set => guiDisassemble.Value = value;
    }

    /// <summary>
    /// The steering wheel cursor icon
    /// </summary>
    public static bool Drive
    {
        get => guiDrive.Value;
        set => guiDrive.Value = value;
    }

    /// <summary>
    /// The seat cursor icon
    /// </summary>
    public static bool Passenger
    {
        get => guiPassenger.Value;
        set => guiPassenger.Value = value;
    }

    /// <summary>
    /// The hand cursor icon
    /// </summary>
    public static bool Use
    {
        get => guiUse.Value;
        set => guiUse.Value = value;
    }

    /// <summary>
    /// The interaction text under the cursor
    /// </summary>
    public static string Interaction
    {
        get => guiInteraction.Value;
        set => guiInteraction.Value = value;
    }

    /// <summary>
    /// Sets <see cref="Interaction"/> to an empty string
    /// </summary>
    public static void ClearInteraction() => Interaction = "";
#else
    public static bool Assemble { get; set; }

    public static bool Buy { get; set; }

    public static bool Disassemble { get; set; }

    public static bool Use { get; set; }

    public static string Interaction { get; set; }
#endif
}