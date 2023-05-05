using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Rendering;
using static UnityEngine.Rendering.DebugUI;

public class PowerBullets : NetworkBehaviour
{
    /**
     *################################ GENERIC VARIABLES ################################
     */
    PlayerWeapon playerWeapon;
    PlayerControlls playerControlls;
    public List<PowerBulletSO> powerBullets;
    /**
     *################################ EXPANDBULLET VARIABLES ################################
     */
    [SerializeField] private int pointsCount;
    [SerializeField] private int MaxRadious;
    [SerializeField] private int Speed;
    [SerializeField] private float ForceExpansion;
    [SerializeField] private GameObject ExpandWave;
    private GameObject InstanceExpandWave;
    private LineRenderer lineRendererExpand;
    /**
     *################################ FLAMEBULLET VARIABLES ################################
     */
    [SerializeField] private GameObject flameParticles;
    /**
     *################################ EXPLOSIONBULLET VARIABLES ################################
     */
    [SerializeField] private GameObject ExplosionParticles;
    private GameObject InstanceExplosion;
    [SerializeField] private GameObject ExplosionWave;
    private GameObject InstanceExplosionWave;
    private bool Exploting;
    private LineRenderer lineRendererExplosion;
    /**
     *################################ TIME-SLOWBULLET VARIABLES ################################
     */
    private bool timeSlow;
    private bool Expanding;

