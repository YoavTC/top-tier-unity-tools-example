using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;

public class MoveCube : MonoBehaviour
{
    public Transform cube;
    public CubeSO cubeData;

    private float speedMultiplier;

    public Vector3 positionA, positionB;
    private bool isAtPointB;

    private bool isMoving; 
    
    void Start()
    {
        isMoving = false;
        isAtPointB = false;
        cube.position = positionA;
        Camera.main.backgroundColor = cubeData.color2;

        speedMultiplier = PlayerPrefs.GetFloat("speed_multiplier");
    }

    [Button]
    void TranslateCube()
    {
        if (isMoving) return;
        
        isMoving = true;
        Vector3 targetPosition = isAtPointB ? positionA : positionB;
        
        StartMoveCube(targetPosition);
        StartRotateCube();
    }
    
    private void StartMoveCube(Vector3 targetPosition)
    {
        cube.DOMove(targetPosition, cubeData.movementDuration)
            .SetEase(Ease.Linear)
            .OnComplete(() => OnMoveCompleteCallback(isAtPointB ? cubeData.color2 : cubeData.color1));
    }
    
    private void StartRotateCube()
    {
        cube.DORotate(
                new Vector3(0, 0, 360 * cubeData.rotations),
                cubeData.movementDuration,
                RotateMode.LocalAxisAdd)
            .SetEase(Ease.Linear);
    }
    
    void OnMoveCompleteCallback(Color targetColor)
    {
        isMoving = false;
        Camera.main.backgroundColor = targetColor;
        isAtPointB = !isAtPointB;
    } 
}
