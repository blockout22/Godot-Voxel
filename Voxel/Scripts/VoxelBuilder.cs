using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class VoxelBuilder
{

    public VoxelWorld voxelWorld;

    private static readonly Dictionary<string, Vector3[]> FACES = new Dictionary<string, Vector3[]>
    {
        { "left", new Vector3[] { new Vector3(-0.5f, 0.5f, -0.5f), new Vector3(-0.5f, 0.5f, 0.5f), new Vector3(-0.5f, -0.5f, 0.5f), new Vector3(-0.5f, -0.5f, -0.5f) } },
        { "front", new Vector3[] { new Vector3(-0.5f, 0.5f, 0.5f), new Vector3(0.5f, 0.5f, 0.5f), new Vector3(0.5f, -0.5f, 0.5f), new Vector3(-0.5f, -0.5f, 0.5f) } },
        { "back", new Vector3[] { new Vector3(0.5f, 0.5f, -0.5f), new Vector3(-0.5f, 0.5f, -0.5f), new Vector3(-0.5f, -0.5f, -0.5f), new Vector3(0.5f, -0.5f, -0.5f) } },
        { "right", new Vector3[] { new Vector3(0.5f, 0.5f, 0.5f), new Vector3(0.5f, 0.5f, -0.5f), new Vector3(0.5f, -0.5f, -0.5f), new Vector3(0.5f, -0.5f, 0.5f) } },
        { "bottom", new Vector3[] { new Vector3(-0.5f, -0.5f, 0.5f), new Vector3(0.5f, -0.5f, 0.5f), new Vector3(0.5f, -0.5f, -0.5f), new Vector3(-0.5f, -0.5f, -0.5f) } },
        { "top", new Vector3[] { new Vector3(-0.5f, 0.5f, -0.5f), new Vector3(0.5f, 0.5f, -0.5f), new Vector3(0.5f, 0.5f, 0.5f), new Vector3(-0.5f, 0.5f, 0.5f) } }
    };

    int[] CUBE_INDICES = {
        0, 1, 2,
        2, 3, 0
    };

    Vector3I[] NEIGHBOR_OFFSETS = {
        new Vector3I(-1, 0, 0),  // Left neighbor
        new Vector3I(0, -1, 0),  // Bottom neighbor
        new Vector3I(0, 0, -1),  // Back neighbor
        new Vector3I(0, 0, 1),   // Front neighbor
        new Vector3I(0, 1, 0),   // Top neighbor
        new Vector3I(1, 0, 0),   // Right neighbor
    };

    int vertexCount = 0;

    SurfaceTool surfaceTool;

    public VoxelBuilder(VoxelWorld _voxelWorld){
        voxelWorld = _voxelWorld;
    }

    public MeshInstance3D build(VoxelChunk chunk){
        VoxelBlock[,,] blockList = chunk.blockList;
        surfaceTool = new SurfaceTool();
        vertexCount = 0;
        surfaceTool.Begin(Mesh.PrimitiveType.Triangles);

        for (int x = 0; x < blockList.GetLength(0); x++){
            for (int y = 0; y < blockList.GetLength(1); y++){
                for (int z = 0; z < blockList.GetLength(2); z++){
                    VoxelBlock block = blockList[x, y, z];
                    if (block != null){
                        object[] neighbors = getNeighbors(chunk, x, y, z);
                        Rect2 uvRect = block.uvCoodds;
                        float uv_offset_x = uvRect.Position.X;
                        float uv_offset_y = uvRect.Position.Y;
                        float uv_grid_size_x = uvRect.Size.X;
                        float uv_grid_size_y = uvRect.Size.Y;

                        Vector2[] faceUVs = {
                            new Vector2(uv_offset_x, uv_offset_y),
                            new Vector2(uv_offset_x + (1.0f * uv_grid_size_x), uv_offset_y), 
                            new Vector2(uv_offset_x + (1.0f * uv_grid_size_x), uv_offset_y + (1.0f * uv_grid_size_y)),
                            new Vector2(uv_offset_x, uv_offset_y + (1.0f * uv_grid_size_y))
                        };

                        int mask = getNeighborsMask(neighbors);

                        // object[] testMask = new object[6]{false, 1, false, false, false, false};
                        // int test = getNeighborsMask(testMask);
                        // string toPint = "";
                        // for (int i = 0; i < neighbors.GetLength(0); i++){
                            // toPint += (neighbors[i] is bool ? "false" : "true") + " ";
                        // }
                        // GD.Print(mask + " : " +  getNeighborsMask(testMask));

                        drawFromMask(mask, x, y, z, faceUVs);
                    }
                }
            }
        }

        if (vertexCount <= 0){
            return null;
        }

        MeshInstance3D instance = new MeshInstance3D();
        // surfaceTool.GenerateNormals();
        instance.Mesh = surfaceTool.Commit();
        return instance;
    }

    private object[] getNeighbors(VoxelChunk chunk, int x, int y, int z){
        VoxelBlock[,,] blockList = chunk.blockList;
        object[] neighbors = new object[6]{false, false, false, false, false, false};
        for (int i = 0; i < NEIGHBOR_OFFSETS.GetLength(0); i++){
            Vector3I offset = NEIGHBOR_OFFSETS[i];
            Vector3I neighbor_pos = new Vector3I(x, y, z) + offset;

            //TODO check neighboring chunks
            if (neighbor_pos.X < 0){
                VoxelChunk neighborChunk = voxelWorld.getVoxelChunkAtGrid(chunk.chunk_position.X - 1, chunk.chunk_position.Y, chunk.chunk_position.Z);
                if (neighborChunk != null){
                    Vector3I neighorBlockPos = new Vector3I(voxelWorld.chunk_size - 1, y, z);
                    VoxelBlock block = neighborChunk.blockList[neighorBlockPos.X, neighorBlockPos.Y, neighorBlockPos.Z];

                    if (block != null){
                        neighbors[i] = neighorBlockPos;
                    }
                }
                continue;
            }

            if (neighbor_pos.Y < 0)
            {
                VoxelChunk neighborChunk = voxelWorld.getVoxelChunkAtGrid(chunk.chunk_position.X, chunk.chunk_position.Y - 1, chunk.chunk_position.Z);
                if (neighborChunk != null){
                    Vector3I neighorBlockPos = new Vector3I(x, voxelWorld.chunk_size - 1, z);
                    VoxelBlock block = neighborChunk.blockList[neighorBlockPos.X, neighorBlockPos.Y, neighorBlockPos.Z];

                    if (block != null){
                        neighbors[i] = neighorBlockPos;
                    }
                }
                continue;
            }

            if (neighbor_pos.Z < 0)
            {
                VoxelChunk neighborChunk = voxelWorld.getVoxelChunkAtGrid(chunk.chunk_position.X, chunk.chunk_position.Y, chunk.chunk_position.Z - 1);
                if (neighborChunk != null){
                    Vector3I neighorBlockPos = new Vector3I(x, y, voxelWorld.chunk_size - 1);
                    VoxelBlock block = neighborChunk.blockList[neighorBlockPos.X, neighorBlockPos.Y, neighorBlockPos.Z];

                    if (block != null){
                        neighbors[i] = neighorBlockPos;
                    }
                }
                continue;
            }

            if (neighbor_pos.X > voxelWorld.chunk_size - 1){
                VoxelChunk neighborChunk = voxelWorld.getVoxelChunkAtGrid(chunk.chunk_position.X + 1, chunk.chunk_position.Y, chunk.chunk_position.Z);
                if (neighborChunk != null){
                    Vector3I neighorBlockPos = new Vector3I(0, y, z);
                    VoxelBlock block = neighborChunk.blockList[neighorBlockPos.X, neighorBlockPos.Y, neighorBlockPos.Z];

                    if (block != null){
                        neighbors[i] = neighorBlockPos;
                    }
                }
                continue;
            }

            if (neighbor_pos.Y > voxelWorld.chunk_size - 1){
                VoxelChunk neighborChunk = voxelWorld.getVoxelChunkAtGrid(chunk.chunk_position.X, chunk.chunk_position.Y + 1, chunk.chunk_position.Z);
                if (neighborChunk != null){
                    Vector3I neighorBlockPos = new Vector3I(x, 0, z);
                    VoxelBlock block = neighborChunk.blockList[neighorBlockPos.X, neighorBlockPos.Y, neighorBlockPos.Z];

                    if (block != null){
                        neighbors[i] = neighorBlockPos;
                    }
                }
                continue;
            }

            if (neighbor_pos.Z > voxelWorld.chunk_size - 1){
                VoxelChunk neighborChunk = voxelWorld.getVoxelChunkAtGrid(chunk.chunk_position.X , chunk.chunk_position.Y, chunk.chunk_position.Z + 1);
                if (neighborChunk != null){
                    Vector3I neighorBlockPos = new Vector3I(x, y, 0);
                    VoxelBlock block = neighborChunk.blockList[neighorBlockPos.X, neighorBlockPos.Y, neighorBlockPos.Z];

                    if (block != null){
                        neighbors[i] = neighorBlockPos;
                    }
                }
                continue;
            }

            if (blockList[neighbor_pos.X, neighbor_pos.Y, neighbor_pos.Z] != null){
                // GD.Print("Neightpos: " + NEIGHBOR_OFFSETS[i].Y );
                //maybe change neighbor_pos to be the blocks global pos and not the chunks block local position
                neighbors[i] = neighbor_pos;
            }
        }
        return neighbors;
    }

    private int getNeighborsMask(object[] neighbors){
        int mask = 0;

        for (int i = 0; i < neighbors.GetLength(0); i++){
            if (!(neighbors[i] is bool)){
                mask |= 1 << i;
            }
        }
        return mask;
    }

    private void drawFromMask(int mask, int x, int y, int z, Vector2[] uvs){
            Mesh mesh = voxelWorld.MASK[mask];
            if(mesh != null){
                Dictionary<string, object> data = voxelWorld.extractMeshData(mesh);
                draw((Vector3[])data["vertices"], (int[])data["indices"], new Vector3(x, y, z), (Vector2[])data["uvs"]);
                return;
            }

        if ((mask & 1 << 0) == 0){
            FACES.TryGetValue("left", out Vector3[] verts);
            draw(verts, CUBE_INDICES, new Vector3(x, y, z), uvs);
        }

        if ((mask & 1 << 1) == 0){
            // GD.Print("Bottom: " + mask);
            FACES.TryGetValue("bottom", out Vector3[] verts);
            draw(verts, CUBE_INDICES, new Vector3(x, y, z), uvs);
        }

        if ((mask & 1 << 2) == 0){
            FACES.TryGetValue("back", out Vector3[] verts);
            draw(verts, CUBE_INDICES, new Vector3(x, y, z), uvs);
        }

        if ((mask & 1 << 3) == 0){
            FACES.TryGetValue("front", out Vector3[] verts);
            draw(verts, CUBE_INDICES, new Vector3(x, y, z), uvs);
        }

        if ((mask & 1 << 4) == 0){
            FACES.TryGetValue("top", out Vector3[] verts);
            draw(verts, CUBE_INDICES, new Vector3(x, y, z), uvs);
        }

        if ((mask & 1 << 5) == 0){
            FACES.TryGetValue("right", out Vector3[] verts);
            draw(verts, CUBE_INDICES, new Vector3(x, y, z), uvs);
        }
    }

    private void draw(Vector3[] verts, int[] indices, Vector3 offset, Vector2[] uvs){
        for (int i = 0; i < verts.GetLength(0); i++){
            Vector3 offset_vert = verts[i] + offset;
            Vector2 uv = uvs[i];

            surfaceTool.SetUV(uv);
            surfaceTool.AddVertex(offset_vert);
            vertexCount += 1;
        }

        for (int i = 0; i < indices.GetLength(0); i++){
            surfaceTool.AddIndex(indices[i] + vertexCount - verts.GetLength(0));
        }
    }
}
