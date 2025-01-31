#version 330 core
out vec4 FragColor;

in vec2 f_Pos;

uniform vec2 u_Resolution;
uniform sampler2D u_Tex;

void main()
{
    vec4 color = texture(u_Tex,fract(f_Pos/u_Resolution));
    FragColor = color;
    
    if(color.a < 0.5) discard;
}