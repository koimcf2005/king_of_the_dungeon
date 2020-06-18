using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{

    public bool selected;
    public bool hasMoved;
    public bool hasAttacked;

    public bool isArcher;
    public bool isBrute;
    public bool isHealer;
    public bool isElite;
    public bool isKing;
    public bool isThief;
    public bool spawnedFromCard;
    public bool isInstance;

    public int shieldsLeft;
    public int tileSpeed;
    public int playerNumber;
    public int attacksLeft;

    public float moveSpeed;

    public int attackRange;
    List<Unit> enemiesInRange = new List<Unit>();
    List<Unit> alliesInRange = new List<Unit>();
    public int health;
    private int maxHealth;
    public int attackDamage;
    public float attackSpeed;
    public int defenceDamage;
    public int normArmor;
    public int armor;
    public int price;

    public DamageIcon dmgIcon;
    public GameObject healIcon;
    public GameObject shieldIcon;
    public GameObject deathPar;
    public GameObject attackHighlight;
    public GameObject healHighlight;
    public SpawnEffect spawnPar;
    public SpawnEffect arrow;
    public ParticleSystem goldPar;
    public GameObject spawnParDeath;
    public Transform crystalPos;

    public HealthIndicator healthIndicator;

    public Animator camAnim;

    public bool isAttacking = true;
    private Animator anim;
    GameMaster gm;

    private void Start() // START * START * START * START
    {
        gm = FindObjectOfType<GameMaster>();
        anim = GetComponentInChildren<Animator>();
        camAnim = FindObjectOfType<Camera>().GetComponent<Animator>();
        if (isInstance == false) StartCoroutine(Spawn(price));
        maxHealth = health;
        gm.ResetTiles();
    }

    public IEnumerator Spawn(int g) // Spawns the unity with diff effects according to its team at diff times
    {
        if (spawnedFromCard == false)
        {
            yield return new WaitForSeconds(2.5f);
            var n = Random.Range(0f, 3f);
            yield return new WaitForSeconds(n);
        }
        else
        {
            if (playerNumber == 1)
            {
                gm.blueGold -= g;
            }
            else if (playerNumber == 2)
            {
                gm.redGold -= g;
            }
        }
        SpawnEffect par = Instantiate(spawnPar, crystalPos.position, Quaternion.identity);
        par.targetPos = transform.position;
        par.speed = Mathf.Abs(transform.position.x - crystalPos.position.x);
        par.arcHeight = 2;
        while (par.targetPos.x != par.transform.position.x || par.targetPos.y != par.transform.position.y)
        {
            yield return null;
        }
        anim.SetTrigger("Spawn");
        par.GetComponent<ParticleSystem>().Stop();
        Instantiate(spawnParDeath, transform.position, Quaternion.identity);
        isAttacking = false;
    }

    private void Update()
    {
        foreach (Unit unit in FindObjectsOfType<Unit>()) // Changes whether or not you can click of this unit
        {
            float zPos = unit.transform.position.y;
            int order = Mathf.RoundToInt(zPos);
            if (!isAttacking)
            {
                unit.transform.position = new Vector3(unit.transform.position.x, unit.transform.position.y, zPos / 2);
            }
            else if (isAttacking)
            {
                unit.transform.position = new Vector3(unit.transform.position.x, unit.transform.position.y, zPos / 2 + 10);
            }
        }
        foreach (CoinColumn column in FindObjectsOfType<CoinColumn>()) // Adds a column to the units team if it is close enough and it is a king
        {
            float zPos = column.transform.position.y;
            int order = Mathf.RoundToInt(zPos);
            column.transform.position = new Vector3(column.transform.position.x, column.transform.position.y, zPos / 2);
        }

    }

    private void OnMouseDown()
    {
        alliesInRange.Clear();

        foreach (Unit units in FindObjectsOfType<Unit>()) // Checks if the unit can be healed
        {
            if (units.alliesInRange.Contains(this) && gm.selectedUnit.hasAttacked == false && gm.selectedUnit.isHealer == true)
            {
                StartCoroutine(Heal(this, gm.selectedUnit));
                gm.selectedUnit.hasAttacked = true;
                ResetAttackHighlights();
                return;
            }
        }
            if (playerNumber != gm.playerTurn && attackHighlight.activeInHierarchy != true) // Checks if the unit team is the same as the players
            {
                camAnim.SetTrigger("Shake");
            }
        
            healthIndicator.transform.position = new Vector3(12, 12, 0);
            ResetAttackHighlights();

            if (selected == true) // Selects and deselects the unit
            {
                selected = false;
                gm.selectedUnit = null;
                gm.ResetTiles();
                alliesInRange.Clear();
            }
            else
            {
                if (playerNumber == gm.playerTurn)
                {
                    if (gm.selectedUnit != null)
                    {
                        gm.selectedUnit.selected = false;
                    }

                    selected = true;
                    gm.selectedUnit = this;
                    gm.ResetTiles();
                    GetEnemies();
                    GetWalkableTiles();
                }
            }
        Collider2D col = Physics2D.OverlapCircle(Camera.main.ScreenToWorldPoint(Input.mousePosition), 0.15f); // Gets the enemy
        Unit unit = col.GetComponent<Unit>();
        if (gm.selectedUnit != null)
        {
            if (gm.selectedUnit.enemiesInRange.Contains(unit) && gm.selectedUnit.hasAttacked == false)
            {
                gm.selectedUnit.StartCoroutine(Attack(unit));
                gm.selectedUnit.attacksLeft -= 1;
                if (gm.selectedUnit.attacksLeft <= 0) gm.selectedUnit.hasAttacked = true;
            }
        }
    }

    public IEnumerator Heal(Unit ally, Unit healer) // Heals the targeted unit
    {
        foreach (Unit unit in FindObjectsOfType<Unit>())
        {
            unit.isAttacking = true;
        }

        UpdateDirection(healer.transform, ally.transform, healer);
        healer.anim.SetTrigger("Attack");
        yield return new WaitForSeconds(1);
        camAnim.SetTrigger("Shake");
        ally.health += 1;
        Instantiate(healer.healIcon, ally.transform.position, Quaternion.identity);
        foreach (Unit unit in FindObjectsOfType<Unit>())
        {
            unit.isAttacking = false;
            unit.alliesInRange.Clear();
        }
    }

    public IEnumerator Attack(Unit enemy)
    {
        foreach (Unit unit in FindObjectsOfType<Unit>())
        {
            unit.isAttacking = true;
        }

        healthIndicator.transform.position = new Vector3(12, 12, 0);

        int deltDamage = gm.selectedUnit.attackDamage - enemy.armor; // Gets the damage vals
        int takenDamage = enemy.defenceDamage - gm.selectedUnit.armor;

        UpdateDirection(gm.selectedUnit.transform, enemy.transform, gm.selectedUnit); // Updates the units facing pos

        gm.selectedUnit.anim.SetTrigger("Attack");

        if (gm.selectedUnit.isArcher == false) yield return new WaitForSeconds(0.66f);

        else // Arrow Spawning ******************************************************************************************************
        {
            yield return new WaitForSeconds(1.4f);
            camAnim.SetTrigger("Shake");
            SpawnEffect arrowInstance = Instantiate(gm.selectedUnit.arrow, gm.selectedUnit.transform.position, Quaternion.identity);
            arrowInstance.arcHeight = 4;

            if (Mathf.Abs(gm.selectedUnit.transform.position.x - enemy.transform.position.x) >= 1)
            {
                arrowInstance.speed = Mathf.Abs(gm.selectedUnit.transform.position.x - enemy.transform.position.x);
                arrowInstance.targetPos = enemy.transform.position;
            }
            else
            {
                arrowInstance.startPos = gm.selectedUnit.transform.position + new Vector3(-0.3f, 0, 0);
                arrowInstance.speed = 0.35f;
                arrowInstance.targetPos = enemy.transform.position + new Vector3(0.3f, 0, 0);
            }

            while (arrowInstance.targetPos.x != arrowInstance.transform.position.x || arrowInstance.targetPos.y != arrowInstance.transform.position.y)
            {
                yield return null;
            }
            StartCoroutine(arrowInstance.KillArrow());
        }   // Arrow Spawning ******************************************************************************************************

        // Attack *****************************************************************************************************
        camAnim.SetTrigger("Shake");

        enemy.UpdateShieldsLeft();

        if (deltDamage >= 1)
        {
            DamageIcon instance = Instantiate(dmgIcon, enemy.transform.position, Quaternion.identity);
            instance.Setup(deltDamage);
            enemy.health -= deltDamage;
        }
        else
        {
            Instantiate(shieldIcon, enemy.transform.position + new Vector3(0, 0.2f, 0), Quaternion.identity);
        }

        if (enemy.health <= 0)
        {
            Instantiate(deathPar, enemy.transform.position, Quaternion.identity);
            Destroy(enemy.gameObject);
            gm.selectedUnit.GetWalkableTiles();

            if (gm.selectedUnit.isThief == true)
            {
                if (gm.selectedUnit.playerNumber == 1 && gm.redGold >= 5)
                {
                    int g = 10;
                    if (gm.redGold < 10) g -= 5;
                    gm.redGold -= g;
                    gm.blueGold += g;
                    if (gm.redGold < 0) gm.redGold = 0;
                    ParticleSystem coin = Instantiate(gm.selectedUnit.goldPar, enemy.transform.position, Quaternion.identity);
                    coin.Play();
                    ParticleSystem coin2 = Instantiate(gm.selectedUnit.goldPar, enemy.transform.position, Quaternion.identity);
                    coin2.Play();
                }
                else if (gm.selectedUnit.playerNumber == 2 && gm.blueGold >= 5)
                {
                    int g = 10;
                    if (gm.blueGold < 10) g -= 5;
                    gm.redGold += g;
                    gm.blueGold -= g;
                    if (gm.blueGold < 0) gm.blueGold = 0;
                    ParticleSystem coin = Instantiate(gm.selectedUnit.goldPar, enemy.transform.position, Quaternion.identity);
                    coin.Play();
                    ParticleSystem coin2 = Instantiate(gm.selectedUnit.goldPar, enemy.transform.position, Quaternion.identity);
                    coin2.Play();
                }
            }

            foreach (Unit unit in FindObjectsOfType<Unit>())
            {
                unit.isAttacking = false;
            }
        }
        else
        {
            if (gm.selectedUnit.isThief == true)
            {
                if (gm.selectedUnit.playerNumber == 1 && gm.redGold >= 5)
                {
                    gm.redGold -= 5;
                    gm.blueGold += 5;
                    ParticleSystem coin = Instantiate(gm.selectedUnit.goldPar, enemy.transform.position, Quaternion.identity);
                    coin.Play();
                }
                else if (gm.selectedUnit.playerNumber == 2 && gm.blueGold >= 5)
                {
                    gm.redGold += 5;
                    gm.blueGold -= 5;
                    ParticleSystem coin = Instantiate(gm.selectedUnit.goldPar, enemy.transform.position, Quaternion.identity);
                    coin.Play();
                }
            }
        }

        if (Mathf.Abs(gm.selectedUnit.transform.position.x - enemy.transform.position.x) // Checks to see if the enemy can attack back
          + Mathf.Abs(gm.selectedUnit.transform.position.y - enemy.transform.position.y) <= 1 && enemy.isHealer == false || enemy.tag == "Range")
        {
            yield return new WaitForSeconds(0.3f);

            UpdateDirection(enemy.transform, gm.selectedUnit.transform, enemy);

            enemy.anim.SetTrigger("Attack");

            if (enemy.isArcher == false) yield return new WaitForSeconds(0.66f);
            else // Enemy arrow spawning *************************************************************************************
            {
                yield return new WaitForSeconds(1.4f);
                camAnim.SetTrigger("Shake");
                SpawnEffect arrowInstance = Instantiate(enemy.arrow, enemy.transform.position, Quaternion.identity);
                arrowInstance.arcHeight = 4;
                if (Mathf.Abs(gm.selectedUnit.transform.position.x - enemy.transform.position.x) >= 1)
                {
                    arrowInstance.speed = Mathf.Abs(gm.selectedUnit.transform.position.x - enemy.transform.position.x);
                    arrowInstance.targetPos = gm.selectedUnit.transform.position;
                }
                else
                {
                    arrowInstance.startPos = enemy.transform.position + new Vector3(-0.3f, 0, 0);
                    arrowInstance.speed = 0.35f;
                    arrowInstance.targetPos = gm.selectedUnit.transform.position + new Vector3(0.3f, 0, 0);
                }

                while (arrowInstance.targetPos.x != arrowInstance.transform.position.x || arrowInstance.targetPos.y != arrowInstance.transform.position.y)
                {
                    yield return null;
                }

                StartCoroutine(arrowInstance.KillArrow());
            } // Enemy arrow spawning *************************************************************************************************************************

            camAnim.SetTrigger("Shake");

            // Enemy attack ************************************************************************************************************************************

            if (takenDamage >= 1)
            {
                DamageIcon instance = Instantiate(dmgIcon, gm.selectedUnit.transform.position, Quaternion.identity);
                instance.Setup(takenDamage);
                gm.selectedUnit.health -= takenDamage;
            }
            else
            {
                Instantiate(shieldIcon, gm.selectedUnit.transform.position, Quaternion.identity);
            }

            if (gm.selectedUnit.health <= 0)
            {
                Instantiate(deathPar, transform.position, Quaternion.identity);
                gm.ResetTiles();
                Destroy(gm.selectedUnit.gameObject);

            }
        }
        // Enemy attack ************************************************************************************************************************************
        gm.UpdateMovesLeft();
        if (attacksLeft > 0) gm.selectedUnit.GetEnemies();
        foreach (Unit unit in FindObjectsOfType<Unit>())
        {
            unit.isAttacking = false;
        }
    }

    public void GetWalkableTiles()
    {
        if (hasMoved == true)
        {
            return;
        }

        foreach (Tile tile in FindObjectsOfType<Tile>())
        {

            if (Mathf.Abs(transform.position.y - tile.transform.position.y) + Mathf.Abs(transform.position.x - tile.transform.position.x) <= tileSpeed)
            {
                if (tile.IsClear() == true)
                {
                    tile.Highlight();
                }
            }
        }
    }

    void GetEnemies()
    {
        enemiesInRange.Clear();
        alliesInRange.Clear();

        foreach (Unit unit in FindObjectsOfType<Unit>())
        {
            if (Mathf.Abs(transform.position.y - unit.transform.position.y) + Mathf.Abs(transform.position.x - unit.transform.position.x) <= attackRange)
            {
                if (gm.selectedUnit.isHealer == false)
                {
                    if (unit.playerNumber != gm.playerTurn && hasAttacked == false)
                    {
                        enemiesInRange.Add(unit);
                        unit.attackHighlight.SetActive(true);
                    }
                }
                else if (gm.selectedUnit.isHealer == true)
                {
                    if (unit.playerNumber == gm.playerTurn && hasAttacked == false && gm.selectedUnit != unit && unit.health < unit.maxHealth + 1 && unit.shieldsLeft == 0)
                    {
                        alliesInRange.Add(unit);
                        unit.healHighlight.SetActive(true);
                    }
                }
            }
        }
    }

    public void ResetAttackHighlights()
    {
        foreach(Unit unit in FindObjectsOfType<Unit>())
        {
            unit.attackHighlight.SetActive(false);
            unit.healHighlight.SetActive(false);
        }
    }

    public void Move(Vector2 tilePos)
    {
        gm.ResetTiles();
        StartCoroutine(StartMovement(tilePos));
        healthIndicator.transform.position = new Vector3(12, 12, 0);
        foreach (CoinColumn column in FindObjectsOfType<CoinColumn>())
        {
            column.CheckColumnStatus(this, false);
        }
    }

    IEnumerator StartMovement(Vector2 tilePos)
    {
        if (transform.position.x > tilePos.x)
        {
            anim.SetTrigger("Left");
        }
        else if (transform.position.x < tilePos.x)
        {
            anim.SetTrigger("Right");
        }
        while (transform.position.x != tilePos.x)
        {
            transform.position = Vector2.MoveTowards(transform.position, new Vector2(tilePos.x, transform.position.y), moveSpeed * Time.deltaTime);
            yield return null;
        }

        if (transform.position.y < tilePos.y)
        {
            anim.SetTrigger("Up");
        }
        else if (transform.position.y > tilePos.y)
        {
            anim.SetTrigger("Down");
        }

        while (transform.position.y != tilePos.y)
        {
            transform.position = Vector2.MoveTowards(transform.position, new Vector2(transform.position.x, tilePos.y), moveSpeed * Time.deltaTime);
            yield return null;
        }

        gm.UpdateMovesLeft();
        hasMoved = true;
        ResetAttackHighlights();
        GetEnemies();

        foreach (CoinColumn column in FindObjectsOfType<CoinColumn>())
        {
            column.CheckColumnStatus(this, true);
        }
    }

    private void OnMouseOver()
    {
        if (this.playerNumber == 1)
        {
            healthIndicator.transform.position = transform.position + new Vector3(0.25f, 0.2f, 0);
        }
        else if (this.playerNumber == 2)
        {
            healthIndicator.transform.position = transform.position + new Vector3(0.25f, 0.2f, 0);
        }

        healthIndicator.Setup(this.health, this.shieldsLeft, this.maxHealth);
    }

    private void OnMouseExit()
    {
        healthIndicator.transform.position = new Vector3(16, 12, 0);
    }

    public void UpdateDirection(Transform thisTransform, Transform facingTransform, Unit unit)
    {
        if (thisTransform.position.x < facingTransform.transform.position.x)
        {
            unit.anim.SetTrigger("Right");
        }
        if (thisTransform.position.x > facingTransform.position.x)
        {
            unit.anim.SetTrigger("Left");
        }
        if (thisTransform.position.y < facingTransform.position.y && unit.isArcher != true && unit.isHealer != true)
        {
            unit.anim.SetTrigger("Up");
        }
        if (thisTransform.position.y > facingTransform.position.y && unit.isArcher != true && unit.isHealer != true)
        {
            unit.anim.SetTrigger("Down");
        }
        if (thisTransform.position.x == facingTransform.transform.position.x && unit.isArcher == true)
        {
            unit.anim.SetTrigger("Right");
        }
    }

    public void UpdateShieldsLeft()
    {
        if (shieldsLeft == 1)
        {
            armor = normArmor;
        }
        if (shieldsLeft != 0) shieldsLeft -= 1;
    }

}
