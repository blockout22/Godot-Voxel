using Godot;
using System;

public partial class VoxelChunk
{
    public MeshInstance3D instance;
    public VoxelWorld voxelWorld;
    public Vector3I chunk_position;
    public BaseMaterial3D material;

    public VoxelBlock[,,] blockList;

    VoxelBuilder builder;

    public VoxelChunk(VoxelWorld _voxelWorld, Vector3I _grid_position){
        voxelWorld = _voxelWorld;
        this.chunk_position = _grid_position;
        blockList = new VoxelBlock[voxelWorld.chunk_size, voxelWorld.chunk_size, voxelWorld.chunk_size];
    }

    public void generate(VoxelGenerator voxelGenerator){
        builder = new VoxelBuilder(voxelWorld);

        // drawTerrain();
        blockList = voxelGenerator.build(voxelWorld, chunk_position);

        // build_mesh();
    }

    public MeshInstance3D buildMesh(){
        instance = builder.build(this);
        if (instance != null){
            instance.Position = new Vector3(chunk_position.X, chunk_position.Y, chunk_position.Z) * voxelWorld.chunk_size;
            // instance.Scale = new Vector3(0.8f, 0.8f, 0.8f);
            instance.CreateTrimeshCollision();
            instance.Mesh.SurfaceSetMaterial(0, material);
        }

        return instance;
    }

    public bool removeBlockAt(int x, int y, int z){
        if (blockList[x, y, z] != null){
            blockList[x, y, z] = null;
            return true;
        }

        return false;
    }

    public VoxelBlock getBlockAt(int x, int y, int z){
        return blockList[x, y, z];
    }

    // private void drawTerrain(){
    //     for (int x = 0; x < voxelWorld.chunk_size; x++){
    //         for (int y = 0; y < voxelWorld.chunk_size; y++){
    //             for (int z = 0; z < voxelWorld.chunk_size; z++){
    //                 Vector3 globalPosition = new Vector3(chunk_position.X * voxelWorld.chunk_size + x,
    //                                                      chunk_position.Y * voxelWorld.chunk_size + y,
    //                                                      chunk_position.Z * voxelWorld.chunk_size + z);
                    
    //                 float noiseHeight = voxelWorld.GetNoiseValue(globalPosition.X, globalPosition.Z) * voxelWorld.maxTerrainHeight;
    //                 blockList[x, y, z] = (globalPosition.Y <= noiseHeight ? null : null);
    //             }
    //         }
    //     }
    // }

    // private void drawBlocks(){
    //     for (int x = 0; x < voxelWorld.chunk_size; x++){
    //         for (int y = 0; y < voxelWorld.chunk_size; y++){
    //             for (int z = 0; z < voxelWorld.chunk_size; z++){
    //                 blockList[x, y, z] = true;
    //             }
    //         }
    //     }
    // }
}
