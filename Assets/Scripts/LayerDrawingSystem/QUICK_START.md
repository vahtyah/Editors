# 🚀 QUICK START GUIDE

## Bắt đầu trong 5 phút!

### 1️⃣ Xem Demo (Ngay lập tức)

```
Unity Menu Bar > Window > Layer Drawing Demo
```

Hoặc

```
Unity Menu Bar > Window > Real World Example
```

### 2️⃣ Code đầu tiên (Copy & Paste)

Tạo file `MyFirstLayerWindow.cs`:

```csharp
using UnityEngine;
using UnityEditor;
using CustomLayerDrawing;

public class MyFirstLayerWindow : EditorWindow
{
    private LayerConfiguration myBackground;

    [MenuItem("Window/My First Layer Window")]
    public static void ShowWindow()
    {
        GetWindow<MyFirstLayerWindow>("My Window");
    }

    private void OnEnable()
    {
        // Tạo background đơn giản
        myBackground = LayerConfiguration.CreateSimpleBackground(Color.blue);
    }

    private void OnGUI()
    {
        // Lấy rect
        Rect rect = GUILayoutUtility.GetRect(100, 100);
        
        // VẼ!
        LayerDrawingSystem.DrawLayers(rect, myBackground);
        
        // Thêm text lên trên
        GUI.Label(rect, "Hello Layer!", EditorStyles.whiteLargeLabel);
    }
}
```

**Mở window**: `Window > My First Layer Window`

### 3️⃣ Nâng cao hơn (3 layers)

```csharp
private void OnEnable()
{
    // Tạo config với 3 layers
    myBackground = new LayerConfiguration(3);
    
    // Layer 1: Background gradient
    myBackground.layers[0] = Layer.CreateGradient(
        Color.blue,
        Color.cyan,
        GradientDirection.Vertical
    );
    
    // Layer 2: Viền trắng
    myBackground.layers[1] = Layer.CreateBorder(
        Color.white,
        2f,      // Độ dày
        5f       // Bo góc
    );
    
    // Layer 3: Highlight góc trên
    myBackground.layers[2] = Layer.CreateSolidColor(
        new Color(1f, 1f, 1f, 0.2f),           // Trắng trong suốt
        new Padding(10, 10, 10, 50)             // Chỉ ở góc trên
    );
}
```

### 4️⃣ Preset có sẵn

```csharp
// Background đơn giản
LayerConfiguration.CreateSimpleBackground(Color.gray)

// Background + Viền
LayerConfiguration.CreateBackgroundWithBorder(
    Color.gray,      // Màu nền
    Color.white,     // Màu viền
    2f,              // Độ dày viền
    5f               // Bo góc
)

// Card với Shadow
LayerConfiguration.CreateCardStyle(
    Color.gray,      // Màu card
    Color.black,     // Màu shadow
    8f               // Bo góc
)
```

## 💡 3 Điều Cần Nhớ

1. **Luôn tạo config trong `OnEnable()`** - Không tạo trong `OnGUI()` (lag!)
2. **Layer order**: Layer sau vẽ lên trên
3. **Padding**: Dùng để tạo khoảng cách giữa các layer

## 🎨 4 Loại Layer

| Loại | Code | Khi nào dùng |
|------|------|--------------|
| `SolidColor` | `Layer.CreateSolidColor(Color.red)` | Background đơn giản |
| `Border` | `Layer.CreateBorder(Color.white, 2f, 5f)` | Chỉ cần viền |
| `RoundedRect` | `Layer.CreateRoundedRect(Color.blue, 8f)` | Card, button bo góc |
| `Gradient` | `Layer.CreateGradient(Color.blue, Color.cyan)` | Background đẹp |

## ❓ Gặp lỗi?

**Không thấy gì vẽ ra:**
- Kiểm tra rect có size > 0 không
- Thêm `Debug.Log(rect)` để xem

**Màu sai:**
- Unity dùng 0-1, KHÔNG phải 0-255
- `Color.red` hoặc `new Color(1f, 0f, 0f)` ✅
- `new Color(255, 0, 0)` ❌

**Bo góc không hiện:**
- Dùng `RoundedRect` hoặc `Border`
- `borderRadius` phải > 0

## 📚 Đọc thêm

Xem `README.md` để biết chi tiết hơn!

---

**Happy Coding! 🎉**

