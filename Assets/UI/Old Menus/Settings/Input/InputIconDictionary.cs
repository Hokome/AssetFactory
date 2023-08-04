using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AssetFactory.Legacy.UI.InputRebinding
{

    [CreateAssetMenu(fileName = "Input Icon Dictionary", menuName = "Input Icon Dictionary", order = 1)]
    public class InputIconDictionary
        : ScriptableObject, IDictionary<string, Sprite>, ISingleton<InputIconDictionary>
    {
        [InspectorName("Pairs")]
        [SerializeField]
        private InputIconIdentifier[] list;

        private Dictionary<string, Sprite> dictionary;
        public Dictionary<string, Sprite> Dictionary
        {
            get
            {
                if (dictionary == null)
                    dictionary = new Dictionary<string, Sprite>(EnumeratePairs());
                return dictionary;
            }
        }

        public static InputIconDictionary Inst
        {
            get => ISingleton<InputIconDictionary>.Inst;
            set => ISingleton<InputIconDictionary>.Inst = value;
        }

        public static bool Exists => ISingleton<InputIconDictionary>.Exists;

        public Sprite this[string key]
        {
            get => (Dictionary)[key];
            set => (Dictionary)[key] = value;
        }
        public ICollection<string> Keys => (Dictionary).Keys;
        public ICollection<Sprite> Values => (Dictionary).Values;
        public int Count => ((ICollection<KeyValuePair<string, Sprite>>)Dictionary).Count;
        public bool IsReadOnly => ((ICollection<KeyValuePair<string, Sprite>>)Dictionary).IsReadOnly;
        public void Add(string key, Sprite value) => (Dictionary).Add(key, value);
        public void Add(KeyValuePair<string, Sprite> item)
            => ((ICollection<KeyValuePair<string, Sprite>>)Dictionary).Add(item);
        public void Clear() => ((ICollection<KeyValuePair<string, Sprite>>)Dictionary).Clear();
        public bool Contains(KeyValuePair<string, Sprite> item)
            => ((ICollection<KeyValuePair<string, Sprite>>)Dictionary).Contains(item);
        public bool ContainsKey(string key) => (Dictionary).ContainsKey(key);
        public void CopyTo(KeyValuePair<string, Sprite>[] array, int arrayIndex)
            => ((ICollection<KeyValuePair<string, Sprite>>)Dictionary).CopyTo(array, arrayIndex);
        public IEnumerator<KeyValuePair<string, Sprite>> GetEnumerator()
            => ((IEnumerable<KeyValuePair<string, Sprite>>)Dictionary).GetEnumerator();
        public bool Remove(string key) => (Dictionary).Remove(key);
        public bool Remove(KeyValuePair<string, Sprite> item)
            => ((ICollection<KeyValuePair<string, Sprite>>)Dictionary).Remove(item);
        public bool TryGetValue(string key, out Sprite value)
            => (Dictionary).TryGetValue(key, out value);
        IEnumerator IEnumerable.GetEnumerator()
            => ((IEnumerable)Dictionary).GetEnumerator();

        public static explicit operator Dictionary<string, Sprite>(InputIconDictionary d) => d.Dictionary;

        private IEnumerable<KeyValuePair<string, Sprite>> EnumeratePairs()
        {
            foreach (var item in list)
                yield return item;
        }
    }
    [Serializable]
    public struct InputIconIdentifier
    {
        public string key;
        public Sprite sprite;

        public static implicit operator KeyValuePair<string, Sprite>(InputIconIdentifier id)
            => new(id.key, id.sprite);
    }
}
