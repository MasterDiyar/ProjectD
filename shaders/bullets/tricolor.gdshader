shader_type canvas_item;

// Экспортируем переменные, чтобы их можно было менять из C#
uniform vec4 color_for_black : source_color = vec4(0.0, 0.0, 0.0, 1.0);
uniform vec4 color_for_white : source_color = vec4(1.0, 1.0, 1.0, 1.0);
uniform vec4 color_for_gray : source_color = vec4(0.5, 0.5, 0.5, 1.0);

// Погрешность (важна, если спрайт сжат или есть сглаживание)
uniform float tolerance = 0.01; 

void fragment() {
    // Берем оригинальный пиксель из текстуры
    vec4 tex_color = texture(TEXTURE, UV);
    
    // Переводим 127 в нормализованный вид (127.0 / 255.0 = 0.498)
    vec3 base_gray = vec3(0.498, 0.498, 0.498);
    vec3 base_black = vec3(0.0, 0.0, 0.0);
    vec3 base_white = vec3(1.0, 1.0, 1.0);
    
    vec3 final_color = tex_color.rgb;
    
    // Проверяем расстояние между текущим цветом пикселя и базовым цветом
    if (distance(tex_color.rgb, base_black) < tolerance) {
        final_color = color_for_black.rgb;
    } else if (distance(tex_color.rgb, base_white) < tolerance) {
        final_color = color_for_white.rgb;
    } else if (distance(tex_color.rgb, base_gray) < tolerance) {
        final_color = color_for_gray.rgb;
    }
    
    // Выводим финальный цвет, сохраняя оригинальную прозрачность пикселя
    COLOR = vec4(final_color, tex_color.a);
}