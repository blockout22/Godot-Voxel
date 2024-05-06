using Godot;
using System;

public abstract partial class VoxelGenerator : CSharpScript
{
    public abstract VoxelBlock[,,] build(VoxelWorld voxelWorld, Vector3I chunkPosition);
}
