using Godot;
using System;
using System.Collections.Generic;

public partial class VoxelWorld : Node
{

	private Dictionary<Vector3, VoxelChunk> chunks = new Dictionary<Vector3, VoxelChunk>();
	public int chunk_size = 16;

    public override void _Ready()
    {
        base._Ready();

		create_chunk(new Vector3(0, 0, 0));
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
    }

	private void create_chunk(Vector3 grid_position){
		if (chunks.ContainsKey(grid_position)){
			GD.Print(grid_position + " Already Exists");
			return;
		}

		VoxelChunk chunk = new VoxelChunk(this, grid_position);
		chunks.Add(grid_position,chunk);

		//Create and add instance to world
		MeshInstance3D instance = chunk.generate();
		AddChild(instance);
	}

}
