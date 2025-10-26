# Layer Drawing System

Hệ thống vẽ nhiều layer background chồng lên nhau cho Unity Editor GUI.

## 📁 Cấu trúc Files

```
LayerDrawingSystem/
├── LayerDrawingSystem.cs          // Core system - vẽ layers
├── LayerConfiguration.cs          // Data structures - config & layers
├── LayerDrawingDemoWindow.cs      // Demo window
├── LayerDrawingExample.cs         // Example component
└── README.md                       // Hướng dẫn này
```

## 🚀 Cách Sử Dụng Cơ Bản

### 1. Tạo Layer Configuration

```csharp
using CustomLayerDrawing;

// Cách 1: Sử dụng preset có sẵn
LayerConfiguration config = LayerConfiguration.CreateSimpleBackground(Color.gray);

// Cách 2: Tạo background với viền
LayerConfiguration config = LayerConfiguration.CreateBackgroundWithBorder(
    Color.gray,      // Màu background
    Color.white,     // Màu viền
    2f,              // Độ dày viền
    5f               // Bán kính bo góc
);

// Cách 3: Tạo card style với shadow
LayerConfiguration config = LayerConfiguration.CreateCardStyle(
    Color.gray,      // Màu card
    Color.black,     // Màu shadow
    8f               // Bán kính bo góc
);
```

### 2. Vẽ Layers trong OnGUI

```csharp
using CustomLayerDrawing;
using UnityEditor;
using UnityEngine;

public class MyEditorWindow : EditorWindow
{
    private LayerConfiguration backgroundConfig;

    private void OnEnable()
    {
        // Khởi tạo config
        backgroundConfig = LayerConfiguration.CreateSimpleBackground(Color.gray);
    }

    private void OnGUI()
    {
        // Tạo rect cần vẽ
        Rect rect = GUILayoutUtility.GetRect(100, 50);
        
        // Vẽ layers
        LayerDrawingSystem.DrawLayers(rect, backgroundConfig);
        
        // Vẽ content lên trên
        GUI.Label(rect, "Hello World!");
    }
}
```

### 3. Tạo Custom Multi-Layer

```csharp
using CustomLayerDrawing;

// Tạo config với 3 layers
LayerConfiguration config = new LayerConfiguration(3);

// Layer 1: Background gradient
config.layers[0] = Layer.CreateGradient(
    new Color(0.2f, 0.3f, 0.5f),    // Màu bắt đầu
    new Color(0.1f, 0.15f, 0.25f),  // Màu kết thúc
    GradientDirection.Vertical       // Hướng gradient
);

// Layer 2: Viền bo góc
config.layers[1] = Layer.CreateBorder(
    Color.white,     // Màu viền
    2f,              // Độ dày viền
    10f,             // Bán kính bo góc
    new Padding(2)   // Padding 2px tất cả các cạnh
);

// Layer 3: Highlight góc trên
config.layers[2] = Layer.CreateSolidColor(
    new Color(1f, 1f, 1f, 0.1f),      // Màu trắng trong suốt
    new Padding(5, 5, 5, 50)           // Padding: left, right, top, bottom
);

// Vẽ
LayerDrawingSystem.DrawLayers(yourRect, config);
```

## 🎨 Các Loại Layer

### 1. **SolidColor** - Màu đặc
```csharp
Layer layer = Layer.CreateSolidColor(Color.blue);
```

### 2. **Border** - Chỉ viền
```csharp
Layer layer = Layer.CreateBorder(
    Color.white,  // Màu viền
    2f,           // Độ dày viền
    5f            // Bán kính bo góc
);
```

### 3. **RoundedRect** - Hình chữ nhật bo góc đầy đủ
```csharp
Layer layer = Layer.CreateRoundedRect(
    Color.blue,   // Màu
    8f            // Bán kính bo góc
);
```

### 4. **Gradient** - Gradient
```csharp
Layer layer = Layer.CreateGradient(
    Color.blue,                      // Màu bắt đầu
    Color.cyan,                      // Màu kết thúc
    GradientDirection.Vertical       // Hướng
);
```

## 📐 Padding System

Padding giúp tạo khoảng cách giữa các layer:

```csharp
// Padding đều 4 cạnh
new Padding(10)                      // 10px tất cả

// Padding ngang và dọc
new Padding(horizontal: 10, vertical: 5)

// Padding custom từng cạnh
new Padding(
    left: 5,
    right: 10,
    top: 15,
    bottom: 20
)
```

