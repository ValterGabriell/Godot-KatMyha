shader_type canvas_item;

uniform float thickness;
uniform vec4 outline_color;

void fragment() {
    vec4 original = texture(TEXTURE, UV);

    // se o pixel do sprite é transparente → checar os vizinhos
    if (original.a == 0.0) {
        float t = thickness;

        // verifica pixels ao redor
        vec4 px1 = texture(TEXTURE, UV + vec2(t, 0.0));
        vec4 px2 = texture(TEXTURE, UV + vec2(-t, 0.0));
        vec4 px3 = texture(TEXTURE, UV + vec2(0.0, t));
        vec4 px4 = texture(TEXTURE, UV + vec2(0.0, -t));

        if (px1.a > 0.0 || px2.a > 0.0 || px3.a > 0.0 || px4.a > 0.0) {
            COLOR = outline_color;
            return;
        }
    }

    // se é pixel do sprite, renderiza normal
    COLOR = original;
}
