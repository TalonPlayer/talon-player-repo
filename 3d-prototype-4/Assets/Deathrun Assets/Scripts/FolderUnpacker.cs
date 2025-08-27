using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class FolderUnpacker
{
    public static List<T> Unpack<T>(Transform folder)
    {
        List<T> list = new List<T>();

        for (int i = 0; i < folder.childCount; i++) {
            list.Add(folder.GetChild(i).GetComponent<T>());
        }
        return list;
    }
}
