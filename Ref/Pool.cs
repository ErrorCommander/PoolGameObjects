using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PoolerSystem.Ref
{
    [System.Serializable]
    public class Pool
    {
        [SerializeField] private GameObject _prefab;
        [SerializeField] private int _initPoolSize;

        public GameObject Tag => _prefab;
        public int PoolSize => _listGO == null ? _initPoolSize : _listGO.Count;

        private Queue<GameObject> _queueReadyGO;
        private List<GameObject> _listGO;
        private Transform _parentPool;
        private ushort _objID = 0;

        public Pool(GameObject prefab, int poolSize)
        {
            _prefab = prefab;
            _initPoolSize = poolSize;
        }

        public void Initialize()
        {
            _queueReadyGO = new Queue<GameObject>();
            _listGO = new List<GameObject>();
            _parentPool = new GameObject("Pool: " + _prefab.name).transform;
            _parentPool.transform.SetParent(Pooler.Instance.transform);
            FillPool();
        }

        public void ChangePoolSize(int poolSize)
        {
            if (_queueReadyGO.Count >= poolSize)
                return;

            _initPoolSize = poolSize;
            FillPool();
        }

        public GameObject TakeGameObject()
        {
            GameObject result;

            if (_queueReadyGO.Count > 0)
                result = _queueReadyGO.Dequeue();
            else
                result = AddGameObject();

            result.SetActive(true);
            return result;
        }

        public void ClearPool()
        {
            _initPoolSize = 0;
            foreach (var go in _listGO)
                MonoBehaviour.Destroy(go);

            _queueReadyGO.Clear();
            _listGO.Clear();
            Debug.Log("Pool is Clean");
        }

        public void DestroyPool()
        {
            ClearPool();
            MonoBehaviour.Destroy(_parentPool.gameObject);
            Debug.Log("Pool destroyed");
        }

        private void FillPool()
        {
            if (_listGO.Count >= _initPoolSize)
                return;

            for (int i = _listGO.Count; i < _initPoolSize; i++)
                AddGameObject();
        }

        private GameObject AddGameObject()
        {
            GameObject newObj = MonoBehaviour.Instantiate(_prefab, _parentPool);
            newObj.SetActive(false);
            newObj.name = string.Format("{0} {1:000}", _prefab.name, _objID++);
            newObj.AddComponent<AutoEnqueue>().Initialize(_queueReadyGO);
            _listGO.Add(newObj);
            _queueReadyGO.Enqueue(newObj);

            return newObj;
        }
    }
}