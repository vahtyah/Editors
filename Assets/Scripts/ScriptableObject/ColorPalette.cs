using UnityEngine;
using VahTyah;

[CreateAssetMenu(fileName = "ColorPalette", menuName = "ColorPalette", order = 0)]
public class ColorPalette : ScriptableObject
{
    [BoxGroup("123", "123")] public Color[] Colors;
}