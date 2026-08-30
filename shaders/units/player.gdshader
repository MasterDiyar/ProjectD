shader_type canvas_item;

// --- Униформы (параметры инспектора) ---

// Режим работы: false - сплошной цвет, true - градиент
uniform bool use_gradient = false;

// Сплошной цвет для замены (если use_gradient == false)
uniform vec4 solid_color : source_color = vec4(1.0, 0.0, 0.0, 1.0); // По умолчанию красный

// Текстура градиента (если use_gradient == true)
// В инспекторе сюда нужно перетащить GradientTexture1D
uniform sampler2D gradient_texture : source_color, filter_nearest;

// Чувствительность к белому цвету (от 0.0 до 1.0)
// Помогает, если белый не идеально чистый (например, из-за сжатия)
uniform float threshold : hint_range(0.0, 1.0) = 0.95;

void fragment() {
    // 1. Получаем исходный цвет пикселя текстуры
    vec4 tex_color = texture(TEXTURE, UV);
    
    // 2. Рассчитываем яркость (grayscale value) пикселя.
    // Используем стандартную формулу яркости, которая учитывает восприятие глазом разных каналов
    float brightness = dot(tex_color.rgb, vec3(0.299, 0.587, 0.114));
    
    // 3. Определяем, является ли этот пиксель "белым" с учетом порога
    if (brightness > threshold) {
        vec3 final_rgb;
        
        if (use_gradient) {
            // -- РЕЖИМ ГРАДИЕНТА --
            // Мы используем UV-координаты текстуры градиента.
            // По оси X (от 0 до 1) мы берем значение яркости исходного пикселя.
            // Черный станет левым краем градиента, белый - правым.
            // Так как мы заменяем только *белый*, мы будем брать цвета из *правой части* градиента.
            final_rgb = texture(gradient_texture, vec2(brightness, 0.0)).rgb;
        } else {
            // -- РЕЖИМ СПЛОШНОГО ЦВЕТА --
            final_rgb = solid_color.rgb;
        }
        
        // Применяем новый цвет, сохраняя исходную прозрачность (альфа-канал)
        COLOR = vec4(final_rgb, tex_color.a);
        
    } else {
        // 4. Если пиксель не белый (например, черный контур или фон), оставляем его как есть
        COLOR = tex_color;
    }
}