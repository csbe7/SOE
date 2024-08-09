using Godot;
using System;

public partial class Game : Node
{
    public float Timescale = 1;

    [ExportCategory("Settings")]
    public bool aimAssist = true;
    public float aimAssistRadius = 1f;
}
