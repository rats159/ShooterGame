#version 330 core
out vec4 FragColor;

in vec2 f_Uv;

uniform sampler2D u_Tex;

void main()
{
    vec4 color = texture(u_Tex,f_Uv);
    FragColor = color;
    
    if(color.a < 0.5) discard;
}