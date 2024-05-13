using Godot;
using Godot.Collections;
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

        // for (int x = 0; x < blockList.GetLength(0); x++)
        // {
        //     for (int y = 0; y < blockList.GetLength(1); y++)
        //     {
        //         for (int z = 0; z < blockList.GetLength(2); z++)
        //         {
        //             VoxelBlock voxelBlock = blockList[x, y, z];
        //             if (voxelBlock != null){
        //                 voxelBlock.parentChunk = this;
        //                 // voxelBlock.localPosition = new Vector3I(x, y, z);
        //                 voxelBlock.localPosition = new Vector3I(x, y, z);
        //                 // voxelBlock.globalPosition = new Vector3I(chunk_position.X * voxelWorld.chunk_size, chunk_position.Y * voxelWorld.chunk_size, chunk_position.Z * voxelWorld.chunk_size) + voxelBlock.localPosition;
        //             }
        //         }
        //     }
        // }

        // build_mesh();
    }

    public MeshInstance3D buildMesh(){
        Mesh mesh = builder.build(this);
        //remove old collision
        if(mesh == null){
            return null;
        }

        if(instance == null){
            instance = new MeshInstance3D();
        }else{
            Array<Node> chilren = instance.GetChildren();

            foreach(Node node in chilren){
                if(node is StaticBody3D){
                    GD.Print("Found static body");
                    instance.RemoveChild(node);

                    break;
                }
            }
        }
        instance.Mesh = mesh;

        // for(int x = 0; x < blockList.GetLength(0); x++){
        //     for(int y = 0; y < blockList.GetLength(1); y++){
        //         for(int z = 0; z < blockList.GetLength(2); z++){
        //             VoxelBlock block = blockList[x, y, z];

        //             if(block != null){
        //                 block.parentChunk = this;
        //                 block.localPosition = new Vector3I(x, y, z);
        //             } 
        //         }
        //     }
        // }

        if (instance != null){
            instance.Position = new Vector3(chunk_position.X, chunk_position.Y, chunk_position.Z) * voxelWorld.chunk_size;
            // instance.Scale = new Vector3(0.8f, 0.8f, 0.8f);
            instance.CreateTrimeshCollision();
            instance.Mesh.SurfaceSetMaterial(0, material);
        }

        return instance;
    }

    private void updateEdgeBlock(VoxelChunk chunk){
        //if any blocks god updated we need to regen the mesh
        bool hasChanged = false;
        if(chunk.chunk_position.X < chunk_position.X){
        // //left neighbor
            for (int y = 0; y < voxelWorld.chunk_size; y++){
                for(int z = 0; z < voxelWorld.chunk_size; z++){
                    
                }
            }
        }
    }

    
    // public void updateNeighbor(VoxelChunk chunk){
    //     bool hasChanged = false;
    // }

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
