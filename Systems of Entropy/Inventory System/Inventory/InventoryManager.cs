using Godot;
using System;

public partial class InventoryManager : Node
{
    Game game;
    camera cam;
    [Export] Inventory inventory;
    
    Node Canvas;
    
    InventoryUI inventoryUI;
    InventoryUI externalInventoryUI;
    [Export] PackedScene inventoryScene;
    [Export] PackedScene quantitySelectionUI;
    QuantitySelector selector;

    [Export] PackedScene itemDrop;

    bool open;

    public SlotButton selectedSlot;

    public override void _Ready()
    {
        game = GetTree().Root.GetChild<Game>(0);
        
        cam = GetParent().GetParent().GetNode<camera>("%CameraPivot");

        Canvas = GetParent().GetParent().GetNode("%CanvasLayer");
        inventoryUI = Canvas.GetNode<InventoryUI>("InventoryUI");

        inventoryUI.inventory = inventory;

        inventoryUI.SlotHovered += OnSlotHovered;
        
        //inventoryUI.UpdateInventory();
        inventoryUI.Hide();
    }


    public override void _Process(double delta)
    {
        bool selectingQuantity = IsInstanceValid(selector);
        bool externalInventoryOpen = IsInstanceValid(externalInventoryUI);
        if (externalInventoryOpen && !IsInstanceValid(externalInventoryUI.inventory)) externalInventoryUI.QueueFree();

        //TOGGLE INVENTORY
        if (Input.IsActionJustPressed("ToggleInventory") && !selectingQuantity)
        {
            if (open)
            {
                game.SetState(Game.GameState.gameplay);
                inventoryUI.Hide();
                if (externalInventoryOpen) externalInventoryUI.QueueFree();
                open = false;
            }
            else{
                game.SetState(Game.GameState.menu);
                inventoryUI.Show();
                open = true;
            }
        }
        
        //DROP ITEM
        if (open && Input.IsActionJustPressed("DropItem") && !selectingQuantity && IsInstanceValid(selectedSlot) && !externalInventoryOpen)
        {
            if (selectedSlot.slotData.amount == 1)
            {
                Drop(1);
                return;
            }
            if (Input.IsActionPressed("ModifierKey"))
            {
                Drop(selectedSlot.slotData.amount);
                return;
            }
            selector = quantitySelectionUI.Instantiate<QuantitySelector>();
            selector.min = 1;
            selector.max = selectedSlot.slotData.amount;
            selector.QuantityConfirmed += Drop;
            inventoryUI.AddChild(selector);
        }
       
        //TRANSFER ITEM
        if (open && Input.IsActionJustPressed("MoveItem") && !selectingQuantity && IsInstanceValid(selectedSlot) && externalInventoryOpen)
        {
            if (selectedSlot.slotData.amount == 1)
            {
                Transfer(1);
                return;
            }
            if (Input.IsActionPressed("ModifierKey"))
            {
                Transfer(selectedSlot.slotData.amount);
                return;
            }
            selector = quantitySelectionUI.Instantiate<QuantitySelector>();
            selector.min = 1;
            selector.max = selectedSlot.slotData.amount;
            selector.QuantityConfirmed += Transfer;
            inventoryUI.AddChild(selector);
        }

        //OPEN CONTAINER
        if (!open && Input.IsActionJustPressed("OpenContainer"))
        {
            Godot.Collections.Dictionary result = cam.ShootRayToMouse((uint)Mathf.Pow(2, 31));
            if (result == null) return;
            foreach(var node in result)
            {
                Inventory foundInventory = ((Node)result["collider"]).GetNodeOrNull<Inventory>("%Inventory");
                if (IsInstanceValid(foundInventory))
                {
                    CreateExternalInventory(foundInventory);
                    game.SetState(Game.GameState.menu);
                    inventoryUI.Show();
                    open = true;
                    break;
                } 
            }
        }
    }
    

    void CreateExternalInventory(Inventory externalInventory)
    {
        if (IsInstanceValid(externalInventoryUI)) externalInventoryUI.QueueFree();
        

        externalInventoryUI = inventoryScene.Instantiate<InventoryUI>();
        externalInventoryUI.external = true;
        externalInventoryUI.inventory = externalInventory;
        
        externalInventoryUI.SlotHovered += OnSlotHovered;

        Canvas.AddChild(externalInventoryUI);
        
        externalInventoryUI.GlobalPosition = new Vector2(inventoryUI.GlobalPosition.X + 500, inventoryUI.GlobalPosition.Y);
        
        externalInventoryUI.UpdateInventory();
        externalInventoryUI.Show();
    }

    public void Drop(int amount)
    {
        if (IsInstanceValid(selectedSlot))
        { 
            var drop = itemDrop.Instantiate<ItemDrop>();
            drop.inventory.AddItem(selectedSlot.slotData.item, Mathf.Min(amount, selectedSlot.slotData.amount));
            inventory.RemoveItem(selectedSlot.slotData, amount);

            GetTree().Root.AddChild(drop);
            drop.GlobalPosition = GetNode<Node3D>("%CharacterBody").GlobalPosition;
            drop.Pool();
                
            inventoryUI.UpdateInventory();
        }
    }

    public void Transfer(int amount)
    {
        if (!IsInstanceValid(selectedSlot) || !IsInstanceValid(externalInventoryUI)) return;
        
        Inventory source, destination;

        if (selectedSlot.external)
        {
            source = externalInventoryUI.inventory;
            destination = inventoryUI.inventory;
        }
        else
        {
            source = inventoryUI.inventory;
            destination = externalInventoryUI.inventory;
        }

        destination.AddItem(selectedSlot.slotData.item, amount);
        source.RemoveItem(selectedSlot.slotData, amount);
        
        externalInventoryUI.UpdateInventory();
        inventoryUI.UpdateInventory();
    }
    
    public void OnSlotHovered(SlotButton slot)
    {
        if (IsInstanceValid(selector)) return;

        Color newColor;
        
        if (IsInstanceValid(selectedSlot))
        {
            newColor = selectedSlot.bg.Color;
            newColor.A -= 0.2f;
            selectedSlot.bg.Color = newColor;
        }
        
        selectedSlot = slot;
        newColor = selectedSlot.bg.Color;
        newColor.A += 0.2f;
        selectedSlot.bg.Color = newColor;
    }
   
}
