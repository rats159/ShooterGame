#version 330 core
layout (location = 0) in vec2 a_Pos;

out vec2 f_Pos;

uniform mat4 u_Transform;
uniform mat4 u_View;
uniform mat4 u_Proj;

void main()
{
    f_Pos = (u_Transform * vec4(a_Pos,0,1)).xy;
    gl_Position = u_Proj * u_View * u_Transform * vec4(a_Pos,0.0, 1.0);
}