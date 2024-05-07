using Godot;
using System;

[GlobalClass]
public partial class HillsTerrainGenerator : VoxelGenerator
{
    public override VoxelBlock[,,] build(VoxelWorld voxelWorld, Vector3I chunkPosition)
    {
        int size = voxelWorld.chunk_size;
        VoxelBlock[,,] blockList = new VoxelBlock[size, size, size];
        for (int x = 0; x < size; x++){
            for (int y = 0; y < size; y++){
                for (int z = 0; z < size; z++){
                    Vector3 globalPosition = new Vector3(chunkPosition.X * size + x,
                                                         chunkPosition.Y * size + y,
                                                         chunkPosition.Z * size + z);
                    
                    float noiseHeight = voxelWorld.GetNoiseValue(globalPosition.X, globalPosition.Z) * voxelWorld.maxTerrainHeight;
                    VoxelBlock voxelBlock = null;

                    if (GD.Randf() < 0.1f){
                        voxelBlock = ((VoxelBlock)voxelWorld.findRegisteredBlockByName("sand"));
                    }else{
                        voxelBlock = ((VoxelBlock)voxelWorld.findRegisteredBlockByName("grass"));
                    }

                    blockList[x, y, z] = (globalPosition.Y <= noiseHeight ? voxelBlock : null);
                }
            }
        }
        return blockList;
    }
}
