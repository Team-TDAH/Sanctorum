using UnityEngine;
using Unity.Cinemachine;

//para que cinemachine detecte cuando "rota" el player
public class CameraFacingOffset : MonoBehaviour
{
    //cuanto se adelanta la camara hacia donde mira el player (ajustar pora ca)
    [SerializeField] private float horizontalOffset = 2f;
    //velocidad del paneo al cambiar de lado (creo q no hace falta tocarlo)
    [SerializeField] private float smoothSpeed = 3f;
    private CinemachinePositionComposer composer;
    private PlayerController player;
    private void Start()
    {
        composer = GetComponent<CinemachinePositionComposer>();
        player = FindAnyObjectByType<PlayerController>();
    }
    private void Update()
    {
        if (composer == null || player == null) return;

        float targetX = horizontalOffset * player.LastFacingDirection;
        Vector3 offset = composer.TargetOffset;
        offset.x = Mathf.Lerp(offset.x, targetX, smoothSpeed * Time.deltaTime);
        composer.TargetOffset = offset;
    }
}