using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using VahTyah;
using VahTyah.LevelEditor;

public class LevelEditor : LevelEditorBase
{
    private PanelNavigator panelNavigator;

    protected override void OnEnable()
    {
        base.OnEnable();
        var panels = new Dictionary<string, IEditorPanel>()
        {
            { "Levels", new LevelsPanel() },
            { "Tiles", new TilesPanel() },
            { "Objects", new ObjectsPanel() },
        };
        panelNavigator = new PanelNavigator(panels);
    }

    protected override void DrawContent()
    {
        var panelArea = new Rect(ResizableSidebar.TotalSize + 12f, 6f, position.width - ResizableSidebar.CurrentWidth - 22f, position.height);
        panelNavigator.Draw(panelArea);
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        panelNavigator.Cleanup();
    }
}

public class LevelsPanel : IEditorPanel
{
    public string Name => "Levels";
    private Vector2 scrollPosition;

    public void Initialize()
    {
    }

    public void OnEnable()
    {
        // Khởi tạo tài nguyên cho panel Levels
    }

    public void OnDisable()
    {
        // Giải phóng tài nguyên cho panel Levels
    }

    public void Draw(Rect rect)
    {
        // Calculate content height
        float contentHeight = 30 + 120 + (30 * 35); // ~1200px
        
        // Create view rect for scrolling
        Rect viewRect = new Rect(0, 0, rect.width - 20f, contentHeight);
        scrollPosition = GUI.BeginScrollView(rect, scrollPosition, viewRect);
        
        // DEMO: Hiển thị thông tin về Rect
        GUI.Label(new Rect(10, 10, viewRect.width - 20, 20), $"Levels Panel - Content: {contentHeight}px | Viewport: {rect.height:F0}px");
        
        // Vẽ một box để thấy rõ vùng vẽ
        GUI.Box(new Rect(10, 40, viewRect.width - 20, 80), "");
        GUI.Label(new Rect(20, 50, viewRect.width - 40, 60), "Panel tự quản lý scroll!\nToạ độ TƯƠNG ĐỐI từ (0,0).\n\nScroll down để xem thêm content!");
        
        // Thêm nhiều levels để test scroll
        float currentY = 130;
        for (int i = 1; i <= 30; i++)
        {
            GUI.Box(new Rect(10, currentY, viewRect.width - 20, 30), $"Level {i}");
            currentY += 35;
        }
        
        GUI.EndScrollView();
    }
}

public class TilesPanel : IEditorPanel
{
    public string Name => "Tiles";
    private Vector2 scrollPosition;

    public void Initialize()
    {
    }

    public void OnEnable()
    {
        // Khởi tạo tài nguyên cho panel Tiles
    }

    public void OnDisable()
    {
        // Giải phóng tài nguyên cho panel Tiles
    }

    public void Draw(Rect rect)
    {
        // Calculate content height
        float contentHeight = 30 + (50 * 30) + 30; // ~1560px
        
        // Create view rect for scrolling
        Rect viewRect = new Rect(0, 0, rect.width - 20f, contentHeight);
        scrollPosition = GUI.BeginScrollView(rect, scrollPosition, viewRect);
        
        // Sử dụng rect.width để responsive
        GUI.Label(new Rect(10, 10, viewRect.width - 20, 20), $"Tiles Panel - {contentHeight}px content (Scroll down!)");
        
        // Vẽ nhiều items để test scrolling
        for (int i = 0; i < 50; i++)
        {
            GUI.Box(new Rect(10, 40 + (i * 30), viewRect.width - 20, 25), $"Tile Item {i + 1}");
        }
        
        // Thêm một label ở cuối để biết đã scroll đến đáy
        GUI.Label(new Rect(10, 40 + (50 * 30), viewRect.width - 20, 30), "=== END OF LIST ===");
        
        GUI.EndScrollView();
    }
}

public class ObjectsPanel : IEditorPanel
{
    public string Name => "Objects";

    public void Initialize()
    {
    }

    public void OnEnable()
    {
        // Khởi tạo tài nguyên cho panel Objects
    }

    public void OnDisable()
    {
        // Giải phóng tài nguyên cho panel Objects
    }

    public void Draw(Rect rect)
    {
        // NO ScrollView - content is small, just draw directly
        // GUILayout.BeginArea expects absolute coordinates - pass rect directly
        GUILayout.BeginArea(rect);
        
        GUILayout.Label("Objects Panel - Không cần scroll (ít content)", EditorStyles.boldLabel);
        GUILayout.Space(10);
        
        GUILayout.Label($"Viewport: {rect.width:F0} x {rect.height:F0}");
        GUILayout.Space(10);
        
        // Chỉ vẽ 5 objects - vừa khít không cần scroll
        for (int i = 1; i <= 5; i++)
        {
            GUILayout.BeginVertical("box");
            GUILayout.Label($"Object {i}");
            GUILayout.Label($"Type: GameObject");
            GUILayout.Label($"Position: ({i * 10}, {i * 5}, 0)");
            GUILayout.EndVertical();
            GUILayout.Space(5);
        }
        
        GUILayout.Label("✓ Panel tự quyết định khi nào cần scroll!", EditorStyles.centeredGreyMiniLabel);
        
        GUILayout.EndArea();
    }
}
