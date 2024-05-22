using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

public partial class VoxelWorld : Node
{

	private Dictionary<Vector3, VoxelChunk> chunks = new Dictionary<Vector3, VoxelChunk>();
	public int chunk_size = 16;
	public float maxTerrainHeight = 20.0f;

	public FastNoiseLite noise;

	public OrmMaterial3D material;

	private readonly Dictionary<Mesh, Dictionary<string, object>> meshCache = new Dictionary<Mesh, Dictionary<string, object>>();

	private Texture2D textureAtlas;
	// public List<Rect2> textureCoordinates = new List<Rect2>();

	//Temp code
	Vector3 lastGridPos = new Vector3(-float.MaxValue, float.MaxValue, float.MaxValue);

	[Export]
	public VoxelBlock[] registeredBlocks;
	// public VoxelBlock[] registeredBlocksInstances;

	// [Export]
	// public Texture2D texture2D;

    // public CSharpScript voxelGeneratorScript;

	[Export]
	public VoxelGenerator voxelGenerator;
	[Export]
	public Mesh[] MASK = new Mesh[64];

	private List<VoxelChunk> loadingChunks = new List<VoxelChunk>();

    public override void _Ready()
    {
        base._Ready();

		createAtlas();

		// assign uv coords to blocks
		// for (int i = 0; i < registeredBlocks.GetLength(0); i++){
		// 	((VoxelBlock)registeredBlocks[i]).UVCoordsTop = textureCoordinates[i];
		// }

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

					// VoxelChunk chunk = new VoxelChunk(this, new Vector3I(0, 0, 0));
					// chunk.generate(voxelGenerator);
					// addChunk(chunk);
					// buildAndRenderChunk(chunk);
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
		Vector3I chunkPos = new Vector3I(
        Mathf.FloorToInt((float)x / chunk_size),
        Mathf.FloorToInt((float)y / chunk_size),
        Mathf.FloorToInt((float)z / chunk_size)
    );
		if (chunks.ContainsKey(chunkPos)){
			VoxelChunk voxelChunk = chunks[chunkPos];
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
			Texture2D topTexture = ((VoxelBlock)registeredBlocks[j]).topTexture;
			Texture2D bottomTexture = ((VoxelBlock)registeredBlocks[j]).bottomTexture;
			Texture2D frontTexture = ((VoxelBlock)registeredBlocks[j]).frontTexture;
			Texture2D backTexture = ((VoxelBlock)registeredBlocks[j]).backTexture;
			Texture2D leftTexture = ((VoxelBlock)registeredBlocks[j]).leftTexture;
			Texture2D rightTexture = ((VoxelBlock)registeredBlocks[j]).rightTexture;
			
			if(topTexture != null){
				totalArea += (topTexture.GetWidth()) * (topTexture.GetHeight());
			}

			if(bottomTexture != null){
				totalArea += (bottomTexture.GetWidth()) * (bottomTexture.GetHeight());
			}

			if(frontTexture != null){
				totalArea += (frontTexture.GetWidth()) * (frontTexture.GetHeight());
			}

			if (backTexture != null){
				totalArea += (backTexture.GetWidth()) * (backTexture.GetHeight());
			}

			if(leftTexture != null){
				totalArea += (leftTexture.GetWidth()) * (leftTexture.GetHeight());
			}

			if(rightTexture != null){
				totalArea += (rightTexture.GetWidth()) * (rightTexture.GetHeight());
			}
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
			Texture2D topTexture = ((VoxelBlock)registeredBlocks[i]).topTexture;
			Texture2D bottomTexture = ((VoxelBlock)registeredBlocks[i]).bottomTexture;
			Texture2D frontTexture = ((VoxelBlock)registeredBlocks[i]).frontTexture;
			Texture2D backTexture = ((VoxelBlock)registeredBlocks[i]).backTexture;
			Texture2D leftTexture = ((VoxelBlock)registeredBlocks[i]).leftTexture;
			Texture2D rightTexture = ((VoxelBlock)registeredBlocks[i]).rightTexture;
			
			if(topTexture != null){
				((VoxelBlock)registeredBlocks[i]).UVCoordsTop = processTexture(image, topTexture, ref x, ref y, ref maxHeightInRow, size, padding);
			}

			if(bottomTexture != null){
				((VoxelBlock)registeredBlocks[i]).UVCoordsBottom = processTexture(image, bottomTexture, ref x, ref y, ref maxHeightInRow, size, padding);
			}

			if(frontTexture != null){
				((VoxelBlock)registeredBlocks[i]).UVCoordsFront = processTexture(image, frontTexture, ref x, ref y, ref maxHeightInRow, size, padding);
			}

			if (backTexture != null){
				((VoxelBlock)registeredBlocks[i]).UVCoordsBack = processTexture(image, backTexture, ref x, ref y, ref maxHeightInRow, size, padding);
			}

			if(leftTexture != null){
				((VoxelBlock)registeredBlocks[i]).UVCoordsLeft = processTexture(image, leftTexture, ref x, ref y, ref maxHeightInRow, size, padding);
			}

			if(rightTexture != null){
				((VoxelBlock)registeredBlocks[i]).UVCoordsRight = processTexture(image, rightTexture, ref x, ref y, ref maxHeightInRow, size, padding);
			}
			
		}

		image.GenerateMipmaps();
		textureAtlas = ImageTexture.CreateFromImage(image);
	}

