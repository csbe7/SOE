using Godot;
using System;
//using System.Numerics;

public partial class Projectile_hitscan : Projectile
{
    public override void _Ready()
    {
        direction = new Vector3(direction.X, 0, direction.Z);
        PhysicsRayQueryParameters3D query = PhysicsRayQueryParameters3D.Create(shooter.GlobalPosition + (Vector3.Up * 1.36f), shooter.GlobalPosition + (direction.Normalized() * distance), 1);
        var spaceState = GetWorld3D().DirectSpaceState;
        var result = spaceState.IntersectRay(query);
       
        if (result.Count > 0)
        {
            
            if (((Node)result["collider"]).IsInGroup("Player"))
            {
                GD.Print("HIT PLAYER");
                QueueFree();
                return;
            }
            CharacterStatus cs = ((Node3D)result["collider"]).GetNodeOrNull<CharacterStatus>("%CharacterStatus");
            if (IsInstanceValid(cs))
            {
                AttackInfo attack = new AttackInfo(weapon.damage, weapon.force, direction.Normalized(), weapon.hitstunDuration);
                cs.TakeDamage(attack);
                //shooter.wm.OnWeaponHit(cs);
            }
        }
        QueueFree();
    }
}
