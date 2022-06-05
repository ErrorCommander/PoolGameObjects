using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace PoolerSystem.Ref
{
    public class Pooler : MonoBehaviour
    {
        [SerializeField] private List<Pool> _pools;
        private Dictionary<GameObject, Pool> _poolsDictionary;

        public static Pooler Instance { get; private set; }
        private Pooler() { }

        static Pooler()
        {
            GameObject newGameObject = new GameObject("PoolerGameObjects");
            Instance = newGameObject.AddComponent<Pooler>();
        }

        private void Awake()
        {
            _pools = new List<Pool>();
            _poolsDictionary = new Dictionary<GameObject, Pool>();
        }

        public bool AddPool(Pool pool)
        {
            if (_poolsDictionary.ContainsKey(pool.Tag))
            {
                if (_poolsDictionary[pool.Tag].PoolSize < pool.PoolSize)
                    _poolsDictionary[pool.Tag].ChangePoolSize(pool.PoolSize);
                return false;
            }

            _poolsDictionary.Add(pool.Tag, pool);
            _pools = _poolsDictionary.Values.ToList();
            pool.Initialize();
            return true;
        }

        public bool AddPool(Pool pool, GameObject poolGameObject)
        {
            pool = new Pool(poolGameObject, pool.PoolSize);
            return AddPool(pool);
        }

        public GameObject[] GetTags()
        {
            return _poolsDictionary.Keys.ToArray();
        }

        public GameObject Spawn(GameObject tag, Vector3 position, Quaternion rotation)
        {
            if (!_poolsDictionary.ContainsKey(tag))
                return null;

            var pool = _poolsDictionary[tag];
            var result = pool.TakeGameObject();
            result.transform.SetPositionAndRotation(position, rotation);

            return result;
        }

        public GameObject Spawn(GameObject tag, Vector3 position, Vector3 rotation)
        {
            return Spawn(tag, position, Quaternion.Euler(rotation));
        }

        public GameObject Spawn(GameObject tag, Vector3 position)
        {
            return Spawn(tag, position, Quaternion.identity);
        }

        public void Clear(GameObject tag)
        {
            if (!_poolsDictionary.ContainsKey(tag))
            {
                Debug.Log("Tag not found");
                return;
            }

            Debug.Log("Pooler -> clear pool " + tag.name);
            _poolsDictionary[tag].ClearPool();

            _pools = _pools.Where((a) => a.Tag != tag).ToList();
            _poolsDictionary.Remove(tag);
        }

        public void Clear(params GameObject[] tags)
        {
            foreach (var tag in tags)
                Clear(tag);
        }

        private void ClearAll()
        {
            if (_poolsDictionary == null)
                return;

            Clear(_poolsDictionary.Keys.ToArray());
            _poolsDictionary.Clear();
            _pools.Clear();
        }

        private void OnDestroy()
        {
            ClearAll();
            Destroy(gameObject);
        }
    }
}