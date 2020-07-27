using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.LWRP;

public class CoinColumn : MonoBehaviour
{

    public Sprite[] ColumnSprites;
    public Color[] LightColors;
    public GameObject[] pars;
    public int columnNumber;
    public bool canSpawnPar = true;

    GameMaster gm;

    public void CheckColumnStatus(Unit hero, bool can)
    {

        canSpawnPar = can;

        foreach (Unit otherHero in FindObjectsOfType<Unit>())
        {
            if (hero.isHero == true)
            {
                if (Mathf.Abs(hero.transform.position.x - transform.position.x) + Mathf.Abs(hero.transform.position.y - transform.position.y) <= 1)
                {
                    GetComponentInChildren<SpriteRenderer>().sprite = ColumnSprites[hero.playerNumber - 1];
                    GetComponentInChildren<UnityEngine.Experimental.Rendering.Universal.Light2D>().color = LightColors[hero.playerNumber - 1];
                    GetComponentInChildren<UnityEngine.Experimental.Rendering.Universal.Light2D>().intensity = 1;
                    columnNumber = hero.playerNumber;
                    if (canSpawnPar == true) Instantiate(pars[hero.playerNumber - 1], transform.position, Quaternion.identity); canSpawnPar = false;
                }
                else if (Mathf.Abs(otherHero.transform.position.x - transform.position.x) + Mathf.Abs(otherHero.transform.position.y - transform.position.y) <= 1 && otherHero.isHero == true)
                {

                    if (canSpawnPar == true)
                    {
                        Instantiate(pars[otherHero.playerNumber - 1], transform.position, Quaternion.identity); canSpawnPar = false;
                        GetComponentInChildren<SpriteRenderer>().sprite = ColumnSprites[otherHero.playerNumber - 1];
                        GetComponentInChildren<UnityEngine.Experimental.Rendering.Universal.Light2D>().color = LightColors[otherHero.playerNumber - 1];
                        GetComponentInChildren<UnityEngine.Experimental.Rendering.Universal.Light2D>().intensity = 1;
                        columnNumber = otherHero.playerNumber;
                    }
                }
            }
        }
        canSpawnPar = true;
    }
}
