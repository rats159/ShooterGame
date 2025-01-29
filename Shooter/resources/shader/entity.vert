#version 330 core
layout (location = 0) in vec2 aPosition;
layout (location = 1) in vec2 aUv;

out vec2 f_Uv;

uniform mat4 u_Transform;
uniform mat4 u_View;
uniform mat4 u_Proj;

void main()
{
    f_Uv = aUv;
    gl_Position = u_Proj * u_View * u_Transform * vec4(aPosition,0.0, 1.0);
}