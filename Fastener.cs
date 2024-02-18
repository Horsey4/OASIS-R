using System;

namespace OASIS;

public delegate void TightnessChangedCallback(int deltaTightness);

public abstract class Fastener : Interactable
{
    public event TightnessChangedCallback OnTightnessChanged;
    public int maxTightness;
    private int tightness;

    public virtual int Tightness
    {
        get => tightness;
        set
        {
            if (value < 0 || value > maxTightness) throw new ArgumentOutOfRangeException(nameof(value),
                "Tightness must be greater than or equal to zero and less than or equal to the maximum tightness");

            var deltaTightness = value - tightness;
            tightness = value;
            OnTightnessChanged?.Invoke(deltaTightness);
        }
    }
}