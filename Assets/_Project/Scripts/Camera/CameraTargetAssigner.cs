using UnityEngine;
using Unity.Cinemachine;

//es para no renegar al cambiar de escena, que se autoasigne el player al inciar una nueva
public class CameraTargetAssigner : MonoBehaviour
{   
    //lo cambie porque al hacer la pelea del jefe quiero que la camara traquee solamente al objeto vacio, no al player
    private void Start()
    {
        var cam = GetComponent<CinemachineCamera>();
        //funciona faicl, si tiene algo ya puesto, directamente no hace nada, sino asigna al player (diria que esta mejor ahora que antes)
        if (cam.Follow != null) return;
        
        var player = FindAnyObjectByType<PlayerController>();
        if (player != null)
            cam.Follow = player.transform;
    }
}