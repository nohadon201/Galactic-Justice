using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoreScriptAbilities : MonoBehaviour
{

    private static CoreScriptAbilities instance;
    public static CoreScriptAbilities Instance => instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
        DontDestroyOnLoad(this.gameObject);
    }
    public void ActivateAbility(int id, PlayerInfo p, GameObject g)
    {
        switch(id)
        {
            case 1:
                StartCoroutine(ChangeVelocity25(p));
                break;
            case 2:
                StartCoroutine(ChangeDamage25(p));
                break;
            case 3:
                StartCoroutine(ChangeShield25(p));
                break;
            case 4:
                StartCoroutine(EnemyIgnorePlayers(p));
                break;
            case 5:
                StartCoroutine(f5(p));
                break;
            case 6:
                StartCoroutine(f6(p));
                break;
            case 7:
                StartCoroutine(f7(p));
                break;
            case 8:
                StartCoroutine(f8(p));
                break;
            default: break;
        }
    }
    private IEnumerator ChangeVelocity25(PlayerInfo p) {
        Debug.Log("1");
        yield return new WaitForSeconds(1);
        Debug.Log("adios 1");
    }

    private IEnumerator ChangeDamage25(PlayerInfo p)
    {
        Debug.Log("2");
        yield return new WaitForSeconds(1);
        Debug.Log("adios 1");
    }

    private IEnumerator ChangeShield25(PlayerInfo p)
    {
        Debug.Log("3");
        yield return new WaitForSeconds(1);
        Debug.Log("adios 1");
    }

    private IEnumerator EnemyIgnorePlayers(PlayerInfo p)
    {
        Debug.Log("4");
        yield return new WaitForSeconds(1);
        Debug.Log("adios 1");
    }

    private IEnumerator f5(PlayerInfo p)
    {
        Debug.Log("5");
        yield return new WaitForSeconds(1);
        Debug.Log("adios 1");
    }

    private IEnumerator f6(PlayerInfo p)
    {
        Debug.Log("6");
        yield return new WaitForSeconds(1);
        Debug.Log("adios 1");
    }

    private IEnumerator f7(PlayerInfo p)
    {
        Debug.Log("7");
        yield return new WaitForSeconds(1);
        Debug.Log("adios 1");
    }

    private IEnumerator f8(PlayerInfo p)
    {
        Debug.Log("8");
        yield return new WaitForSeconds(1);
        Debug.Log("adios 1");
    }
}