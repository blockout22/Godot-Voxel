using Godot;
using System;

[GlobalClass]
public partial class HillsTerrainGenerator : VoxelGenerator
{

    [Export]
    public string blockName1 = "sand";

    [Export]
    public string blockName2 = "grass";

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

                    float rand = GD.Randf();
                    if (rand < 0.1f){
                        voxelBlock = (BlockSand)voxelWorld.findRegisteredBlockByName(blockName1).CloneAs<BlockSand>();
                        ((BlockSand)voxelBlock).helloSand();
                    }else{
                        voxelBlock = (VoxelBlock)voxelWorld.findRegisteredBlockByName(blockName2).CloneAs<VoxelBlock>();
                    }
                    // else if(rand < 0.8){
                    //     // voxelBlock = (VoxelBlock)voxelWorld.findRegisteredBlockByName("water");
                    // }else{
                    //     // voxelBlock = (VoxelBlock)voxelWorld.findRegisteredBlockByName("brick");
                    // }

                    blockList[x, y, z] = (globalPosition.Y <= noiseHeight ? voxelBlock : null);
                }
            }
        }
        return blockList;
    }
}
