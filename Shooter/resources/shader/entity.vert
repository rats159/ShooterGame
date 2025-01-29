#version 330 core
layout (location = 0) in vec2 aPosition;

uniform mat4 u_Transform;
uniform mat4 u_View;
uniform mat4 u_Proj;

void main()
{
    gl_Position = u_Proj * u_View * u_Transform * vec4(aPosition,0.0, 1.0);
}