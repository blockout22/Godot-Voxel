using Godot;
using System;

[GlobalClass]
public partial class IslandTerrainGenerator : VoxelGenerator
{

    [Export]
    public string blockName = "grass";
    
    [Export]
    private double radius = 10;
    [Export]
    private double height = 20;

    public override VoxelBlock[,,] build(VoxelWorld voxelWorld, Vector3I chunkPosition)
    {
        int size = voxelWorld.chunk_size;
        VoxelBlock[,,] blockList = new VoxelBlock[size, size, size];

        // Apply activation probability and spacing rules
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
                        if(isInside(globalPosition.X, globalPosition.Y, globalPosition.Z)){
                            blockList[x, y, z] = voxelWorld.findRegisteredBlockByName("grass").Clone();
                        }
                    }
                }
            }

        return blockList;
    }

    public bool isInside(double x, double y, double z) {
        // Check if the point is within the cone's height range
        if (y < 0 || y > height) {
            return false;
        }
        
        // Calculate the maximum radius at the height z
        double maxRadiusAtY = (radius / height) * y;
        
        // Check if the point (x, y) is inside the circle of radius maxRadiusAtZ at height z
        return (x * x + z * z) <= (maxRadiusAtY * maxRadiusAtY);
    }

}
