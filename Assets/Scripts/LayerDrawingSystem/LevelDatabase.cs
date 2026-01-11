using UnityEngine;

[CreateAssetMenu(fileName = "LevelDatabase", menuName = "Level/LevelDataBase", order = 0)]
public class LevelDatabase : ScriptableObject
{
    [SerializeField] private LevelData[] _levelData;

    public LevelData[] LevelData => _levelData;

    public int AmountOfLevels => _levelData.Length;
    
    public LevelData GetLevel(int i)
    {
        if (i < AmountOfLevels && i >= 0)
            return _levelData[i];

        return null;
    }
}