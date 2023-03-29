using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
[CreateAssetMenu]
public class Mission1 : Mission
{
    [SerializeField]
    private int NumberOfEnemiesToDefeat;
    [SerializeField] 
    private int NumberOfEnemiesDefeated;
    public override void initValues()
    {
        Title = "Undefeatable";
        Description = "Destroy 25 enemies";
        Points = 500;
        NumberOfEnemiesToDefeat = 25;
        NumberOfEnemiesDefeated = 0;
    }
    public override void execute(GameEvent gameEvent)
    {
        if(typeof(Mission1Event) == gameEvent.GetType()) {
            NumberOfEnemiesDefeated++;
            Done = NumberOfEnemiesDefeated >= NumberOfEnemiesToDefeat;
        }
    }

     
}
