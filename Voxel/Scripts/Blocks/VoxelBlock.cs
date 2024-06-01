using Godot;
using System;
using System.Reflection;

[GlobalClass]
public partial class VoxelBlock : Resource
{
    [Export] 
    public String name{get; set;} = "block";
    [Export]
    public String description;
    [Export]
    public ShaderMaterial material;
    // [Export]
    // public Texture2D topTexture;
    // [Export]
    // public Texture2D bottomTexture;
    // [Export]
    // public Texture2D frontTexture;
    // [Export]
    // public Texture2D backTexture;
    // [Export]
    // public Texture2D leftTexture;
    // [Export]
    // public Texture2D rightTexture;

    // public Rect2 uvCoodds = new Rect2(new Vector2(0, 0), new Vector2(1, 1));
    // public Rect2 UVCoordsLeft = new Rect2(new Vector2(0, 0), new Vector2(1, 1));
    // public Rect2 UVCoordsBottom = new Rect2(new Vector2(0, 0), new Vector2(1, 1));
    // public Rect2 UVCoordsBack = new Rect2(new Vector2(0, 0), new Vector2(1, 1));
    // public Rect2 UVCoordsFront = new Rect2(new Vector2(0, 0), new Vector2(1, 1));
    // public Rect2 UVCoordsTop = new Rect2(new Vector2(0, 0), new Vector2(1, 1));
    // public Rect2 UVCoordsRight = new Rect2(new Vector2(0, 0), new Vector2(1, 1));
    public VoxelChunk parentChunk;
    public Vector3I localPosition;
    public Vector3I globalPosition;

    public T CloneAs<T>() where T : VoxelBlock, new(){
        // VoxelBlock voxelBlock = new VoxelBlock();
        // voxelBlock.name = name;
        // voxelBlock.description = description;
        // voxelBlock.topTexture = topTexture;
        // voxelBlock.leftTexture = leftTexture;
        // voxelBlock.rightTexture = rightTexture;
        // voxelBlock.bottomTexture = bottomTexture;
        // voxelBlock.frontTexture = frontTexture;
        // voxelBlock.backTexture = backTexture;

        // voxelBlock.UVCoordsLeft = UVCoordsLeft;
        // voxelBlock.UVCoordsBottom = UVCoordsBottom;
        // voxelBlock.UVCoordsBack = UVCoordsBack;
        // voxelBlock.UVCoordsFront = UVCoordsFront;
        // voxelBlock.UVCoordsTop = UVCoordsTop;
        // voxelBlock.UVCoordsRight = UVCoordsRight;

        T copy = new T();
        CopyProperties(this, copy);
        return copy;
    }

    private void CopyProperties(VoxelBlock source, VoxelBlock destination)
    {
        destination.name = source.name;
        destination.description = source.description;
        destination.material = source.material;
        // destination.topTexture = source.topTexture;
        // destination.leftTexture = source.leftTexture;
        // destination.rightTexture = source.rightTexture;
        // destination.bottomTexture = source.bottomTexture;
        // destination.frontTexture = source.frontTexture;
        // destination.backTexture = source.backTexture;

        // destination.UVCoordsLeft = source.UVCoordsLeft;
        // destination.UVCoordsBottom = source.UVCoordsBottom;
        // destination.UVCoordsBack = source.UVCoordsBack;
        // destination.UVCoordsFront = source.UVCoordsFront;
        // destination.UVCoordsTop = source.UVCoordsTop;
        // destination.UVCoordsRight = source.UVCoordsRight;
    }

