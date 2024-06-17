using Godot;
using System;


public partial class CharacterController : CharacterBody3D
{
    public Game game;

    camera camPivot;
    Camera3D cam;
    AnimationTree at;

    public WeaponManager wm;
    public AnimationController ac;
    
    [Export] PackedScene spawn;

    [ExportCategory("Physics")]
    public float localTimeScale = 1;
    [Export] float speed = 5;
    [Export] public float Acceleration = 10;
    [Export] public float crouchSpeedPercentagePenalty = 30;
    [Export] public float strafeSpeedPercentagePenalty = 20;
    //[Export] public float gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle() * 6;
    
    [ExportCategory("State variables")]
    /*[Export] public Godot.Collections.Dictionary movementState;
    public MovementState currMoveState;*/

    public Vector3 forwardDir;
    
    public bool onFloor;
    public bool holdingWeapon;
    public bool isCrouching;
    public bool recoiling;

    
    public Vector2 inputDir;
    public Vector3 moveDir;
    

    //[Signal] public delegate void MovementStateChangedEventHandler(MovementState newState);
    [Signal] public delegate void StateChangedEventHandler();
    

    
    public override void _Ready()
    {
        game = GetTree().Root.GetChild<Game>(0);
        camPivot =  GetTree().Root.GetChild<Node3D>(1).GetNode<camera>("%CameraPivot");
        cam = camPivot.GetNode<Camera3D>("Camera3D");

        at = GetParent().GetNode<Node3D>("Mesh").GetNode<AnimationTree>("AnimationTree");

        ac = GetNode<AnimationController>("%AnimationController");
        wm = GetNode<WeaponManager>("%WeaponManager");


        //SetMovementState((MovementState)movementState["idle"]);

        forwardDir = GlobalBasis.Z;
    }

    void GetInput()
    {
        if (game.Timescale * localTimeScale == 0) return;

        if (Input.IsActionJustPressed("Debug"))
        {
            var Spawn = spawn.Instantiate<ItemDrop>();
            GetTree().Root.AddChild(Spawn);
            Spawn.GlobalPosition = this.GlobalPosition;
            Spawn.Pool();
            
        }


        bool updateState = false;

        if (Input.IsActionPressed("Move"))
        {
            //inputDir = Input.GetVector("Left", "Right", "Down", "Up"); 
            inputDir = new Vector2(Input.GetActionStrength("Right") - Input.GetActionStrength("Left"), Input.GetActionStrength("Up") - Input.GetActionStrength("Down"));
            Vector2 inputNorm = inputDir.Normalized();
            moveDir = (game.flattenVector(cam.GlobalBasis.X).Normalized() * inputNorm.X) + (game.flattenVector(-cam.GlobalBasis.Z).Normalized() * inputNorm.Y).Normalized();

            if (moveDir != Vector3.Zero)
            {
                //SetMovementState((MovementState)movementState["run"]);
            }
        }
        else
        {
            inputDir = Vector2.Zero;
            moveDir = Vector3.Zero;
            //SetMovementState((MovementState)movementState["idle"]);
        }

        if (Input.IsActionJustPressed("Crouch"))
        {
            isCrouching = !isCrouching;
            updateState = true;
        }
        if (Input.IsActionJustPressed("Weapon Switch"))
        {
            holdingWeapon = !holdingWeapon;
            updateState = true;
        }
        
        if (Input.IsActionPressed("Attack") && !recoiling && holdingWeapon)
        {
            wm.UseWeapon();
        }

        if (updateState) EmitSignal(SignalName.StateChanged);
    }

    public override void _Process(double delta)
    {
        GetInput();
    }

    public override void _PhysicsProcess(double delta)
    {
        GetForwardDir();
        Move((float)delta);
        
        MoveAndSlide();
    }
    
     
    void Move(float delta)
    {
        Vector3 velocity;
        float finalSpeed = speed;

        
        if (isCrouching)
        {
            finalSpeed -= game.GetPercentage(speed, crouchSpeedPercentagePenalty);
        }
        if (holdingWeapon && forwardDir.Dot(moveDir) < 0.1f)
        {
            finalSpeed -= game.GetPercentage(speed, strafeSpeedPercentagePenalty);
        }

        velocity = moveDir * finalSpeed * game.Timescale;
        Velocity = velocity;
        
       
    }

    void GetForwardDir()
    {
        if (holdingWeapon)
        {
            Godot.Collections.Dictionary r = camPivot.ShootRayToMouse(1);
            
            if (game.aimAssist && r != null)
            {
                Vector3 targetPos = aimAssist((Vector3)r["position"]);
                if (targetPos != GlobalPosition) forwardDir = targetPos - GlobalPosition;
                else forwardDir = (Vector3)r["position"] - GlobalPosition;
            }
            else if (r != null)forwardDir = (Vector3)r["position"] - GlobalPosition;
            forwardDir = game.flattenVector(forwardDir).Normalized();
        }
        else
        {
           forwardDir = moveDir.Normalized();
        }
    }
    
    
    /*public void SetMovementState(MovementState ms)
    {
        //var oldMoveState = currMoveState;
        if (currMoveState != ms) EmitSignal(SignalName.MovementStateChanged, ms);
        currMoveState = ms;
    }*/

    Vector3 aimAssist(Vector3 position)
    {
        PhysicsShapeQueryParameters3D query = new PhysicsShapeQueryParameters3D();
        Transform3D queryTransform = query.Transform;
        queryTransform.Origin = position;
        query.Transform = queryTransform;
        SphereShape3D sphere = new SphereShape3D();
        sphere.Radius = game.aimAssistRadius;
        query.Shape = sphere;

        var space = GetWorld3D().DirectSpaceState;

        var result = space.IntersectShape(query);

        foreach(Godot.Collections.Dictionary r in result)
        {
            if (GetTree().GetNodesInGroup("Player").Contains((Node)r["collider"]) || GetTree().GetNodesInGroup("Floor").Contains((Node)r["collider"]))
            {
                continue;
            }
             
            CharacterStatus cs = ((Node3D)r["collider"]).GetNodeOrNull<CharacterStatus>("%CharacterStatus");
            if (IsInstanceValid(cs))
            {
                return ((Node3D)r["collider"]).GlobalPosition;
            }
        }
        return this.GlobalPosition;
    }

}
