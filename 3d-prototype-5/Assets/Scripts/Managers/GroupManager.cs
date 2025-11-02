using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroupManager : MonoBehaviour
{
    public static GroupManager Instance;
    public List<Group> groups;
    void Awake()
    {
        Instance = this;
    }
    public void AssignToGroup(string _groupName, MyEntity entity, Sprite logo = null)
    {
        Group newGroup = new Group(_groupName);

        if (logo != null)
            newGroup.groupImage = logo;

        foreach (Group group in groups)
        {
            if (group.groupName == _groupName)
            {
                newGroup = group;
            }
        }

        newGroup.members.Add(entity);

        groups.Add(newGroup);

        entity.group = newGroup;
        entity.brain.hasGroup = true;
    }
}
