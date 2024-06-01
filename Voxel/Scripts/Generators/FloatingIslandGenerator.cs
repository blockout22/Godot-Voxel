using Godot;
using System;

[GlobalClass]
public partial class FloatingIslandGenerator : VoxelGenerator
{

    [Export]
    private int MaxTerrainHeight = 100;
    [Export]
    private float Frequency = 0.01f; 
    private FastNoiseLite noise;

    public FloatingIslandGenerator()
    {
        noise = new FastNoiseLite();
        noise.Seed = (int)GD.Randi() + 1;
        noise.NoiseType = FastNoiseLite.NoiseTypeEnum.Simplex;
        noise.SetFrequency(Frequency);
    }

    public override VoxelBlock[,,] build(VoxelWorld voxelWorld, Vector3I chunkPosition)
    {
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

                    float height = noise.GetNoise2D(globalPosition.X, globalPosition.Z) * MaxTerrainHeight;
                    if (globalPosition.Y < height)
                    {
                        float value = noise.GetNoise3D(globalPosition.X, globalPosition.Y, globalPosition.Z);
                        if (value > 0.4f)
                        {
                            blockList[x, y, z] = (VoxelBlock)voxelWorld.registeredBlocks[0].CloneAs<VoxelBlock>();
                        }
                    }
                }
            }
        }

        return blockList;
    }

}
