using Godot;
using System;

public partial class Follow : Node3D
{
    Node3D target;

    public override void _Ready()
    {
        target = GetNode<Node3D>("../CharacterBody");
    }

    public override void _PhysicsProcess(double delta)
    {
        GlobalPosition = target.GlobalPosition;
    }
}
