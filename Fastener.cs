using System;

namespace OASIS;

public delegate void TightnessChangedCallback(int deltaTightness);

/// <summary>
/// Provides a common interface for securing <see cref="Attachable"/> components
/// </summary>
public abstract class Fastener : Interactable
{
    public event TightnessChangedCallback OnTightnessChanged;
    public int maxTightness;

    /// <summary>
    /// The current tightness stage
    /// </summary>
    public int Tightness { get; private set; }

    /// <summary>
    /// Sets the tightness stage to <paramref name="value"/>
    /// </summary>
    /// <param name="value">The new tightness stage</param>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public virtual void SetTightness(int value)
    {
        if (value < 0 || value > maxTightness) throw new ArgumentOutOfRangeException(nameof(value),
            "Tightness must be greater than or equal to zero and less than or equal to the maximum tightness");
        if (Tightness == value) return;

        var deltaTightness = value - Tightness;
        Tightness = value;
        OnTightnessChanged?.Invoke(deltaTightness);
    }
}