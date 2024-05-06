using Godot;
using System;

public partial class VoxelChunk
{
    public VoxelWorld voxelWorld;
    Vector3 chunk_position;

    bool[,,] blockList;

    public VoxelChunk(VoxelWorld _voxelWorld, Vector3 _grid_position){
        voxelWorld = _voxelWorld;
        this.chunk_position = _grid_position;
        blockList = new bool[voxelWorld.chunk_size, voxelWorld.chunk_size, voxelWorld.chunk_size];
    }

    public MeshInstance3D generate(){
        VoxelBuilder builder = new VoxelBuilder(voxelWorld);

        drawTerrain();

        return build_mesh(builder);
    }

    private MeshInstance3D build_mesh(VoxelBuilder builder){
        MeshInstance3D instance = builder.build(blockList);
        if (instance != null){
            instance.Position = new Vector3(chunk_position.X, chunk_position.Y, chunk_position.Z) * voxelWorld.chunk_size;
            instance.CreateTrimeshCollision();
            // instance.Mesh.SurfaceSetMaterial(0, material)
        }

        return instance;
    }

    private void drawTerrain(){
        for (int x = 0; x < voxelWorld.chunk_size; x++){
            for (int y = 0; y < voxelWorld.chunk_size; y++){
                for (int z = 0; z < voxelWorld.chunk_size; z++){
                    Vector3 globalPosition = new Vector3(chunk_position.X * voxelWorld.chunk_size + x,
                                                         chunk_position.Y * voxelWorld.chunk_size + y,
                                                         chunk_position.Z * voxelWorld.chunk_size + z);
                    
                    float noiseHeight = voxelWorld.GetNoiseValue(globalPosition.X, globalPosition.Z) * voxelWorld.maxTerrainHeight;
                    blockList[x, y, z] = globalPosition.Y <= noiseHeight;
                }
            }
        }
    }

    private void drawBlocks(){
        for (int x = 0; x < voxelWorld.chunk_size; x++){
            for (int y = 0; y < voxelWorld.chunk_size; y++){
                for (int z = 0; z < voxelWorld.chunk_size; z++){
                    blockList[x, y, z] = true;
                }
            }
        }
    }
}
