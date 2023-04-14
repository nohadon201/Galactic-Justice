using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

public class PowerBullets : MonoBehaviour
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
    void Awake()
    {
        DefaultValues();
    }
    void Start()
    {
        InstanceExplosion = Instantiate(ExplosionParticles);
        InstanceExplosionWave = Instantiate(ExplosionWave);
        InstanceExplosion.SetActive(false);
        InstanceExplosionWave.SetActive(false);
        //  Expand
        lineRendererExpand = InstanceExpandWave.GetComponent<LineRenderer>();
        lineRendererExpand.positionCount = pointsCount + 1;
        lineRendererExpand.enabled = true;
        //  Explosion
        lineRendererExplosion = InstanceExplosionWave.GetComponent<LineRenderer>();
        lineRendererExplosion.positionCount = pointsCount + 1;
        lineRendererExplosion.enabled = true;

    }
    private void DefaultValues()
    {
        //  Generic Variables
        playerWeapon = GetComponent<PlayerWeapon>();
        playerControlls = GetComponent<PlayerControlls>();
        powerBullets = (Resources.LoadAll<PowerBulletSO>("Player/Host/PowerBullets/")).ToList();  
        
        //  FlameBullet Variables
        flameParticles = Resources.Load<GameObject>("ParticlesSystem/Fire");

        //  ExpandBullet Variables
        ExpandWave = Resources.Load<GameObject>("ParticlesSystem/ExpansionWave");
        InstanceExpandWave = Instantiate(ExpandWave);
        InstanceExpandWave.SetActive(false);
        //  ExplosionBullet Variables
        ExplosionParticles = Resources.Load<GameObject>("ParticlesSystem/Explosion");
        ExplosionWave = Resources.Load<GameObject>("ParticlesSystem/ExplosionWave");
        Exploting = false;
        //  TimeSlow Bullet
        timeSlow = false;   
    }

    void Update()
    {
       
    }
    /**
     * ################################ PRINCIPAL FUNCTION ################################
     */
    public void execute(RaycastHit hit, bool byPowerBullet)
    {
        foreach(PowerBulletSO powerBullet in powerBullets) {
            if(powerBullet.currentInvestmentValue > 0)
            {
                switch(powerBullet.id)
                {
                    case PowerBulletID.STUNE:
                        StuneBullet(hit, powerBullet);
                        break;
                    case PowerBulletID.EXPAND:
                        ExpandBullet(hit, powerBullet);
                        break;
                    case PowerBulletID.DOUBLE_FORCE:
                        DoubleForceBullet(hit, powerBullet);
                        break;
                    case PowerBulletID.PIERCING:
                        PiercingBullet(hit, powerBullet);
                        break;
                    case PowerBulletID.MULTIPLIER:
                        MultiplierBullet(hit, powerBullet);
                        break;
                    case PowerBulletID.BOUNCING:
                        BouncingBullet(hit, powerBullet);
                        break;
                    case PowerBulletID.BOUCING_SURFACE:
                        BouncingSurfaceBullet(hit, powerBullet, byPowerBullet);
                        break;
                    case PowerBulletID.FLAME:
                        FlameBullet(hit, powerBullet);
                        break;
                    case PowerBulletID.HEALTH_STEALTH:
                        HealthStealthBullet(hit, powerBullet);
                        break;
                    case PowerBulletID.SHIELD_STEALTH:
                        ShieldStealthBullet(hit, powerBullet);
                        break;
                    case PowerBulletID.EXPLOSIVE:
                        ExplosiveBullet(hit, powerBullet);   
                        break;
                    case PowerBulletID.CRITICAL:
                        CriticalBullet(hit, powerBullet);    
                        break;
                    case PowerBulletID.TERRIFIER:
                        TerrifierBullet(hit, powerBullet);
                        break;
                    case PowerBulletID.TIMESLOW:
                        TimeSlowBullet(hit, powerBullet);
                        break;
                    case PowerBulletID.CRAZYFIER:
                        CrazyBullet(hit, powerBullet);   
                        break;
                }
            }
        }
    }
    public void LoadPointsPowers(bool param)
    {
        if (param) return;
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
        }
    }
    /**
     * ################################ STUNE BULLET FUNCTIONS ################################
     */
    private void StuneBullet(RaycastHit hit, PowerBulletSO powerBulletData)
    {
        Debug.Log("################################# STUNE BULLET #################################");
        float rand = Random.Range(0, 1);
        if(rand<= powerBulletData.currentInvestmentValue)
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
    private void ExpandBullet(RaycastHit hit, PowerBulletSO powerBulletData) { Debug.Log("################################# EXPAND BULLET #################################"); StartCoroutine(Expansion(hit)); }
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
    private void DoubleForceBullet(RaycastHit hit, PowerBulletSO powerBulletData)
    {
        Debug.Log("################################# DOUBLE FORCE BULLET #################################");
        float val = 0.5f * powerBulletData.currentInvestmentValue;
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
    private void PiercingBullet(RaycastHit hit, PowerBulletSO powerBulletData)
    {
        Debug.Log("################################# PIERCING BULLET #################################");
        float rand = Random.Range(0, 1);
        if (rand <= powerBulletData.currentInvestmentValue)
        {
            if (hit.transform.tag == "Enemy")
            {
                playerWeapon.RayCastTo(hit.transform.GetChild(2).transform.position, -hit.normal, true);
            }
        }
    }
    /**
     * ################################ MULTIPLIER BULLET FUNCTIONS ################################
     */
    private void MultiplierBullet(RaycastHit hit, PowerBulletSO powerBulletData)
    {
        Debug.Log("################################# MULTIPLIER BULLET #################################");
        float rand = Random.Range(0, 1);
        if (rand <= powerBulletData.currentInvestmentValue)
        {
            if (hit.transform.tag == "Enemy")
            {
                playerWeapon.RayCastTo(hit.transform.GetChild(2).transform.position, -hit.transform.forward + new Vector3(0.5f, 0, 0), true);
                playerWeapon.RayCastTo(hit.transform.GetChild(2).transform.position, -hit.transform.forward + new Vector3(-0.5f, 0, 0), true);
            }
        }
    }
    /**
     * ################################ BOUNCING BULLET FUNCTIONS ################################
     */
    private void BouncingBullet(RaycastHit hit, PowerBulletSO powerBulletData)
    {
        Debug.Log("################################# BOUNCING BULLET #################################");
        float rand = Random.Range(0, 1);
        if (rand <= powerBulletData.currentInvestmentValue)
        {
            if (hit.transform.tag == "Enemy")
            {
                Vector3 direction = (hit.transform.position - transform.position).normalized;
                Vector3 reflection = Vector3.Reflect(direction, hit.normal);
                playerWeapon.RayCastTo(hit.point, reflection, true);
            }
        }
    }
    /**
     * ################################ BOUNCING SURFACE BULLET FUNCTIONS ################################
     */
    private void BouncingSurfaceBullet(RaycastHit hit, PowerBulletSO powerBulletData, bool byPowerBullet)
    {
        Debug.Log("################################# BOUNCING SURFACE BULLET #################################");
        float rand = Random.Range(0, 1);
        if (rand <= powerBulletData.currentInvestmentValue && !byPowerBullet)
        {
            if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Obstruction"))
            {
                Vector3 direction = (hit.point - transform.position).normalized;
                Vector3 reflection = Vector3.Reflect(direction, hit.normal);
                Debug.DrawLine(hit.point, hit.point + (reflection * 5), Color.red, 3f);
                playerWeapon.RayCastTo(hit.point, reflection, true);
            }
        }
    }
    /**
     * ################################ FLAME BULLET FUNCTIONS ################################
     */
    private void FlameBullet(RaycastHit hit, PowerBulletSO powerBulletData)
    {
        Debug.Log("################################# FLAME BULLET #################################");
        float rand = Random.Range(0, 1);
        if (rand <= powerBulletData.currentInvestmentValue)
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
    private void HealthStealthBullet(RaycastHit hit, PowerBulletSO powerBulletData)
    {
        Debug.Log("################################# HEALTH-STEALTH BULLET #################################");
        playerControlls.OwnInfo.playersCurrentHealth += playerWeapon.CurrentConfiguration.CurrentDamageWeapon * powerBulletData.currentInvestmentValue;
    }
    /**
     * ################################ SHIELD-STEALTH BULLET FUNCTIONS ################################ 
     */
    private void ShieldStealthBullet(RaycastHit hit, PowerBulletSO powerBulletData)
    {
        Debug.Log("################################# SHIELD-STEALTH BULLET #################################");
        playerControlls.OwnInfo.playersCurrentShield += playerWeapon.CurrentConfiguration.CurrentDamageWeapon * powerBulletData.currentInvestmentValue;
    }
    /**
    * ################################ EXPLOSIVE BULLET FUNCTIONS ################################
    */
    private void ExplosiveBullet(RaycastHit hit, PowerBulletSO powerBulletData)
    {
        Debug.Log("################################# EXPLOSIVE BULLET #################################");
        float rand = Random.Range(0, 1);
        if (rand <= powerBulletData.currentInvestmentValue)
        {
            if (!Exploting)
            {
                Exploting = true;
                StartCoroutine(Explosion(hit));
                StartCoroutine(ExplosionParticlesCoroutine(hit));
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
    private void CriticalBullet(RaycastHit hit, PowerBulletSO powerBulletData)
    {
        Debug.Log("################################# CRITICAL BULLET #################################");
        float rand = Random.Range(0, 1);
        if (rand <= powerBulletData.currentInvestmentValue)
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
    private void TerrifierBullet(RaycastHit hit, PowerBulletSO powerBulletData)
    {
        Debug.Log("################################# TERRIFIER BULLET #################################");
        float rand = Random.Range(0, 1);
        if (rand <= powerBulletData.currentInvestmentValue)
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
    private void TimeSlowBullet(RaycastHit hit, PowerBulletSO powerBulletData)
    {
        Debug.Log("################################# TIMESLOW BULLET #################################");
        float rand = Random.Range(0, 1);
        if (rand <= powerBulletData.currentInvestmentValue)
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
    private void CrazyBullet(RaycastHit hit, PowerBulletSO powerBulletData)
    {
        Debug.Log("################################# CRAZYFIER BULLET #################################");
        float rand = Random.Range(0, 1);
        if (rand <= powerBulletData.currentInvestmentValue)
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
    private IEnumerator ExplosionParticlesCoroutine(RaycastHit hit)
    {
        InstanceExplosion.transform.position = hit.point;
        InstanceExplosion.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        InstanceExplosion.SetActive(false);
    }
    private IEnumerator Explosion(RaycastHit hit)
    {
        InstanceExplosionWave.SetActive(true);
        InstanceExplosion.transform.position = hit.point;
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
            flameParticles.transform.position = gameObject.transform.position;
            flameParticles.transform.parent = gameObject.transform;
            for (int a = 0; a < 10; a++)
            {
                eb.GetHit(5);
                yield return new WaitForSeconds(1f);
            }
            Destroy(flameParticles);
        }
    }
    private IEnumerator TimeSlow(EnemyBehaviour enemyBehaviour)
    {
        float a = enemyBehaviour.CooldownAttack;
        float b = enemyBehaviour.velocity;
        enemyBehaviour.CooldownAttack += enemyBehaviour.CooldownAttack * 0.25f;
        enemyBehaviour.velocity += enemyBehaviour.velocity * 0.25f;
        yield return new WaitForSeconds(10f);
        enemyBehaviour.CooldownAttack += a * 0.25f;
        enemyBehaviour.velocity += b * 0.25f;
        timeSlow = false;
    }
}
