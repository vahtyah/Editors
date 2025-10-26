# 📝 Custom List Demo - Hướng dẫn

## 🎯 Giới thiệu

**CustomListDemo** là một ví dụ hoàn chỉnh về cách xây dựng một Custom List với đầy đủ tính năng sử dụng **Layer Drawing System**.

## 🚀 Cách mở Demo

```
Unity Menu Bar > Window > Custom List Demo
```

## ✨ Tính năng

### 📋 List Features
- ✅ **Add/Remove Items** - Thêm và xóa items
- ✅ **Select/Expand** - Click để chọn và mở rộng
- ✅ **Drag Handle Visual** - Biểu tượng kéo thả (☰)
- ✅ **Scroll View** - Cuộn khi có nhiều items
- ✅ **Empty State** - Hiển thị khi list rỗng
- ✅ **Context Actions** - Nút xóa trên mỗi item

### 🎨 Visual Features
- ✅ **Global Background** - Gradient tổng thể với viền
- ✅ **Header** - Gradient với title và info
- ✅ **List Background** - Background riêng cho list area
- ✅ **Element States** - Normal vs Selected backgrounds
- ✅ **Badges** - Level badge và Active status
- ✅ **Health Bar** - Progress bar với gradient động
- ✅ **Custom Buttons** - Buttons với hover effect
- ✅ **Color Coding** - Màu sắc phân biệt trạng thái

### 🔧 Editable Properties
Mỗi item có thể chỉnh sửa:
- **Name** (TextField)
- **Level** (IntSlider 1-100)
- **Health** (Slider 0-200 với visual bar)
- **Color** (ColorField)
- **Active Status** (Toggle)

## 🎨 Layer Drawing sử dụng

### 1. Global Background
```csharp
// Gradient tổng thể + viền
globalBackground = new LayerConfiguration(2);
globalBackground.layers[0] = Layer.CreateGradient(
    new Color(0.15f, 0.15f, 0.2f),
    new Color(0.1f, 0.1f, 0.15f),
    GradientDirection.Vertical
);
globalBackground.layers[1] = Layer.CreateBorder(
    new Color(0.3f, 0.3f, 0.35f),
    1f,
    8f
);
```

### 2. Header Background
```csharp
// Gradient với viền dưới
headerBackground = new LayerConfiguration(2);
headerBackground.layers[0] = Layer.CreateGradient(
    new Color(0.25f, 0.35f, 0.55f),
    new Color(0.2f, 0.3f, 0.5f),
    GradientDirection.Vertical
);
headerBackground.layers[1] = Layer.CreateBorder(
    new Color(0.4f, 0.5f, 0.7f),
    2f,
    0f
);
headerBackground.layers[1].borderWidth = new Vector4(0, 0, 0, 2); // Chỉ viền dưới
```

### 3. Element Selected Background
```csharp
// Rounded rect với viền
elementSelectedBackground = new LayerConfiguration(2);
elementSelectedBackground.layers[0] = Layer.CreateRoundedRect(
    new Color(0.3f, 0.5f, 0.7f),
    4f,
    new Padding(2f)
);
elementSelectedBackground.layers[1] = Layer.CreateBorder(
    new Color(0.5f, 0.7f, 0.9f),
    1f,
    4f,
    new Padding(2f)
);
```

### 4. Health Bar (Dynamic)
```csharp
// Gradient động theo health %
LayerConfiguration healthFill = new LayerConfiguration(1);
Color healthColor = Color.Lerp(
    new Color(0.8f, 0.2f, 0.2f),  // Đỏ (low health)
    new Color(0.2f, 0.8f, 0.3f),  // Xanh (high health)
    healthPercent
);
healthFill.layers[0] = Layer.CreateGradient(
    healthColor, 
    healthColor * 0.8f, 
    GradientDirection.Horizontal
);
```

### 5. Custom Button với Hover
```csharp
// Màu thay đổi khi hover
bool isHover = buttonRect.Contains(Event.current.mousePosition);
Color finalColor = isHover ? color * 1.2f : color;

LayerConfiguration customBtnBg = new LayerConfiguration(2);
customBtnBg.layers[0] = Layer.CreateRoundedRect(finalColor, 4f);
customBtnBg.layers[1] = Layer.CreateBorder(finalColor * 1.3f, 1f, 4f);
```

