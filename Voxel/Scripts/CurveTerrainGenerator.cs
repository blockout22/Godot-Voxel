using Godot;
using System;

[GlobalClass]
public partial class CurveTerrainGenerator : VoxelGenerator
{

    [Export]
    private int MaxTerrainHeight = 10;

    [Export]
    public Curve terrainCurve = new Curve();

    public override VoxelBlock[,,] build(VoxelWorld voxelWorld, Vector3I chunkPosition)
    {
        int size = voxelWorld.chunk_size;
        VoxelBlock[,,] blockList = new VoxelBlock[size, size, size];
        

        for (int x = 0; x < size; x++)
        {
            for (int z = 0; z < size; z++)
            {
                // Normalize X and Z coordinates to be in the range of the Curve
                float normalizedX = (float)x / size;
                float normalizedZ = (float)z / size;

                // Average X and Z normalized coordinates and apply the curve
                float curveInput = (normalizedX + normalizedZ) / 2;
                float heightFraction = terrainCurve.SampleBaked(curveInput);

                // Calculate the actual height based on the MaxTerrainHeight
                int terrainHeight = Mathf.RoundToInt(heightFraction * MaxTerrainHeight);

                // Generate the terrain column up to the calculated terrain height
                for (int y = 0; y < terrainHeight && y < size; y++)
                {
                    Vector3 globalPosition = new Vector3(
                        chunkPosition.X * size + x,
                        chunkPosition.Y * size + y,
                        chunkPosition.Z * size + z
                    );

                    if (globalPosition.Y < terrainHeight){
                        // Example rule: Assign the first block type to this column
                        blockList[x, y, z] = (VoxelBlock)voxelWorld.registeredBlocks[0].Clone();
                    }
                }
            }
        }

        return blockList;
    }
}
