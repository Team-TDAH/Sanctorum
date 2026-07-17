using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//empieza neutral, luego del dialogo se mueve al centro y empieza a ciclar ataques con una pausita entre medio para hacerle daño comodo(por ahora solo fase 1)
public class Boss1Controller : MonoBehaviour
{
    //maquina de estaods compleja(para mi) pero bonita
    private enum BossState { Neutral, MovingToCenter, Idle, Attacking, MidFightDialogue, Dead }
    [SerializeField] private DialogueChannel dialogueChannel;
    //el objeto a donde ira el boss, aproveche el mismo objeto que esta traquando la cinemachine para que la vista sea de la sala completa
    [SerializeField] private Transform centerPoint;
    [SerializeField] private GameObject dialogueObject;
    [SerializeField] private float moveToCenterSpeed = 40f;//subi mucho el valor porque aveces podias "empujarlo" y se tardaba bastante, ahora si lo empujas igual llega rapido
    //pausa entre ataques, lo estaremos cambiando bastante, no quiero que el personaje se quede "bobo" luego de un ataque, quizas agregamos una animacion de preparar el hechizo entre medio
    [SerializeField] private float pauseAttacks = 2.5f;
    [Header("Faroles del Umbral config")]
    //tengo que asignar lampara x lampara, estas deben tener el componente BossLamp que adminstra los proyectiles de ellas
    [SerializeField] private List<BossLamp> lamps = new();
    //cambie de idea, antes era al azar entre 3-5 lamparas atacando, ahora es algo progresivo(me retaron, volvera a ser entre 3-5 lamparas)
    [SerializeField] private int minLampsActive = 3;
    [SerializeField] private int maxLampsActive = 5;
    //termine dejando que dure tanto tiempo y no que dispare tanta cantidad de proyectiles
    [SerializeField] private float lampsDuration = 15f;
    //probabilidad de que salga este ataque, no es exactamente 40% pero da idea
    private float lampsProb = 40f;
    //----------
    [Header("orbes nocturnos config")]
    [SerializeField] private GameObject nightOrbPrefab;
    //cuantos orbes se lanzan en total(cambiarlo)
    [SerializeField] private int orbCount = 20;
    //tiempo entre un orbe y orbe, recordar de ajustarlo, se nota mucho
    [SerializeField] private float orbInterval = 0.4f;
    [SerializeField] private float orbSpeed = 7f;
    [SerializeField] private float orbLifetime = 6f;
    [SerializeField] private int orbDamage = 30;
    private float orbsProb = 40f;
    //------
    [Header("embestida espectral config")]
    [SerializeField] private GameObject spectralPrefab;
    [SerializeField] private float spectralSpeed = 40f; //se movia muy lento, minimo 40 para q sea desafiante
    [SerializeField] private float spectralLifetime = 5f;
    //peso bajo para que salga menos seguido que los otros
    private float spectralProb = 20f;
    //----------------
    //runtime necesario para que respete siempre los estaods
    private BossState state = BossState.Neutral;
    private BossHealth bossHealth;
    private Transform playerTransform;
    //para no repetir el mismo ataque dos veces seguidas(pasa mucho sino)
    private int lastAttackIndex = -1;
    //guarda la posi de donde arranca el boss para luego volver ahi
    private Vector3 startPosition;
    [SerializeField] private float returnSpeed = 40f; //tendre q cambiarlo, no se si sera igual que la ida
    //--
    //para desbloquear el dash cuando llegue a mitad d evida el boss
    [Header("dialogo y habilidad q desbloquea")]
    [SerializeField] private DialogueSO secondPhaseDialogue;
    [SerializeField] private BoolVariable abilityToUnlock;
    private bool waitDialogueClose;

    private void Awake()
    {
        bossHealth = GetComponent<BossHealth>();
        startPosition = transform.position;
    }

