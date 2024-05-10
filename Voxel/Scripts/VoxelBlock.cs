using Godot;
using System;

[GlobalClass]
public partial class VoxelBlock : Resource
{
    [Export] 
    public String name{get; set;} = "block";
    [Export]
    public String description;
    [Export]
    public Texture2D texture;

    public Rect2 uvCoodds = new Rect2(new Vector2(0, 0), new Vector2(1, 1));
    public VoxelChunk parentChunk;
    public Vector3I localPosition;
    public Vector3I globalPosition;
}
