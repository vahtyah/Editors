using UnityEditor;
using UnityEngine;

public class BrushPanel : EditorPanel
{
    private int gridX = 5; // Row (hàng)
    private int gridY = 5; // Column (cột)
    private int cellSize = 30;
    
    // Lưu trữ màu sắc của từng cell trong 2 grids
    private Color[,] grid1Colors;
    private Color[,] grid2Colors;
    
    // Lưu trữ texture của từng cell
    private Texture2D[,] grid1Textures;
    private Texture2D[,] grid2Textures;
    
    // Texture hiện tại để vẽ
    private Texture2D currentTexture;
    
    // Màu hiện tại để tô cho texture
    private Color currentColor = Color.white;
    
    // Color Palette ScriptableObject
    private ColorPalette colorPalette;
    
    // Màu mặc định
    private Color defaultCellColor = new Color(0.3f, 0.3f, 0.3f, 1f);
    private Color hoverCellColor = new Color(0.5f, 0.5f, 0.5f, 1f);
    private Color selectedCellColor = new Color(0.2f, 0.6f, 0.8f, 1f);
    
    public override string Name => "Brushes";
    
    public override void OnEnable()
    {
        InitializeGrids();
        LoadColorPalette();
    }
    
    private void LoadColorPalette()
    {
        // Load ColorPalette từ Assets
        colorPalette = AssetDatabase.LoadAssetAtPath<ColorPalette>("Assets/ScriptableObjects/ColorPalette.asset");
        
        if (colorPalette == null)
        {
            Debug.LogWarning("ColorPalette not found at Assets/ScriptableObjects/ColorPalette.asset");
        }
    }
    
    private void InitializeGrids()
    {
        grid1Colors = new Color[gridX, gridY];
        grid2Colors = new Color[gridX, gridY];
        grid1Textures = new Texture2D[gridX, gridY];
        grid2Textures = new Texture2D[gridX, gridY];
        
        // Khởi tạo màu mặc định
        for (int row = 0; row < gridX; row++)
        {
            for (int col = 0; col < gridY; col++)
            {
                grid1Colors[row, col] = defaultCellColor;
                grid2Colors[row, col] = defaultCellColor;
                grid1Textures[row, col] = null;
                grid2Textures[row, col] = null;
            }
        }
    }
    
    public override void Draw()
    {
        EditorGUILayout.LabelField("Brush Panel Content Goes Here", EditorStyles.boldLabel);
        EditorGUILayout.Space();
        
        // Controls for grid size
        EditorGUILayout.BeginHorizontal();
        int newX = EditorGUIHelper.DrawIntField("X (Rows)", gridX, 80);
        int newY = EditorGUIHelper.DrawIntField("Y (Columns)", gridY, 80);
        EditorGUILayout.EndHorizontal();
        
        // Nếu kích thước thay đổi, khởi tạo lại grids
        if (newX != gridX || newY != gridY)
        {
            gridX = Mathf.Max(1, newX);
            gridY = Mathf.Max(1, newY);
            InitializeGrids();
        }
        
        cellSize = EditorGUIHelper.DrawIntField("Cell Size", cellSize, 100);
        cellSize = Mathf.Max(10, cellSize);
        
        EditorGUILayout.Space(10);
        
        // Texture selection
        currentTexture = EditorGUIHelper.DrawObjectField<Texture2D>("Current Texture", currentTexture, false, 100);
        
        // Color selection for tinting texture
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Current Color", GUILayout.Width(100));
        currentColor = EditorGUILayout.ColorField(currentColor);
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.Space(10);
        
        // Color Palette buttons
        DrawColorPaletteButtons();
        
        EditorGUILayout.Space(20);
        
        // Draw the two grids centered
        DrawCenteredGrids();
    }
    
    private void DrawColorPaletteButtons()
    {
        if (colorPalette == null || colorPalette.Colors == null || colorPalette.Colors.Length == 0)
        {
            EditorGUILayout.HelpBox("No Color Palette loaded. Please create a ColorPalette at Assets/ScriptableObjects/ColorPalette.asset", MessageType.Warning);
            return;
        }
        
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Color Palette:", GUILayout.Width(100));
        
        float buttonSize = 30f;
        for (int i = 0; i < colorPalette.Colors.Length; i++)
        {
            Color paletteColor = colorPalette.Colors[i];
            bool isSelected = ColorDistance(currentColor, paletteColor) < 0.01f;
            
            // Tạo style cho button vuông
            GUIStyle buttonStyle = new GUIStyle(GUI.skin.button);
            buttonStyle.normal.background = MakeTex(2, 2, paletteColor);
            buttonStyle.hover.background = MakeTex(2, 2, Color.Lerp(paletteColor, Color.white, 0.2f));
            buttonStyle.active.background = MakeTex(2, 2, paletteColor);
            
            // Vẽ border trắng cho button được chọn
            if (isSelected)
            {
                buttonStyle.border = new RectOffset(3, 3, 3, 3);
                buttonStyle.padding = new RectOffset(0, 0, 0, 0);
            }
            
            // Vẽ button hình vuông không có text
            Color oldBgColor = GUI.backgroundColor;
            GUI.backgroundColor = isSelected ? Color.white : new Color(0.9f, 0.9f, 0.9f, 1f);
            
            if (GUILayout.Button("", buttonStyle, GUILayout.Width(buttonSize), GUILayout.Height(buttonSize)))
            {
                currentColor = paletteColor;
            }
            
            GUI.backgroundColor = oldBgColor;
            
            // Vẽ border highlight cho button được chọn
            if (isSelected)
            {
                Rect lastRect = GUILayoutUtility.GetLastRect();
                Handles.BeginGUI();
                Handles.color = Color.white;
                
                // Vẽ viền trắng dày
                float thickness = 3f;
                Handles.DrawAAPolyLine(thickness,
                    new Vector3(lastRect.xMin, lastRect.yMin),
                    new Vector3(lastRect.xMax, lastRect.yMin),
                    new Vector3(lastRect.xMax, lastRect.yMax),
                    new Vector3(lastRect.xMin, lastRect.yMax),
                    new Vector3(lastRect.xMin, lastRect.yMin));
                
                Handles.EndGUI();
            }
        }
        
        EditorGUILayout.EndHorizontal();
    }
    
