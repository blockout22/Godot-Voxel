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

        drawBlocks();

        return build_mesh(builder);
    }

    private MeshInstance3D build_mesh(VoxelBuilder builder){
        MeshInstance3D instance = builder.build(blockList);
        GD.Print("is my Mesh nbull?> " + instance);
        if (instance != null){
            instance.Position = new Vector3(chunk_position.X, chunk_position.Y, chunk_position.Z) * voxelWorld.chunk_size;
            instance.CreateTrimeshCollision();
            // instance.Mesh.SurfaceSetMaterial(0, material)
        }

        return instance;
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
