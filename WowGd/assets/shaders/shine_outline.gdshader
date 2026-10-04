shader_type canvas_item;

uniform float fade : hint_range(0.01, 1.0) = 0.5;
uniform float roundness : hint_range(0.0, 1.0) = 0.5;
uniform bool screen_space = false;
uniform vec4 color : source_color;

void fragment() {
	vec2 gx = vec2(dFdx(UV.x), dFdy(UV.x));
	vec2 gy = vec2(dFdx(UV.y), dFdy(UV.y));
	vec2 rect_size = 1.0 / max(vec2(length(gx), length(gy)), vec2(1e-6));

	float alpha;

	if (screen_space) {
		vec2 half_size = rect_size * 0.5;
		float min_half = min(half_size.x, half_size.y);

		vec2 p = (UV - 0.5) * rect_size;

		float fade_px = fade * min_half;
		float radius = roundness * min_half;

		vec2 core_half = half_size - fade_px;
		float core_r = clamp(radius - fade_px, 0.0, min(core_half.x, core_half.y));

		vec2 q = abs(p) - core_half + core_r;
		float sd = length(max(q, 0.0)) + min(max(q.x, q.y), 0.0) - core_r;

		alpha = 1.0 - smoothstep(0.0, fade_px, sd);
	} else {
		vec2 p = (UV - 0.5) * 2.0;

		float exponent = mix(2.0, 20.0, 1.0 - roundness);
		vec2 ap = abs(p);

		float dist = pow(
			pow(ap.x, exponent) + pow(ap.y, exponent),
			1.0 / exponent
		);

		alpha = smoothstep(0.0, fade, 1.0 - dist);
	}

	COLOR = color;
	COLOR.a *= alpha;
}
