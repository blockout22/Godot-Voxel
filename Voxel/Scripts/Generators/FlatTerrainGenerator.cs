using Godot;
using System;

[GlobalClass]
public partial class FlatTerrainGenerator : VoxelGenerator
{

    [Export]
    private int MaxTerrainHeight = 5;

    public override VoxelBlock[,,] build(VoxelWorld voxelWorld, Vector3I chunkPosition){
        int size = voxelWorld.chunk_size;
        VoxelBlock[,,] blockList = new VoxelBlock[size, size, size];

        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                for (int z = 0; z < size; z++)
                {
                    Vector3 globalPosition = new Vector3(
                        chunkPosition.X * size + x,
                        chunkPosition.Y * size + y,
                        chunkPosition.Z * size + z
                    );

                    // Example rule: Simple flat terrain generation
                    if (globalPosition.Y < MaxTerrainHeight)
                    {
                        if(GD.Randf() < 0.5f){
                            blockList[x, y, z] = (VoxelBlock)voxelWorld.registeredBlocks[0].CloneAs<VoxelBlock>();
                        }else{
                             blockList[x, y, z] = (VoxelBlock)voxelWorld.registeredBlocks[1].CloneAs<VoxelBlock>();
                        }
                    }
                }
            }
        }

        return blockList;
    }
}
