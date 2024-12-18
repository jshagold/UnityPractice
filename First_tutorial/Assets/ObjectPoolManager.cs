using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class ObjectPoolManager : MonoBehaviour
{
    private Dictionary<int, List<GameObject>> dics = new Dictionary<int, List<GameObject>>();


    public GameObject GetObjectByPrefab(GameObject prefab, Transform parent, Vector3 position, Quaternion rotation)
    {
        int hashCode = prefab.GetHashCode();
        Debug.Log(hashCode);

        if (!dics.ContainsKey(hashCode))
        {
            dics.Add(hashCode, new List<GameObject>());
        }

        List<GameObject> gameObjects = dics[hashCode];
        foreach (var item in gameObjects)
        {
            if (!item.gameObject.activeInHierarchy)
            {
                item.gameObject.transform.localPosition = position;
                item.gameObject.transform.localRotation = rotation;
                item.gameObject.SetActive(true);
                
                return item;
            }
        }

        GameObject obj = Instantiate(prefab, parent);
        obj.gameObject.transform.localPosition = position;
        obj.gameObject.transform.localRotation = rotation;
        dics[hashCode].Add(obj);

        return obj;
    }
}
