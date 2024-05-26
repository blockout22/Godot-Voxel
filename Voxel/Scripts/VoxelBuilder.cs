using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public partial class VoxelBuilder
{

    public VoxelWorld voxelWorld;

    public enum LOD {
        LOW = 4, MED = 2, HIGH = 1
    }

    private static readonly Dictionary<string, Vector3[]> FACES = new Dictionary<string, Vector3[]>
    {
        { "left", new Vector3[] { new Vector3(-0.5f, 0.5f, -0.5f), new Vector3(-0.5f, 0.5f, 0.5f), new Vector3(-0.5f, -0.5f, 0.5f), new Vector3(-0.5f, -0.5f, -0.5f) } },
        { "bottom", new Vector3[] { new Vector3(-0.5f, -0.5f, 0.5f), new Vector3(0.5f, -0.5f, 0.5f), new Vector3(0.5f, -0.5f, -0.5f), new Vector3(-0.5f, -0.5f, -0.5f) } },
        { "back", new Vector3[] { new Vector3(0.5f, 0.5f, -0.5f), new Vector3(-0.5f, 0.5f, -0.5f), new Vector3(-0.5f, -0.5f, -0.5f), new Vector3(0.5f, -0.5f, -0.5f) } },
        { "front", new Vector3[] { new Vector3(-0.5f, 0.5f, 0.5f), new Vector3(0.5f, 0.5f, 0.5f), new Vector3(0.5f, -0.5f, 0.5f), new Vector3(-0.5f, -0.5f, 0.5f) } },
        { "right", new Vector3[] { new Vector3(0.5f, 0.5f, 0.5f), new Vector3(0.5f, 0.5f, -0.5f), new Vector3(0.5f, -0.5f, -0.5f), new Vector3(0.5f, -0.5f, 0.5f) } },
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

    // SurfaceTool surfaceTool;
    // private List<SurfaceTool> surfaces = new List<SurfaceTool>();
    private Dictionary<Material, VoxelSurface> surfaces = new Dictionary<Material, VoxelSurface>();

    private static readonly Vector2[] DefaultTopUVs = new Vector2[]
    {
        new Vector2(0, 0),
        new Vector2(1, 0),
        new Vector2(1, 1),
        new Vector2(0, 1)
    };

    private static readonly Vector2[] DefaultBottomUVs = new Vector2[]
    {
        new Vector2(0, 0),
        new Vector2(1, 0),
        new Vector2(1, 1),
        new Vector2(0, 1)
    };

    private static readonly Vector2[] DefaultFrontUVs = new Vector2[]
    {
        new Vector2(0, 0),
        new Vector2(1, 0),
        new Vector2(1, 1),
        new Vector2(0, 1)
    };

    private static readonly Vector2[] DefaultBackUVs = new Vector2[]
    {
        new Vector2(0, 0),
        new Vector2(1, 0),
        new Vector2(1, 1),
        new Vector2(0, 1)
    };

    private static readonly Vector2[] DefaultLeftUVs = new Vector2[]
    {
        new Vector2(0, 0),
        new Vector2(1, 0),
        new Vector2(1, 1),
        new Vector2(0, 1)
    };

    private static readonly Vector2[] DefaultRightUVs = new Vector2[]
    {
        new Vector2(0, 0),
        new Vector2(1, 0),
        new Vector2(1, 1),
        new Vector2(0, 1)
    };

    public VoxelBuilder(VoxelWorld _voxelWorld){
        voxelWorld = _voxelWorld;
        // Vector3 direction = CalculateDirection(1);
        // GD.Print(direction);
    }

    public Mesh build(VoxelChunk chunk, LOD lod){
        VoxelBlock[,,] blockList = chunk.blockList;
        surfaces.Clear();
        // surfaceTool = new SurfaceTool();
        // vertexCount = 0;
        // surfaceTool.Begin(Mesh.PrimitiveType.Triangles);

        int lodScale = (int)lod;

        for (int x = 0; x < blockList.GetLength(0); x += lodScale){
            for (int y = 0; y < blockList.GetLength(1); y += lodScale){
                for (int z = 0; z < blockList.GetLength(2); z += lodScale){
                    VoxelBlock block = blockList[x, y, z];
                    if (block != null){
                        block.parentChunk = chunk;
                        block.localPosition = new Vector3I(x, y, z);
                        int neighbors = getNeighbors(chunk, x, y, z);                        

                        // Vector2[] topUVS = createFaceUVs(block.UVCoordsTop);
                        // Vector2[] bottomUVS = (block.bottomTexture != null) ? createFaceUVs(block.UVCoordsBottom) : topUVS;
                        // Vector2[] frontUVS = (block.frontTexture != null) ? createFaceUVs(block.UVCoordsFront) : topUVS;
                        // Vector2[] backUVS = (block.backTexture != null) ? createFaceUVs(block.UVCoordsBack) : topUVS;
                        // Vector2[] leftUVS = (block.leftTexture != null) ? createFaceUVs(block.UVCoordsLeft) : topUVS;
                        // Vector2[] rightUVS = (block.rightTexture != null) ? createFaceUVs(block.UVCoordsRight) : topUVS;

                        // int mask = getNeighborsMask(neighbors);

                        // //left = 1, bottom = 2, back = 4, front = 8, top = 16, right = 32
                        // object[] testMask = new object[6]{false, false, false, false, false, false};
                        // int test = getNeighborsMask(testMask);
                        // GD.Print(test);
                        // string toPint = "";
                        // for (int i = 0; i < neighbors.GetLength(0); i++){
                            // toPint += (neighbors[i] is bool ? "false" : "true") + " ";
                        // }
                        // GD.Print(mask + " : " +  getNeighborsMask(testMask));
                        

                        drawFromMask(block, neighbors, x, y, z, lodScale, DefaultTopUVs, DefaultBottomUVs, DefaultFrontUVs, DefaultBackUVs, DefaultLeftUVs, DefaultRightUVs, chunk);
                    }
                }
            }
        }

        if (surfaces.Count <= 0){
            return null;
        }

        ArrayMesh mesh = new ArrayMesh();

        foreach (var kvp in surfaces){
            kvp.Value.surfaceTool.GenerateNormals();
            kvp.Value.surfaceTool.GenerateTangents();
            kvp.Value.surfaceTool.Commit(mesh);
        }

        // MeshInstance3D instance = new MeshInstance3D();
        // // surfaceTool.GenerateNormals();
        // instance.Mesh = mesh;
        // return surfaceTool.Commit();
        return mesh;
    }

    //offset = position inside the altas 
    //grid size = size of the image in the grid 0-1
    private Vector2[] createFaceUVs(Rect2 uvRect){
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

        return faceUVs;
    }

    private int getNeighbors(VoxelChunk chunk, int x, int y, int z){
        VoxelBlock[,,] blockList = chunk.blockList;
        // object[] neighbors = new object[6]{false, false, false, false, false, false};
        int bitmask = 0;
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
                        // neighbors[i] = neighorBlockPos;
                        bitmask |= (1 << i);
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
                        bitmask |= (1 << i);
                        // neighbors[i] = neighorBlockPos;
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
                        bitmask |= (1 << i);
                        // neighbors[i] = neighorBlockPos;
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
                        bitmask |= (1 << i);
                        // neighbors[i] = neighorBlockPos;
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
                        bitmask |= (1 << i);
                        // neighbors[i] = neighorBlockPos;
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
                        bitmask |= (1 << i);
                        // neighbors[i] = neighorBlockPos;
                    }
                }
                continue;
            }

            if (blockList[neighbor_pos.X, neighbor_pos.Y, neighbor_pos.Z] != null){
                // GD.Print("Neightpos: " + NEIGHBOR_OFFSETS[i].Y );
                //maybe change neighbor_pos to be the blocks global pos and not the chunks block local position
                bitmask |= (1 << i);
                // neighbors[i] = neighbor_pos;
            }
        }
        return bitmask;
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

    private Vector2[] applyUVAdjustment(Vector2[] originalUVs, Vector2[] faceUVs)
    {
        Vector2 uvOffset = faceUVs[0];
        Vector2 uvScale = new Vector2(faceUVs[1].X - faceUVs[0].X, faceUVs[2].Y - faceUVs[0].Y);

        Vector2[] adjustedUVs = new Vector2[originalUVs.Length];

        for (int i = 0; i < originalUVs.Length; i++)
        {
            adjustedUVs[i] = uvOffset + new Vector2(
                originalUVs[i].X * uvScale.X,
                originalUVs[i].Y * uvScale.Y
            );
        }

        return adjustedUVs;
    }

    private static Dictionary<string, Vector3[]> CopyFaces(Dictionary<string, Vector3[]> original)
    {
        var copy = new Dictionary<string, Vector3[]>();

        foreach (var item in original)
        {
            Vector3[] vectorsCopy = new Vector3[item.Value.Length];
            item.Value.CopyTo(vectorsCopy, 0);
            copy.Add(item.Key, vectorsCopy);
        }

        return copy;
    }

    private Vector3[] changeVectorVertexFromArray(Vector3[] array, float fromX, float fromY, float fromZ, float toX, float toY, float toZ){
        for (int i = 0; i < array.Length; i++){
            Vector3 old = new Vector3(array[i].X, array[i].Y, array[i].Z);
            if(array[i].X == fromX && array[i].Y == fromY && array[i].Z == fromZ){
                array[i].X = toX;
                array[i].Y = toY;
                array[i].Z = toZ;
            }
        }

        return array;
    }

    private static Vector3[] DeepCopyVectorArray(Vector3[] original)
    {
        Vector3[] copy = new Vector3[original.Length];
        for (int i = 0; i < original.Length; i++)
        {
            copy[i] = new Vector3(original[i].X, original[i].Y, original[i].Z);
        }
        return copy;
    }

    // private int drawSmooth(int _mask, Vector3 offset, Vector2[] uvs){
    //     int mask = _mask;
    //     if (mask == 0){
    //         return _mask;
    //     }

    //     int neighborCount = 0;
    //     while(mask != 0){
    //         mask &= mask - 1;
    //         neighborCount++;
    //     }

    //     int processedMask = 0;

    //     if(neighborCount > 1){
    //         Vector3[] left = DeepCopyVectorArray(FACES["left"]);
    //         Vector3[] bottom = DeepCopyVectorArray(FACES["bottom"]);
    //         Vector3[] back = DeepCopyVectorArray(FACES["back"]);
    //         Vector3[] front = DeepCopyVectorArray(FACES["front"]);
    //         Vector3[] top = DeepCopyVectorArray(FACES["top"]);
    //         Vector3[] right = DeepCopyVectorArray(FACES["right"]);

    //         foreach(Vector3 vertex in left){
    //             float totalX = vertex.X;
    //             float totalY = vertex.Y;
    //             float totalZ = vertex.Z;
    //             int count = 1;

    //             for(int x = -1; x <= 1; x++){
    //                 for(int y = -1; x <= 1; y++){
    //                     for(int z = -1; z <= 1; z++){
    //                         int _x = (int)(offset.X + x);
    //                         int _y = (int)(offset.Y + y);
    //                         int _z = (int)(offset.Z + z);

    //                         if(_x > 0 && _y > 0 && _z > 0 && _x < voxelWorld.chunk_size - 1 && _y < voxelWorld.chunk_size - 1 && _z < voxelWorld.chunk_size - 1){

    //                         }
    //                     }
    //                 }
    //             }
    //         }

    //         bool drawLeft = true;
    //         bool drawBottom = true;
    //         bool drawBack = true;
    //         bool drawFront = true;
    //         bool drawTop = true;
    //         bool drawRight = true;


    //         // //1001100 left-front-top
    //         if(((_mask & 1 << 0) == 0) && ((_mask & 1 << 3) == 0) && ((_mask & 1 << 4) == 0)){
    //             left = changeVectorVertexFromArray(left, -0.5f, 0.5f, 0.5f, 0, 0, 0);
    //             front = changeVectorVertexFromArray(front, -0.5f, 0.5f, 0.5f, 0, 0, 0);
    //             top = changeVectorVertexFromArray(top, -0.5f, 0.5f, 0.5f, 0, 0, 0);

    //             drawLeft = true;
    //             drawFront = true;
    //             drawTop = true;
    //             // processedMask |= (1 << 0) | (1 << 3) | (1 << 4);

    //             if((processedMask & (1 << 0)) == 0){
    //                 processedMask |= 1 << 0;
    //             }

    //             if((processedMask & (1 << 3)) == 0){
    //                 processedMask |= 1 << 3;
    //             }

    //             if((processedMask & (1 << 4)) == 0){
    //                 processedMask |= 1 << 4;
    //             }
    //         }

    //         //1001100 front-top-right
    //         if(((_mask & 1 << 3) == 0) && ((_mask & 1 << 4) == 0) && ((_mask & 1 << 5) == 0)){
    //             front = changeVectorVertexFromArray(front, 0.5f, 0.5f, 0.5f, 0, 0, 0);
    //             top = changeVectorVertexFromArray(top, 0.5f, 0.5f, 0.5f, 0, 0, 0);
    //             right = changeVectorVertexFromArray(right, 0.5f, 0.5f, 0.5f, 0, 0, 0);

    //             drawFront = true;
    //             drawTop = true;
    //             drawRight = true;

    //             if((processedMask & (1 << 3)) == 0){
    //                 processedMask |= 1 << 3;
    //             }

    //             if((processedMask & (1 << 4)) == 0){
    //                 processedMask |= 1 << 4;
    //             }

    //             if((processedMask & (1 << 5)) == 0){
    //                 processedMask |= 1 << 5;
    //             }
    //         }

    //         //0101001 bottom-front-right
    //         if(((_mask & 1 << 1) == 0) && ((_mask & 1 << 3) == 0) && ((_mask & 1 << 5) == 0)){
    //             bottom = changeVectorVertexFromArray(bottom, 0.5f, -0.5f, 0.5f, 0, 0, 0);
    //             front = changeVectorVertexFromArray(front, 0.5f, -0.5f, 0.5f, 0, 0, 0);
    //             right = changeVectorVertexFromArray(right, 0.5f, -0.5f, 0.5f, 0, 0, 0);

    //             drawBottom = true;
    //             drawFront = true;
    //             drawRight = true;

    //             if((processedMask & (1 << 1)) == 0){
    //                 processedMask |= 1 << 1;
    //             }

    //             if((processedMask & (1 << 3)) == 0){
    //                 processedMask |= 1 << 3;
    //             }

    //             if((processedMask & (1 << 5)) == 0){
    //                 processedMask |= 1 << 5;
    //             }
    //         }

    //         //110100 left-bottom-front
    //         if(((_mask & 1 << 0) == 0) && ((_mask & 1 << 1) == 0) && ((_mask & 1 << 3) == 0)){
    //             left = changeVectorVertexFromArray(left, -0.5f, -0.5f, 0.5f, 0, 0, 0);
    //             bottom = changeVectorVertexFromArray(bottom, -0.5f, -0.5f, 0.5f, 0, 0, 0);
    //             front = changeVectorVertexFromArray(front, -0.5f, -0.5f, 0.5f, 0, 0, 0);

    //             drawLeft = true;
    //             drawBottom = true;
    //             drawFront = true;

    //             if((processedMask & (1 << 0)) == 0){
    //                 processedMask |= 1 << 0;
    //             }

    //             if((processedMask & (1 << 1)) == 0){
    //                 processedMask |= 1 << 1;
    //             }

    //             if((processedMask & (1 << 3)) == 0){
    //                 processedMask |= 1 << 3;
    //             }
    //         }
            
    //         if(drawLeft){
    //             draw(left, CUBE_INDICES, offset, 1.0f, uvs);
    //         }

    //         if(drawBottom){
    //             draw(bottom, CUBE_INDICES, offset, 1.0f, uvs);
    //         }

    //         if(drawBack){
    //             draw(back, CUBE_INDICES, offset, 1.0f, uvs);
    //         }

    //         if(drawFront){
    //             draw(front, CUBE_INDICES, offset, 1.0f, uvs);
    //         }

    //         if(drawTop){
    //             draw(top, CUBE_INDICES, offset, 1.0f, uvs);
    //         }

    //         if(drawRight){
    //             draw(right, CUBE_INDICES, offset, 1.0f, uvs);
    //         }
    //     }


    //     return _mask ^ processedMask;
    // }

    public Vector3 CalculateDirection(int bitmask)
    {
        Vector3 result = Vector3.Zero;

        for (int i = 0; i < NEIGHBOR_OFFSETS.Length; i++)
        {
            if ((bitmask & (1 << i)) != 0) // Check if the ith bit is set
            {
                result += NEIGHBOR_OFFSETS[i]; // Add the corresponding direction to the result
            }
        }

        return result.Normalized(); // Normalize the result to return a unit vector
    }

    public void moveVerticesDirection(Vector3[] vertices, Vector3 direction, float strength){
        int zeros = 0;

        if (direction.X == 0){
            zeros++;
        }

        if (direction.Y == 0){
            zeros++;
        }

        if (direction.Z == 0){
            zeros++;
        }

        if(zeros >= 2){
            return;
        }

        for (int i = 0; i < vertices.Length; i++){
            // Vector3 moved = (vertices[i] + direction) * strength;
            Vector3 moved = vertices[i];
            moved += direction * strength;

            moved.X = Mathf.Clamp(moved.X, -0.5f, 0.5f);
            moved.Y = Mathf.Clamp(moved.Y, -0.5f, 0.5f);
            moved.Z = Mathf.Clamp(moved.Z, -0.5f, 0.5f);
            vertices[i] = moved;
        }
    }

    private void drawFromMask(VoxelBlock block, int mask, int x, int y, int z, float scale, Vector2[] topUVS, Vector2[] bottomUVS, Vector2[] frontUVS, Vector2[] backUVS, Vector2[] leftUVS, Vector2[] rightUVS, VoxelChunk chunk){
        Vector3 offset = new Vector3(x, y, z);
        // Mesh mesh = voxelWorld.MASK[mask];
        // if(mesh != null){
        //     Dictionary<string, object> data = voxelWorld.extractMeshData(mesh);

        //     //TODO draw the correct direction of uvs
        //         Vector2[] adjustedUVs = applyUVAdjustment((Vector2[])data["uvs"], topUVS);
        //     draw((Vector3[])data["vertices"], (int[])data["indices"], offset, scale, adjustedUVs);
        //     return;
        // }

        // int mask = drawSmooth(_mask, offset, uvs);

        // Vector3 direction = CalculateDirection(mask);
        // float strength = 0.5f;

        VoxelSurface voxelSurface = null;
        if(!surfaces.ContainsKey(block.material)){
            voxelSurface = new VoxelSurface(block);
            surfaces[block.material] = voxelSurface;
        }else{
            voxelSurface = surfaces[block.material];
        }

        if ((mask & 1 << 0) == 0){
            FACES.TryGetValue("left", out Vector3[] verts);
            // Vector3[] copy = DeepCopyVectorArray(verts);
            // moveVerticesDirection(copy, direction, strength);
            draw(voxelSurface, verts, CUBE_INDICES, offset, scale, leftUVS);
        }

        if ((mask & 1 << 1) == 0){
            FACES.TryGetValue("bottom", out Vector3[] verts);
            // Vector3[] copy = DeepCopyVectorArray(verts);
            // moveVerticesDirection(copy, direction, strength);
            draw(voxelSurface, verts, CUBE_INDICES, offset, scale, bottomUVS);
        }

        if ((mask & 1 << 2) == 0){
            FACES.TryGetValue("back", out Vector3[] verts);
            // Vector3[] copy = DeepCopyVectorArray(verts);
            // moveVerticesDirection(copy, direction, strength);
            draw(voxelSurface, verts, CUBE_INDICES, offset, scale, backUVS);
        }

        if ((mask & 1 << 3) == 0){
            FACES.TryGetValue("front", out Vector3[] verts);
            // Vector3[] copy = DeepCopyVectorArray(verts);
            // moveVerticesDirection(copy, direction, strength);
            draw(voxelSurface, verts, CUBE_INDICES, offset, scale, frontUVS);
        }

        if ((mask & 1 << 4) == 0){
            FACES.TryGetValue("top", out Vector3[] verts);
            // Vector3[] copy = DeepCopyVectorArray(verts);
            // moveVerticesDirection(copy, direction, strength);
            draw(voxelSurface, verts, CUBE_INDICES, offset, scale, topUVS);
        }

        if ((mask & 1 << 5) == 0){
            FACES.TryGetValue("right", out Vector3[] verts);
            // Vector3[] copy = DeepCopyVectorArray(verts);
            // moveVerticesDirection(copy, direction, strength);
            draw(voxelSurface, verts, CUBE_INDICES, offset, scale, rightUVS);
        }

        
    }

    private void draw(VoxelSurface voxelSurface, Vector3[] verts, int[] indices, Vector3 offset, float scale, Vector2[] uvs){
        for (int i = 0; i < verts.GetLength(0); i++){
            Vector3 offset_vert = (verts[i] * scale) + offset;
            Vector2 uv = uvs[i];
            voxelSurface.surfaceTool.SetUV(uv);
            voxelSurface.surfaceTool.AddVertex(offset_vert);
            voxelSurface.vertexCount += 1;
        }

        for (int i = 0; i < indices.GetLength(0); i++){
            voxelSurface.surfaceTool.AddIndex(indices[i] + voxelSurface.vertexCount - verts.GetLength(0));
        }
    }
}
