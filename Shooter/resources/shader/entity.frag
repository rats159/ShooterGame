#version 330 core
out vec4 FragColor;

in vec2 f_Uv;

void main()
{
    FragColor = vec4(f_Uv, 0.0f, 1.0f);
}