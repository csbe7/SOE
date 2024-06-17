using Godot;
using System;
using System.Reflection.Metadata.Ecma335;

public partial class Inventory : Node
{
    [Export] public InventoryData inv;
    
    [Signal] public delegate void ItemAddedEventHandler(SlotData slot, int amount);
    [Signal] public delegate void ItemRemovedEventHandler(SlotData slot, int amount);
     
    public void AddItem(Item item, int amount)
    {
        foreach (SlotData slot in inv.items)
        {
           if (slot.item == item && slot.amount < item.maxStack)
           {
               int spaceLeft = item.maxStack - slot.amount;
               int toAdd = Mathf.Min(amount, spaceLeft);
               slot.amount += toAdd;
               amount -= toAdd;
            
               if (amount <= 0)
               {
                   EmitSignal(SignalName.ItemAdded, slot, amount);
                   return;
               } 
           }
        }
 

        while(amount > 0)
        {
            SlotData newSlot = new SlotData();
            int toAdd = Mathf.Min(amount, item.maxStack);
            newSlot.item = item;
            newSlot.amount = toAdd;
            amount -= toAdd;

            inv.items.Add(newSlot);
            EmitSignal(SignalName.ItemAdded, newSlot, amount);
        }
    }


    public int RemoveItem(SlotData toRemove, int amount)
    {
        foreach(SlotData slot in inv.items)
        {
            if (slot == toRemove)
            {
               slot.amount -= amount;
               if (slot.amount <= 0)
               {
                   inv.items.Remove(slot);
               }
               EmitSignal(SignalName.ItemRemoved, slot, amount);
               return 1;
            }
        }

        return 0;
    }


    public int RemoveItemPos(int index, int amount)
    {
        if (index > inv.items.Count-1) return 0;
        SlotData slot = inv.items[index];
        slot.amount -= amount;
        if (slot.amount <= 0)
        {
            inv.items.RemoveAt(index);
        }
        EmitSignal(SignalName.ItemRemoved, slot, amount);
        return 1;

    }
    

    public bool isEmpty()
    {
        return (inv.items.Count == 0);
    }
}
