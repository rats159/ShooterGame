#version 330 core
out vec4 FragColor;

in vec2 f_UV;

uniform sampler2D u_Fbo;

void main() {
    FragColor = texture(u_Fbo, f_UV);
}