using UnityEngine;

[CreateAssetMenu(fileName = "Cube Scriptable Object")]
public class CubeSO : ScriptableObject
{
    public Color color1;
    public Color color2;

    public float movementDuration;
    public float rotations;
}
