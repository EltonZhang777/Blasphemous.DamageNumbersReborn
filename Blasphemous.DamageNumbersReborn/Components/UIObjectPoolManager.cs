using Blasphemous.DamageNumbersReborn.Extensions;
using Blasphemous.Framework.UI;
using Framework.Pooling;
using System.Collections.Generic;
using UnityEngine;

namespace Blasphemous.DamageNumbersReborn.Components;
internal class UIObjectPoolManager : MonoBehaviour
{
    private static UIObjectPoolManager _pixelPerfect;
    private static UIObjectPoolManager _highRes;
    private readonly Dictionary<int, Queue<ObjectInstance>> poolDictionary = [];

    public static UIObjectPoolManager PixelPerfect
    {
        get
        {
            _pixelPerfect ??= UIModder.Parents.CanvasStandard.gameObject.GetOrElseAddComponent<UIObjectPoolManager>();
            return _pixelPerfect;
        }
    }
    public static UIObjectPoolManager HighRes
    {
        get
        {
            _highRes ??= UIModder.Parents.CanvasHighRes.gameObject.GetOrElseAddComponent<UIObjectPoolManager>();
            return _highRes;
        }
    }

    public void CreatePool(GameObject prefab, int poolSize)
    {
        int instanceID = prefab.GetInstanceID();
        if (!poolDictionary.ContainsKey(instanceID))
        {
            poolDictionary.Add(instanceID, new Queue<ObjectInstance>());
            GameObject pool = new(prefab.name + " pool");
            pool.transform.SetParent(transform);
            for (int i = 0; i < poolSize; i++)
            {
                ObjectInstance objectInstance = new(UnityEngine.Object.Instantiate<GameObject>(prefab));
                poolDictionary[instanceID].Enqueue(objectInstance);
                objectInstance.SetParent(pool.transform);
            }
        }
        else
        {
            GameObject pool = GameObject.Find(prefab.name + " pool");
            if (pool == null)
            {
                return;
            }
            for (int j = 0; j < poolSize; j++)
            {
                ObjectInstance objectInstance2 = new(UnityEngine.Object.Instantiate<GameObject>(prefab));
                poolDictionary[instanceID].Enqueue(objectInstance2);
                objectInstance2.SetParent(pool.transform);
            }
        }
    }

    public ObjectInstance ReuseObject(GameObject prefab, Vector3 position, Quaternion rotation, bool createPoolIfNeeded = false, int poolSize = 1)
    {
        int instanceID = prefab.GetInstanceID();
        ObjectInstance objectInstance = null;
        if (poolDictionary.ContainsKey(instanceID))
        {
            objectInstance = poolDictionary[instanceID].Dequeue();
            poolDictionary[instanceID].Enqueue(objectInstance);
            objectInstance.Reuse(position, rotation);
        }
        else if (createPoolIfNeeded)
        {
            CreatePool(prefab, poolSize);
            objectInstance = ReuseObject(prefab, position, rotation, false, 1);
        }
        return objectInstance;
    }


    public class ObjectInstance
    {
        private readonly GameObject gameObject;
        private readonly bool hasPoolObjectComponent;
        private readonly PoolObject[] poolObjectScripts;
        private readonly Transform transform;

        public GameObject GameObject
        {
            get
            {
                return gameObject;
            }
        }

        public ObjectInstance(GameObject objectInstance)
        {
            gameObject = objectInstance;
            transform = gameObject.transform;
            gameObject.SetActive(false);
            if (gameObject.GetComponent<PoolObject>())
            {
                hasPoolObjectComponent = true;
                poolObjectScripts = gameObject.GetComponents<PoolObject>();
            }
        }

        public void Reuse(Vector3 position, Quaternion rotation)
        {
            if (!gameObject)
            {
                return;
            }
            gameObject.SetActive(true);
            transform.position = position;
            transform.rotation = rotation;
            if (hasPoolObjectComponent)
            {
                foreach (PoolObject poolObject in poolObjectScripts)
                {
                    poolObject.OnObjectReuse();
                }
            }
        }

        public void SetParent(Transform parent)
        {
            transform.SetParent(parent);
        }
    }
}
