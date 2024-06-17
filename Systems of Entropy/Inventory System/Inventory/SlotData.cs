using Godot;
using System;

[GlobalClass]
public partial class SlotData : Resource
{
    [Export] public Item item;
    [Export] public int amount = 1;
    
}
