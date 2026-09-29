using UnityEngine;

public class PlayerAudio : MonoBehaviour
{
    [Header("Pasos base")]
    [SerializeField] private AK.Wwise.Event footstepEvent;
    public void PlayFootstep()
    {
        if (footstepEvent != null)
            footstepEvent.Post(gameObject);
    }
}