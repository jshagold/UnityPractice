using System;
using TMPro;
using UnityEngine;

public class ItemUnit : MonoBehaviour
{
    [SerializeField] private TMP_Text text;
    private ItemUnitInfo itemUnitinfo;
    private Action<ItemUnit> removeAction;

    public void SetInit(ItemUnitInfo itemUnitInfo, Action<ItemUnit> removeAction)
    {
        this.itemUnitinfo = itemUnitInfo;
        this.removeAction = removeAction;
        text.text = itemUnitInfo.name;
    }

    public void OnClick_remove()
    {
        Debug.Log($"Click itemId:{itemUnitinfo.id}, itemName: {itemUnitinfo.name}");
        //transform.parent.GetComponent<ItemController>().RemoveItem(this); // �������� ���
        //ItemController.RemoveItem(this); // �������� ���
        //FindAnyObjectByType<ItemController>();
        this.removeAction?.Invoke(this); // Callback
    }
}
