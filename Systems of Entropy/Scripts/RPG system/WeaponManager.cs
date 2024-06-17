using Godot;
using System;

public partial class WeaponManager : Node
{
    CharacterController c;

    BoneAttachment3D handAttachment;
    SkeletonIK3D leftArmIK;


    [Export] public Weapon currWeapon;
    

    public override void _Ready()
    {
        c = GetParent().GetNode<CharacterController>("CharacterBody");

        handAttachment = GetNode<BoneAttachment3D>("%weaponAttachment");
        leftArmIK = GetNode<SkeletonIK3D>("%left_arm_IK3D");
        if (IsInstanceValid(currWeapon)) LoadWeapon();
    }

    public void LoadWeapon()
    {
        var Weapon = handAttachment.GetNode<Node3D>("Weapon"); 
        var weaponMesh = Weapon.GetNode<MeshInstance3D>("MeshInstance3D");
        weaponMesh.Mesh = currWeapon.mesh;
        Weapon.Position = currWeapon.position;
        Weapon.Rotation = currWeapon.rotation;

        Node3D leftArmTarget = new Node3D();
        leftArmTarget.Position = currWeapon.leftArm_target_position;
        leftArmTarget.Rotation = currWeapon.leftArm_target_rotation;
        Weapon.AddChild(leftArmTarget);
        

        leftArmIK.TargetNode = leftArmTarget.GetPath(); 
    }

    public void UseWeapon()
    {
        var proj = currWeapon.projectile.Instantiate<Projectile>();
        proj.shooter = c;
        proj.direction = c.forwardDir;
        proj.weapon = currWeapon;
        proj.distance = 60f;
        AddChild(proj);
        proj.GlobalPosition = c.GlobalPosition;

        c.ac.StartRecoil();
    }

    public void OnWeaponHit(CharacterStatus target)
    {
        GD.Print("HIT");
    }
}