## 🎯 Examples

### Example 1: Footer Buttons như CustomList
```csharp
LayerConfiguration footerConfig = new LayerConfiguration(2);

// Layer 1: Background
footerConfig.layers[0] = Layer.CreateRoundedRect(
    new Color(0.3f, 0.3f, 0.3f),
    4f,
    new Padding(0, 0, -1, 1)
);

// Layer 2: Border
footerConfig.layers[1] = Layer.CreateBorder(
    new Color(0.5f, 0.5f, 0.5f),
    1f,
    4f,
    new Padding(0, 0, -1, 0)
);
```

### Example 2: Header với Gradient
```csharp
LayerConfiguration headerConfig = new LayerConfiguration(2);

// Layer 1: Gradient background
headerConfig.layers[0] = Layer.CreateGradient(
    new Color(0.2f, 0.4f, 0.6f),
    new Color(0.15f, 0.3f, 0.45f),
    GradientDirection.Vertical
);

// Layer 2: Border dưới
headerConfig.layers[1] = Layer.CreateBorder(
    Color.white,
    new Vector4(0, 0, 0, 1),  // Chỉ viền dưới
    0f
);
```

### Example 3: Card với Shadow
```csharp
LayerConfiguration cardConfig = new LayerConfiguration(2);

// Layer 1: Shadow (offset)
cardConfig.layers[0] = Layer.CreateRoundedRect(
    new Color(0f, 0f, 0f, 0.3f),  // Đen trong suốt
    8f,
    new Padding(0, 2, 2, 0)        // Offset xuống phải
);

// Layer 2: Card chính
cardConfig.layers[1] = Layer.CreateRoundedRect(
    new Color(0.3f, 0.3f, 0.3f),
    8f
);
```

## 🧪 Xem Demo

### 1. Mở Demo Window
```
Unity Menu: Window > Layer Drawing Demo
```

### 2. Tạo GameObject với Example Component
```
1. Tạo Empty GameObject
2. Add Component: LayerDrawingExample
3. Xem Inspector để thấy custom preview
```

## 🔧 Advanced Usage

### Bật/Tắt Layer
```csharp
config.layers[0].enabled = false;  // Tắt layer
config.layers[0].enabled = true;   // Bật layer
```

### Thay đổi màu động
```csharp
config.layers[0].color = Color.red;
```

### Thêm layer sau khi tạo
```csharp
Layer newLayer = Layer.CreateBorder(Color.white, 1f, 5f);
config.AddLayer(newLayer);
```

### Custom BorderWidth và BorderRadius
```csharp
Layer layer = new Layer();
layer.type = LayerType.Border;
layer.borderWidth = new Vector4(1, 2, 1, 2);  // left, top, right, bottom
layer.borderRadius = new Vector4(5, 5, 0, 0);  // topLeft, topRight, bottomRight, bottomLeft
```

## 📝 So sánh với CustomList System

| Feature | CustomList | LayerDrawing System |
|---------|-----------|-------------------|
| Độc lập | ❌ Gắn với CustomList | ✅ Hoàn toàn độc lập |
| Dễ sử dụng | ⚠️ Phức tạp | ✅ Đơn giản |
| Gradient | ❌ Không có | ✅ Có |
| Serializable | ✅ Có | ✅ Có |
| Custom Inspector | ⚠️ Khó | ✅ Dễ |

## 🎓 Tips

1. **Layer Order**: Layer cuối trong mảng sẽ vẽ lên trên cùng
2. **Performance**: Nên cache LayerConfiguration thay vì tạo mới mỗi frame
3. **Padding Negative**: Có thể dùng padding âm để mở rộng layer ra ngoài rect
4. **Repaint**: Chỉ vẽ khi `Event.current.type == EventType.Repaint`

## 🐛 Troubleshooting

**Q: Không thấy layer vẽ ra?**
- Kiểm tra `Event.current.type == EventType.Repaint`
- Kiểm tra rect có size > 0
- Kiểm tra layer.enabled = true

**Q: Màu không đúng?**
- Unity dùng Color với range 0-1, không phải 0-255
- Kiểm tra alpha channel (Color.a)

**Q: Bo góc không hiện?**
- Kiểm tra borderRadius > 0
- Kiểm tra layer type là Border hoặc RoundedRect

## 📄 License
Free to use in your Unity projects.

