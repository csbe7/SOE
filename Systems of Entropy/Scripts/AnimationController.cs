using Godot;
using System;


public partial class AnimationController : Node
{
    CharacterController c;
    Node3D meshPivot;
    
    AnimationTree at;

    AnimationNodeStateMachine stateMachine;
	AnimationNodeStateMachinePlayback stateMachinePlayback;
    
    Tween tween;

    [Export]public float animationTimescale = 1;
    [Export]public float rotationSpeed;
    Vector2 dir = Vector2.Zero;
    [Export]float lerpSpeed = 7f;

    [ExportCategory("IK")]
    public Skeleton3D skeleton;

    public SkeletonIK3D leftArmIK;
    public SkeletonIK3D rightArmIK;

    public SkeletonIK3D headIK;

    //public Node3D weapon;
    
    public override void _Ready()
    {
        c = GetParent().GetNode<CharacterController>("CharacterBody");
        meshPivot = GetParent().GetNode<Node3D>("Mesh");
        
        at = meshPivot.GetNode<AnimationTree>("AnimationTree");

        stateMachine = (AnimationNodeStateMachine)at.TreeRoot;
		stateMachinePlayback = (AnimationNodeStateMachinePlayback)at.Get("parameters/playback");
        
        skeleton = meshPivot.GetNode<Skeleton3D>("%GeneralSkeleton");

        rightArmIK = meshPivot.GetNode<SkeletonIK3D>("%right_arm_IK3D");
        leftArmIK = GetNode<SkeletonIK3D>("%left_arm_IK3D");
        headIK = GetNode<SkeletonIK3D>("%head_IK3D");

        //c.MovementStateChanged += onChangeMovementState;
        c.StateChanged += onChangeState;
        //stateMachinePlayback.Start("MoveState");
    }
    

    /*public void onChangeMovementState(MovementState ms)
    {
        if (IsInstanceValid(tween)) tween.Kill();
        
        tween = CreateTween();
        tween.TweenProperty(at, "parameters/MoveState/movementBlend/blend_position", ms.id, 0.2);
        //tween.Parallel().TweenProperty(at, "parameters/MoveState/animationTimescale/scale", ms.animationSpeed, 0.7);
    }*/

    public void onChangeState()
    {
        if (c.holdingWeapon)
        {
            leftArmIK.Start();
            at.Set("parameters/MoveState/hasWeapon/transition_request", "has_weapon");
            if (c.isCrouching)
            {
                at.Set("parameters/MoveState/isCrouchingGun/transition_request", "is_crouching");
            }
            else at.Set("parameters/MoveState/isCrouchingGun/transition_request", "not_crouching");
        }
        else
        {
            leftArmIK.Stop();
            at.Set("parameters/MoveState/hasWeapon/transition_request", "no_weapon");
            if (c.isCrouching)
            {
                at.Set("parameters/MoveState/isCrouchingUnarmed/transition_request", "is_crouching");
            }
            else at.Set("parameters/MoveState/isCrouchingUnarmed/transition_request", "not_crouching");
        }
    }

    
    
