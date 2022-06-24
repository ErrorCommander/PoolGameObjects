using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

namespace PoolerSystem.Tag
{
    public class Pooler : MonoBehaviour
    {
        [SerializeField] private List<Pool> _pools;
        private Dictionary<string, Pool> _poolsDictionary;

        public static Pooler Instance => _instance ??= new GameObject("Pooler GameObjects").AddComponent<Pooler>();
        private static Pooler _instance;
        private Pooler() { }

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
            pool = new Pool(pool.Tag, poolGameObject, pool.PoolSize);
            return AddPool(pool);
        }

        public string[] GetTags()
        {
            return _poolsDictionary.Keys.ToArray();
        }

        public GameObject Spawn(string tag, Vector3 position, Quaternion rotation)
        {
            if (!_poolsDictionary.ContainsKey(tag))
                return null;

            var pool = _poolsDictionary[tag];
            var result = pool.TakeGameObject();
            result.transform.SetPositionAndRotation(position, rotation);

            return result;
        }

        public GameObject Spawn(string tag, Vector3 position, Vector3 rotation)
        {
            return Spawn(tag, position, Quaternion.Euler(rotation));
        }

        public GameObject Spawn(string tag, Vector3 position)
        {
            return Spawn(tag, position, Quaternion.identity);
        }

        public bool ClearPool(string tag)
        {
            if (!_poolsDictionary.ContainsKey(tag))
            {
                Debug.Log("Tag not found");
                return false;
            }

            Debug.Log("Pooler -> clear pool " + tag);
            _poolsDictionary[tag].ClearPool();
            return true;
        }

        public void ClearPool(params string[] tags)
        {
            foreach (var tag in tags)
                ClearPool(tag);
        }

        public bool DestroyPool(string tag)
        {
            if (!_poolsDictionary.ContainsKey(tag))
            {
                Debug.Log("Tag not found");
                return false;
            }

            Debug.Log("Pooler -> Destroy pool " + tag);
            _poolsDictionary[tag].DestroyPool();
            _pools.Remove(_poolsDictionary[tag]);
            _poolsDictionary.Remove(tag);
            return true;
        }

        public void DestroyPool(params GameObject[] tags)
        {
            foreach (var tag in tags)
                DestroyPool(tag);
        }

        //-------------PRIVATE METODS-------------

        private void Initialize()
        {
            _instance ??= this;
            Debug.Log("Actual " + Instance.name);
            _poolsDictionary = new Dictionary<string, Pool>();
            _pools ??= new List<Pool>();
            foreach (var pool in _pools)
                AddPool(pool);

            SceneManager.sceneUnloaded += UnloadScene;
        }

        private void ClearAll()
        {
            if (_poolsDictionary == null)
                return;

            ClearPool(_poolsDictionary.Keys.ToArray());
            _poolsDictionary.Clear();
            _pools.Clear();
        }

        private void ClearAllPools()
        {
            if (_pools == null || _pools.Count == 0)
                return;

            ClearPool(GetTags());
        }

        private void OnDestroy()
        {
            ClearAll();
            Destroy(gameObject);
        }

        private void UnloadScene(Scene arg0)
        {
            _instance = null;
            SceneManager.sceneUnloaded -= UnloadScene;
        }

        private void Awake()
        {
            Initialize();
        }
    }
}