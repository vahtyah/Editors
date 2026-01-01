using CustomInspector;
using UnityEngine;

public class Test : MonoBehaviour
{
    [CustomBoxGroup("SafeArea", "Safe Area")]
    public RectTransform safeAreaRectTransform;
    
    [CustomBoxGroup("SafeArea")]
    public GameObject coinsPanel;
    
    [CustomBoxGroup("SafeArea")]
    public GameObject levelNumberText;
    
    [CustomBoxGroup("SafeArea")]
    public GameObject stagePanel;
    
    [CustomBoxGroup("Gameplay", "Gameplay Timer")]
    public GameObject gameplayTimer;
    
    [CustomBoxGroup("Gameplay")]
    public GameObject noMoreMovesIndicator;
    
    // Ví dụ với style khác
    [CustomBoxGroup("Settings", "Game Settings", BoxStyle. Accent)]
    public int maxMoves = 30;
    
    [CustomBoxGroup("Settings")]
    public float timeLimit = 120f;
    
    [CustomBoxGroup("Settings")]
    public bool enableHints = true;
    
    // Property không có group - vẫn hiển thị bình thường
    public string playerName;
    public int score;
}


