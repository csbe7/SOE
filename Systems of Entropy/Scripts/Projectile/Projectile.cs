using Godot;
using System;

public partial class Projectile : Node3D
{
    //[Signal] public delegate void HitEventHandler(Godot.Collections.Dictionary hitInfo);
    public Weapon weapon; 
    public CharacterController shooter;
    public Vector3 direction;
    public float distance;
    public float speed;
}
