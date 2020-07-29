using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.LWRP;

public class HeroAbilities : MonoBehaviour
{

    public int abilityNumber;
    GameMaster gm;

    // Start is called before the first frame update
    void Start()
    {
        gm = FindObjectOfType<GameMaster>();
    }

    // Update is called once per frame
    void Update()
    {
        if (abilityNumber == gm.playerTurn)
        {
            GetComponentInChildren<UnityEngine.Experimental.Rendering.Universal.Light2D>().color = Color.white;
        }
        else
        {
            GetComponentInChildren<UnityEngine.Experimental.Rendering.Universal.Light2D>().color = Color.black;
        }
    }
}