    public VoxelBlock getNeighbor(Neighbor neighbor){
		VoxelBlock block = null;
		switch(neighbor){
			case Neighbor.LEFT_DOWN_BACK:
				block = parentChunk.voxelWorld.getVoxelBlockAt(globalPosition.X + -1, globalPosition.Y + -1, globalPosition.Z + -1);
				break;
			case Neighbor.LEFT_DOWN:
				block = parentChunk.voxelWorld.getVoxelBlockAt(globalPosition.X + -1, globalPosition.Y + -1, globalPosition.Z + 0);
				break;
			case Neighbor.LEFT_DOWN_FRONT:
				block = parentChunk.voxelWorld.getVoxelBlockAt(globalPosition.X + -1, globalPosition.Y + -1, globalPosition.Z + 1);
				break;
			case Neighbor.LEFT_BACK:
				block = parentChunk.voxelWorld.getVoxelBlockAt(globalPosition.X + -1, globalPosition.Y + 0, globalPosition.Z + -1);
				break;
			case Neighbor.LEFT:
				block = parentChunk.voxelWorld.getVoxelBlockAt(globalPosition.X + -1, globalPosition.Y + 0, globalPosition.Z + 0);
				break;
			case Neighbor.LEFT_FRONT:
				block = parentChunk.voxelWorld.getVoxelBlockAt(globalPosition.X + -1, globalPosition.Y + 0, globalPosition.Z + 1);
				break;
			case Neighbor.LEFT_UP_BACK:
				block = parentChunk.voxelWorld.getVoxelBlockAt(globalPosition.X + -1, globalPosition.Y + 1, globalPosition.Z + -1);
				break;
			case Neighbor.LEFT_UP:
				block = parentChunk.voxelWorld.getVoxelBlockAt(globalPosition.X + -1, globalPosition.Y + 1, globalPosition.Z + 0);
				break;
			case Neighbor.LEFT_UP_FRONT:
				block = parentChunk.voxelWorld.getVoxelBlockAt(globalPosition.X + -1, globalPosition.Y + 1, globalPosition.Z + 1);
				break;
			case Neighbor.DOWN_BACK:
				block = parentChunk.voxelWorld.getVoxelBlockAt(globalPosition.X + 0, globalPosition.Y + -1, globalPosition.Z + -1);
				break;
			case Neighbor.DOWN:
				block = parentChunk.voxelWorld.getVoxelBlockAt(globalPosition.X + 0, globalPosition.Y + -1, globalPosition.Z + 0);
				break;
			case Neighbor.DOWN_FRONT:
				block = parentChunk.voxelWorld.getVoxelBlockAt(globalPosition.X + 0, globalPosition.Y + -1, globalPosition.Z + 1);
				break;
			case Neighbor.BACK:
				block = parentChunk.voxelWorld.getVoxelBlockAt(globalPosition.X + 0, globalPosition.Y + 0, globalPosition.Z + -1);
				break;
			case Neighbor.FRONT:
				block = parentChunk.voxelWorld.getVoxelBlockAt(globalPosition.X + 0, globalPosition.Y + 0, globalPosition.Z + 1);
				break;
			case Neighbor.UP_BACK:
				block = parentChunk.voxelWorld.getVoxelBlockAt(globalPosition.X + 0, globalPosition.Y + 1, globalPosition.Z + -1);
				break;
			case Neighbor.UP:
				block = parentChunk.voxelWorld.getVoxelBlockAt(globalPosition.X + 0, globalPosition.Y + 1, globalPosition.Z + 0);
				break;
			case Neighbor.UP_FRONT:
				block = parentChunk.voxelWorld.getVoxelBlockAt(globalPosition.X + 0, globalPosition.Y + 1, globalPosition.Z + 1);
				break;
			case Neighbor.RIGHT_DOWN_BACK:
				block = parentChunk.voxelWorld.getVoxelBlockAt(globalPosition.X + 1, globalPosition.Y + -1, globalPosition.Z + -1);
				break;
			case Neighbor.RIGHT_DOWN:
				block = parentChunk.voxelWorld.getVoxelBlockAt(globalPosition.X + 1, globalPosition.Y + -1, globalPosition.Z + 0);
				break;
			case Neighbor.RIGHT_DOWN_FRONT:
				block = parentChunk.voxelWorld.getVoxelBlockAt(globalPosition.X + 1, globalPosition.Y + -1, globalPosition.Z + 1);
				break;
			case Neighbor.RIGHT_BACK:
				block = parentChunk.voxelWorld.getVoxelBlockAt(globalPosition.X + 1, globalPosition.Y + 0, globalPosition.Z + -1);
				break;
			case Neighbor.RIGHT:
				block = parentChunk.voxelWorld.getVoxelBlockAt(globalPosition.X + 1, globalPosition.Y + 0, globalPosition.Z + 0);
				break;
			case Neighbor.RIGHT_FRONT:
				block = parentChunk.voxelWorld.getVoxelBlockAt(globalPosition.X + 1, globalPosition.Y + 0, globalPosition.Z + 1);
				break;
			case Neighbor.RIGHT_UP_BACK:
				block = parentChunk.voxelWorld.getVoxelBlockAt(globalPosition.X + 1, globalPosition.Y + 1, globalPosition.Z + -1);
				break;
			case Neighbor.RIGHT_UP:
				block = parentChunk.voxelWorld.getVoxelBlockAt(globalPosition.X + 1, globalPosition.Y + 1, globalPosition.Z + 0);
				break;
			case Neighbor.RIGHT_UP_FRONT:
				block = parentChunk.voxelWorld.getVoxelBlockAt(globalPosition.X + 1, globalPosition.Y + 1, globalPosition.Z + 1);
				break;
		}

        return block;
    }

    public enum Neighbor{
        LEFT_DOWN_BACK, 
        LEFT_DOWN, 
        LEFT_DOWN_FRONT, 
        LEFT_BACK, 
        LEFT, 
        LEFT_FRONT, 
        LEFT_UP_BACK, 
        LEFT_UP, 
        LEFT_UP_FRONT, 
        DOWN_BACK, 
        DOWN, 
        DOWN_FRONT, 
        BACK, 
        FRONT, 
        UP_BACK, 
        UP, 
        UP_FRONT, 
        RIGHT_DOWN_BACK, 
        RIGHT_DOWN, 
        RIGHT_DOWN_FRONT, 
        RIGHT_BACK, 
        RIGHT, 
        RIGHT_FRONT, 
        RIGHT_UP_BACK, 
        RIGHT_UP, 
        RIGHT_UP_FRONT
    }

}
