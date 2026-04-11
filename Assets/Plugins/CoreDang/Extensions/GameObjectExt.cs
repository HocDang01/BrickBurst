using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

public static class GameObjectExt
{
    public static List<GameObject> FindAllChildren(this GameObject gameObject)
    {
        List<GameObject> list = new List<GameObject>();
        foreach(Transform child in gameObject.transform) list.Add(child.gameObject);
        return list;
    }

    public static GameObject Find(this GameObject gameObject, string name)
    {
        Transform child = gameObject.transform.Find(name);
        return child ? child.gameObject : null;
    }

    public static void Destroy(this MonoBehaviour mono)
    {
        if(!Application.isPlaying) Object.DestroyImmediate(mono.gameObject);
        else Object.Destroy(mono.gameObject);
    }

    public static void CallInNextFrame(this MonoBehaviour mono, Action action)
    {
        mono.StartCoroutine(DelayNextFrame(action));
    }
    static IEnumerator DelayNextFrame(Action action)
    {
        yield return null;
        action();
    }

    public static void RemoveAllChildren(this MonoBehaviour mono)
    {
        List<Transform> list = new List<Transform>();
        foreach(Transform child in mono.transform) list.Add(child);
        if(Application.isEditor)
            list.ForEach(child => Object.DestroyImmediate(child.gameObject));
        else
            list.ForEach(child => Object.Destroy(child.gameObject));
    }

    public static void RemoveAllComponents(this GameObject gameObject)
    {
        gameObject.GetComponents<Component>().ForEach(component =>
        {
            if(!component.GetType().Equals(typeof(Transform)))
            {
                if(!Application.isPlaying) Object.DestroyImmediate(component);
                else Object.Destroy(component);
            }
        });
    }

    public static void RemoveAllComponents<T>(this GameObject gameObject) where T : Component
    {
        T[] components = gameObject.GetComponents<T>();
        if(!Application.isPlaying) components.ForEach(comp => Object.DestroyImmediate(comp));
        else components.ForEach(comp => Object.Destroy(comp));
    }

    public static void RemoveComponent<T>(this GameObject gameObject) where T : Component
    {
        T component = gameObject.GetComponent<T>();
        if (!component) return;
        if(!Application.isPlaying) Object.DestroyImmediate(component);
        else Object.Destroy(component);
    }

    public static void RemoveAllChildren(this GameObject gameObject)
    {
        List<Transform> list = new List<Transform>();
        foreach(Transform child in gameObject.transform) list.Add(child);
        if(!Application.isPlaying)
            list.ForEach(child => Object.DestroyImmediate(child.gameObject));
        else
            list.ForEach(child => Object.Destroy(child.gameObject));
    }
}