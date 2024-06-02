shader_type spatial;
varying vec2 v_UV;

uniform sampler2D topTexture;
uniform sampler2D bottomTexture;
uniform sampler2D frontTexture;
uniform sampler2D backTexture;
uniform sampler2D leftTexture;
uniform sampler2D rightTexture;

uniform sampler2D topNormal;
uniform sampler2D bottomNormal;
uniform sampler2D frontNormal;
uniform sampler2D backNormal;
uniform sampler2D leftNormal;
uniform sampler2D rightNormal;

void vertex() {
	v_UV = UV;
}

vec2 remapUVS(vec2 minUV, vec2 maxUV)
{
	return vec2((v_UV.x - minUV.x) / (maxUV.x - minUV.x), (v_UV.y - minUV.y) / (maxUV.y - minUV.y));
}

bool uvInBounds(vec2 minUV, vec2 maxUV){
	return v_UV.x >= minUV.x && v_UV.x < maxUV.x && v_UV.y >= minUV.y && v_UV.y <= maxUV.y;
}

void fragment() {
	vec4 color = vec4(1.0, 1.0, 1.0, 1.0);
	vec4 normal = vec4(0, 0, 0, 0);
	
	vec2 topMinUV = vec2(0.0, 2.0 / 3.0);
	vec2 topMaxUV = vec2(1.0 / 3.0, 1.0);
	vec2 bottomMinUV = vec2(1.0 / 3.0, 2.0 / 3.0);
	vec2 bottomMaxUV = vec2(2.0 / 3.0, 1.0);
	vec2 frontMinUV = vec2(2.0 / 3.0, 2.0 / 3.0);
	vec2 frontMaxUV = vec2(1.0, 1.0);
	vec2 backMinUV = vec2(0.0, 0.0);
	vec2 backMaxUV = vec2(1.0 / 3.0, 1.0 / 3.0);
	vec2 leftMinUV = vec2(1.0 / 3.0, 0.0);
	vec2 leftMaxUV = vec2(2.0 / 3.0, 1.0 / 3.0);
	vec2 rightMinUV = vec2(2.0 / 3.0, 0.0);
	vec2 rightMaxUV = vec2(1.0, 1.0 / 3.0);
	
	vec2 topUVS = remapUVS(topMinUV, topMaxUV);
	vec2 bottomUVS = remapUVS(bottomMinUV, bottomMaxUV);
	vec2 frontUVS = remapUVS(frontMinUV, frontMaxUV);
	vec2 backUVS = remapUVS(backMinUV, backMaxUV);
	vec2 leftUVS = remapUVS(leftMinUV, leftMaxUV);
	vec2 rightUVS = remapUVS(rightMinUV, rightMaxUV);
	
	vec4 topSide = texture(topTexture, topUVS);
	vec4 bottomSide = texture(bottomTexture, bottomUVS);
	vec4 frontSide = texture(frontTexture, frontUVS);
	vec4 backSide = texture(backTexture, backUVS);
	vec4 leftSide = texture(leftTexture, leftUVS);
	vec4 rightSide = texture(rightTexture, rightUVS);
	
	vec4 topSideN = texture(topNormal, topUVS);
	vec4 bottomSideN = texture(bottomNormal, bottomUVS);
	vec4 frontSideN = texture(frontNormal, frontUVS);
	vec4 backSideN = texture(backNormal, backUVS);
	vec4 leftSideN = texture(leftNormal, leftUVS);
	vec4 rightSideN = texture(rightNormal, rightUVS);

    // Top face: (0, 2/3) to (1/3, 1)
    if(uvInBounds(topMinUV, topMaxUV)){
		color = topSide;
		normal = topSideN;
    }
	//Left face
	else if(uvInBounds(leftMinUV, leftMaxUV))
	{
		if(leftSide.a != 0.0){
			color = leftSide;
			normal = leftSideN;
		}else{
			color = topSide;
		}
	}
	//Right face
	else if(uvInBounds(rightMinUV, rightMaxUV))
	{
		if(rightSide.a != 0.0){
			color = rightSide;
			normal = rightSideN;
		}else{
			color = topSide;
		}
	}
	//Front face
	else if(uvInBounds(frontMinUV, frontMaxUV))
	{
		if(frontSide.a != 0.0){
			color = frontSide;
			normal = frontSideN;
		}else{
			color = topSide;
		}
	}
	//Back face
	else if(uvInBounds(backMinUV, backMaxUV))
	{
		if(backSide.a != 0.0){
			color = backSide;
			normal = backSideN;
		}else{
			color = topSide;
		}
	}
	//Bottom face
	else if(uvInBounds(bottomMinUV, bottomMaxUV))
	{
		if(bottomSide.a != 0.0){
			color = bottomSide;
			normal = bottomSideN;
		}else{
			color = topSide;
		}
	}

    ALBEDO = color.rgb;
	NORMAL = normal.rgb;
}