    public override void _PhysicsProcess(double delta)
    {
        float timescale = animationTimescale * c.localTimeScale * c.game.Timescale;
        at.Set("parameters/MoveState/animationTimescale/scale", timescale);
        float Delta = (float)delta * animationTimescale * c.localTimeScale * c.game.Timescale; 
       if (IsInstanceValid(tween)) tween.CustomStep(Delta);
        at.Advance(Delta);
        
        //MOVE DIRECTION AND GUN TOGGLE
        Vector3 inputDir = new Vector3(c.inputDir.X, 0f, c.inputDir.Y);
        inputDir = inputDir.Rotated(Vector3.Up, (float)Math.PI/4);
        Vector3 moveDir3D = (inputDir.Z * meshPivot.GlobalBasis.Z) + (inputDir.X * meshPivot.GlobalBasis.X);
        Vector2 moveDir = new Vector2 (moveDir3D.X, moveDir3D.Z);

        dir = dir.Lerp(moveDir, Delta * lerpSpeed);
        
        if (c.holdingWeapon)
        {
            if (c.isCrouching)
            {
                at.Set("parameters/MoveState/gunCrouchBlend/blend_position", dir);
            }
            else at.Set("parameters/MoveState/gunIdleBlend/blend_position", dir);
            Rotate(c.forwardDir, rotationSpeed, Delta);
        }
        else
        {
            //at.Set("parameters/MoveState/movementBlend/blend_position", c.currMoveState.id);
            if (c.Velocity.Length() > 0.1f)
            {
                if (IsInstanceValid(tween)) tween.Kill();
        
                tween = CreateTween();
                tween.TweenProperty(at, "parameters/MoveState/movementBlend/blend_position", 1, 0.2);
                tween.Parallel().TweenProperty(at, "parameters/MoveState/movementBlendCrouch/blend_position", 1, 0.2);
                tween.Pause();
                //tween.Parallel().TweenProperty(at, "parameters/MoveState/animationTimescale/scale", ms.animationSpeed, 0.7);
            }
            else 
            {
                if (IsInstanceValid(tween)) tween.Kill();
        
                tween = CreateTween();
                tween.TweenProperty(at, "parameters/MoveState/movementBlend/blend_position", 0, 0.1);
                tween.Parallel().TweenProperty(at, "parameters/MoveState/movementBlendCrouch/blend_position", 0, 0.1);
                tween.Pause();
            }

            if (c.Velocity.Length() > 0)
            {
                Rotate(c.Velocity, rotationSpeed, Delta);
            }

        }

        //at.Set("parameters/MoveState/animationTimescale", animationTimescale * c.localTimeScale * c.game.Timescale);
        
        //RECOIL
        if (c.recoiling) Recoil(Delta);

    }



    public void Rotate(Vector3 direction, float speed, float delta)
	{
        Vector3 scale = meshPivot.Scale;

		float angle = Mathf.LerpAngle(meshPivot.GlobalRotation.Y, Mathf.Atan2(direction.X, direction.Z), speed * delta); 
		Vector3 newRotation = new Vector3(meshPivot.GlobalRotation.X, angle, meshPivot.GlobalRotation.Z);
		meshPivot.GlobalRotation = newRotation;

		meshPivot.Scale = scale;
	}

    

    //RECOIL
    Node3D rightHandTarget; //set in ready
    Node3D headTarget;
    public void StartRecoil()
    {
        int bone = skeleton.FindBone(rightArmIK.TipBone);
        Transform3D boneTransformLocal = skeleton.GetBoneGlobalPose(bone);
        Vector3 StartPos = boneTransformLocal.Origin;//skeleton.ToGlobal(boneTransformLocal.Origin);
        
        rightHandTarget = GetNode<Node3D>("%right_arm_target");
        //rightHandTarget.Position = StartPos;
        rightHandTarget.Basis = boneTransformLocal.Basis;
        
        //set target pos
        Vector3 newPos = StartPos;
        newPos.Z = newPos.Z - (c.wm.currWeapon.kickback);
        rightHandTarget.Position = newPos;

        bone = skeleton.FindBone("Neck");
        boneTransformLocal = skeleton.GetBoneGlobalPose(bone);
        StartPos = boneTransformLocal.Origin;

        headTarget = GetNode<Node3D>("%head_target");
        //headTarget.Position = StartPos;
        headTarget.Basis = boneTransformLocal.Basis;

        newPos = StartPos; //- (headTarget.Basis.Z * c.wm.currWeapon.kickback);
        newPos.Z = newPos.Z -(c.wm.currWeapon.kickback * 0.5f);
        headTarget.Position = newPos;

        c.recoiling = true;
        rightArmIK.Start();
        headIK.Start();
    }
    
    float recoilProgress = 0;
    float curveSample;
    public void Recoil(float delta)
    {
        curveSample = c.wm.currWeapon.recoilCurve.Sample(recoilProgress);
        
        rightArmIK.Interpolation = curveSample;
        headIK.Interpolation = curveSample;
        /*Transform3D boneTransformLocal = skeleton.GetBoneGlobalPose(chest);
        boneTransformLocal.Origin = chestStartPos - (c.forwardDir.Normalized() * c.wm.currWeapon.kickback * curveSample * 0.05f);
        skeleton.SetBonePosePosition(chest, boneTransformLocal.Origin);
        skeleton.SetBonePoseRotation(chest, boneTransformLocal.Basis.GetRotationQuaternion());*/
        
        recoilProgress += delta * c.wm.currWeapon.recoilSpeed;

        if (recoilProgress > 1)
        {
            recoilProgress = 0;
            rightArmIK.Stop();
            headIK.Stop();
            c.recoiling = false;
        }
    }
}
