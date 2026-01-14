using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Group
{
    public string groupName;
    public Entity leader;
    public List<Entity> members = new List<Entity>();
    public Objective currentObjective;
    public Sprite groupImage;
    public Transform target;
    public Group(string name)
    {
        groupName = name;
    }

    public void PromoteLeaderRandom()
    {
        if (leader != null && leader.isAlive)
            leader.brain.isLeader = false;

        leader = Helper.RandomElement(members);
        leader.brain.isLeader = true;
    }

    public void AssignLineFormation(float spacing = .75f)
    {
        if (leader == null || members == null || members.Count == 0) return;
        List<Entity> nonLeaders = members.FindAll(m => m != leader && m != null && m.isAlive);
        for (int i = 0; i < nonLeaders.Count; i++)
        {
            Entity m = nonLeaders[i];
            m.brain.inFormation = true;
            m.movement.agent.autoBraking = true;
            Vector3 targetPos = leader.transform.position;
            if ((i + 1) % 2 == 0)
                targetPos += leader.transform.right * (-(i + 1) * spacing);
            else
                targetPos += leader.transform.right * ((i + 1) * spacing);
            targetPos += leader.transform.forward * .5f;
            m.movement.MoveTo(targetPos);
            m.movement.agent.speed = leader.movement.agent.speed * 1.5f;
        }
    }

    public void BreakFormation()
    {
        foreach (Entity e in members)
        {
            e.brain.inFormation = false;
        }
    }
}
