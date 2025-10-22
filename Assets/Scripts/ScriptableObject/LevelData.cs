using UnityEngine;

[CreateAssetMenu(fileName = "LevelData", menuName = "ScriptableObjects/LevelData", order = 1)]
public class LevelData : ScriptableObject
{
    public int levelNumber;
    public int width;
    public int height;
    public Color backgroundColor;
    public string[] layout;
}