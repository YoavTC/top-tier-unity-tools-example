using DG.Tweening;
using UnityEngine;

public class MovementDetection : MonoBehaviour
{
    public Vector3 originPosition;
    public MeshRenderer lightObject;

    public Color activeColor;
    public Color inactiveColor;
    
    void Update()
    {
        bool isHit = Physics.Raycast(originPosition, Vector3.down);
        lightObject.material.DOColor(isHit ? activeColor : inactiveColor, 0f);
    }
}
