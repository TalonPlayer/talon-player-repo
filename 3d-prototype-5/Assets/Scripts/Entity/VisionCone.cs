using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisionCone : MonoBehaviour
{
    public Entity entity;

    void OnTriggerEnter(Collider other)
    {
        if (entity.brain.isHuman)
        {
            Entity e = other.GetComponent<Entity>();
            if (!e) return;

            if (e.CompareTag("Zombie") && !entity.brain.visibleEnemies.Contains(e))
            {
                entity.brain.visibleEnemies.Add(e);
                entity.brain.isAggro = true;
                if (entity.brain.nearbyAllies != null)
                {
                    foreach (Entity a in entity.brain.nearbyAllies)
                    {
                        if (!a) continue;
                        foreach (Entity enemy in a.brain.visibleEnemies)
                        {
                            if (enemy && !entity.brain.visibleEnemies.Contains(enemy))
                            {
                                entity.brain.visibleEnemies.Add(enemy);
                                entity.brain.isAggro = true;
                                entity.body.Play("IsAggro", true);
                            }
                        }
                    }
                }

            }

        }
        else if (!entity.brain.isHuman)
        {
            Entity e = other.GetComponent<Entity>();
            if (!e) return;

            if (e.CompareTag("Human") && !entity.brain.visibleEnemies.Contains(e))
            {
                entity.brain.visibleEnemies.Add(e);
                entity.brain.isAggro = true;
                if (entity.brain.nearbyAllies != null)
                {
                    foreach (Entity a in entity.brain.nearbyAllies)
                    {
                        if (!a) continue;
                        foreach (Entity enemy in a.brain.visibleEnemies)
                        {
                            if (enemy && !entity.brain.visibleEnemies.Contains(enemy))
                            {
                                entity.brain.visibleEnemies.Add(enemy);
                                entity.brain.isAggro = true;
                                entity.body.Play("IsAggro", true);
                            }
                        }
                    }
                }
            }
        }

    }
}
