using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CoreDang
{
    public class PoolSystem : SingletonMono<PoolSystem>
    {
        private static readonly int sizePreload = 5;

        public static readonly Dictionary<int, GameObject> cachedObjects = new Dictionary<int, GameObject>();
        public static readonly Dictionary<int, GameObject> prefabObjectMaps = new Dictionary<int, GameObject>();

        public static T RequireObject<T>(T source) where T : Component => RequireObject(source.gameObject).GetComponent<T>();
        public static T RequireObject<T>(GameObject source) where T : Component => RequireObject(source).GetComponent<T>();

        public static T RequireObject<T>(GameObject source, int preloadCount) where T : Component => RequireObject(source, preloadCount).GetComponent<T>();

        public static void SetObject(string key, GameObject gameObject)
        {
            int hash = key.GetHashCode();
            if (cachedObjects.ContainsKey(hash))
            {
                if (cachedObjects[hash] != gameObject)
                    cachedObjects[hash] = gameObject;
            }
            else
                cachedObjects[hash] = gameObject;
        }

        public static GameObject RequireObject(string key)
        {
            int hash = key.GetHashCode();
            if (!cachedObjects.TryGetValue(hash, out GameObject res))
            {
                // try get from resources
                res = RequireObjectFromResources(key);
                if (!res)
                    Debug.LogError($"Call SetObject first to make pool for key '{key}'");

                return res;
            }

            return RequireObject(res);
        }

        public static GameObject RequireObjectFromResources(string path)
        {
            int hash = path.GetHashCode();
            if (!cachedObjects.TryGetValue(hash, out GameObject res))
            {
                res = (GameObject)Resources.Load(path, typeof(GameObject));
                if (!res)
                    Debug.LogError("DefaultPool failed to load \"" + path + "\" . Make sure it's in a \"Resources\" folder.");

                cachedObjects[hash] = res;
            }

            return RequireObject(res);
        }

        public static GameObject RequireObject(GameObject source)
        {
            return RequireObject(source, sizePreload);
        }

        public static GameObject RequireObject(GameObject source, int preloadCount)
        {
            var pool = Ins.GetPool(source.GetInstanceID());
            if (pool == null) pool = Ins.CreatePool(source);

            pool.UpdateFreeObject();
            if (pool.FreeCount == 0)
                pool.PreLoad(preloadCount);

            var obj = pool.RequiredObject();
            prefabObjectMaps[obj.GetInstanceID()] = source;
            return obj;
        }

        public static void ReturnObject<T>(T returned) where T : Component => ReturnObject(returned.gameObject);

        public static void ReturnObject(GameObject returnedObject)
        {
            int instanceID = returnedObject.GetInstanceID();
            if (prefabObjectMaps.TryGetValue(instanceID, out GameObject source))
            {
                ReturnObject(source, returnedObject);
                prefabObjectMaps.Remove(instanceID);
            }
            else
            {
                Debug.LogWarning("PoolSystem: " + returnedObject.name + " does not exist in Prefab Object Maps");
            }
        }

        static void ReturnObject(GameObject source, GameObject returnedObject)
        {
            if (!Ins) return;

            int uniqueId = source.GetInstanceID();
            var pool = Ins.GetPool(uniqueId);
            if (pool == null)
            {
                Debug.Log("Object with Id: " + uniqueId + " does not have pool");
                return;
            }
            pool.ReturnObject(returnedObject);
        }

        public static void ReleasePool<T>(T source) where T : Component => Ins?._ReleasePool(source.gameObject);
        public static void ReleasePool(GameObject source) => Ins?._ReleasePool(source);

        public static void Cleanup() => Ins?._Cleanup();

        // INTERNAL SYSTEM ----------------------------------------------------------------------------------------------------------------------------------------
        public bool hideObjectsInHierarchy;
        readonly Dictionary<int, Pool> pools = new Dictionary<int, Pool>();
        readonly Dictionary<int, GameObject> containers = new Dictionary<int, GameObject>();

        Pool GetPool(int instanceId)
        {
            return pools.TryGetValue(instanceId, out Pool pool) ? pool : null;
        }

        Pool CreatePool(GameObject source)
        {
            int instanceId = source.GetInstanceID();
            if (pools.TryGetValue(instanceId, out Pool pool)) return pool;
            if (source.GetComponent<RectTransform>()) pool = new RectTransformPool(source);
            else pool = new Pool(source);
            pools[instanceId] = pool;
            return pool;
        }

        GameObject GetContainer(int instanceId, string name)
        {
            if (!containers.TryGetValue(instanceId, out GameObject container) || container == null)
            {
                container = new GameObject(name);
                containers[instanceId] = container;
                container.transform.parent = transform;

                GetPool(instanceId).container = container;
            }
            return containers[instanceId];
        }

        void _ReleasePool(GameObject source) => _ReleasePool(source.GetInstanceID());

        void _ReleasePool(int instanceId)
        {
            if (Ins.pools.TryGetValue(instanceId, out Pool pool))
            {
                var list = pool.AllList;
                foreach (var item in list)
                    Destroy(item);

                pools.Remove(instanceId);
            }
        }

        void _Cleanup()
        {
            foreach (var instanceId in containers.Keys)
            {
                if (Ins.pools.TryGetValue(instanceId, out Pool pool))
                {
                    pool.CleanUp();
                }
            }
        }

        class Pool
        {
            readonly List<GameObject> freeObjects = new List<GameObject>();
            readonly List<GameObject> requiredObjects = new List<GameObject>();

            internal GameObject container;
            readonly GameObject source;
            readonly int instanceId;

            internal Pool(GameObject source)
            {
                this.source = source;
                instanceId = source.GetInstanceID();
            }

            internal int FreeCount => freeObjects.Count;

            internal void CleanUp()
            {
                UpdateFreeObject();
                foreach (var obj in freeObjects)
                    Destroy(obj);
                freeObjects.Clear();
            }

            static Vector3 s_initPosition = Vector3.zero;
            internal void PreLoad(int size)
            {
                GameObject container = Ins.GetContainer(instanceId, source.name + "s");
                container.transform.position = s_initPosition;

                GameObject newObj;
                for (int i = 0; i < size; i++)
                {
                    newObj = Instantiate(source, s_initPosition, Quaternion.identity, container.transform);
                    newObj.SetActive(false);
                    AddNewObject(newObj);
                    if (Ins.hideObjectsInHierarchy)
                        newObj.hideFlags = HideFlags.HideInHierarchy;
                }
            }

            internal GameObject RequiredObject()
            {
                if (freeObjects.Count == 0) throw new IndexOutOfRangeException("List is empty!");
                GameObject obj = freeObjects[0];
                freeObjects.Remove(obj);
                obj.SetActive(true);
                requiredObjects.Add(obj);
                return obj;
            }
            internal void ReturnObject(GameObject obj)
            {
                obj.SetActive(false);
                ReturnToContainer(obj);
                requiredObjects.Remove(obj);
                if (!freeObjects.Contains(obj))
                    freeObjects.Add(obj);
            }

            internal IEnumerable<GameObject> FreeList => freeObjects;

            internal IEnumerable<GameObject> AllList => freeObjects.Concat(requiredObjects);

            internal void UpdateFreeObject()
            {
                if (freeObjects.Count == 0 && requiredObjects.Count > 0)
                {
                    var list = requiredObjects.ToList();
                    foreach (var obj in list)
                    {
                        if (!obj)
                        {
                            requiredObjects.Remove(obj);
                            continue;
                        }

                        if (!obj.activeSelf)
                        {
                            ReturnObject(obj);
                        }
                    }
                }
            }

            internal virtual void ReturnToContainer(GameObject obj)
            {
                obj.transform.parent = container.transform;
                obj.transform.localPosition = Vector3.zero;
            }

            protected virtual void AddNewObject(GameObject newObj)
            {
                freeObjects.Add(newObj);
            }

            protected virtual void CleanUp(GameObject obj)
            {
                Destroy(obj);
            }
        }

        class RectTransformPool : Pool
        {
            readonly Dictionary<int, RectTransform> rects = new Dictionary<int, RectTransform>();

            internal RectTransformPool(GameObject source) : base(source)
            {
            }

            protected override void AddNewObject(GameObject newObj)
            {
                base.AddNewObject(newObj);
                rects[newObj.GetInstanceID()] = newObj.GetComponent<RectTransform>();
            }

            internal override void ReturnToContainer(GameObject obj)
            {
                if (rects.TryGetValue(obj.GetInstanceID(), out var rect))
                {
                    rect.SetParent(container.transform);
                }
            }

            protected override void CleanUp(GameObject obj)
            {
                base.CleanUp(obj);
                if (obj) rects.Remove(obj.GetInstanceID());
            }
        }
    }
}