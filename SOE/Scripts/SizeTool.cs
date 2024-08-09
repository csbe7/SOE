using Godot;
using System;


[Tool]
public partial class SizeTool : Node
{
    [Export] public Vector2 size;
    CollisionShape3D shape3D;
    MeshInstance3D mesh3D;
    ShaderMaterial mat;

    BoxShape3D shape;
    PlaneMesh mesh;

    [Export]Color color;

    public override void _Ready()
    {
        shape3D = GetNode<CollisionShape3D>("CollisionShape3D");
        mesh3D = GetNode<MeshInstance3D>("MeshInstance3D");
        
        shape = (BoxShape3D)shape3D.Shape;
        mesh = (PlaneMesh)mesh3D.Mesh;
            
 
        //mat = (ShaderMaterial)mesh.Material;
        
        shape.Size = new Vector3(size.X, 0.01f, size.Y);
        mesh.Size = size;
        
        //mat.SetShaderParameter("color", color);
    }
    


    public override void _PhysicsProcess(double delta)
    {   
        shape.Size = new Vector3(size.X, 0.01f, size.Y);
        mesh.Size = size;
        
        //mat.SetShaderParameter("color", color);
    }

}