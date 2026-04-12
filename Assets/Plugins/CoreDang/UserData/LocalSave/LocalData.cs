using BayatGames.SaveGameFree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace CoreDang
{
    public class LocalData : SingletonMono<LocalData>
    {
#if UNITY_EDITOR
        const bool useEncodeFileName = false;
        const bool useEncode = false;
#else
        const bool useEncodeFileName = true;
        const bool useEncode = true;
#endif

        const string password = "vgamespass";

#if !UNITY_EDITOR && UNITY_WEBGL
        float SaveDataFrequency { get; set; } = 2.0f; //min
#else
        float SaveDataFrequency { get; set; } = 20.0f; //min
#endif

        float LastSaveTime { get; set; } = 0;

        readonly Dictionary<string, BaseUserData> userDatas = new Dictionary<string, BaseUserData>();

        /// <summary>
        /// Global flag that allows all data can be writen on disk or not
        /// </summary>
        public static bool s_allowSave = true;

        public static T Get<T>() where T : BaseUserData
        {
            return Ins?.Load<T>();
        }

        public static void Save(BaseUserData data)
        {
            Ins._Save(data);
        }

        public static void DeleteAll()
        {
            foreach (var pair in Ins.userDatas.ToList())
            {
                var data = pair.Value;
                if (data.preventDelete) continue;

                data.OnWillDeleted();

                Type type = data.GetType();
                string id = type.Name;
                SaveGame.Delete(EncodeString(id));

                Ins.userDatas.Remove(pair.Key);
            }
        }

        static string EncodeString(string id)
        {
            var encodeId = useEncodeFileName ? Convert.ToBase64String(Encoding.Default.GetBytes(id)) : id;
            if (Application.platform == RuntimePlatform.WindowsPlayer)
            {
                return encodeId + "_win";
            }
            else
            {
                return encodeId;
            }
        }

        void _Save(BaseUserData data)
        {
            if (!data.saveLocally) return;

            data.dirty = false;
            Type type = data.GetType();
            string id = type.Name;
            SaveGame.Save(EncodeString(id), data, useEncode, password);
        }

        T Load<T>() where T : BaseUserData
        {
            Type type = typeof(T);
            string id = type.Name;
            string identifier = EncodeString(id);

            if (!userDatas.ContainsKey(id))
            {
                if (!SaveGame.Exists(identifier))
                {
                    CreateNew<T>(type, id);
                }
                else
                {
                    try
                    {
                        T obj = SaveGame.Load<T>(identifier, useEncode, password);
                        userDatas[id] = obj;
                        obj.OnLoad();
                    }
                    catch (Exception e)
                    {
                        // Failed to load
                        Debug.LogWarning("LocalData failed to " + type.Name + " data with Exeception: " + e.Message);

                        // create new data to continue playing
                        CreateNew<T>(type, id);

                    }
                }
            }

            return (T)userDatas[id];
        }

        void CreateNew<T>(Type type, string id) where T : BaseUserData
        {
            T obj = (T)Activator.CreateInstance(type);
            userDatas[id] = obj;
            obj.OnInit();
            obj.OnLoad();
            _Save(obj);
        }

        public void LateUpdate()
        {
            if (LastSaveTime < Time.unscaledTime)
            {
                SaveAll();
                LastSaveTime = Time.unscaledTime + SaveDataFrequency;
            }
        }

        private void OnApplicationPause(bool pause)
        {
            if (pause)
            {
                foreach (var userData in userDatas.Values)
                    userData.OnPause();

                SaveAll();
                LastSaveTime = Time.unscaledTime + SaveDataFrequency;
            }
        }

        private void OnApplicationQuit()
        {
            foreach (var userData in userDatas.Values)
                userData.OnQuit();

            SaveAll(true);
        }

        private void SaveAll(bool forced = false)
        {
            if (!s_allowSave) return;

            if (forced)
            {
                foreach (var userData in userDatas.Values)
                    _Save(userData);
            }
            else
            {
                foreach (var userData in userDatas.Values)
                {
                    if (userData.dirty)
                    {
                        _Save(userData);
                    }
                }
            }
        }
    }

#if UNITY_EDITOR
    public static class DataLocalTool
    {
        [MenuItem("CoreDang/Clear Save Data", false, 99)]
        private static void ResetLocalData()
        {
            SaveGame.DeleteAll();
            PlayerPrefs.DeleteAll();
        }
    }
#endif
}