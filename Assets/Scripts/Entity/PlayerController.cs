using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Extensions;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using System.Linq;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour
{
    public LayerMask fowMask;
    public LayerMask obsticals;
    public LayerMask transitions;

    public UIDocument mainUI;


    private uint experience;
    private Combat playerCombat;
    private Inventory inventory;

    private void Awake()
    {
        inventory = GetComponent<Inventory>();
        playerCombat = GetComponent<Combat>();

        if (DataManager.saveData == null)
        {
            DataManager.saveData = new SaveData();
            
            DataManager.saveData.stats = playerCombat.stats;
            DataManager.saveData.effects = playerCombat.effects;

            playerCombat.SetStats(30, 10, 10, 7, 6, 1);
        }
        else
        {
            playerCombat.stats = DataManager.saveData.stats;
            playerCombat.effects = DataManager.saveData.effects;
        }

        inventory.UpdateStats(playerCombat);

        playerCombat.updated = combatUpdate;

        EventsDispatcher.statsChanged(playerCombat);
    }

    private void combatUpdate()
    {
        if (playerCombat.GetStat(Stat.health) < 1)
        {
            DataManager.saveData = null;
            Destroy(gameObject);
        }
    }

    public void Eat()
    {
        var fullness = playerCombat.GetStat(Stat.fullness);

        if (fullness > 0 && TurnManager.IsCalm())
        {
            var maxHealth = playerCombat.GetStat(Stat.maxHealth);
            var health = playerCombat.GetStat(Stat.health);

            if (health >= maxHealth)
            {
                return;
            }

            var maxHealAmount = maxHealth - health;

            var healAmount = Mathf.Clamp(maxHealth / 10, 0, maxHealAmount);

            health += healAmount;

            TurnManager.AddMessage($"You heal for {healAmount}");

            playerCombat.stats[Stat.health] = health;

            fullness--;

            DataManager.saveData.fullness = fullness;

            playerCombat.stats[Stat.fullness] = fullness;

            EventsDispatcher.statsChanged(playerCombat);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        RevealFOW();
    }

    public void RevealFOW()
    {
        var overlapDistance = inventory.HasLight ? 3f : 2.4f;
        var fowColliders = Physics2D.OverlapCircleAll(transform.position, overlapDistance, fowMask);
        foreach (var fowCollider in fowColliders)
        {
            var renderer = fowCollider.GetComponent<SpriteRenderer>();
            var distance = Mathf.Round(Vector3.Distance(fowCollider.transform.position, transform.position));

            if (inventory.HasLight)
            {
                if (distance > 2 && fowCollider.gameObject.tag != "Enemies")
                {
                    renderer.color = renderer.color.WithAlpha(0.5f);
                }
                else
                {
                    renderer.color = renderer.color.Opaque();
                }
            }
            else
            {
                if (distance > 1)
                {
                    if (fowCollider.gameObject.tag != "Enemies")
                    {
                        renderer.color = renderer.color.WithAlpha(0.5f);
                    }
                    else
                    {
                        // can't see enemies in the dark
                        renderer.color = renderer.color.Transparent();
                    }
                }
                else
                {
                    renderer.color = renderer.color.Opaque();
                }
            }
        }
    }

    public void Move(Vector3 target)
    {
        // check it 
        
        // TODO: Probably make this more efficient
        inventory.UpdateStats(playerCombat);
        var collider = Physics2D.OverlapCircle(target, .1f, obsticals);

        var current = gameObject.transform.position;

        if (collider != null)
        {
            
            if (collider.gameObject.tag == "Enemies")
            {
                // needed because the return statement
                RevealFOW();

                var enemyTarget = collider.gameObject.GetComponent<Combat>();

                if (inventory.MainWeaponSlot != null)
                {
                    playerCombat.Attack(enemyTarget, Stat.strength, inventory.MainWeaponSlot.maxHit);
                }
                else
                {
                    playerCombat.Attack(enemyTarget, Stat.strength, 1);
                }

                TurnManager.RunTurns(); // attacks are an action
                return;
            }

            var targetX = target.WithY(current.y);
            var targetY = target.WithX(current.x);

            if (Physics2D.OverlapCircle(targetX, .1f) == null)
            {
                target = targetX;
            }
            else if (Physics2D.OverlapCircle(targetY, .1f) == null)
            {
                target = targetY;
            }
            else
            {
                target = gameObject.transform.position;
            }
        }

        if (gameObject.transform.position != target)
        {
            gameObject.transform.position = target;

            if (Physics2D.OverlapCircle(gameObject.transform.position, .1f, transitions))
            {
                TurnManager.messages.Clear();
                SceneManager.LoadScene(SceneManager.GetActiveScene().name, LoadSceneMode.Single);
            }

            TurnManager.RunTurns();

            var dropsHolders = Physics2D.OverlapCircleAll(transform.position, 0.1f, LayerMask.GetMask("Drops"))
                .Select(x => x.GetComponent<DropsHolder>())
                .ToList();

            EventsDispatcher.dropsChanged(dropsHolders);

            // 
            Eat();
        }

        RevealFOW();

        EventsDispatcher.statsChanged(playerCombat);
    }


}
