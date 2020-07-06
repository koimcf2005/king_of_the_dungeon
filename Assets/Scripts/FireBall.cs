using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBall : MonoBehaviour
{
    public SpawnEffect fireball;
    public GameObject fireballDeath;
    public Transform crystalPos;
    [HideInInspector]
    public Animator camAnim;

    // Start is called before the first frame update
    void Start()
    {
        camAnim = FindObjectOfType<Camera>().GetComponent<Animator>();
        if (transform.position.x >= -7 && transform.position.x <= 7)
        {
            StartCoroutine(Fireball());
        }
    }

    public IEnumerator Fireball()
    {
        SpawnEffect fireballPar = Instantiate(fireball, crystalPos.transform.position, Quaternion.identity);
        fireballPar.targetPos = transform.position;
        fireballPar.speed = Mathf.Abs(transform.position.x - crystalPos.position.x);
        fireball.arcHeight = 5;
        while (fireballPar.targetPos.x != fireballPar.transform.position.x || fireballPar.targetPos.y != fireballPar.transform.position.y)
        {
            yield return null;
        }
        fireballPar.GetComponent<ParticleSystem>().Stop();
        Instantiate(fireballDeath, transform.position, Quaternion.identity);
        camAnim.SetTrigger("Shake");
        foreach (Unit unit in FindObjectsOfType<Unit>())
        {
            if (Mathf.Abs(transform.position.x - unit.transform.position.x) <= 1 && Mathf.Abs(transform.position.y - unit.transform.position.y) <= 1)
            {
                if (unit.shieldsLeft >= 1)
                {
                    unit.UpdateShieldsLeft();
                    Instantiate(unit.shieldIcon, unit.transform.position, Quaternion.identity);
                }
                else
                {
                    unit.health -= 1;
                    Instantiate(unit.dmgIcon, unit.transform.position, Quaternion.identity);
                }
                if (unit.health <= 0)
                {
                    Instantiate(unit.deathPar, unit.transform.position, Quaternion.identity);
                    Destroy(unit.gameObject);
                }
            }
        }
    }
}
