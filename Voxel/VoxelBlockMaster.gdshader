shader_type spatial;
varying vec2 v_UV;

uniform sampler2D topTexture;
uniform sampler2D bottomTexture;
uniform sampler2D frontTexture;
uniform sampler2D backTexture;
uniform sampler2D leftTexture;
uniform sampler2D rightTexture;

void vertex() {
	v_UV = UV;
}

vec2 remapUVS(vec2 minUV, vec2 maxUV)
{
	return vec2((v_UV.x - minUV.x) / (maxUV.x - minUV.x), (v_UV.y - minUV.y) / (maxUV.y - minUV.y));
}

void fragment() {
	vec4 color = vec4(1.0, 1.0, 1.0, 1.0); // Default color (black)
	
	vec4 topSide = texture(topTexture, remapUVS(vec2(0.0, 2.0 / 3.0), vec2(1.0 / 3.0, 1.0)));
	vec4 bottomSide = texture(bottomTexture, remapUVS(vec2(1.0 / 3.0, 2.0 / 3.0), vec2(2.0 / 3.0, 1.0)));
	vec4 frontSide = texture(frontTexture, remapUVS(vec2(2.0 / 3.0, 2.0 / 3.0), vec2(1.0, 1.0)));
	vec4 backSide = texture(backTexture, remapUVS(vec2(0.0, 0.0), vec2(1.0 / 3.0, 1.0 / 3.0)));
	vec4 leftSide = texture(leftTexture, remapUVS(vec2(1.0 / 3.0, 0.0), vec2(2.0 / 3.0, 1.0 / 3.0)));
	vec4 rightSide = texture(rightTexture, remapUVS(vec2(2.0 / 3.0, 0.0), vec2(1.0, 1.0 / 3.0)));

    // Top face: (0, 2/3) to (1/3, 1)
    if(v_UV.x >= 0.0 && v_UV.x < 1.0 / 3.0 && v_UV.y >= 2.0 / 3.0 && v_UV.y <= 1.0){
		color = topSide;
    }
	//Left face
	else if(v_UV.x >= 1.0 / 3.0 && v_UV.x < 2.0 / 3.0 && v_UV.y >= 0.0 && v_UV.y < 1.0 / 3.0)
	{
		if(leftSide.a != 0.0){
			color = leftSide;
		}else{
			color = topSide;
		}
	}
	//Right face
	else if(v_UV.x >= 2.0 / 3.0 && v_UV.x <= 1.0 && v_UV.y >= 0.0 && v_UV.y < 1.0 / 3.0)
	{
		if(rightSide.a != 0.0){
			color = rightSide;
		}else{
			color = topSide;
		}
	}
	//Front face
	else if(v_UV.x >= 2.0 / 3.0 && v_UV.x <= 1.0 && v_UV.y >= 2.0 / 3.0 && v_UV.y <= 1.0)
	{
		if(frontSide.a != 0.0){
			color = frontSide;
		}else{
			color = topSide;
		}
	}
	//Back face
	else if(v_UV.x >= 0.0 && v_UV.x < 1.0 / 3.0 && v_UV.y >= 0.0 && v_UV.y < 1.0 / 3.0)
	{
		if(backSide.a != 0.0){
			color = backSide;
		}else{
			color = topSide;
		}
	}
	//Bottom face
	else if(v_UV.x >= 1.0 / 3.0 && v_UV.x < 2.0 / 3.0 && v_UV.y >= 2.0 / 3.0 && v_UV.y <= 1.0)
	{
		if(bottomSide.a != 0.0){
			color = bottomSide;
		}else{
			color = topSide;
		}
	}

    ALBEDO = color.rgb;
}

//void light() {
	// Called for every pixel for every light affecting the material.
	// Uncomment to replace the default light processing function with this one.
//}
