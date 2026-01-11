using System;
using VahTyah;

public class LevelHandler : LevelsHandlerBase
{
    public override string GetPropertyName =>  "_levelData";
    public override Type GetLevelDatabaseType => typeof(LevelDatabase);
    public override Type GetLevelType => typeof(LevelData);
    public override string LevelFolderPath => "Assets/_Project/Data/Levels";
}