    private void OnEnable()
    {
        //cuando termina el dialogo con el jefe, arranca la pelea
        if (dialogueChannel != null)
            dialogueChannel.OnDialogueClosed += HandleDialogueClosed;
    }
    private void OnDisable()
    {
        if (dialogueChannel != null)
            dialogueChannel.OnDialogueClosed -= HandleDialogueClosed;
    }
    private void Start()
    {
        var player = FindAnyObjectByType<PlayerController>();
        if (player != null) playerTransform = player.transform;
    }
    private void Update()
    {
        if (bossHealth != null && bossHealth.IsDead && state != BossState.Dead)
        {
            state = BossState.Dead;
            StopAllCoroutines();
            DeactivateAllLamps();
            StartCoroutine(DeathSequence());
        }
    }
    //----------------------------------------------------------------------------------------------
    //inicio de la pelea

    private void HandleDialogueClosed()
    {
        //solo arranca si estaba en neutro
        if (state == BossState.Neutral)
        {
            StartCoroutine(StartFightSequence());
            return;
        }
        //cuando llega  amitad de vida empieza el dialogo y no se vuelve a repetir
        if (waitDialogueClose)
        {
            waitDialogueClose = false;
        }
    }
    private IEnumerator StartFightSequence()
    {
        state = BossState.MovingToCenter;
        //para que no se le pueda hablar mientras esta en transicion, bug encontrado a las 03:24 con katze
        //no me gusto para nada la solucion, pero funciona, siento que puse en el lugar incorrecto el NPCDielogue, pero ahora no puedo pensar
        var npcDialogue = GetComponent<NPCDialogue>();
        if (npcDialogue != null) npcDialogue.enabled = false;
        //la idea era q apagara todo el objeto del dialogo, pero tengo ese objeto en el mismo lugar que boss1controller, asi que problemas
        if (dialogueObject != null) dialogueObject.SetActive(false);


        //se chocaba con objetos en la transicion, asi que mejor quitar colisiones mientras se transiciona(era la idea pero al final con dividir collisiones y poner trigger donde)
        
        //se mueve rapidamente al centro de la pantalla(aveces se mueve x el ataque de embestida)
        while (centerPoint != null &&
               Vector2.Distance(transform.position, centerPoint.position) > 0.1f)
        {
            transform.position = Vector2.MoveTowards(
            transform.position,
            centerPoint.position,
            moveToCenterSpeed * Time.deltaTime);
            yield return null;
        }
        //una vez llega, le damos a pelea
        bossHealth?.StartFight();
        state = BossState.Idle;
        StartCoroutine(AttackLoop());
    }
    //------
    //ciclo de ataques
    private IEnumerator AttackLoop()
    {
        while (state != BossState.Dead)
        {
            //aca aplicamos la pausa luego de cada ataque
            state = BossState.Idle;
            yield return new WaitForSeconds(pauseAttacks);
            if (state == BossState.Dead) yield break;

            //chequea entre ataques
            bool alreadyUnlocked = abilityToUnlock != null && abilityToUnlock.Value;
            if (!alreadyUnlocked && bossHealth != null && !bossHealth.IsDead && bossHealth.HealthPercent <= 0.5f)
            {
                yield return StartCoroutine(MidFightDialogueSequence());
                if (state == BossState.Dead) yield break;
            }

            state = BossState.Attacking;
            //se elija al azar con esa funcion entre los ataques, los prob influyen
            int attack = ChooseAttack();
            lastAttackIndex = attack;
            //depende de cual salga sera el ataque q haga, y luego repite, facil de entender
            switch (attack)
            {
                case 0:
                    yield return StartCoroutine(LampAttack());
                    break;
                case 1:
                    yield return StartCoroutine(NightOrbsAttack());
                    break;
                case 2:
                    yield return StartCoroutine(SpectralChargeAttack());
                    break;
            }
        }
    }

