using System.Collections;
using UnityEngine;
using Unity.Cinemachine;
//para la transicion de camara "normal" con la q venia el player a una camara puesta en el centro de la sala, asi no confunde tanto las dimensiones
public class CameraIntroTransition : MonoBehaviour
{
    [SerializeField] private CinemachineCamera playerCamera;
    [SerializeField] private CinemachineCamera roomCamera;
    [SerializeField] private CinemachineBrain brain;
    [SerializeField] private PlayerController playerController;
    //capaz alla q subirlo mas, luego elegira furia
    [SerializeField] private float holdOnPlayer = 0.6f;
    //la prioridad mas alta es la que manda, asi hace el cambio solo
    private const int PRIORITY_ACTIVE = 20;
    private const int PRIORITY_IDLE = 0;
    //para q haga la transicion si llegamos viajando y no agregar mas cosas
    private bool arrivedByTransition;
    private void Awake()
    {
        //todo x las dudas me olvide de asignarlos, si fuese ordenado no haria falta
        if (brain == null) brain = FindAnyObjectByType<CinemachineBrain>();
        if (playerController == null) playerController = FindAnyObjectByType<PlayerController>();

        arrivedByTransition = !string.IsNullOrEmpty(BossFerryman.PendingConnectionId);
        //en cuyo caso de q pongamos un checkpoint dentro
        if (!arrivedByTransition) return;
        
        if (playerCamera != null) playerCamera.Priority = PRIORITY_ACTIVE;
        if (roomCamera != null) roomCamera.Priority = PRIORITY_IDLE;
    }
    private IEnumerator Start()
    {
        if (!arrivedByTransition) yield break;
        //q no se pueda mover hasta terminar la transicion
        if (playerController != null) playerController.InputEnabled = false;

        yield return new WaitForSeconds(holdOnPlayer);
        //interpola la posicion solo
        if (playerCamera != null) playerCamera.Priority = PRIORITY_IDLE;
        if (roomCamera != null) roomCamera.Priority = PRIORITY_ACTIVE;
        //muchas veces tuve q agregar un frame para q lo tome bien sin pisar
        yield return null;
        while (brain != null && brain.IsBlending)
            yield return null;

        if (playerController != null) playerController.InputEnabled = true;
    }
}