    private Texture2D MakeTex(int width, int height, Color col)
    {
        Color[] pix = new Color[width * height];
        for (int i = 0; i < pix.Length; i++)
        {
            pix[i] = col;
        }
        
        Texture2D result = new Texture2D(width, height);
        result.SetPixels(pix);
        result.Apply();
        
        return result;
    }
    
    private float ColorDistance(Color a, Color b)
    {
        return Mathf.Abs(a.r - b.r) + Mathf.Abs(a.g - b.g) + Mathf.Abs(a.b - b.b) + Mathf.Abs(a.a - b.a);
    }
    
    private void DrawCenteredGrids()
    {
        // Calculate grid dimensions (Y=columns=width, X=rows=height)
        float gridWidth = gridY * cellSize;
        float gridHeight = gridX * cellSize;
        float spacing = 20f;
        
        // Get the rect for drawing area
        Rect drawArea = GUILayoutUtility.GetRect(
            gridWidth * 2 + spacing, 
            gridHeight,
            GUILayout.ExpandWidth(true)
        );
        
        // Calculate center position
        float totalWidth = gridWidth * 2 + spacing;
        float startX = drawArea.x + (drawArea.width - totalWidth) / 2;
        float startY = drawArea.y;
        
        // Draw first grid
        Rect grid1Rect = new Rect(startX, startY, gridWidth, gridHeight);
        DrawGrid(grid1Rect, grid1Colors, grid1Textures, "Grid 1");
        
        // Draw second grid
        Rect grid2Rect = new Rect(startX + gridWidth + spacing, startY, gridWidth, gridHeight);
        DrawGrid(grid2Rect, grid2Colors, grid2Textures, "Grid 2");
    }
    
    private void DrawGrid(Rect rect, Color[,] gridColors, Texture2D[,] gridTextures, string label)
    {
        Event e = Event.current;
        
        // Vẽ từng cell riêng biệt
        for (int row = 0; row < gridX; row++)
        {
            for (int col = 0; col < gridY; col++)
            {
                // Tính toán vị trí của từng cell
                float cellX = rect.x + col * cellSize;
                float cellY = rect.y + row * cellSize;
                Rect cellRect = new Rect(cellX, cellY, cellSize, cellSize);
                
                // Lấy texture và màu hiện tại của cell
                Texture2D cellTexture = gridTextures[row, col];
                Color cellColor = gridColors[row, col]; // Màu gốc được lưu
                
                // Kiểm tra hover
                bool isHovered = cellRect.Contains(e.mousePosition);
                
                // Vẽ cell background
                Color displayColor = cellColor;
                if (isHovered && e.type == EventType.Repaint && cellTexture == null)
                {
                    // Chỉ áp dụng hover effect cho background khi không có texture
                    displayColor = Color.Lerp(cellColor, hoverCellColor, 0.3f);
                }
                EditorGUI.DrawRect(cellRect, displayColor);
                
                // Vẽ texture nếu có
                if (cellTexture != null)
                {
                    // Áp dụng màu tint cho texture - d��ng màu GỐC đã lưu
                    Color oldColor = GUI.color;
                    GUI.color = cellColor; // Dùng màu gốc, không phải displayColor
                    GUI.DrawTexture(cellRect, cellTexture, ScaleMode.ScaleToFit);
                    GUI.color = oldColor;
                }
                
                // Vẽ viền cell
                Handles.BeginGUI();
                Handles.color = new Color(0.1f, 0.1f, 0.1f, 1f);
                Handles.DrawSolidRectangleWithOutline(cellRect, Color.clear, Handles.color);
                Handles.EndGUI();
                
                // Xử lý click
                if (isHovered && e.type == EventType.MouseDown && e.button == 0)
                {
                    if (currentTexture != null)
                    {
                        // Nếu có texture được chọn - vẽ texture vào cell với màu hiện tại
                        gridTextures[row, col] = currentTexture;
                        gridColors[row, col] = currentColor; // Sử dụng màu hiện tại để tô texture
                    }
                    else
                    {
                        // Nếu không có texture - tô màu
                        gridColors[row, col] = currentColor;
                        gridTextures[row, col] = null;
                    }
                    e.Use();
                    GUI.changed = true;
                }
                else if (isHovered && e.type == EventType.MouseDown && e.button == 1)
                {
                    // Click phải - reset về màu mặc định và xóa texture
                    gridColors[row, col] = defaultCellColor;
                    gridTextures[row, col] = null;
                    e.Use();
                    GUI.changed = true;
                }
            }
        }
        
        // Draw border around entire grid
        Handles.BeginGUI();
        Handles.color = Color.white;
        Handles.DrawSolidRectangleWithOutline(rect, Color.clear, Color.white);
        Handles.EndGUI();
        
        // Draw label below grid
        Rect labelRect = new Rect(rect.x, rect.y + rect.height + 5, rect.width, 20);
        EditorGUI.LabelField(labelRect, label, EditorStyles.centeredGreyMiniLabel);
    }
}