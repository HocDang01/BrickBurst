#if UNITY_WEBGL && !UNITY_EDITOR
using System.Runtime.InteropServices;
#endif

namespace BayatGames.SaveGameFree
{
    internal class SaveGameWebGL
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        static extern bool ExistJS(string filePath);

        [DllImport("__Internal")]
        static extern void DeleteJS(string filePath);

        [DllImport("__Internal")]
        static extern void DeleteAllJS();

        [DllImport("__Internal")]
        static extern void SaveDataJS(string filePath, string data);

        [DllImport("__Internal")]
        static extern string ReadDataJS(string filePath);
#else
        static bool ExistJS(string filePath) { return false; }
        static void SaveDataJS(string filePath, string data) { }
        static void DeleteJS(string filePath) { }
        static void DeleteAllJS() { }
        static string ReadDataJS(string filePath) { return ""; }
#endif
        private static string ToDataName(string filePath)
        {
            return "/savegame" + filePath.Substring(filePath.LastIndexOf("/"));
        }

        internal static bool Exist(string filePath)
        {
            return ExistJS(ToDataName(filePath));
        }

        internal static void Delete(string filePath)
        {
            DeleteJS(ToDataName(filePath));
        }

        internal static void DeleteAll()
        {
            DeleteAllJS();
        }

        internal static void SaveData(string filePath, string data)
        {
            SaveDataJS(ToDataName(filePath), data);
        }

        internal static string ReadData(string filePath)
        {
            return ReadDataJS(ToDataName(filePath));
        }
    }
}