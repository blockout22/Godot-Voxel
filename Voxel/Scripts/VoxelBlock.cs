using Godot;
using System;

[GlobalClass]
public partial class VoxelBlock : Resource
{
    [Export] public String name{get; set;} = "Some block";
    [Export]
    public String description;
    [Export]
    public Texture2D texture;
}