	private Rect2 processTexture(Image image, Texture2D texture2d, ref int x, ref int y, ref int maxHeightInRow, float size, int padding){
		Image texture = texture2d.GetImage();
		int texWidth = texture.GetWidth();
		int texHeight = texture.GetHeight();

		if (x + texWidth + padding > size){
			x = 0;
			y += maxHeightInRow + padding;
			maxHeightInRow = 0;
		}

		if (y + texHeight + padding > size){
			GD.Print("Texture atlas to small");
		}
		image.BlitRect(texture, new Rect2I(Vector2I.Zero, new Vector2I(texWidth, texHeight)), new Vector2I(x, y));
		Rect2 texCoords = new Rect2((float)x / size, (float)y / size, (float)texWidth / size, (float)texHeight / size);
		// textureCoordinates.Add(texCoords);

		x += texWidth + padding;
		maxHeightInRow = Mathf.Max(maxHeightInRow, texHeight);
		return texCoords;
	}

	public Dictionary<string, object> extractMeshData(Mesh mesh)
    {
		// Check if the mesh data is already cached
        if (meshCache.TryGetValue(mesh, out Dictionary<string, object> cachedData))
        {
            return cachedData;
        }

        var dataDict = new Dictionary<string, object>();

        if (mesh is Mesh)
        {
            int surfaceCount = mesh.GetSurfaceCount();
            List<Vector3> verticesList = new List<Vector3>();
            List<Vector3> normalsList = new List<Vector3>();
            List<int> indicesList = new List<int>();
            List<Vector2> uvsList = new List<Vector2>();

            int indexOffset = 0;

            for (int i = 0; i < surfaceCount; i++)
            {
                Godot.Collections.Array arrays = mesh.SurfaceGetArrays(i);
                Vector3[] vertices = (Vector3[])arrays[(int)Mesh.ArrayType.Vertex];
                Vector3[] normals = (Vector3[])arrays[(int)Mesh.ArrayType.Normal];
                int[] indices = (int[])arrays[(int)Mesh.ArrayType.Index];

                // Add vertices and normals to the respective lists
                verticesList.AddRange(vertices ?? new Vector3[0]);
                normalsList.AddRange(normals ?? new Vector3[0]);

                // Adjust the indices based on the current offset
                if (indices != null)
                {
                    for (int j = 0; j < indices.Length; j++)
                    {
                        indicesList.Add(indices[j] + indexOffset);
                    }
                }

                indexOffset += vertices?.Length ?? 0;

                // Handle UV data
                if (arrays.Count > (int)Mesh.ArrayType.TexUV)
                {
                    Vector2[] uvs = (Vector2[])arrays[(int)Mesh.ArrayType.TexUV];
                    uvsList.AddRange(uvs ?? new Vector2[0]);
                }
            }

            dataDict["vertices"] = verticesList.ToArray();
            dataDict["normals"] = normalsList.ToArray();
            dataDict["indices"] = indicesList.ToArray();
            dataDict["uvs"] = uvsList.ToArray();
        }
        else
        {
            dataDict["vertices"] = new Vector3[0];
            dataDict["normals"] = new Vector3[0];
            dataDict["indices"] = new int[0];
            dataDict["uvs"] = new Vector2[0];
        }

		// Cache the extracted data
        meshCache[mesh] = dataDict;

        return dataDict;
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

		if(loadingChunks.Count > 0){
			loadingChunks[0].buildMesh(VoxelBuilder.LOD.HIGH);
			loadingChunks.RemoveAt(0);
		}

		Camera3D camera = GetViewport().GetCamera3D();
		if(camera == null){
			return;
		}
		Vector3 controllerPos = camera.GlobalPosition;
		Vector3I grid_position = new Vector3I((int)MathF.Floor(controllerPos.X / chunk_size), (int)MathF.Floor(controllerPos.Y / chunk_size), (int)MathF.Floor(controllerPos.Z / chunk_size));

		if(grid_position != lastGridPos){
			for(int x = -2; x < 3; x++){
				for(int y = -1; y < 2; y++){
					for(int z = -2; z < 3; z++){
						Vector3I chunk_position = new Vector3I(x, y, z) + grid_position;
						if(!chunks.ContainsKey(chunk_position)){
							// create_chunk(chunk_position);
							if (!Multiplayer.IsServer()){
								Rpc("request_chunk", chunk_position);
								//Ask the server what needs rendering in this location
							}else{
								VoxelChunk chunk = new VoxelChunk(this, chunk_position);
								chunk.generate(voxelGenerator);
								addChunk(chunk);
								buildAndRenderChunk(chunk);
							}
						}
					}	
				}
			}
			lastGridPos = grid_position;
		}
    }

