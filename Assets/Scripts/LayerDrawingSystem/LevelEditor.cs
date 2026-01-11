using UnityEngine;
using VahTyah;

public class LevelEditor : LevelEditorBase
{
    protected override LevelsHandlerBase GetLevelHandler => new LevelHandler();

    protected override void DrawContent()
    {
        GUILayout.Label("Level Editor Content Area");
    }
}
