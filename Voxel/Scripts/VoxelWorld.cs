using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class VoxelWorld : Node
{

	private Dictionary<Vector3, VoxelChunk> chunks = new Dictionary<Vector3, VoxelChunk>();
	public int chunk_size = 16;
	public float maxTerrainHeight = 20.0f;

	public FastNoiseLite noise;

	public OrmMaterial3D material;

	//Temp code
	Vector3 lastGridPos = new Vector3(0, 0, 0);

	[Export]
	public VoxelBlock[] registeredBlocks;
	// public VoxelBlock[] registeredBlocksInstances;

	[Export]
	public Texture2D texture2D;

	 [Export]
    // public CSharpScript voxelGeneratorScript;

	public VoxelGenerator voxelGenerator;

    public override void _Ready()
    {
        base._Ready();


		material = new OrmMaterial3D();
		GD.Print(((VoxelBlock)registeredBlocks[0]).name);
		Texture2D atlas = ((VoxelBlock)registeredBlocks[0]).texture;
		material.AlbedoTexture = atlas;

		// voxelGenerator = voxelGeneratorScript.New().As<VoxelGenerator>();
		// GD.Print(voxelGenerator);
		if(voxelGenerator == null){
			voxelGenerator = new FlatTerrainGenerator();
		// 	GD.Print("N~ULL");
		}

		// GD.Print(voxelGenerator);

		noise = new FastNoiseLite();
		noise.Seed = (int)GD.Randi();
		// noise.FractalOctaves = 4;
    }

	public VoxelBlock findRegisteredBlockByName(string name){
		return registeredBlocks.FirstOrDefault(block => block != null && block.name == name);
	}

	public float GetNoiseValue(float x, float z)
    {
        return noise.GetNoise2D(x, z);
    }

    public override void _Process(double delta)
    {
        base._Process(delta);

		Vector3 controllerPos = GetViewport().GetCamera3D().GlobalPosition;
		Vector3I grid_position = new Vector3I((int)MathF.Floor(controllerPos.X / chunk_size), (int)MathF.Floor(controllerPos.Y / chunk_size), (int)MathF.Floor(controllerPos.Z / chunk_size));

		if(grid_position != lastGridPos){
			for(int x = -2; x < 3; x++){
				for(int y = -1; y < 2; y++){
					for(int z = -2; z < 3; z++){
						Vector3I chunk_position = new Vector3I(x, y, z) + grid_position;
						if(!chunks.ContainsKey(chunk_position)){
							create_chunk(chunk_position);
						}
					}	
				}
			}
			lastGridPos = grid_position;
		}
    }

	private void create_chunk(Vector3I grid_position){
		if (chunks.ContainsKey(grid_position)){
			GD.Print(grid_position + " Already Exists");
			return;
		}

		VoxelChunk chunk = new VoxelChunk(this, grid_position);
		chunk.material = material;
		chunks.Add(grid_position,chunk);

		//Create and add instance to world
		MeshInstance3D instance = chunk.generate(voxelGenerator);
		if (instance != null){
			AddChild(instance);
		}
		// GD.Print("Created chunk at: " + grid_position);
	}
}
