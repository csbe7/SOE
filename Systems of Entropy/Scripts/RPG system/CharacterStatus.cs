using Godot;
using System;

public partial class CharacterStatus : Node
{
    
    [Export]public Godot.Collections.Dictionary stats = new Godot.Collections.Dictionary
	{
		//STATUS STATS
		{"Health", new Stat(100)},
        {"CurrentHealth", new Stat(100)},
		{"Stamina", new Stat(100)},
		{"CurrentStamina", new Stat (100)},
        {"Tiredness", new Stat(100)},
        {"CurrentTiredness", new Stat(100)},

        //MAIN STATS
		{"Strenght", new Stat(1)},
		{"Agility", new Stat(1)},
		{"Endurance", new Stat(1)},
        {"Perception", new Stat(1)},

		
	};

   


    public void TakeDamage(AttackInfo attack)
    {
        /*GD.Print(attack.damage);
        SetStatValue("CurrentHealth", -attack.damage, 1);
        ClampStatValue("CurrentHealth", 0, GetStatValue("Health", true), 0);*/
        var hp =  ((Stat)stats["CurrentHealth"]).baseValue; 
		hp = Mathf.Clamp(hp-attack.damage, 0f, ((Stat)stats["Health"]).ModValue);
		SetStatValue("CurrentHealth", hp, 0);
        GD.Print("Damagrd");
    }
    


    //STAT MANIPULATION
    public float GetStatValue(string stat, bool mod) //true = modValue  false = baseValue
	{
		float value;
        if (mod)
		{
			value = ((Stat)stats[stat]).ModValue; 
            ((Stat)stats[stat]).hasBeenModified = true;
		}
		else
		{
			value = ((Stat)stats[stat]).baseValue; 
		}

		return value;
	}
    public void SetStatValue(string stat, float value, int mode)
    {
        switch (mode)
        {
            case 0: //SET
            ((Stat)stats[stat]).baseValue = value;
            break;

            case 1: //ADD
            ((Stat)stats[stat]).baseValue += value;
            break;
        }
    }
    public void ClampStatValue(string stat, float min, float max, int mode)
    {
        switch (mode)
        {
            case 0: //BASE
            ((Stat)stats[stat]).baseValue = Mathf.Clamp(((Stat)stats[stat]).baseValue, min, max);
            break;
        }
    }
    public void AddStatModifier(StatModifier mod, string targetStat)
	{
		((Stat)stats[targetStat]).AddModifier(mod);
	}
	public void RemoveStatModifier(StatModifier mod, string targetStat)
    {
        ((Stat)stats[targetStat]).RemoveModifier(mod);
    }
}
