using UnityEngine;

//variable booleana que se puede usar en cualquier script, sin necesidad de tener referencias del player (lo usare bastante para desbloquear habilidades luego de cada boss)
[CreateAssetMenu(fileName = "BoolVariable", menuName = "Variables/Bool Variable")]
public class BoolVariable : ScriptableObject
{
    [SerializeField] private bool initialValue;
    //valor runtime separado del initial para poder resetearlo entre escenas
    [System.NonSerialized] private bool runtimeValue;
    [System.NonSerialized] private bool initialized;

    public bool Value
    {
        get
        {
            //inicializamos con el valor del inspector la primera vez que se lee, superimportante, luego vere si utilizo prefabs que creo que seran obligatorios
            if (!initialized)
            {
                runtimeValue = initialValue;
                initialized = true;
            }
            return runtimeValue;
        }
        set
        {
            if (!initialized) initialized = true;
            runtimeValue = value;
        }
    }

    //util para resetear al valor inicial entre escenas (no creo utilizar, la idea es otra)
    public void Reset()
    {
        initialized = false;
    }
}