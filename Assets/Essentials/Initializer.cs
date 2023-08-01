using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace AssetFactory
{

    [CreateAssetMenu(fileName = "Initializer", menuName = "Initializer", order = 1)]
    public class Initializer : ScriptableObject
    {
        const string INITIALIZER_PATH = "Assets/Initializer.asset";

        [SerializeField] private Object[] initializeOnSceneLoad = new Object[0];

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void InitializeObjects()
        {
            Initializer obj = AssetDatabase.LoadAssetAtPath<Initializer>(INITIALIZER_PATH);
            if (obj.initializeOnSceneLoad == null) return;
            for (int i = 0; i < obj.initializeOnSceneLoad.Length; i++)
            {
                Instantiate(obj.initializeOnSceneLoad[i]);
            }
        }
    }
}
