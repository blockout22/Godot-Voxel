using Godot;
using System;
using System.Collections.Generic;

public partial class VoxelWorld : Node
{

	private Dictionary<Vector3, VoxelChunk> chunks = new Dictionary<Vector3, VoxelChunk>();
	public int chunk_size = 16;
	public float maxTerrainHeight = 20.0f;

	public FastNoiseLite noise;

	//Temp code
	Vector3 lastGridPos = new Vector3(0, 0, 0);

    public override void _Ready()
    {
        base._Ready();

		noise = new FastNoiseLite();
		noise.Seed = (int)GD.Randi();
		noise.FractalOctaves = 4;

		// for (int x = -chunk_size; x < chunk_size / 2; x++){
        //     for (int y = -chunk_size; y < chunk_size / 2; y++){
        //         for (int z = -chunk_size; z < chunk_size / 2; z++){
		// 			create_chunk(new Vector3(x, y, z));
		// 		}
		// 	}
		// }
    }

	public float GetNoiseValue(float x, float z)
    {
        return noise.GetNoise2D(x, z);
    }

    public override void _Process(double delta)
    {
        base._Process(delta);

		Vector3 controllerPos = GetViewport().GetCamera3D().GlobalPosition;
		Vector3 grid_position = new Vector3(MathF.Floor(controllerPos.X / chunk_size), MathF.Floor(controllerPos.Y / chunk_size), MathF.Floor(controllerPos.Z / chunk_size));

		if(grid_position != lastGridPos){
			for(int x = -2; x < 3; x++){
				for(int y = -1; y < 2; y++){
					for(int z = -2; z < 3; z++){
						Vector3 chunk_position = new Vector3(x, y, z) + grid_position;
						if(!chunks.ContainsKey(chunk_position)){
							create_chunk(chunk_position);
						}
					}	
				}
			}
			lastGridPos = grid_position;
		}
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
		if (instance != null){
			AddChild(instance);
		}
		GD.Print("Created chunk at: " + grid_position);
	}
}
