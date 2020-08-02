using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Unit : MonoBehaviour
{
    [HideInInspector]
    public bool selected;
    [HideInInspector]
    public bool hasMoved;
    [HideInInspector]
    public bool hasAttacked;
    [HideInInspector]
    public bool hasBuffed;
    [HideInInspector]
    public int price;

    public int playerNumber;

    public bool hasGraveEffect;
    public Unit skeleton;

    [Space]
    [Header("IDs & Unit Type Bools")]
    public string nameID;
    public bool isHero;
    public bool isCrystal;
    public bool isInstance;
    public bool spawnedFromCard;
    [Space]
    [Header("Move Stats & Types")]
    public int tileSpeed;
    public float moveSpeed;
    public bool isSquare;
    public bool isCross;
    public bool isDiagonal;
    public bool isQueen;
    [Space]
    [Header("Attack Stats")]
    public int attacksLeft;
    public int attackRange;
    public int attackDamage;
    public int normAttackDmg;
    public float attackSpeed;
    public bool isEnraged;
    public bool isWeakened;

    List<Unit> enemiesInRange = new List<Unit>();
    List<Unit> alliesInRange = new List<Unit>();

    [Space]
    [Header("Health Stats")]
    public int health;
    public int maxHealth;
    public int shieldsLeft;
    public int defenceDamage;
    public int normArmor;
    public int armor;
    public bool isPoisoned;
    [Space]
    [Header("Icons")]
    public HealthIndicator healthIndicator;
    public DamageIcon dmgIcon;
    public GameObject healIcon;
    public GameObject shieldIcon;
    public GameObject armoredIcon;
    [Space]
    [Header("Highlights")]
    public GameObject attackHighlight;
    public GameObject healHighlight;
    public GameObject armorHightlight;
    [Space]
    [Header("Pars")]
    public GameObject deathPar;
    public SpawnEffect spawnPar;
    public GameObject spawnParDeath;
    public ParticleSystem goldPar;
    public ParticleSystem sparkPar;
    public GameObject rageEffect;
    public ParticleSystem yellEffect;
    public GameObject poisonEffect;
    public GameObject weaknessEffect;
    public GameObject graveEffect;
    [Space]
    [Header("Rand")]
    public GameObject rayCollider;
    public Transform crystalPos;
    public SpawnEffect arrow;
    [HideInInspector]
    public Animator camAnim;
    public bool isAttacking = true;
    private Animator anim;
    GameMaster gm;

    private void Start() // START * START * START * START
    {
        gm = FindObjectOfType<GameMaster>();
        anim = GetComponentInChildren<Animator>();
        camAnim = FindObjectOfType<Camera>().GetComponent<Animator>();
        if (isInstance == false && isHero == false && nameID != "skeleton") StartCoroutine(Spawn(price));
        else if (isHero == true || nameID == "skeleton")
        {
            anim.SetTrigger("Spawn");
            isAttacking = false;
        }
        maxHealth = health;
        normAttackDmg = attackDamage;
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
        if (transform.position.x >= -7 && transform.position.x <= 7 && isCrystal == false)
        {
            SpawnEffect par = Instantiate(spawnPar, crystalPos.position + new Vector3(0.01f, 0, 0), Quaternion.identity);
            par.targetPos = transform.position;
            par.speed = Mathf.Abs(transform.position.x - crystalPos.position.x);
            par.arcHeight = 2;
            while (par.targetPos.x != par.transform.position.x || par.targetPos.y != par.transform.position.y)
            {
                yield return null;
            }
            par.GetComponent<ParticleSystem>().Stop();
            Instantiate(spawnParDeath, transform.position, Quaternion.identity);
        }
        if (isCrystal == false) anim.SetTrigger("Spawn");
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
        if (isEnraged == true)
        {
            attackDamage = normAttackDmg + 1;
            rageEffect.SetActive(true);
        }
        else if (isWeakened == true)
        {
            attackDamage = normAttackDmg - 1;
            weaknessEffect.SetActive(true);
        }
        else
        {
            attackDamage = normAttackDmg;
            rageEffect.SetActive(false);
            weaknessEffect.SetActive(false);
        }
        if (isPoisoned == true)
        {
            poisonEffect.SetActive(true);
        }
        else
        {
            poisonEffect.SetActive(false);
        }

        if (hasGraveEffect == true)
        {
            graveEffect.SetActive(true);
        }
        else
        {
            graveEffect.SetActive(false);
        }

        if (isEnraged == true && isWeakened == true)
        {
            isEnraged = false;
            isWeakened = false;
        }
    }

    private void OnMouseDown()
    {
        alliesInRange.Clear();
        if (attackHighlight.activeInHierarchy == false)
        {
            healthIndicator.transform.position = new Vector3(25, 12, 0);
            ResetAttackHighlights();
        }

        foreach (Unit units in FindObjectsOfType<Unit>()) // Checks if the unit can be healed
        {
            if (units.alliesInRange.Contains(this) && gm.selectedUnit.hasAttacked == false && gm.selectedUnit.nameID == "healer")
            {
                StartCoroutine(Heal(this, gm.selectedUnit));
                gm.selectedUnit.hasAttacked = true;
                ResetAttackHighlights();
                return;
            }
            if (units.alliesInRange.Contains(this) && gm.selectedUnit.hasBuffed == false && gm.selectedUnit.nameID == "armorer")
            {
                StartCoroutine(Armor(this, gm.selectedUnit));
                gm.selectedUnit.hasBuffed = true;
                ResetAttackHighlights();
                return;
            }
        }

        if (selected == true || isCrystal == true) // Selects and deselects the unit
        {
            selected = false;
            gm.selectedUnit = null;
            gm.ResetTiles();
            alliesInRange.Clear();
        }
        else if (attackHighlight.activeInHierarchy == false)
        {
            if (gm.selectedUnit != null)
            {
                gm.selectedUnit.selected = false;
            }
                selected = true;
                gm.selectedUnit = this;
                gm.ResetTiles();

            if (playerNumber == gm.playerTurn)
            {
                GetEnemies();
                GetWalkableTiles();
            }

        }
        Collider2D col = Physics2D.OverlapCircle(Camera.main.ScreenToWorldPoint(Input.mousePosition), 0.000001f); // Gets the enemy
        Unit unit = col.GetComponent<Unit>();
        if (gm.selectedUnit != null)
        {
            if (gm.selectedUnit.enemiesInRange.Contains(unit) && gm.selectedUnit.hasAttacked == false)
            {
                gm.selectedUnit.StartCoroutine(Attack(unit));
                gm.selectedUnit.attacksLeft -= 1;
                ResetAttackHighlights();
                if (gm.selectedUnit.attacksLeft <= 0) gm.selectedUnit.hasAttacked = true;
            }
        }
    }

    public IEnumerator Armor(Unit ally, Unit armorer)
    {
        UpdateDirection(armorer.transform, ally.transform, armorer, true);
        armorer.anim.SetTrigger("Armor");
        yield return new WaitForSeconds(0.848f);
        Instantiate(armorer.sparkPar, armorer.transform.position + new Vector3(-0.09f, 0), Quaternion.identity);
        yield return new WaitForSeconds(0.7f);
        Instantiate(armorer.sparkPar, armorer.transform.position + new Vector3(-0.09f, 0), Quaternion.identity);
        yield return new WaitForSeconds(0.4f);
        foreach (Unit unit in FindObjectsOfType<Unit>())
        {
            unit.isAttacking = true;
        }
        ally.armor = 10;
        ally.shieldsLeft += 1;
        Instantiate(armorer.armoredIcon, ally.transform.position, Quaternion.identity);
        if (armorer.playerNumber == 1) gm.blueGold -= 15;
        else if (armorer.playerNumber == 2) gm.redGold -= 15;
        Instantiate(armorer.goldPar, armorer.transform.position, Quaternion.identity);
        foreach (Unit unit in FindObjectsOfType<Unit>())
        {
            unit.isAttacking = false;
            unit.alliesInRange.Clear();
        }
    }

    public IEnumerator Heal(Unit ally, Unit healer) // Heals the targeted unit
    {
        foreach (Unit unit in FindObjectsOfType<Unit>())
        {
            unit.isAttacking = true;
        }

        UpdateDirection(healer.transform, ally.transform, healer, true);
        healer.anim.SetTrigger("Attack");
        yield return new WaitForSeconds(1);
        camAnim.SetTrigger("Shake");
        ally.health += 1;
        Instantiate(healer.healIcon, ally.transform.position, Quaternion.identity);
        ally.isPoisoned = false;
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

        gm.selectedUnit.isEnraged = false;
        healthIndicator.transform.position = new Vector3(12, 12, 0);

        int deltDamage = gm.selectedUnit.attackDamage - enemy.armor; // Gets the damage vals
        int takenDamage = enemy.defenceDamage - gm.selectedUnit.armor;

        UpdateDirection(gm.selectedUnit.transform, enemy.transform, gm.selectedUnit, false); // Updates the units facing pos

        gm.selectedUnit.anim.SetTrigger("Attack");

        if (gm.selectedUnit.nameID != "nox" && gm.selectedUnit.nameID != "archer" && gm.selectedUnit.nameID != "necromancer")
        {
            yield return new WaitForSeconds(0.66f);
        }
        else // Arrow Spawning ******************************************************************************************************
        {
            if (gm.selectedUnit.nameID != "necromancer") yield return new WaitForSeconds(1.4f);
            if (gm.selectedUnit.nameID == "necromancer") yield return new WaitForSeconds(0.66f);
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
        gm.selectedUnit.camAnim.SetTrigger("Shake");

        enemy.UpdateShieldsLeft();

        if (deltDamage >= 1)
        {
            DamageIcon instance = Instantiate(dmgIcon, enemy.transform.position, Quaternion.identity);
            instance.Setup(deltDamage);
            enemy.health -= deltDamage;
            if (gm.selectedUnit.nameID == "nox") enemy.isPoisoned = true;
        }
        else
        {
            Instantiate(shieldIcon, enemy.transform.position + new Vector3(0, 0.2f, 0), Quaternion.identity);
            if (gm.selectedUnit.nameID == "nox") enemy.isPoisoned = true;
        }

        if (enemy.health <= 0)
        {
            if (enemy.hasGraveEffect == true) Instantiate(enemy.skeleton, enemy.transform.position, Quaternion.identity);
            Instantiate(deathPar, enemy.transform.position, Quaternion.identity);
            Destroy(enemy.gameObject);
            gm.selectedUnit.GetWalkableTiles();

            if (gm.selectedUnit.nameID == "thief")
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
            if (gm.selectedUnit.nameID == "berserk")
            {
                ParticleSystem par = Instantiate(gm.selectedUnit.yellEffect, gm.selectedUnit.transform.position, Quaternion.identity);
                par.Play();
                foreach (Unit unit in FindObjectsOfType<Unit>())
                {
                    if (Mathf.Abs(gm.selectedUnit.transform.position.x - unit.transform.position.x) <= 1
                     && Mathf.Abs(gm.selectedUnit.transform.position.y - unit.transform.position.y) <= 1
                     && unit.nameID == "healer" && unit.playerNumber == gm.selectedUnit.playerNumber || unit == gm.selectedUnit)
                    {
                        unit.isEnraged = true;
                    }
                }
            }
            foreach (Unit unit in FindObjectsOfType<Unit>())
            {
                unit.isAttacking = false;
            }
        }
        else
        {
            if (gm.selectedUnit.nameID == "thief")
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
          + Mathf.Abs(gm.selectedUnit.transform.position.y - enemy.transform.position.y) <= 1 && enemy.nameID == "healer" && enemy.isCrystal == false || enemy.tag == "Range")
        {
            yield return new WaitForSeconds(0.3f);

            UpdateDirection(enemy.transform, gm.selectedUnit.transform, enemy, false);

            enemy.anim.SetTrigger("Attack");

            if (enemy.nameID != "archer" && enemy.nameID != "nox" && enemy.nameID != "necromancer") yield return new WaitForSeconds(0.66f);
            else // Enemy arrow spawning *************************************************************************************
            {
                if (gm.selectedUnit.nameID != "necromancer") yield return new WaitForSeconds(1.4f);
                if (gm.selectedUnit.nameID == "necromancer") yield return new WaitForSeconds(0.66f);
                gm.selectedUnit.camAnim.SetTrigger("Shake");
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

            gm.selectedUnit.camAnim.SetTrigger("Shake");

            // Enemy attack ************************************************************************************************************************************

            if (takenDamage >= 1)
            {
                DamageIcon instance = Instantiate(dmgIcon, gm.selectedUnit.transform.position, Quaternion.identity);
                instance.Setup(takenDamage);
                gm.selectedUnit.health -= takenDamage;
                if (enemy.nameID == "nox") gm.selectedUnit.isPoisoned = true;
            }
            else
            {
                Instantiate(shieldIcon, gm.selectedUnit.transform.position, Quaternion.identity);
                if (enemy.nameID == "nox") gm.selectedUnit.isPoisoned = true;
            }

            if (gm.selectedUnit.health <= 0)
            {
                if (enemy.nameID == "berserk")
                {
                    ParticleSystem par = Instantiate(enemy.yellEffect, enemy.transform.position, Quaternion.identity);
                    par.Play();
                    foreach (Unit unit in FindObjectsOfType<Unit>())
                    {
                        if (Mathf.Abs(enemy.transform.position.x - unit.transform.position.x) <= 1
                         && Mathf.Abs(enemy.transform.position.y - unit.transform.position.y) <= 1
                         && unit.nameID == "healer" && unit.playerNumber == enemy.playerNumber || unit == enemy)
                        {
                            unit.isEnraged = true;
                        }
                    }
                }
                if (enemy.nameID == "thief")
                {
                    if (enemy.playerNumber == 1 && gm.redGold >= 5)
                    {
                        gm.redGold -= 5;
                        gm.blueGold += 5;
                        ParticleSystem coin = Instantiate(enemy.goldPar, enemy.transform.position, Quaternion.identity);
                        coin.Play();
                    }
                    if (enemy.playerNumber == 2 && gm.blueGold >= 5)
                    {
                        gm.redGold += 5;
                        gm.blueGold -= 5;
                        ParticleSystem coin = Instantiate(enemy.goldPar, enemy.transform.position, Quaternion.identity);
                        coin.Play();
                    }
                }

                if (gm.selectedUnit.hasGraveEffect == true) Instantiate(gm.selectedUnit.skeleton, gm.selectedUnit.transform.position, Quaternion.identity);
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
            float XAbs = Mathf.Abs(transform.position.x - tile.transform.position.x);
            float YAbs = Mathf.Abs(transform.position.y - tile.transform.position.y);

            if (isSquare && XAbs <= tileSpeed && YAbs <= tileSpeed)
            {
                if (tile.IsClear() == true)
                {
                    tile.Highlight();
                }
            }
            else if (isCross && transform.position.y == tile.transform.position.y && XAbs <= tileSpeed ||
            isCross && transform.position.x == tile.transform.position.x && YAbs <= tileSpeed)
            {
                if (tile.IsClear() == true)
                {
                    int layer_mask = LayerMask.GetMask("RayCollider");
                    RaycastHit2D ray = Physics2D.Raycast(tile.transform.position, transform.position - tile.transform.position, 100, layer_mask);
                    Debug.DrawRay(tile.transform.position, transform.position - tile.transform.position, Color.red, 5, false);
                    Debug.Log(ray.collider.name);
                    if (ray.collider.gameObject == rayCollider.gameObject)
                    {
                        Debug.Log("I HIT THE TARGET!!!");
                        tile.Highlight();
                    }
                }
            }
            else if (isDiagonal && Mathf.Abs(YAbs - XAbs) <= 0 && XAbs <= tileSpeed && YAbs <= tileSpeed)
            {
                if (tile.IsClear() == true)
                {
                    tile.Highlight();
                }
            }
            else if (isQueen && Mathf.Abs(YAbs - XAbs) <= 0 && XAbs <= tileSpeed && YAbs <= tileSpeed ||
            isQueen && transform.position.y == tile.transform.position.y && XAbs <= tileSpeed ||
            isQueen && transform.position.x == tile.transform.position.x && YAbs <= tileSpeed)
            {
                if (tile.IsClear() == true)
                {
                    tile.Highlight();
                }
            }
            else if (isSquare == false && isCross == false && isDiagonal == false && isQueen == false && YAbs + XAbs <= tileSpeed)
            {
                if (tile.IsClear() == true)
                {
                    int layer_mask = LayerMask.GetMask("RayCollider");
                    RaycastHit2D ray = Physics2D.Raycast(tile.transform.position, transform.position - tile.transform.position, 100, layer_mask);
                    Debug.DrawRay(tile.transform.position, transform.position - tile.transform.position, Color.red, 5, false);
                    Debug.Log(ray.collider.name);
                    if (ray.collider.gameObject == rayCollider.gameObject)
                    {
                        Debug.Log("I HIT THE TARGET!!!");
                        tile.Highlight();
                    }
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
            if (nameID != "healer" && Mathf.Abs(transform.position.y - unit.transform.position.y) + Mathf.Abs(transform.position.x - unit.transform.position.x) <= attackRange)
            {
                if (unit.playerNumber != gm.playerTurn && hasAttacked == false)
                {
                    enemiesInRange.Add(unit);
                    unit.attackHighlight.SetActive(true);
                }
            }
            if (Mathf.Abs(transform.position.y - unit.transform.position.y) <= attackRange && Mathf.Abs(transform.position.x - unit.transform.position.x) <= attackRange)
            {
                if (nameID == "healer" && hasAttacked == false|| nameID == "armorer" && hasBuffed == false)
                {
                    if (unit.playerNumber == gm.playerTurn && gm.selectedUnit != unit && unit.health < unit.maxHealth + 1 && unit.shieldsLeft == 0 && unit.isCrystal == false)
                    {
                        if (playerNumber == 1 && gm.blueGold >= 15 || playerNumber == 2 && gm.redGold >= 15 || nameID == "healer")
                        {
                            if (nameID == "healer") unit.healHighlight.SetActive(true);
                            else if (nameID == "armorer") unit.armorHightlight.SetActive(true);
                            alliesInRange.Add(unit);
                        }
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
            unit.armorHightlight.SetActive(false);
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

        hasMoved = true;
        ResetAttackHighlights();
        GetEnemies();
        gm.UpdateMovesLeft();

        foreach (CoinColumn column in FindObjectsOfType<CoinColumn>())
        {
            column.CheckColumnStatus(this, true);
        }
    }

    private void OnMouseOver()
    {
        if (this.isCrystal == false && this.isHero == false)
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
    }

    private void OnMouseExit()
    {
        healthIndicator.transform.position = new Vector3(25, 12, 0);
    }

    public void UpdateDirection(Transform thisTransform, Transform facingTransform, Unit unit, bool isBuffing)
    {
        if (thisTransform.position.x < facingTransform.transform.position.x)
        {
            unit.anim.SetTrigger("Right");
        }
        if (thisTransform.position.x > facingTransform.position.x)
        {
            unit.anim.SetTrigger("Left");
        }
        if (thisTransform.position.y < facingTransform.position.y && unit.nameID != "archer" && unit.nameID != "nox" && isBuffing != true)
        {
            unit.anim.SetTrigger("Up");
        }
        if (thisTransform.position.y > facingTransform.position.y && unit.nameID != "archer" && unit.nameID != "nox" && isBuffing != true)
        {
            unit.anim.SetTrigger("Down");
        }
        if (thisTransform.position.x == facingTransform.transform.position.x && unit.nameID != "archer")
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
