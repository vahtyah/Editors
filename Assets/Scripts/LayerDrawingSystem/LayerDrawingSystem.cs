using UnityEngine;
using UnityEditor;

namespace CustomLayerDrawing
{
    /// <summary>
    /// System vẽ nhiều layer background chồng lên nhau
    /// Sử dụng: LayerDrawingSystem.DrawLayers(rect, layerConfig);
    /// </summary>
    public static class LayerDrawingSystem
    {
        private static Rect tempRect = new Rect();

        /// <summary>
        /// Vẽ nhiều layer background lên một vùng Rect
        /// </summary>
        /// <param name="targetRect">Vùng cần vẽ</param>
        /// <param name="config">Cấu hình các layer</param>
        public static void DrawLayers(Rect targetRect, LayerConfiguration config)
        {
            if (Event.current.type != EventType.Repaint || config == null || config.layers == null)
            {
                return;
            }

            // Vẽ từng layer theo thứ tự (layer sau đè lên layer trước)
            foreach (Layer layer in config.layers)
            {
                if (!layer.enabled)
                {
                    continue;
                }

                DrawSingleLayer(targetRect, layer);
            }
        }

        /// <summary>
        /// Vẽ một layer đơn lẻ
        /// </summary>
        private static void DrawSingleLayer(Rect targetRect, Layer layer)
        {
            // Tính toán vùng vẽ thực tế (có padding)
            CalculateLayerRect(targetRect, layer, out tempRect);

            // Vẽ theo loại layer
            switch (layer.type)
            {
                case LayerType.SolidColor:
                    DrawSolidColor(tempRect, layer);
                    break;

                case LayerType.Border:
                    DrawBorder(tempRect, layer);
                    break;

                case LayerType.RoundedRect:
                    DrawRoundedRect(tempRect, layer);
                    break;

                case LayerType.Gradient:
                    DrawGradient(tempRect, layer);
                    break;
            }
        }

        /// <summary>
        /// Tính toán Rect với padding
        /// </summary>
        private static void CalculateLayerRect(Rect originalRect, Layer layer, out Rect result)
        {
            result = new Rect();
            result.x = originalRect.x + layer.padding.left;
            result.y = originalRect.y + layer.padding.top;
            result.xMax = originalRect.xMax - layer.padding.right;
            result.yMax = originalRect.yMax - layer.padding.bottom;
        }

        /// <summary>
        /// Vẽ hình chữ nhật màu đặc
        /// </summary>
        private static void DrawSolidColor(Rect rect, Layer layer)
        {
            EditorGUI.DrawRect(rect, layer.color);
        }

        /// <summary>
        /// Vẽ viền với bo góc
        /// </summary>
        private static void DrawBorder(Rect rect, Layer layer)
        {
            GUI.DrawTexture(
                rect,
                EditorGUIUtility.whiteTexture,
                ScaleMode.StretchToFill,
                true,
                0,
                layer.color,
                layer.borderWidth,
                layer.borderRadius
            );
        }

        /// <summary>
        /// Vẽ hình chữ nhật bo góc đầy đủ
        /// </summary>
        private static void DrawRoundedRect(Rect rect, Layer layer)
        {
            GUI.DrawTexture(
                rect,
                EditorGUIUtility.whiteTexture,
                ScaleMode.StretchToFill,
                true,
                0,
                layer.color,
                layer.borderWidth * 100, // Nhân 100 để fill toàn bộ
                layer.borderRadius
            );
        }

        /// <summary>
        /// Vẽ gradient
        /// </summary>
        private static void DrawGradient(Rect rect, Layer layer)
        {
            // Tạo texture gradient tạm thời
            Texture2D gradientTexture = CreateGradientTexture(layer.color, layer.gradientEndColor, layer.gradientDirection);
            GUI.DrawTexture(rect, gradientTexture, ScaleMode.StretchToFill);
            Object.DestroyImmediate(gradientTexture);
        }

        /// <summary>
        /// Tạo texture gradient
        /// </summary>
        private static Texture2D CreateGradientTexture(Color startColor, Color endColor, GradientDirection direction)
        {
            int width = direction == GradientDirection.Horizontal ? 256 : 1;
            int height = direction == GradientDirection.Vertical ? 256 : 1;

            Texture2D texture = new Texture2D(width, height);

            for (int i = 0; i < (direction == GradientDirection.Horizontal ? width : height); i++)
            {
                float t = i / (direction == GradientDirection.Horizontal ? (float)width : (float)height);
                Color color = Color.Lerp(startColor, endColor, t);

                if (direction == GradientDirection.Horizontal)
                {
                    for (int y = 0; y < height; y++)
                    {
                        texture.SetPixel(i, y, color);
                    }
                }
                else
                {
                    for (int x = 0; x < width; x++)
                    {
                        texture.SetPixel(x, i, color);
                    }
                }
            }

            texture.Apply();
            return texture;
        }
    }
}

