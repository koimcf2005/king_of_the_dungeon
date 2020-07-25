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

    public void CheckColumnStatus(Unit king, bool can)
    {

        canSpawnPar = can;

        foreach (Unit otherKing in FindObjectsOfType<Unit>())
        {
            if (king.nameID == "king")
            {
                if (Mathf.Abs(king.transform.position.x - transform.position.x) + Mathf.Abs(king.transform.position.y - transform.position.y) <= 1)
                {
                    GetComponentInChildren<SpriteRenderer>().sprite = ColumnSprites[king.playerNumber - 1];
                    GetComponentInChildren<UnityEngine.Experimental.Rendering.Universal.Light2D>().color = LightColors[king.playerNumber - 1];
                    GetComponentInChildren<UnityEngine.Experimental.Rendering.Universal.Light2D>().intensity = 1;
                    columnNumber = king.playerNumber;
                    if (canSpawnPar == true) Instantiate(pars[king.playerNumber - 1], transform.position, Quaternion.identity); canSpawnPar = false;
                }
                else if (Mathf.Abs(otherKing.transform.position.x - transform.position.x) + Mathf.Abs(otherKing.transform.position.y - transform.position.y) <= 1 && otherKing.nameID == "king")
                {

                    if (canSpawnPar == true)
                    {
                        Instantiate(pars[otherKing.playerNumber - 1], transform.position, Quaternion.identity); canSpawnPar = false;
                        GetComponentInChildren<SpriteRenderer>().sprite = ColumnSprites[otherKing.playerNumber - 1];
                        GetComponentInChildren<UnityEngine.Experimental.Rendering.Universal.Light2D>().color = LightColors[otherKing.playerNumber - 1];
                        GetComponentInChildren<UnityEngine.Experimental.Rendering.Universal.Light2D>().intensity = 1;
                        columnNumber = otherKing.playerNumber;
                    }
                }
            }
        }
        canSpawnPar = true;
    }
}
