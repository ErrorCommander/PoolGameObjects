using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoEnqueue : MonoBehaviour
{
    private Queue<GameObject> _queue;

    public void Initialize(Queue<GameObject> queue)
    {
        if (_queue == null)
            _queue = queue;
    }

    private void OnDisable()
    {
        if (_queue != null)
            _queue.Enqueue(this.gameObject);
    }
}
