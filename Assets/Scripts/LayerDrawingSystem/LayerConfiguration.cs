using System;
using UnityEngine;

namespace CustomLayerDrawing
{
    /// <summary>
    /// Cấu hình chứa nhiều layer để vẽ
    /// </summary>
    [Serializable]
    public class LayerConfiguration
    {
        [SerializeField] public Layer[] layers;

        public LayerConfiguration()
        {
            layers = new Layer[0];
        }

        public LayerConfiguration(int layerCount)
        {
            layers = new Layer[layerCount];
            for (int i = 0; i < layerCount; i++)
            {
                layers[i] = new Layer();
            }
        }

        /// <summary>
        /// Thêm layer mới
        /// </summary>
        public void AddLayer(Layer layer)
        {
            Layer[] newLayers = new Layer[layers.Length + 1];
            for (int i = 0; i < layers.Length; i++)
            {
                newLayers[i] = layers[i];
            }
            newLayers[layers.Length] = layer;
            layers = newLayers;
        }

        /// <summary>
        /// Tạo config mẫu - Background đơn giản
        /// </summary>
        public static LayerConfiguration CreateSimpleBackground(Color color)
        {
            LayerConfiguration config = new LayerConfiguration(1);
            config.layers[0] = Layer.CreateSolidColor(color);
            return config;
        }

        /// <summary>
        /// Tạo config mẫu - Background với viền
        /// </summary>
        public static LayerConfiguration CreateBackgroundWithBorder(Color backgroundColor, Color borderColor, float borderWidth = 1f, float borderRadius = 0f)
        {
            LayerConfiguration config = new LayerConfiguration(2);
            
            // Layer 1: Background
            config.layers[0] = Layer.CreateRoundedRect(backgroundColor, borderRadius);
            
            // Layer 2: Border (đè lên)
            config.layers[1] = Layer.CreateBorder(borderColor, borderWidth, borderRadius);
            
            return config;
        }

        /// <summary>
        /// Tạo config mẫu - Card style với shadow
        /// </summary>
        public static LayerConfiguration CreateCardStyle(Color cardColor, Color shadowColor, float cornerRadius = 4f)
        {
            LayerConfiguration config = new LayerConfiguration(2);
            
            // Layer 1: Shadow (offset xuống và phải một chút)
            config.layers[0] = Layer.CreateRoundedRect(shadowColor, cornerRadius);
            config.layers[0].padding = new Padding(0, 2, 2, 0); // Offset shadow
            
            // Layer 2: Card chính
            config.layers[1] = Layer.CreateRoundedRect(cardColor, cornerRadius);
            
            return config;
        }
    }

    /// <summary>
    /// Một layer đơn lẻ
    /// </summary>
    [Serializable]
    public class Layer
    {
        [SerializeField] public bool enabled = true;
        [SerializeField] public LayerType type = LayerType.SolidColor;
        [SerializeField] public Color color = Color.white;
        [SerializeField] public Color gradientEndColor = Color.black;
        [SerializeField] public GradientDirection gradientDirection = GradientDirection.Vertical;
        [SerializeField] public Padding padding = new Padding();
        [SerializeField] public Vector4 borderWidth = Vector4.zero;
        [SerializeField] public Vector4 borderRadius = Vector4.zero;

        public Layer()
        {
        }

        /// <summary>
        /// Tạo layer màu đặc
        /// </summary>
        public static Layer CreateSolidColor(Color color, Padding padding = null)
        {
            Layer layer = new Layer();
            layer.type = LayerType.SolidColor;
            layer.color = color;
            layer.padding = padding ?? new Padding();
            return layer;
        }

        /// <summary>
        /// Tạo layer viền
        /// </summary>
        public static Layer CreateBorder(Color color, float borderWidth = 1f, float borderRadius = 0f, Padding padding = null)
        {
            Layer layer = new Layer();
            layer.type = LayerType.Border;
            layer.color = color;
            layer.borderWidth = Vector4.one * borderWidth;
            layer.borderRadius = Vector4.one * borderRadius;
            layer.padding = padding ?? new Padding();
            return layer;
        }

        /// <summary>
        /// Tạo layer hình chữ nhật bo góc
        /// </summary>
        public static Layer CreateRoundedRect(Color color, float borderRadius = 4f, Padding padding = null)
        {
            Layer layer = new Layer();
            layer.type = LayerType.RoundedRect;
            layer.color = color;
            layer.borderWidth = Vector4.one; // Sẽ được nhân 100 khi vẽ
            layer.borderRadius = Vector4.one * borderRadius;
            layer.padding = padding ?? new Padding();
            return layer;
        }

        /// <summary>
        /// Tạo layer gradient
        /// </summary>
        public static Layer CreateGradient(Color startColor, Color endColor, GradientDirection direction = GradientDirection.Vertical, Padding padding = null)
        {
            Layer layer = new Layer();
            layer.type = LayerType.Gradient;
            layer.color = startColor;
            layer.gradientEndColor = endColor;
            layer.gradientDirection = direction;
            layer.padding = padding ?? new Padding();
            return layer;
        }
    }

    /// <summary>
    /// Padding cho layer
    /// </summary>
    [Serializable]
    public class Padding
    {
        [SerializeField] public float left;
        [SerializeField] public float right;
        [SerializeField] public float top;
        [SerializeField] public float bottom;

        public Padding()
        {
            left = right = top = bottom = 0;
        }

        public Padding(float all)
        {
            left = right = top = bottom = all;
        }

        public Padding(float left, float right, float top, float bottom)
        {
            this.left = left;
            this.right = right;
            this.top = top;
            this.bottom = bottom;
        }

        public Padding(float horizontal, float vertical)
        {
            left = right = horizontal;
            top = bottom = vertical;
        }
    }

    /// <summary>
    /// Loại layer
    /// </summary>
    [Serializable]
    public enum LayerType
    {
        SolidColor,     // Màu đặc
        Border,         // Chỉ viền
        RoundedRect,    // Hình chữ nhật bo góc đầy đủ
        Gradient        // Gradient
    }

    /// <summary>
    /// Hướng gradient
    /// </summary>
    [Serializable]
    public enum GradientDirection
    {
        Horizontal,
        Vertical
    }
}

