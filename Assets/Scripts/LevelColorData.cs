
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/LevelColorData", order = 1)]
public class LevelColorData : ScriptableObject
{
    public int level;
    public Color _MapColor;
    public Color _WordColor;
}
