shader_type canvas_item;

// Псевдослучайная функция
float hash(vec2 p) {
    return fract(sin(dot(p, vec2(127.1, 311.7))) * 43758.5453123);
}

// Порог отсечения: минимальная и максимальная случайная яркость
uniform float min_threshold : hint_range(0.0, 1.0) = 0.2;
uniform float max_threshold : hint_range(0.0, 1.0) = 0.8;

void fragment() {
    // 1. Считываем текстуру частицы
    vec4 tex = texture(TEXTURE, UV);
    
    // 2. Генерируем уникальный сид для частицы на основе ее входного цвета
    // COLOR в начале fragment() содержит значение, пришедшее из CPUParticles (включая ramp)
    float particle_seed = hash(COLOR.rg + vec2(COLOR.b, COLOR.a));
    
    // 3. Вычисляем случайный порог яркости индивидуально для этой частицы
    float cutoff = mix(min_threshold, max_threshold, particle_seed);

    // 4. Считаем относительную яркость (luminance) текущего пикселя текстуры
    float luminance = dot(tex.rgb, vec3(0.299, 0.587, 0.114));

    // 5. Если пиксель темнее порога — делаем полностью прозрачным
    if (luminance < cutoff) {
        discard; // либо tex.a = 0.0;
    }

    // 6. Сохраняем исходный цвет из Initial Ramp / частицы, модулируя его текстурой
    COLOR = tex * COLOR;
}