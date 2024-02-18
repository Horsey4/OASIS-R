using System;

namespace OASIS;

public delegate void TightnessChangedCallback(int deltaTightness);

public abstract class Fastener : Interactable
{
    public event TightnessChangedCallback OnTightnessChanged;
    public int maxTightness;

    public int Tightness { get; private set; }

    public void SetTightness(int value) => SetTightness(value, true);

    public virtual void SetTightness(int value, bool notify)
    {
        if (value < 0 || value > maxTightness) throw new ArgumentOutOfRangeException(nameof(value),
            "Tightness must be greater than or equal to zero and less than or equal to the maximum tightness");
        if (Tightness == value) return;

        var deltaTightness = value - Tightness;
        Tightness = value;
        if (notify) OnTightnessChanged?.Invoke(deltaTightness);
    }
}