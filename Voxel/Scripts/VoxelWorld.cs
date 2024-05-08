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

	// [Export]
	// public Texture2D texture2D;

	 [Export]
    // public CSharpScript voxelGeneratorScript;

	public VoxelGenerator voxelGenerator;

	private Texture2D textureAtlas;
	public List<Rect2> textureCoordinates = new List<Rect2>();

    public override void _Ready()
    {
        base._Ready();

		createAtlas();

		// assign uv coords to blocks
		for (int i = 0; i < registeredBlocks.GetLength(0); i++){
			((VoxelBlock)registeredBlocks[i]).uvCoodds = textureCoordinates[i];
		}

		//TODO Create texture atlas from all blocks
		material = new OrmMaterial3D();
		Texture2D atlas = textureAtlas;
		material.AlbedoTexture = atlas;
		if(voxelGenerator == null){
			voxelGenerator = new FlatTerrainGenerator();
		}

		noise = new FastNoiseLite();
		noise.Seed = (int)GD.Randi();
		// noise.FractalOctaves = 4;

		//Test Code
		// create_chunk(new Vector3I(0, 0, 0));
		
		// VoxelChunk chunk = getVoxelChunkAt(0, 0, 0);
		// chunk.removeBlockAt(5, 0, 5);
		// Vector3I gridPosition = chunk.chunk_position;
		// regenChunk(chunk);

		ulong startTime = Time.GetTicksMsec();

		// List<VoxelChunk> tempChunkStorage = new List<VoxelChunk>();
		// for(int x = -5; x < 5; x++){
		// 	for(int y = -5; y < 5; y++){
		// 		for(int z = -5; z < 5; z++){
		// 			VoxelChunk chunk = new VoxelChunk(this, new Vector3I(x, y, z));
		// 			chunk.generate(voxelGenerator);
		// 			tempChunkStorage.Add(chunk);
		// 			addChunk(chunk);
		// 		}
		// 	}
		// }

		// foreach(VoxelChunk chunk in tempChunkStorage){
		// 	buildAndRenderChunk(chunk);
		// }

		ulong endTime = Time.GetTicksMsec() - startTime;
        GD.Print("Chunk Build time: " + endTime);
		// create_chunk(new Vector3I(0, -1, 0));
		// create_chunk(new Vector3I(1, -1,  0));
		// getVoxelChunkAtGrid(1, -1,  0);
    }

	// takes in global world coords and returns the chunk 
	public VoxelChunk getVoxelChunkAt(int x, int y, int z){
		Vector3I chunk_pos = new Vector3I(x / chunk_size, y / chunk_size, z / chunk_size);

		if (chunks.ContainsKey(chunk_pos)){
			VoxelChunk voxelChunk = chunks[chunk_pos];
			return voxelChunk;
		}

		return null;
	}

	public VoxelChunk getVoxelChunkAtGrid(int x, int y, int z){
		Vector3I chunk_pos = new Vector3I(x, y, z);
		if (chunks.ContainsKey(chunk_pos)){
			VoxelChunk voxelChunk = chunks[chunk_pos];
			return voxelChunk;
		}
		return null;
	}

	public VoxelBlock getVoxelBlockAt(int x, int y, int z){
		VoxelChunk chunk = getVoxelChunkAt(x, y, z);		

		if(chunk != null){
			int localX = ((x % chunk_size) + chunk_size) % chunk_size;
			int localY = ((y % chunk_size) + chunk_size) % chunk_size;
			int localZ = ((z % chunk_size) + chunk_size) % chunk_size;

			VoxelBlock voxelBlock = chunk.getBlockAt(localX, localY, localZ);
			return voxelBlock;	
		}

		return null;
	}

    // private int mod(){
    // 	return ((val % div) + div) % div;
    // }

    private void createAtlas(){

		int padding = 2;
		int totalArea = 0;
		for(int j = 0; j < registeredBlocks.GetLength(0); j++)
		{
			Texture2D texture = ((VoxelBlock)registeredBlocks[j]).texture;
			if (texture == null) continue;

			totalArea += (texture.GetWidth()) * (texture.GetHeight());
		}

		float size = Mathf.Pow(2, Mathf.Ceil(Mathf.Log(Mathf.Sqrt(totalArea)) / Mathf.Log(2)));
		//add padding
		size = size + (registeredBlocks.GetLength(0) * padding);
		// float calc = Mathf.Pow(2, Mathf.Ceil(Mathf.Log(Mathf.Sqrt(totalArea)) / Mathf.Log(2)));
		// GD.Print("calc: " + (calc + (registeredBlocks.GetLength(0) * padding)));

		Image image = Image.Create((int)size, (int)size, false, Image.Format.Rgb8);
		image.Fill(new Color(0, 0, 0, 0));

		int x = 0;
		int y = 0;
		int maxHeightInRow = 0;

		for(int i = 0; i < registeredBlocks.GetLength(0); i++){
			Texture2D texture2d = ((VoxelBlock)registeredBlocks[i]).texture; 
			Image texture = texture2d.GetImage();

			if (texture == null){
				continue;
			}

			int texWidth = texture.GetWidth();
			int texHeight = texture.GetHeight();

			if (x + texWidth + padding > size){
				x = 0;
				y += maxHeightInRow + padding;
				maxHeightInRow = 0;
			}

			if (y + texHeight + padding > size){
				GD.Print("Texture atlas to small");
				break;
			}
				image.BlitRect(texture, new Rect2I(Vector2I.Zero, new Vector2I(texWidth, texHeight)), new Vector2I(x, y));
				Rect2 texCoords = new Rect2((float)x / size, (float)y / size, (float)texWidth / size, (float)texHeight / size);
           		textureCoordinates.Add(texCoords);

				x += texWidth + padding;
				maxHeightInRow = Mathf.Max(maxHeightInRow, texHeight);
			}

			image.GenerateMipmaps();
			textureAtlas = ImageTexture.CreateFromImage(image);
	}

	 private int nextPowerOfTwo(int value)
    {
        value--;
        value |= value >> 1;
        value |= value >> 2;
        value |= value >> 4;
        value |= value >> 8;
        value |= value >> 16;
        value++;
        return value;
    }

	// private void calculateAtlasSize(int numImages){
	// 	int rows = (int) Mathf.Ceil(Mathf.Sqrt(numImages));
	// 	int cols = rows;

	// 	int maxImageDimension = 0;
	// 	for(int i = 0; i < numImages; i++){
	// 		int imageWidth = 
	// 	}
	// }

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
							// create_chunk(chunk_position);

							VoxelChunk chunk = new VoxelChunk(this, chunk_position);
							chunk.generate(voxelGenerator);
							addChunk(chunk);
							buildAndRenderChunk(chunk);
						}
					}	
				}
			}
			lastGridPos = grid_position;
		}
    }

	private void regenChunk(VoxelChunk chunk){
		RemoveChild(chunk.instance);
		MeshInstance3D instance = chunk.buildMesh();

		AddChild(instance);
	}

	private void addChunk(VoxelChunk _chunk){
		if (chunks.ContainsKey(_chunk.chunk_position)){
			GD.Print(_chunk.chunk_position + " Already Exists");
			return;
		}

		// VoxelChunk chunk = new VoxelChunk(this, grid_position);
		_chunk.material = material;
		// chunk.chunk_position = new Vector3I(grid_position.X, grid_position.Y, grid_position.Z);
		chunks.Add(_chunk.chunk_position, _chunk);
	}

	private void buildAndRenderChunk(VoxelChunk _chunk){
		//Create and add instance to world
		MeshInstance3D instance = _chunk.buildMesh();
		if (instance != null){
			AddChild(instance);
		}
		// GD.Print("Created chunk at: " + grid_position);
	}
}
