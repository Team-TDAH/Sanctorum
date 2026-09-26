using UnityEngine;
 
//para que rote "fluidamente" la animacion cuando se da vuelvta
public class MiharuFlip : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;
    //no croe tocarlo mas pero x las dudas
    [SerializeField] private float turnSpeed = 12f;
    private float targetYRotation;
 
 
    private void Start()
    {
        //x las dudas, para que no se rompa en caso de no ehaberlo asignado
        if (playerController == null)
            playerController = GetComponentInParent<PlayerController>();
 
        targetYRotation = playerController.LastFacingDirection >= 0f ? 0f : 180f;
        transform.localRotation = Quaternion.Euler(0f, targetYRotation, 0f);
    }
 
    private void Update()
    {
        if (playerController == null) return;
 
        //derecha mira a 0 grados, izquierda a 180
        targetYRotation = playerController.LastFacingDirection >= 0f ? 0f : 180f;
        //para que gire "suavemente
        Quaternion target = Quaternion.Euler(0f, targetYRotation, 0f);
        transform.localRotation = Quaternion.Lerp(transform.localRotation, target, turnSpeed * Time.deltaTime);
    }
}
