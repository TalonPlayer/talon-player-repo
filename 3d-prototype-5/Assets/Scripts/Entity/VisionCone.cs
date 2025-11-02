using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisionCone : MonoBehaviour
{
    public MyEntity entity;

    void OnTriggerEnter(Collider other)
    {
        if (entity.brain.isHuman)
        {
            MyEntity e = other.GetComponent<MyEntity>();
            if (!e) return;

            if (e.CompareTag("Zombie") && !entity.brain.visibleEnemies.Contains(e))
            {
                entity.brain.visibleEnemies.Add(e);
                entity.brain.isAggro = true;
                if (entity.brain.nearbyAllies != null)
                {
                    foreach (MyEntity a in entity.brain.nearbyAllies)
                    {
                        if (!a) continue;
                        foreach (MyEntity enemy in a.brain.visibleEnemies)
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
            MyEntity e = other.GetComponent<MyEntity>();
            if (!e) return;

            if (e.CompareTag("Human") && !entity.brain.visibleEnemies.Contains(e))
            {
                entity.brain.visibleEnemies.Add(e);
                entity.brain.isAggro = true;
                if (entity.brain.nearbyAllies != null)
                {
                    foreach (MyEntity a in entity.brain.nearbyAllies)
                    {
                        if (!a) continue;
                        foreach (MyEntity enemy in a.brain.visibleEnemies)
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
