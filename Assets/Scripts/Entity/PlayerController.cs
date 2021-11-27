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

    private List<DropsHolder> dropsHolders = new List<DropsHolder>();

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
            inventory.fullness = DataManager.saveData.fullness;
        }

        inventory.UpdateStats(playerCombat);

        playerCombat.updated = UpdateUI;
        
        UpdateUI();
    }

    private void UpdateUI()
    {
        mainUI.rootVisualElement.Query<Label>("healthLabel").First().text = $"{playerCombat.GetStat(Stat.health)}/{playerCombat.GetStat(Stat.maxHealth)}";
        mainUI.rootVisualElement.Query<Label>("strengthLabel").First().text = playerCombat.GetStatForUI(Stat.strength);
        mainUI.rootVisualElement.Query<Label>("dexterityLabel").First().text = playerCombat.GetStatForUI(Stat.dexterity);
        mainUI.rootVisualElement.Query<Label>("knowledgeLabel").First().text = playerCombat.GetStatForUI(Stat.knowledge);
        mainUI.rootVisualElement.Query<Label>("defenseLabel").First().text = playerCombat.GetStatForUI(Stat.defense);

        mainUI.rootVisualElement.Query<Label>("fullnessLabel").First().text = inventory.fullness.ToString();

        // am I dead?
        if (playerCombat.GetStat(Stat.health) < 1)
        {
            DataManager.saveData = null;
            Destroy(gameObject);
        }

        var dropsContainer = mainUI.rootVisualElement.Query<VisualElement>("dropsContainer").First();
        dropsContainer.Clear();

        if (dropsHolders.Count > 0)
        {
            dropsContainer.visible = true;
            var dropsLabel = new Label()
            {
                text = "Drops"
            };
            dropsLabel.AddToClassList("items-text");
            dropsContainer.Add(dropsLabel);

            foreach (var holder in dropsHolders)
            {
                foreach (var item in holder.items)
                {
                    var btn = new Button()
                    {
                        text = $"{item.name} ({item.quantity})",
                    };

                    btn.AddToClassList("grab-button");

                    btn.clicked += () =>
                    {
                        holder.items.Remove(item);

                        inventory.AddItem(item.MakeItem());

                        if (holder.items.Count == 0)
                        {
                            dropsHolders.Remove(holder);
                            Destroy(holder.gameObject);
                        }

                        btn.Blur();

                        var evnt = MouseLeaveEvent.GetPooled();
                        evnt.target = dropsContainer;
                        dropsContainer.SendEvent(evnt);

                        UpdateUI();
                    };

                    dropsContainer.Add(btn);
                }
            }
        }
        else
        {
            // Send a mouse leave to remove focus in case it's captured
            dropsContainer.visible = false;
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
        // TODO: Probably make this more efficient
        inventory.UpdateStats(playerCombat);
        var collider = Physics2D.OverlapCircle(target, .1f, obsticals);

        var current = gameObject.transform.position;

        if (collider != null)
        {
            if (collider.gameObject.tag == "Enemies")
            {
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

            if (Physics2D.OverlapCircle(gameObject.transform.position, .1f, transitions))
            {
                TurnManager.messages.Clear();
                SceneManager.LoadScene(SceneManager.GetActiveScene().name, LoadSceneMode.Single);
            }

            TurnManager.RunTurns();

            dropsHolders = Physics2D.OverlapCircleAll(transform.position, 0.1f, LayerMask.GetMask("Drops"))
                .Select(x => x.GetComponent<DropsHolder>())
                .ToList();
        }

        RevealFOW();

        UpdateUI();
    }


}
