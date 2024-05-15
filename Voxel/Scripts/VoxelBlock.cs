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
    public Texture2D topTexture;
    [Export]
    public Texture2D bottomTexture;
    [Export]
    public Texture2D frontTexture;
    [Export]
    public Texture2D backTexture;
    [Export]
    public Texture2D leftTexture;
    [Export]
    public Texture2D rightTexture;

    // public Rect2 uvCoodds = new Rect2(new Vector2(0, 0), new Vector2(1, 1));
    public Rect2 UVCoordsLeft = new Rect2(new Vector2(0, 0), new Vector2(1, 1));
    public Rect2 UVCoordsBottom = new Rect2(new Vector2(0, 0), new Vector2(1, 1));
    public Rect2 UVCoordsBack = new Rect2(new Vector2(0, 0), new Vector2(1, 1));
    public Rect2 UVCoordsFront = new Rect2(new Vector2(0, 0), new Vector2(1, 1));
    public Rect2 UVCoordsTop = new Rect2(new Vector2(0, 0), new Vector2(1, 1));
    public Rect2 UVCoordsRight = new Rect2(new Vector2(0, 0), new Vector2(1, 1));
    public VoxelChunk parentChunk;
    public Vector3I localPosition;
    public Vector3I globalPosition;

    public VoxelBlock Clone(){
        VoxelBlock voxelBlock = new VoxelBlock();
        voxelBlock.name = name;
        voxelBlock.description = description;
        voxelBlock.topTexture = topTexture;
        voxelBlock.leftTexture = leftTexture;
        voxelBlock.rightTexture = rightTexture;
        voxelBlock.bottomTexture = bottomTexture;
        voxelBlock.frontTexture = frontTexture;
        voxelBlock.backTexture = backTexture;

        voxelBlock.UVCoordsLeft = UVCoordsLeft;
        voxelBlock.UVCoordsBottom = UVCoordsBottom;
        voxelBlock.UVCoordsBack = UVCoordsBack;
        voxelBlock.UVCoordsFront = UVCoordsFront;
        voxelBlock.UVCoordsTop = UVCoordsTop;
        voxelBlock.UVCoordsRight = UVCoordsRight;
        return voxelBlock;
    }
}
