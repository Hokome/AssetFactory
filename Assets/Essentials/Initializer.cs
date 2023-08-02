using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

using Object = UnityEngine.Object;

namespace AssetFactory
{
    [CreateAssetMenu(fileName = "Initializer", menuName = "Initializer", order = 1)]
    public class Initializer : ScriptableObject
    {
        const string INITIALIZER_PATH = "Assets/Essentials/Initializer.asset";

        [SerializeField] private Object[] initializeOnSceneLoad = new Object[0];

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void InitializeObjects()
        {
            Initializer obj = AssetDatabase.LoadAssetAtPath<Initializer>(INITIALIZER_PATH);
            if (obj == null)
            {
                Debug.LogError("The initializer object is null. Check the path of the initializer.");
#if UNITY_EDITOR
                EditorApplication.isPlaying = false;
#else
                UnityEngine.Diagnostics.Utils.ForceCrash(UnityEngine.Diagnostics.ForcedCrashCategory.FatalError);
#endif
            }
            if (obj.initializeOnSceneLoad == null) return;
            for (int i = 0; i < obj.initializeOnSceneLoad.Length; i++)
            {
                Instantiate(obj.initializeOnSceneLoad[i]);
            }
        }
    }
}
