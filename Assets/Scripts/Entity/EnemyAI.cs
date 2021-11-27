using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Extensions;
using UnityEngine.U2D;

public class EnemyAI : MonoBehaviour
{
    public LayerMask layerMask;
    public Enemy enemy;
    public SpriteAtlas sprites;

    public Inventory inventory;

    private Combat combat;

    private bool moveOnUpdate = false;

    void Start()
    {
        GetComponent<SpriteRenderer>().sprite = sprites.GetSprite("rl_" + enemy.spriteLetter);
        
        // always start these guys transparent
        GetComponent<SpriteRenderer>().color = ColorsManager.GetColor(enemy.color).Transparent();

        combat = gameObject.GetComponent<Combat>();

        combat.SetStats(enemy.health, enemy.mana, enemy.strength, enemy.dexterity, enemy.knowledge, enemy.defense);

        combat.updated = CombatUpdated;
    }

    private void CombatUpdated()
    {
        if (combat.GetStat(Stat.health) < 1)
        {
            var drop = enemy.dropTable?.GetItem();
            if (drop != null && inventory != null)
            {
                inventory.currentDrops.Add(drop.MakeItem());
            }

            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        if (moveOnUpdate)
        {
            Collider2D colliderplayer = Physics2D.OverlapCircle(transform.position, 4, LayerMask.GetMask("Player"));

            if (colliderplayer != null)
            {
                MoveTowardPlayer(colliderplayer);
            }
            else
            {
                MakeMove();
            }
            
            moveOnUpdate = false;
        }
    }

    private void MoveTowardPlayer(Collider2D colliderplayer)
    {
        var playerPosition = colliderplayer.gameObject.transform.position;
        var current = transform.position;

        Debug.Log(current);

        var diff = (playerPosition - current).Clamp();
        var target = current + diff;

        var targetX = target.WithY(current.y);
        var targetY = target.WithX(current.x);

        if (playerPosition == target)
        {
            // Don't move into the player!
            combat.Attack(colliderplayer.gameObject.GetComponent<Combat>(), Stat.strength, enemy.maxHit);
            return;
        }

        if (Physics2D.OverlapCircle(targetX, .1f, layerMask) == null)
        {
            target = targetX;
        }
        else if (Physics2D.OverlapCircle(targetY, .1f, layerMask) == null)
        {
            target = targetY;
        }
        else
        {
            target = gameObject.transform.position;
        }

        transform.position = target;
    }

    private void MakeMove()
    {
        var corner = transform.position;


        List<Vector2> hashMap = new List<Vector2>();
        hashMap.Add(new Vector2(-1, 1));
        hashMap.Add(new Vector2(0, 1));
        hashMap.Add(new Vector2(1, 1));

        hashMap.Add(new Vector2(-1, 0));
        hashMap.Add(new Vector2(1, 0));

        hashMap.Add(new Vector2(-1, -1));
        hashMap.Add(new Vector2(0, -1));
        hashMap.Add(new Vector2(1, -1));

        Vector2 target = Vector2.zero;

        while (hashMap.Count > 0)
        {
            var temp = hashMap[Random.Range(0, hashMap.Count - 1)];
            target = temp + corner.ToVector2();

            if (!Physics2D.OverlapCircle(target, .1f, layerMask))
            {
                break;
            }

            hashMap.Remove(temp);
        }
        
        // If there's even one left do the move, otherwise no option was found
        if (hashMap.Count > 0)
        {
            transform.position = target;
        }
        
    }

    public void MakeTurn()
    {
        moveOnUpdate = true;
    }

    private void OnEnable()
    {
        TurnManager.Register(this);
    }

    private void OnDisable()
    {
        // This can fail if the turn manager is killed first
        TurnManager.DeRegister(this);
    }
}
