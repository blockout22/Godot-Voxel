using Godot;
using System;

[GlobalClass]
public partial class SphereTerrainGenerator : VoxelGenerator
{

    [Export]
    private int SphereRadius { get; set; } = 5;

    [Export]
    private int SphereSpacing { get; set; } = 20;

    public override VoxelBlock[,,] build(VoxelWorld voxelWorld, Vector3I chunkPosition)
    {
        int chunkSize = voxelWorld.chunk_size;
        VoxelBlock[,,] blockList = new VoxelBlock[chunkSize, chunkSize, chunkSize];

        // Calculate the global coordinates of the chunk
        Vector3I globalChunkOrigin = new Vector3I(
            chunkPosition.X * chunkSize,
            chunkPosition.Y * chunkSize,
            chunkPosition.Z * chunkSize
        );

        // Calculate the range of sphere centers that could influence this chunk
        Vector3I startCenter = new Vector3I(
            ((globalChunkOrigin.X) / SphereSpacing) * SphereSpacing,
            ((globalChunkOrigin.Y) / SphereSpacing) * SphereSpacing,
            ((globalChunkOrigin.Z) / SphereSpacing) * SphereSpacing
        );

        Vector3I endCenter = new Vector3I(
            ((globalChunkOrigin.X + chunkSize - 1) / SphereSpacing) * SphereSpacing,
            ((globalChunkOrigin.Y + chunkSize - 1) / SphereSpacing) * SphereSpacing,
            ((globalChunkOrigin.Z + chunkSize - 1) / SphereSpacing) * SphereSpacing
        );

        // Iterate over all possible sphere centers influencing this chunk
        for (int cx = startCenter.X; cx <= endCenter.X + SphereSpacing; cx += SphereSpacing)
        {
            for (int cy = startCenter.Y; cy <= endCenter.Y + SphereSpacing; cy += SphereSpacing)
            {
                for (int cz = startCenter.Z; cz <= endCenter.Z + SphereSpacing; cz += SphereSpacing)
                {
                    Vector3I sphereCenter = new Vector3I(cx, cy, cz);

                    // Render the sphere
                    for (int localX = 0; localX < chunkSize; localX++)
                    {
                        int globalX = globalChunkOrigin.X + localX;

                        for (int localZ = 0; localZ < chunkSize; localZ++)
                        {
                            int globalZ = globalChunkOrigin.Z + localZ;

                            for (int localY = 0; localY < chunkSize; localY++)
                            {
                                int globalY = globalChunkOrigin.Y + localY;

                                // Calculate the distance from the current voxel to the sphere's center
                                float distance = Mathf.Sqrt(
                                    Mathf.Pow(globalX - sphereCenter.X, 2) +
                                    Mathf.Pow(globalY - sphereCenter.Y, 2) +
                                    Mathf.Pow(globalZ - sphereCenter.Z, 2)
                                );

                                if (distance <= SphereRadius)
                                {
                                    blockList[localX, localY, localZ] = (VoxelBlock)voxelWorld.registeredBlocks[0].CloneAs<VoxelBlock>();
                                }
                            }
                        }
                    }
                }
            }
        }

        return blockList;
    }

}
