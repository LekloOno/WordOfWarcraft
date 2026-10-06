shader_type canvas_item;

uniform sampler2D gradient : repeat_disable, filter_linear;
uniform vec2 label_size = vec2(100.0, 30.0);
uniform bool vertical = true;

varying vec2 local_pos;

void vertex() {
    local_pos = VERTEX; // position relative to the Label's top-left
}

void fragment() {
    float t = vertical ? local_pos.y / label_size.y : local_pos.x / label_size.x;
    COLOR *= texture(gradient, vec2(clamp(t, 0.0, 1.0), 0.5));
}

//void light() {
//	// Called for every pixel for every light affecting the CanvasItem.
//	// Uncomment to replace the default light processing function with this one.
//}