    //el dialogo, el boss se queda quieto mientras, quizas agregar algun efecto de luz que de a entender el tiempo frenado
    private IEnumerator MidFightDialogueSequence()
    {
        state = BossState.MidFightDialogue;
        waitDialogueClose = true;

        if (dialogueChannel != null && secondPhaseDialogue != null)
            dialogueChannel.RequestDialogue(secondPhaseDialogue);

        while (waitDialogueClose)
            yield return null;

        //para q quede para siempre desbloqueada
        if (abilityToUnlock != null) abilityToUnlock.Value = true;
        SaveLoadManagerJson.Instance?.SaveGame();

        state = BossState.Idle;
    }
    //calculo para elegir el ataque, teniendo en cuenta q no repita ataques, luego si agrego mas ataques tendria mas sentido los prob
    private int ChooseAttack()
    {
        float[] weights = { lampsProb, orbsProb, spectralProb };
        //el ataque que acaba de hacer pasa a tener ej lampProb = 0f; para que no vuelva a salir
        if (lastAttackIndex >= 0 && lastAttackIndex < weights.Length)
            weights[lastAttackIndex] = 0f;

        float total = 0f;
        foreach (float w in weights) total += w;
        //caso que no ocurrira, pero si todos quedan con 0 en prob, elegira al azar
        if (total <= 0f) return Random.Range(0, weights.Length);

        float roll = Random.Range(0f, total);
        float acc = 0f;
        for (int i = 0; i < weights.Length; i++)
        {
            acc += weights[i];
            if (roll <= acc) return i;
        }
        return 0;
    }
    private IEnumerator LampAttack()
    {
        //calculo simple para determinear cuantas lamparas van, el contador lampAttackCount va agrandandose y agregando mas lamparas hasta llegar al max (retrocedi, me retaron repito)
        int count = Mathf.Min(minLampsActive, maxLampsActive + 1);
        count = Mathf.Min(count, lamps.Count);

        //para que elija verdaderamente al azar entre las lamparas
        List<BossLamp> shuffled = new List<BossLamp>(lamps);
        for (int i = 0; i < shuffled.Count; i++)
        {
            int j = Random.Range(i, shuffled.Count);
            (shuffled[i], shuffled[j]) = (shuffled[j], shuffled[i]);
        }
        //se activa uno x uno de la lista creada
        List<BossLamp> active = new List<BossLamp>();
        for (int i = 0; i < count; i++)
        {
            shuffled[i].Activate(playerTransform);
            active.Add(shuffled[i]);
        }
        //mientras disparan el jefe quiero (a esperar confirmacion de que el jefe se quede quieto)
        yield return new WaitForSeconds(lampsDuration);
        //vuelven a su color "normal" (calido)
        foreach (var lamp in active)
            lamp.Deactivate();
    }
    private void DeactivateAllLamps()
    {
        foreach (var lamp in lamps)
            if (lamp != null) lamp.Deactivate();
    }
    private IEnumerator NightOrbsAttack()
    {
        if (nightOrbPrefab == null) yield break;

        for (int i = 0; i < orbCount; i++)
        {
            if (state == BossState.Dead) yield break;
            if (playerTransform == null) yield break;
            //que apunten a donde el player, luego vere si quiero "errar" un poco, porque es muy complicado
            Vector2 dir = (playerTransform.position - transform.position).normalized;
            GameObject orb = Instantiate(nightOrbPrefab, transform.position, Quaternion.identity);
            //especificare q hace en el script del proyectil
            orb.GetComponent<ZigzagProjectile>()?.Initialize(dir, orbSpeed, orbLifetime, orbDamage);
            yield return new WaitForSeconds(orbInterval);
        }
    }
    private IEnumerator SpectralChargeAttack()
    {
        if (spectralPrefab == null || playerTransform == null) yield break;
        //sale disparado a la pos del player y sigue de largo(no vuelve como planeamos), mas simple de lo q pense
        Vector2 dir = (playerTransform.position - transform.position).normalized;
        GameObject spectral = Instantiate(spectralPrefab, transform.position, Quaternion.identity);
        spectral.GetComponent<SpectralCharge>()?.Initialize(dir, spectralSpeed, spectralLifetime);
        yield return new WaitForSeconds(spectralLifetime * 0.5f);
    }
    //----
    //secuencia de muerte en el q vuelve a donde arranco
    private IEnumerator DeathSequence()
    {
        //sin colisiones durante el regreso, igual que en la ida al centro
        var col = GetComponentInChildren<Collider2D>();
        if (col != null) col.enabled = false;

        //!!! aca iria animacion de derrota (no lo veo mucho en este jefe pero bueno)

        while (Vector2.Distance(transform.position, startPosition) > 0.1f)
        {
            transform.position = Vector2.MoveTowards(
                transform.position,
                startPosition,
                returnSpeed * Time.deltaTime);
            yield return null;
        }
        bossHealth.CompleteDeath();
    }
}