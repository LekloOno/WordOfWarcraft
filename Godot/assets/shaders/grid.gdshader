shader_type spatial;
render_mode unshaded;

uniform vec4 grid_color : source_color = vec4(0.2, 0.2, 0.2, 1.0);
uniform float line_width = 0.08;

void vertex() {
	// Called for every vertex the material is visible on.
}

void fragment() {
	vec3 world_pos = (INV_VIEW_MATRIX * vec4(VERTEX, 1.0)).xyz;
	vec2 cell = fract(world_pos.xz);
	vec2 dist = min(cell, 1.0 - cell);
	float d = min(dist.x, dist.y);
	float grid = 1.0 - smoothstep(0.0, line_width, d);
	ALPHA = grid * grid_color.a;
	ALBEDO = grid_color.rgb;
}

//void light() {
//	// Called for every pixel for every light affecting the material.
//	// Uncomment to replace the default light processing function with this one.
//}
