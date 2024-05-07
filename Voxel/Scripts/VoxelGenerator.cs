using Godot;
using System;

[GlobalClass]
public abstract partial class VoxelGenerator : Resource
{
    public abstract VoxelBlock[,,] build(VoxelWorld voxelWorld, Vector3I chunkPosition);
}
