using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Extensions;
using UnityEngine.UIElements;

public class PlayerController : MonoBehaviour
{
    public LayerMask fowMask;
    public LayerMask obsticals;
    public UIDocument mainUI;

    private Combat playerCombat;

    private void Awake()
    {
        playerCombat = GetComponent<Combat>();
        playerCombat.updated = UpdateUI;
        playerCombat.SetStats(30, 10, 10, 7, 6, 1);
        UpdateUI();
    }

    private void UpdateUI()
    {
        mainUI.rootVisualElement.Query<Label>("healthLabel").First().text = $"{playerCombat.GetStat(Stat.health)}/{playerCombat.GetStat(Stat.maxHealth)}";
        mainUI.rootVisualElement.Query<Label>("strengthLabel").First().text = playerCombat.GetStatForUI(Stat.strength);
        mainUI.rootVisualElement.Query<Label>("dexterityLabel").First().text = playerCombat.GetStatForUI(Stat.dexterity);
        mainUI.rootVisualElement.Query<Label>("knowledgeLabel").First().text = playerCombat.GetStatForUI(Stat.knowledge);
        mainUI.rootVisualElement.Query<Label>("defenseLabel").First().text = playerCombat.GetStatForUI(Stat.defense);
    }

    // Start is called before the first frame update
    void Start()
    {
        RevealFOW();
    }

    public void RevealFOW()
    {
        var fowColliders = Physics2D.OverlapCircleAll(transform.position, 3, fowMask);
        foreach (var fowCollider in fowColliders)
        {
            var renderer = fowCollider.GetComponent<SpriteRenderer>();
            renderer.color = renderer.color.Opaque();

            if (renderer.gameObject.name.StartsWith("Floor"))
            {
                Destroy(renderer.gameObject.GetComponent<BoxCollider2D>());
            }
        }
    }

    public void Move(Vector3 target)
    {
        var collider = Physics2D.OverlapCircle(target, .1f, obsticals);

        var current = gameObject.transform.position;

        if (collider != null)
        {
            Debug.Log(collider);
            if (collider.gameObject.tag == "Enemies")
            {
                var enemyTarget = collider.gameObject.GetComponent<Combat>();
                playerCombat.Attack(enemyTarget, Stat.strength, 8);

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

            RevealFOW();

            TurnManager.RunTurns();
        }
        
    }
}
