using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroIcons : MonoBehaviour
{

    public Unit hero;

    // Update is called once per frame
    void Update()
    {
       if (hero == null)
        {
            Destroy(this.gameObject);
        } 
    }
}
