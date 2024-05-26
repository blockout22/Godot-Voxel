using Godot;
using System;

public partial class VoxelSurface : Node
{
    public int vertexCount = 0;
    public SurfaceTool surfaceTool = new SurfaceTool();

    public VoxelSurface(VoxelBlock block){
        surfaceTool.Begin(Mesh.PrimitiveType.Triangles);
        surfaceTool.SetMaterial(block.material);
    }
}