## 📊 Cấu trúc Code

### Data Model
```csharp
[System.Serializable]
public class Item
{
    public string name;
    public int level;
    public float health;
    public Color color;
    public bool isActive;
}
```

### Main Methods
```csharp
DrawCustomList()       // Vẽ toàn bộ list
├── DrawHeader()       // Vẽ header
├── DrawList()         // Vẽ list items
│   └── DrawElement()  // Vẽ từng element
│       └── DrawElementExpanded() // Vẽ expanded content
└── DrawFooter()       // Vẽ footer với buttons
```

## 🎓 Học từ Demo này

### 1. Layering System
Xem cách sử dụng nhiều layer để tạo depth:
- Background layer (dưới cùng)
- Border layer (trên background)
- Content layer (trên cùng)

### 2. State Management
Học cách quản lý state:
- `selectedIndex` - Item đang được chọn
- `isSelected` - Hiển thị khác khi selected
- `isHover` - Hover effect cho buttons

### 3. Dynamic Colors
Xem cách tạo màu động:
- Health bar color based on value
- Button hover effect
- Selected vs normal state

### 4. Responsive Layout
Học cách layout responsive:
- `GUILayout.ExpandWidth(true)` - Expand theo width
- `GUILayout.FlexibleSpace()` - Flexible spacing
- `GUILayoutUtility.GetRect()` - Custom rect sizing

### 5. Event Handling
Xử lý events:
- Mouse click detection
- Button click
- List item selection

## 🔍 So sánh với CustomList gốc

| Feature | CustomList gốc | CustomListDemo |
|---------|---------------|----------------|
| **Complexity** | Rất phức tạp | Đơn giản, dễ hiểu |
| **Dependencies** | SerializedProperty | Simple List<T> |
| **Drag & Drop** | Có (phức tạp) | Chưa có (đơn giản hóa) |
| **Pagination** | Có | Scroll view |
| **Background** | HandleDrawingBackground... | LayerDrawingSystem ✅ |
| **Code lines** | ~1000+ lines | ~400 lines |
| **Learning curve** | Cao | Thấp |

## 💡 Ứng dụng thực tế

Demo này có thể áp dụng cho:

1. **Inventory System** - Quản lý items trong game
2. **Character List** - Danh sách nhân vật
3. **Quest List** - Danh sách nhiệm vụ
4. **Settings Panel** - Panel cài đặt
5. **Debug Console** - Console debug với list
6. **Level Editor** - Editor cho levels/stages

## 🛠️ Tùy chỉnh

### Thay đổi màu chủ đạo
```csharp
// Trong InitializeLayerConfigs(), thay đổi màu sắc
headerBackground.layers[0] = Layer.CreateGradient(
    YOUR_COLOR_1,  // Thay màu của bạn
    YOUR_COLOR_2,
    GradientDirection.Vertical
);
```

### Thêm field mới
```csharp
// 1. Thêm property vào Item class
public int experiencePoints = 0;

// 2. Thêm vào DrawElementExpanded()
GUILayout.BeginHorizontal();
GUILayout.Label("Experience:", labelStyle, GUILayout.Width(80));
item.experiencePoints = EditorGUILayout.IntField(item.experiencePoints);
GUILayout.EndHorizontal();
```

### Thay đổi layout
```csharp
// Thay đổi constants
private const float HEADER_HEIGHT = 60f;      // Tăng header
private const float ELEMENT_HEIGHT = 40f;     // Tăng element height
private const float ELEMENT_EXPANDED_BASE = 200f; // Tăng expanded height
```

## 🎯 Thử thách

Sau khi hiểu demo, thử:

1. ✅ Thêm search/filter functionality
2. ✅ Thêm sort by level/name
3. ✅ Thêm drag & drop để reorder
4. ✅ Thêm context menu (right click)
5. ✅ Thêm export/import data
6. ✅ Thêm undo/redo system

## 📚 Đọc thêm

- `README.md` - Tài liệu Layer Drawing System
- `QUICK_START.md` - Hướng dẫn nhanh
- `RealWorldExample.cs` - Ví dụ Player Stats UI
- `LayerDrawingDemoWindow.cs` - Demo cơ bản

---

**Chúc bạn học tốt! 🎉**

