using Godot;
using System;

[GlobalClass]
public partial class Weapon : Item
{
    public enum WeaponType
    {
        meele,
        ranged,
    }
    [ExportCategory("Info")]
    [Export] WeaponType weaponType;

    [ExportCategory("Visual Settings")]
    [Export] public Mesh mesh;
    [Export] public Vector3 rotation;
    [Export] public Vector3 position;
    [Export] public Vector3 scale;
    
    [ExportCategory("Gamepay Settings")]
    [Export] public PackedScene projectile;
    [Export] public float damage;
    [Export] public float force;
    [Export] public float hitstunDuration;
    [Export] public int ammo;
    [Export] public int currAmmo;
    [Export] public float reloadTime;
    [Export] public float drawTime;

    [ExportCategory("IK")]
    [Export] public Vector3 leftArm_target_rotation;
    [Export] public Vector3 leftArm_target_position;

    [ExportCategory("Recoil")]
    [Export] public float kickback;
    [Export] public float kickup;
    [Export] public float recoilSpeed;
    [Export] public Curve recoilCurve;
}
