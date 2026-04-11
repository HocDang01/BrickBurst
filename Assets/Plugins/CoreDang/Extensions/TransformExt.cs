using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

public static class TransformExtension
{
    public static Transform[] FindAllChildren(this Transform gameObject)
    {
        List<Transform> list = new List<Transform>();
        foreach(Transform child in gameObject.transform) list.Add(child);
        return list.ToArray();
    }

    public static void RemoveAllChildren(this Transform transform)
    {
        List<Transform> list = new List<Transform>();
        foreach(Transform child in transform) list.Add(child);
        if(!Application.isPlaying) list.ForEach(child => Object.DestroyImmediate(child.gameObject));
        else list.ForEach(child => Object.Destroy(child.gameObject));
    }

    public static void RemoveChild(this Transform transform, string name)
    {
        Transform child = transform.Find(name);
        if(child) Object.Destroy(child.gameObject);
    }
}