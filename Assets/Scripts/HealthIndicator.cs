using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.LWRP;

public class HealthIndicator : MonoBehaviour
{
    public Sprite[] damageSprites;
    public Sprite shieldIcon;
    public Sprite heartIcon;
    public Sprite purpleHeartIcon;
    public GameObject heart;
    public GameObject heartLight;
    public Color color;
    public Color purpleColor;

    public void Start()
    {
    }

    public void Setup(int health, int shieldsLeft, int maxHealth)
    {

        float hoverAmount = 0.3f;
        float normAmount = 0.5f;

        heart.GetComponent<SpriteRenderer>().sprite = heartIcon;
        heartLight.GetComponent<UnityEngine.Experimental.Rendering.Universal.Light2D>().color = Color.red;
        GetComponent<SpriteRenderer>().sprite = damageSprites[health - 1];
        GetComponent<SpriteRenderer>().color = Color.white;

        if (health > maxHealth)
        {
            heartLight.GetComponent<UnityEngine.Experimental.Rendering.Universal.Light2D>().color = purpleColor;
            GetComponent<SpriteRenderer>().color = Color.magenta;
            heart.GetComponent<SpriteRenderer>().sprite = purpleHeartIcon;
        }

        if (shieldsLeft > 0)
        {
            heart.GetComponent<SpriteRenderer>().sprite = shieldIcon;
            heart.GetComponent<SpriteRenderer>().color = color;
            heartLight.GetComponent<UnityEngine.Experimental.Rendering.Universal.Light2D>().color = color;
            GetComponent<SpriteRenderer>().sprite = damageSprites[shieldsLeft - 1];
            GetComponent<SpriteRenderer>().color = color;
        }
    }

}