    /**
     *################################ NETWORK VARIABLES ################################
     */
    public NetworkList<PowerBulletNetworkInfo> powerBulletsValues;
    public void Awake()
    {
        powerBulletsValues = new NetworkList<PowerBulletNetworkInfo>();
    }
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (!IsOwner) return;
        DefaultValues();
        
    }
    private void DefaultValues()
    {
        //  Generic Variables
        playerWeapon = GetComponent<PlayerWeapon>();
        playerControlls = GetComponent<PlayerControlls>();

        if ((!IsServer && IsOwner) || (IsServer && !IsOwner))
            powerBullets = (Resources.LoadAll<PowerBulletSO>("Player/Client/PowerBullets/")).ToList();  
        else
            powerBullets = (Resources.LoadAll<PowerBulletSO>("Player/Host/PowerBullets/")).ToList();
        
        if (IsServer)
        {
            Debug.Log("aaaaaa");
            int a = 0;
            foreach (PowerBulletSO powerB in powerBullets)
            {
                powerBulletsValues.Add(new PowerBulletNetworkInfo(powerB.id, powerB.currentInvestmentValue));
                Debug.Log(powerBulletsValues[a].id + " " + powerBulletsValues[a].InvestValue);
                a++;
            }
            
        }
        //  FlameBullet Variables
        if ((!IsServer && IsOwner) || (IsServer && !IsOwner))
            flameParticles = Resources.Load<GameObject>("ParticlesSystem/Client/Fire");
        else
            flameParticles = Resources.Load<GameObject>("ParticlesSystem/Server/Fire");

        //  ExpandBullet Variables
        if ((!IsServer && IsOwner) || (IsServer && !IsOwner)) 
            ExpandWave = Resources.Load<GameObject>("ParticlesSystem/Client/ExpansionWave");
        
        else
            ExpandWave = Resources.Load<GameObject>("ParticlesSystem/Server/ExpansionWave");

        InstanceExpandWave = Instantiate(ExpandWave);
        InstanceExpandWave.SetActive(false);



        //  ExplosionBullet Variables
        Exploting = false;
        if ((!IsServer && IsOwner) || (IsServer && !IsOwner))
        {
            ExplosionParticles = Resources.Load<GameObject>("ParticlesSystem/Client/Explosion");
            ExplosionWave = Resources.Load<GameObject>("ParticlesSystem/Client/ExplosionWave");
        }
        else
        {
            ExplosionParticles = Resources.Load<GameObject>("ParticlesSystem/Server/Explosion");
            ExplosionWave = Resources.Load<GameObject>("ParticlesSystem/Server/ExplosionWave");
        }
        InstanceExplosion = Instantiate(ExplosionParticles);
        InstanceExplosionWave = Instantiate(ExplosionWave);
        InstanceExplosion.SetActive(false);
        InstanceExplosionWave.SetActive(false);

        //  TimeSlow Bullet
        timeSlow = false;

        //  LineRenderers
        
        //Expand
        lineRendererExpand = InstanceExpandWave.GetComponent<LineRenderer>();
        lineRendererExpand.positionCount = pointsCount + 1;
        lineRendererExpand.enabled = true;

        //Explosion
        lineRendererExplosion = InstanceExplosionWave.GetComponent<LineRenderer>();
        lineRendererExplosion.positionCount = pointsCount + 1;
        lineRendererExplosion.enabled = true;
    }

    /**
     * ################################ PRINCIPAL FUNCTION ################################
     */
    public void execute(RaycastHit hit, bool byPowerBullet)
    {
        foreach(PowerBulletNetworkInfo powerBulletInfo in powerBulletsValues) {
            if (powerBulletInfo.InvestValue > 0)
            {
                switch(powerBulletInfo.id)
                {
                    case PowerBulletID.STUNE:
                        StuneBullet(hit, powerBulletInfo.InvestValue);
                        break;
                    case PowerBulletID.EXPAND:
                        ExpandBullet(hit);
                        ExpandBulletClientRpc(hit.transform.position);
                        break;
                    case PowerBulletID.DOUBLE_FORCE:
                        DoubleForceBullet(hit, powerBulletInfo.InvestValue);
                        break;
                    case PowerBulletID.PIERCING:
                        PiercingBullet(hit, powerBulletInfo.InvestValue);
                        break;
                    case PowerBulletID.MULTIPLIER:
                        MultiplierBullet(hit, powerBulletInfo.InvestValue);
                        break;
                    case PowerBulletID.BOUNCING:
                        BouncingBullet(hit, powerBulletInfo.InvestValue);
                        break;
                    case PowerBulletID.BOUCING_SURFACE:
                        BouncingSurfaceBullet(hit, powerBulletInfo.InvestValue, byPowerBullet);
                        break;
                    case PowerBulletID.FLAME:
                        FlameBullet(hit, powerBulletInfo.InvestValue);
                        break;
                    case PowerBulletID.HEALTH_STEALTH:
                        HealthStealthBullet(hit, powerBulletInfo.InvestValue);
                        break;
                    case PowerBulletID.SHIELD_STEALTH:
                        ShieldStealthBullet(hit, powerBulletInfo.InvestValue);
                        break;
                    case PowerBulletID.EXPLOSIVE:
                        ExplosiveBullet(hit, powerBulletInfo.InvestValue);   
                        break;
                    case PowerBulletID.CRITICAL:
                        CriticalBullet(hit, powerBulletInfo.InvestValue);    
                        break;
                    case PowerBulletID.TERRIFIER:
                        TerrifierBullet(hit, powerBulletInfo.InvestValue);
                        break;
                    case PowerBulletID.TIMESLOW:
                        TimeSlowBullet(hit, powerBulletInfo.InvestValue);
                        break;
                    case PowerBulletID.CRAZYFIER:
                        CrazyBullet(hit, powerBulletInfo.InvestValue);   
                        break;
                }
            }
        }
    }
    public void LoadPointsPowers()
    {
        foreach(PowerBulletSO powerBulletSO in powerBullets)
        {
            if(powerBulletSO.Points>= (powerBulletSO.ScaleInvestment*4))
            {
                powerBulletSO.currentInvestmentValue = powerBulletSO.InvestmentValue * 3;
            }else if(powerBulletSO.Points >= (powerBulletSO.ScaleInvestment * 2))
            {
                powerBulletSO.currentInvestmentValue = powerBulletSO.InvestmentValue * 2;
            }
            else if(powerBulletSO.Points >= powerBulletSO.ScaleInvestment)
            {
                powerBulletSO.currentInvestmentValue = powerBulletSO.InvestmentValue;
            }
            else
            {
                powerBulletSO.currentInvestmentValue = 0;
            }
            UpdateValuePowerBulletServerRpc((int)powerBulletSO.id, powerBulletSO.currentInvestmentValue);
        }
    }
    [ServerRpc]
    public void UpdateValuePowerBulletServerRpc(int id, float value)
    {
        Debug.Log("START");
        for(int a = 0; a < powerBulletsValues.Count; a++)
        {
            if ((int)powerBulletsValues[a].id == id)
            {
                powerBulletsValues[a] = new PowerBulletNetworkInfo((PowerBulletID)id, value);
            }
            Debug.Log((int)powerBulletsValues[a].id+" "+ powerBulletsValues[a].InvestValue);
        }
        Debug.Log("");
    }
    /**
     * ################################ STUNE BULLET FUNCTIONS ################################
     */
    private void StuneBullet(RaycastHit hit, float powerBulletData)
    {
        Debug.Log("################################# STUNE BULLET #################################");
        float rand = Random.Range(0f, 1f);
        if(rand<= powerBulletData)
        {
            EnemyBehaviour eb = hit.transform.gameObject.GetComponent<EnemyBehaviour>();
            if (eb != null)
            {
                eb.ChangeState(StateOfEnemy.STUNED);
            }
        }
    }
    /**
     * ################################ EXPAND BULLET FUNCTIONS ################################
     */
    [ClientRpc]
    private void ExpandBulletClientRpc(Vector3 hitPosition)
    {
        if (!Expanding) { 
            Expanding = true;
            StartCoroutine(ExpansionClient(hitPosition));
        }
    }
    private void ExpandBullet(RaycastHit hit) { Debug.Log("################################# EXPAND BULLET #################################"); StartCoroutine(Expansion(hit)); }
    private void DrawTheLine(float currentRadious)
    {
        float angleBetweenPoints = 360f / pointsCount;
        for(int a = 0; a <= pointsCount; a++) {
            float angle = a * angleBetweenPoints * Mathf.Deg2Rad;
            Vector3 direction = new Vector3(Mathf.Sin(angle), 0f, Mathf.Cos(angle));
            Vector3 position = direction * currentRadious;  
            lineRendererExpand.SetPosition(a, position);  
        }
    }
    private void ExpansionWave(Vector3 explosionOrigin, float currentRadious)    
    {
        Collider[] colliders = Physics.OverlapSphere(explosionOrigin, currentRadious);
        for(int i = 0; i < colliders.Length; i++)
        {
            Rigidbody rb = colliders[i].GetComponent<Rigidbody>();
            if (!rb) continue; 
            Vector3 direction = (colliders[i].transform.position - explosionOrigin).normalized;
            rb.AddForce(direction * ForceExpansion, ForceMode.Impulse);
        } 
    }
    /**
     * ################################ DOUBLE FORCE BULLET FUNCTIONS ################################
     */
    private void DoubleForceBullet(RaycastHit hit, float powerBulletData)
    {
        Debug.Log("################################# DOUBLE FORCE BULLET #################################");
        float val = 0.5f * powerBulletData;
        if (hit.transform.tag == "Enemy")
        {
            Rigidbody rb = hit.transform.gameObject.GetComponent<Rigidbody>();
            if (!rb) return;
            rb.AddForceAtPosition(-hit.normal * playerWeapon.CurrentConfiguration.CurrentForce * val, hit.point);
        }
    }
    /**
     * ################################ PIERCING BULLET FUNCTIONS ################################
     */
    private void PiercingBullet(RaycastHit hit, float powerBulletData)
    {
        Debug.Log("################################# PIERCING BULLET #################################");
        float rand = Random.Range(0f, 1f);
        if (rand <= powerBulletData)
        {
            if (hit.transform.tag == "Enemy")
            {
                Vector3 v = hit.transform.GetChild(2).transform.position;
                playerWeapon.RayCastToServerRpc(v, -hit.normal, true);
                Debug.DrawLine(v, v + (-hit.normal * 10), Color.green, 3f);
            }
        }
    }
    /**
     * ################################ MULTIPLIER BULLET FUNCTIONS ################################
     */
    private void MultiplierBullet(RaycastHit hit, float powerBulletData)
    {
        Debug.Log("################################# MULTIPLIER BULLET #################################");
        float rand = Random.Range(0f, 1f);
        if (rand <= powerBulletData)
        {
            if (hit.transform.tag == "Enemy")
            {
                Vector3 dir = -hit.transform.forward + hit.transform.right * 0.5f;
                Vector3 dir2 = -hit.transform.forward + hit.transform.right * -0.5f;
                Vector3 piercingPoint = hit.transform.GetComponent<EnemyBehaviour>().PiercingPoint.position;
                playerWeapon.RayCastToServerRpc(piercingPoint, dir.normalized, true);
                playerWeapon.RayCastToServerRpc(piercingPoint, dir2.normalized, true);
            }
        }
    }
    /**
     * ################################ BOUNCING BULLET FUNCTIONS ################################
     */
    private void BouncingBullet(RaycastHit hit, float powerBulletData)
    {
        Debug.Log("################################# BOUNCING BULLET #################################");
        float rand = Random.Range(0f, 1f);
        if (rand <= powerBulletData)
        {
            if (hit.transform.tag == "Enemy")
            {
                Vector3 direction = (hit.transform.position - transform.position).normalized;
                Vector3 reflection = Vector3.Reflect(direction, hit.normal);
                playerWeapon.RayCastToServerRpc(hit.point, reflection, true);
                Debug.DrawLine(hit.point, hit.point + (reflection * 10), Color.green, 3f);
            }
        }
    }
    /**
     * ################################ BOUNCING SURFACE BULLET FUNCTIONS ################################
     */
    private void BouncingSurfaceBullet(RaycastHit hit, float powerBulletData, bool byPowerBullet)
    {
        Debug.Log("################################# BOUNCING SURFACE BULLET #################################");
        float rand = Random.Range(0f, 1f);
        if (rand <= powerBulletData && !byPowerBullet)
        {
            if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Obstruction"))
            {
                Vector3 direction = (hit.point - transform.position).normalized;
                Vector3 reflection = Vector3.Reflect(direction, hit.normal);
                Debug.DrawLine(hit.point, hit.point + (reflection * 5), Color.red, 3f);
                playerWeapon.RayCastToServerRpc(hit.point, reflection, true);
            }
        }
    }
    /**
     * ################################ FLAME BULLET FUNCTIONS ################################
     */
    private void FlameBullet(RaycastHit hit, float powerBulletData)
    {
        Debug.Log("################################# FLAME BULLET #################################");
        float rand = Random.Range(0f, 1f);
        if (rand <= powerBulletData)
        {
            if (hit.transform.tag == "Enemy")
            {
                EnemyBehaviour eb = hit.transform.gameObject.GetComponent<EnemyBehaviour>();
                if (eb != null)
                {
                    StartCoroutine(DamageFlame(eb, hit.transform.gameObject)); 
                }
            }
        }
    }
    /**
     * ################################ HEALTH-STEALTH BULLET FUNCTIONS ################################ 
     */
    private void HealthStealthBullet(RaycastHit hit, float powerBulletData)
    {
        Debug.Log("################################# HEALTH-STEALTH BULLET #################################");
        EnemyBehaviour eb = hit.transform.gameObject.GetComponent<EnemyBehaviour>();    
        if(eb != null)
        { 
            Debug.Log("Health Antes: " + playerControlls.OwnInfo.playersCurrentHealth);
            playerControlls.OwnInfo.playersCurrentHealth += playerWeapon.CurrentConfiguration.CurrentDamageWeapon * powerBulletData;
            if (playerControlls.OwnInfo.playersMaxHealth < playerControlls.OwnInfo.playersCurrentHealth) playerControlls.OwnInfo.playersCurrentHealth = playerControlls.OwnInfo.playersMaxHealth;
            Debug.Log("Health Despues: " + playerControlls.OwnInfo.playersCurrentHealth);
        }
    }
    /**
     * ################################ SHIELD-STEALTH BULLET FUNCTIONS ################################ 
     */
    private void ShieldStealthBullet(RaycastHit hit, float powerBulletData)
    {
        Debug.Log("################################# SHIELD-STEALTH BULLET #################################");
        EnemyBehaviour eb = hit.transform.gameObject.GetComponent<EnemyBehaviour>();
        if (eb != null)
        {
            Debug.Log("Shield Antes: " + playerControlls.OwnInfo.playersCurrentShield);
            playerControlls.OwnInfo.playersCurrentShield += playerWeapon.CurrentConfiguration.CurrentDamageWeapon * powerBulletData;
            if (playerControlls.OwnInfo.playersMaxShield < playerControlls.OwnInfo.playersCurrentShield) playerControlls.OwnInfo.playersCurrentShield = playerControlls.OwnInfo.playersMaxShield;
            Debug.Log("Shield Despues: " + playerControlls.OwnInfo.playersCurrentShield);
        }
    }
    /**
    * ################################ EXPLOSIVE BULLET FUNCTIONS ################################
    */
    [ClientRpc]
    private void ExplosiveBulletClientRpc(Vector3 ExplosionPoint)
    {
        if (!IsOwner) return;
         Exploting = true;
         StartCoroutine(ExplosionClient(ExplosionPoint));
         StartCoroutine(ExplosionParticlesCoroutine(ExplosionPoint));
    }
    private void ExplosiveBullet(RaycastHit hit, float powerBulletData)
    {
        Debug.Log("################################# EXPLOSIVE BULLET #################################");
        float rand = Random.Range(0f, 1f);
        if (rand <= powerBulletData)
        {
            if (!Exploting)
            {
                Exploting = true;
                StartCoroutine(Explosion(hit));
                StartCoroutine(ExplosionParticlesCoroutine(hit.point));
                ExplosiveBulletClientRpc(hit.point);
            }
        } 
    }
    private void DrawExplosionTheLine(float currentRadious)
    {
        float angleBetweenPoints = 360f / pointsCount;
        for (int a = 0; a <= pointsCount; a++)
        {
            float angle = a * angleBetweenPoints * Mathf.Deg2Rad;
            Vector3 direction = new Vector3(Mathf.Sin(angle), 0f, Mathf.Cos(angle));
            Vector3 position = direction * currentRadious;
            lineRendererExplosion.SetPosition(a, position);
        }
    }
    private void ExplosionExpansionWave(Vector3 explosionOrigin, float currentRadious)
    {
        Collider[] colliders = Physics.OverlapSphere(explosionOrigin, currentRadious);
        for (int i = 0; i < colliders.Length; i++)
        {
            Rigidbody rb = colliders[i].GetComponent<Rigidbody>();
            if (!rb) continue;
            Vector3 direction = (colliders[i].transform.position - explosionOrigin).normalized;
            rb.AddForce(direction * ForceExpansion, ForceMode.Impulse);
            EnemyBehaviour eb = colliders[i].GetComponent<EnemyBehaviour>();
            if (!eb) continue;
            eb.GetHit(10);
        }
    }
    /**
     * ################################ CRITICAL BULLET FUNCTIONS ################################
     */
    private void CriticalBullet(RaycastHit hit, float powerBulletData)
    {
        Debug.Log("################################# CRITICAL BULLET #################################");
        float rand = Random.Range(0f, 1f);
        if (rand <= powerBulletData)
        {
            EnemyBehaviour eb = hit.transform.gameObject.GetComponent<EnemyBehaviour>();
            if (eb != null)
            {
                eb.GetHit(playerWeapon.CurrentConfiguration.CurrentDamageWeapon * 2);
            }
        }
    }
    /**
     * ################################ TERRIFIER BULLET FUNCTIONS ################################
     */
    private void TerrifierBullet(RaycastHit hit, float powerBulletData)
    {
        Debug.Log("################################# TERRIFIER BULLET #################################");
        float rand = Random.Range(0f, 1f);
        if (rand <= powerBulletData)
        {
            EnemyBehaviour eb = hit.transform.gameObject.GetComponent<EnemyBehaviour>();
            if (eb != null)
            {
                eb.ChangeState(StateOfEnemy.TERRIFIED);
            }
        }
    }
    /**
     * ################################ TIMESLOW BULLET FUNCTIONS ################################
     */
    private void TimeSlowBullet(RaycastHit hit, float powerBulletData)
    {
        Debug.Log("################################# TIMESLOW BULLET #################################");
        float rand = Random.Range(0f, 1f);
        if (rand <= powerBulletData)
        {
            EnemyBehaviour eb = hit.transform.gameObject.GetComponent<EnemyBehaviour>();
            if (eb != null && !timeSlow)
            {
                timeSlow = true;
                StartCoroutine(TimeSlow(eb));
            }
        }
    }
    /**
     * ################################ CRAZYFIER BULLET FUNCTIONS ################################
     */
    private void CrazyBullet(RaycastHit hit, float powerBulletData)
    {
        Debug.Log("################################# CRAZYFIER BULLET #################################");
        float rand = Random.Range(0f, 1f);
        if (rand <= powerBulletData)
        {
            EnemyBehaviour eb = hit.transform.gameObject.GetComponent<EnemyBehaviour>();
            if (eb != null)
            {
                eb.ChangeState(StateOfEnemy.CRAZY);
            }
        }
    }
    /**
     * ################################ COROUTINES ################################
     */
    private IEnumerator Expansion(RaycastHit hit)
    {
        InstanceExpandWave.SetActive(true);
        InstanceExpandWave.transform.position = hit.transform.position;
        lineRendererExpand.enabled = true;
        float currentRadious = 0f;
        while (currentRadious < MaxRadious)
        {
            currentRadious += Time.deltaTime * Speed;
            DrawTheLine(currentRadious);
            ExpansionWave(hit.transform.position, currentRadious);
            yield return null;
        }
        InstanceExpandWave.SetActive(false);
    }
    private IEnumerator ExpansionClient(Vector3 v3)
    {
        InstanceExpandWave.SetActive(true);
        InstanceExpandWave.transform.position = v3;
        lineRendererExpand.enabled = true;
        float currentRadious = 0f;
        while (currentRadious < MaxRadious)
        {
            currentRadious += Time.deltaTime * Speed;
            DrawTheLine(currentRadious);
            yield return null;
        }
        InstanceExpandWave.SetActive(false);
        Expanding = false;
    }
    private IEnumerator ExplosionParticlesCoroutine(Vector3 hit)
    {
        InstanceExplosion.transform.position = hit;
        InstanceExplosion.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        InstanceExplosion.SetActive(false);
    }
    private IEnumerator ExplosionClient(Vector3 hit)
    {
        InstanceExplosionWave.SetActive(true);
        InstanceExplosionWave.transform.position = hit;
        float currentRadious = 0f;
        while (currentRadious < MaxRadious)
        {
            currentRadious += Time.deltaTime * Speed;
            DrawExplosionTheLine(currentRadious);
            yield return null;
        }
        InstanceExplosionWave.SetActive(false);
        Exploting = false;
    }
    private IEnumerator Explosion(RaycastHit hit)
    {
        InstanceExplosionWave.SetActive(true);
        InstanceExplosionWave.transform.position = hit.point;
        float currentRadious = 0f;
        while (currentRadious < MaxRadious)
        {
            currentRadious += Time.deltaTime * Speed;
            DrawExplosionTheLine(currentRadious);
            ExplosionExpansionWave(hit.transform.position, currentRadious);
            yield return null;
        }
        InstanceExplosionWave.SetActive(false);
        Exploting = false;
    }
    private IEnumerator DamageFlame(EnemyBehaviour eb, GameObject gameObject)
    {
        if(gameObject.transform.childCount == 3)
        {
            GameObject flameParticlesInstantiation = Instantiate(flameParticles);
            flameParticlesInstantiation.transform.position = gameObject.transform.position;
            flameParticlesInstantiation.transform.parent = gameObject.transform;
            for (int a = 0; a < 10; a++)
            {
                eb.GetHit(5);
                yield return new WaitForSeconds(1f);
            }
            Destroy(flameParticlesInstantiation);
        }
    }
    private IEnumerator TimeSlow(EnemyBehaviour enemyBehaviour)
    {
        float a = enemyBehaviour.CooldownAttack;
        float b = enemyBehaviour.velocity;
        enemyBehaviour.CooldownAttack -= enemyBehaviour.CooldownAttack * 0.25f;
        enemyBehaviour.velocity -= enemyBehaviour.velocity * 0.25f;
        yield return new WaitForSeconds(15f);
        enemyBehaviour.CooldownAttack += a * 0.25f;
        enemyBehaviour.velocity += b * 0.25f;
        timeSlow = false;
    }
}
public struct PowerBulletNetworkInfo : INetworkSerializeByMemcpy, System.IEquatable<PowerBulletNetworkInfo>
{
    public PowerBulletID id;
    public float InvestValue;
    public PowerBulletNetworkInfo(PowerBulletID id, float investValue)
    {
        this.id = id;   
        this.InvestValue = investValue; 
    }

    public bool Equals(PowerBulletNetworkInfo other)
    {
        return this.id == other.id && this.InvestValue == other.InvestValue;
    }
}
