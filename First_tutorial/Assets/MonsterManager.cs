using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterManager : MonoBehaviour
{
    [SerializeField] private GameObject monsterAPrefab;
    [SerializeField] private GameObject monsterBPrefab;
    [SerializeField] private Transform monsterParent;

    private List<MonsterA> monsterAList = new List<MonsterA>();

    private ObjectPoolManager objectPoolManager; // cache

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        objectPoolManager = FindAnyObjectByType<ObjectPoolManager>();

    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Z))
        {
            //MonsterA[] monsterAs = monsterParent.GetComponentsInChildren<MonsterA>(true); // 리소스를 좀 먹는다.

            //bool isFound = false;
            //foreach(var item in monsterAList)
            //{
            //    if(!item.gameObject.activeInHierarchy)
            //    {
            //        item.gameObject.transform.localPosition = Vector3.zero;
            //        item.gameObject.transform.localRotation = Quaternion.identity;
            //        item.gameObject.SetActive(true);
            //        isFound = true;
            //        break;
            //    }
            //}

            //if(!isFound)
            //{
            //    GameObject obj = Instantiate(monsterAPrefab, monsterParent);
            //    obj.transform.localPosition = Vector3.zero;
            //    obj.transform.localRotation = Quaternion.identity;
            //    monsterAList.Add(obj.GetComponent<MonsterA>());
            //}

            GameObject obj = objectPoolManager.GetObjectByPrefab(monsterAPrefab, monsterParent, Vector3.zero, Quaternion.identity);
            //Debug.Log(obj.GetHashCode());
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            GameObject obj = objectPoolManager.GetObjectByPrefab(monsterBPrefab, monsterParent, Vector3.zero, Quaternion.identity);
        }
    }
}
