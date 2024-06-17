using Godot;
using System;

[GlobalClass]
public partial class Game : Node
{
    public enum GameState
    {
        menu,
        gameplay,
    }
    public GameState state;

    public float Timescale = 1;
    private float lastTimescale = 1;
    public void SetState(GameState s)
    {
        switch(s)
        {
            case GameState.menu:
          
            lastTimescale = Timescale;
            Timescale = 0;
            break;

            case GameState.gameplay:
          
            Timescale = lastTimescale;
            break;
        }
    }

    [ExportCategory("Gameplay Settings")]
    public bool aimAssist = true;
    public float aimAssistRadius = 1f;

    

    //functions
    public float GetPercentage(float origin, float percentage)
    {
        float r;
        r = (origin/100) * percentage;
        return r;
    }

    public Vector3 flattenVector(Vector3 v)
    {
        return new Vector3(v.X, 0, v.Z);
    }
}
