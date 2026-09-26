using System.Collections;
using UnityEngine;
//congela el game unos milisegundos para dar sensacion de impaacto
public class HitStopManager : MonoBehaviour
{
    [SerializeField] private HitStopChannel hitStopChannel;
    //cduracion del congelamiento
    [SerializeField] private float defaultDuration = 0.05f;
    //0 congela del todo, 0.5 slowmotion
    [SerializeField, Range(0f, 0.5f)] private float frozenTimeScale = 0f;
    private Coroutine currentFreeze;
    private void OnEnable()
    {
        if (hitStopChannel != null)
            hitStopChannel.OnHitStopRequested += HandleRequest;
    }
    private void OnDisable()
    {
        if (hitStopChannel != null)
            hitStopChannel.OnHitStopRequested -= HandleRequest;
    }
    private void HandleRequest(float duration)
    {
        Freeze(duration > 0f ? duration : defaultDuration);
    }
    private void Freeze(float duration)
    {
        if (Time.timeScale == 0f) return;

        //para q no se acumulen hitstops
        if (currentFreeze != null)
            StopCoroutine(currentFreeze);

        currentFreeze = StartCoroutine(FreezeRoutine(duration));
    }
    private IEnumerator FreezeRoutine(float duration)
    {
        Time.timeScale = frozenTimeScale;
        yield return new WaitForSecondsRealtime(duration);

        //solo restauramos si nadie pauso mientras tanto
        if (Time.timeScale == frozenTimeScale)
            Time.timeScale = 1f;

        currentFreeze = null;
    }
}