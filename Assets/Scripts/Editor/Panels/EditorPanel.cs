using UnityEditor;

public abstract class EditorPanel
{
    public abstract string Name { get; }
    
    public virtual void OnEnable() { }
    public virtual void OnDisable() { }
    public abstract void Draw();
}

