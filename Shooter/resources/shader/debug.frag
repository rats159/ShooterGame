#version 330 core
out vec4 FragColor;

in vec3 f_Color;

void main() {
    FragColor = vec4(f_Color, 1.0);
}