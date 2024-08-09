using Godot;
using System;


[GlobalClass]
public partial class Stat : RefCounted
{
    public float minValue, maxValue;

	public float baseValue;
	
	private float modValue;
	public float ModValue{
		get{
			if (hasBeenModified) {
				QuickSort(modifiers, 0, modifiers.Count - 1);
				ModValue = Calculate();
                hasBeenModified = false;
			}
			return modValue;
		}
		private set{
			modValue = value;
		}
	}

	private Godot.Collections.Array<StatModifier> modifiers;
	private bool hasBeenModified = true;

	public Stat(float v, float min, float max)
	{
		baseValue = v;
		minValue = min;
		maxValue = max;

		modifiers = new Godot.Collections.Array<StatModifier>{};
	}

	public Stat(float v) : this(v, 0, 10000) { } 

	public Stat() : this(0, 0, 10000) { }


	public void AddModifier(StatModifier mod)
	{
		modifiers.Add(mod);
        hasBeenModified = true;
	}

	public void RemoveModifier(StatModifier mod)
	{
		modifiers.Remove(mod);
        hasBeenModified = true;
	}

	private float Calculate()
    {
        float finalValue = baseValue;

        foreach(StatModifier sm in modifiers)
        {
			switch (sm.mode){
				case StatModifier.Mode.Flat:
                finalValue += sm.value;
				break;

				case StatModifier.Mode.PercentageFromBase:
				finalValue += (baseValue/100) * sm.value;
				break;
			}
           
		}

		return finalValue;
	}

	private static void QuickSort(Godot.Collections.Array<StatModifier> array, int start, int end)
	{
		if (end <= start) return;

		StatModifier temp;
		int i, j = start-1;

        for(i = start; i < end; i++)
		{
			if (array[i].order < array[end].order)
			{
				j++;
				temp = array[j];
                array[j] = array[i];
				array[i] = temp;
			}
		}
		j++;
		temp = array[j];
		array[j] = array[end];
		array[end] = temp;

		QuickSort(array, start, j-1);
		QuickSort(array, j+1, end);
	
	}

	
}
