using UnityEngine;

//debuff de la embestida del boss (+15% de danio recibido por 30s)
//como PlayerHealth no tiene multiplicador de danio y no quiero tocarlo,
//este componente se agrega solo al player al ser embestido, y los ataques
//del boss lo consultan ANTES de llamar a TakeDamage: el danio llega ya amplificado
public class DebuffFragilidad : MonoBehaviour
{
    private float multiplicador = 1f;
    private float timer;

    public bool Activo => timer > 0f;

    private void Update()
    {
        if (timer > 0f)
        {
            timer -= Time.deltaTime;
            if (timer <= 0f)
                multiplicador = 1f;
        }
    }

    public void Aplicar(float porcentaje, float duracion)
    {
        //si te embisten de nuevo con el debuff activo, se renueva la duracion
        multiplicador = 1f + porcentaje;
        timer = duracion;
        //TODO: icono del debuff en la ui
    }

    //helpers estaticos para que cada ataque del boss no repita el mismo codigo

    public static void AplicarA(GameObject objetivo, float porcentaje, float duracion)
    {
        DebuffFragilidad frag = objetivo.GetComponent<DebuffFragilidad>();
        if (frag == null)
            frag = objetivo.AddComponent<DebuffFragilidad>(); //se agrega solo la primera vez
        frag.Aplicar(porcentaje, duracion);
    }

    public static int Amplificar(GameObject objetivo, int danioBase)
    {
        DebuffFragilidad frag = objetivo.GetComponent<DebuffFragilidad>();
        if (frag != null && frag.Activo)
            return Mathf.RoundToInt(danioBase * frag.multiplicador);
        return danioBase;
    }
}