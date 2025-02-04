#version 330 core
layout(location = 0) in vec2 a_Pos;
layout(location = 1) in vec3 a_Col;

uniform mat4 u_View;
uniform mat4 u_Proj;
out vec3 f_Color;

void main() {
    gl_Position = u_View * u_Proj * vec4(a_Pos, 0, 1.0);
    f_Color = a_Col;
}