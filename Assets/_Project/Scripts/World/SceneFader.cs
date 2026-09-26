using System.Collections;
using UnityEngine;

//fundido estilo hollow knight usando el shader FakeLight del sprite SHADOW hijo del player
//como el sprite ya sigue al player, la oscuridad se cierra sobre el sin calcular nada
public class SceneFader : MonoBehaviour
{
    //el SpriteRenderer del SHADOW, el que tiene el material con el shader
    [SerializeField] private SpriteRenderer shadowRenderer;
    //nombre de referencia de la propiedad en el shader graph, se verifica en el blackboard
    [SerializeField] private string darknessProperty = "_DarknessStrength";
    //oscuridad normal durante el gameplay, el valor que elegiste que te gusta
    [SerializeField, Range(0f, 60f)] private float gameplayDarkness = 4f;
    //oscuridad con la pantalla completamente tapada
    [SerializeField, Range(0f, 60f)] private float closedDarkness = 60f;
    [SerializeField] private float fadeDuration = 0.8f;
    //curva para que el cierre no sea lineal, con ease queda mucho mejor
    [SerializeField] private AnimationCurve fadeCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    //true mientras se abre o se cierra, lo consulta la intro de camara para esperarlo
    public bool IsFading { get; private set; }

    //el sprite renderer ya usa property block, asi que escribimos por ahi y no sobre el material
    private MaterialPropertyBlock block;
    private int darknessId;


    private void Awake()
    {
        darknessId = Shader.PropertyToID(darknessProperty);
        block = new MaterialPropertyBlock();

        if (shadowRenderer == null)
            shadowRenderer = GetComponent<SpriteRenderer>();

        //arrancamos tapados, el fundido de entrada abre hasta el valor de gameplay
        SetDarkness(closedDarkness);
    }

    private IEnumerator Start()
    {
        yield return FadeIn();
    }


    //----------------------------------------------------------------------------------------------

    //lo llama el Start al cargar la escena
    public IEnumerator FadeIn()
    {
        yield return Animate(closedDarkness, gameplayDarkness);
    }

    //lo llama el BossFerryman antes de cargar la escena nueva
    public IEnumerator FadeOut()
    {
        yield return Animate(gameplayDarkness, closedDarkness);
    }

private IEnumerator Animate(float from, float to)
    {
        if (shadowRenderer == null) yield break;

        IsFading = true;
        float timer = 0f;
        while (timer < fadeDuration)
        {
            //arreglo de bug que en el primer frame se comia todo el shader
            timer += Mathf.Min(Time.deltaTime, 0.05f);
            float t = fadeCurve.Evaluate(Mathf.Clamp01(timer / fadeDuration));
            SetDarkness(Mathf.Lerp(from, to, t));
            yield return null;
        }

        SetDarkness(to);
        IsFading = false;
    }

    private void SetDarkness(float value)
    {
        if (shadowRenderer == null) return;

        shadowRenderer.GetPropertyBlock(block);
        block.SetFloat(darknessId, value);
        shadowRenderer.SetPropertyBlock(block);
    }
}