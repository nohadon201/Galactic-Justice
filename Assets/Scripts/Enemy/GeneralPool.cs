using Unity.Netcode;
using UnityEngine;

public class GeneralPool : NetworkBehaviour
{
    [SerializeField]
    private int poolSize = 50;
    private static GeneralPool instance;
    public static GeneralPool Instance { get { return instance; } }
    public GameObject projectilePrefab;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
            return;
        }
        projectilePrefab = Resources.Load<GameObject>("Prefabs/Enemy/Projectile");
    }
    private void Start()
    {
        if (!IsServer)
        {
            return;
        }
        for (int a = 0; a < poolSize; a++)
        {
            GameObject go = Instantiate(projectilePrefab, transform);
            //go.GetComponent<NetworkObject>().Spawn();
            go.SetActive(false);
        }
    }
    public GameObject getProjectile()
    {
        if (!IsServer) return null;
        for (int a = 0; a < transform.childCount; a++)
        {
            GameObject go = transform.GetChild(a).gameObject;
            if (!go.activeSelf)
            {
                go.SetActive(true);
                return go;
            }
        }
        return null;
    }

    public void returnProjectile(GameObject projectile)
    {
        if (!IsServer) return;

        projectile.SetActive(false);
        projectile.transform.SetParent(transform, false);
        projectile.GetComponent<NetworkObject>().Despawn(false);
    }

}
