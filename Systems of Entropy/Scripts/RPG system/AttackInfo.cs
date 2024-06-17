using Godot;
using System;

public partial class AttackInfo : Node
{
    public float damage;
    public float knockback;
    public Vector3 knockbackDir;
    public float hitstunDuration;

    public AttackInfo(float dmg, float kb, Vector3 kbDir, float htsDur)
    {
        damage = dmg;
        knockback = kb;
        knockbackDir = kbDir;
        hitstunDuration = htsDur;
    }
}
