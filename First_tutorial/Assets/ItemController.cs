using System.Collections.Generic;
using UnityEngine;

public class ItemUnitInfo
{
    public readonly int id;
    public readonly string name;

    public ItemUnitInfo(int id, string name)
    {
        this.id = id;
        this.name = name;
    }
}


public class ItemController : MonoBehaviour
{
    [SerializeField] private GameObject itemPrefab;
    private List<ItemUnit> itemUnits = new List<ItemUnit>(); // 생성한 정보를 담는 list

    private List<ItemUnitInfo> itemUnitInfos = new List<ItemUnitInfo>(); // 정보


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        itemUnitInfos.Add(new ItemUnitInfo(1,"Normal bird"));        
        itemUnitInfos.Add(new ItemUnitInfo(2,"Rare bird"));        
        itemUnitInfos.Add(new ItemUnitInfo(3,"Unique bird"));

        foreach(var unit in itemUnitInfos)
        {
            GameObject obj = Instantiate(itemPrefab, transform);
            ItemUnit itemUnit = obj.GetComponent<ItemUnit>();
            itemUnit.SetInit(unit, RemoveItem);
            itemUnits.Add(itemUnit);
        }
    }

    public void RemoveItem(ItemUnit itemUnit)
    {
        Destroy(itemUnit.gameObject);
        itemUnits.Remove(itemUnit);
    }
}
