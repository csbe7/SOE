using Godot;
using System;

public partial class StatModifier : Node
{
    public enum Mode
    {
       Flat,
       PercentageFromBase,
       Percentage,
    }

	public readonly float value;
    public readonly Mode mode; 
    public readonly int order;
	public StatModifier(float v, Mode m, int o)
    {
        value = v;
        mode = m;
        order = o;
    }

    public StatModifier(float v, Mode m) : this(v, m, (int)m * 10) { }
}