	[Rpc(MultiplayerApi.RpcMode.AnyPeer)]
	private void request_chunk(Vector3I chunk_position){
		GD.Print("Requested chunk at: " + chunk_position + " : ");
		if (Multiplayer.IsServer()){
			int clientID = Multiplayer.GetRemoteSenderId();
			VoxelChunk chunk = getVoxelChunkAtGrid(chunk_position.X, chunk_position.Y, chunk_position.Z);

			if(chunk == null){
				//if the chunk doesn't exist on the server yet then generate the new chunk
				chunk = new VoxelChunk(this, chunk_position);
				chunk.generate(voxelGenerator);
				addChunk(chunk);
				buildAndRenderChunk(chunk);
			}

			RpcId(clientID, "UpdateClientChunks", chunk.chunk_position, ToRpcArray(chunk.blockList));
		}
	}

	[Rpc(MultiplayerApi.RpcMode.Authority, CallLocal = true)]
	public void UpdateClientChunks(Vector3I chunkPos, Godot.Collections.Dictionary blocks){
		if(!Multiplayer.IsServer()){
			if(!chunks.ContainsKey(chunkPos)){
				VoxelChunk chunk = new VoxelChunk(this, chunkPos);
				for(int x = 0; x < chunk_size; x++){
					for(int y = 0; y < chunk_size; y++){
						for(int z = 0; z < chunk_size; z++){
							string key = x + "," + y + "," + z;
							if(blocks.ContainsKey(key)){
								VoxelBlock block = findRegisteredBlockByName((string)blocks[key]);
								chunk.blockList[x, y, z] = block.CloneAs<VoxelBlock>();
							}
						}
					}
				}

				addChunk(chunk);
				buildAndRenderChunk(chunk);
			}
		}
	}

	private Godot.Collections.Dictionary ToRpcArray(VoxelBlock[,,] blocks){
		Godot.Collections.Dictionary dictionary = new Godot.Collections.Dictionary();
		foreach(var b in blocks){
			if(b != null){
				string key = b.localPosition.X + "," + b.localPosition.Y + "," + b.localPosition.Z;
				dictionary.Add(key, b.name);
			}
		}

		return dictionary;
	}

	public void regenChunk(VoxelChunk chunk){
		// RemoveChild(chunk.instance);
		// MeshInstance3D instance = chunk.buildMesh(VoxelBuilder.LOD.HIGH);
		loadingChunks.Add(chunk);
		// AddChild(instance);
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
		MeshInstance3D instance = _chunk.buildMesh(VoxelBuilder.LOD.LOW);
		loadingChunks.Add(_chunk);
		if (instance != null){
			AddChild(instance);
		}

		// ThreadPool.QueueUserWorkItem(_ =>
		// {
		// 	_chunk.buildMesh(VoxelBuilder.LOD.HIGH);
		// });

		// await _chunk.buildMesh(VoxelBuilder.LOD.HIGH);
		// GD.Print("Created chunk at: " + grid_position);
	}
}
