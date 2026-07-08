using UnityEngine;
using Unity.Cinemachine;

//es para no renegar al cambiar de escena, que se autoasigne el player al inciar una nueva
public class CameraTargetAssigner : MonoBehaviour
{
    private void Start()
    {
        var cam = GetComponent<CinemachineCamera>();
        var player = FindAnyObjectByType<PlayerController>();
        if (cam != null && player != null)
            cam.Follow = player.transform;
    }
}