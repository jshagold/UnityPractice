using System.Collections.Generic;
using UnityEngine;

public class WindowInfo
{
    public readonly GameObject prefab;
    public readonly GameObject gameObject;

    public WindowInfo(GameObject prefab, GameObject gameObject)
    {
        this.prefab = prefab;
        this.gameObject = gameObject;
    }
}

public class WindowManager : MonoBehaviour
{
    private List<WindowInfo> lists = new List<WindowInfo>();

    public GameObject NewWindow(GameObject prefab, Transform parent)
    {
        foreach (var list in lists)
        {
            if (list.prefab == prefab)
            {
                list.gameObject.SetActive(true);
                list.gameObject.transform.SetAsLastSibling();
                return list.gameObject;
            }
        }

        GameObject obj = Instantiate(prefab, parent);
        lists.Add(new WindowInfo(prefab, obj));
        return obj;
    }

    public GameObject ChangeWindow(GameObject prefab, Transform parent)
    {
        foreach (var list in lists)
        {
            list.gameObject.SetActive(false);
        }

        foreach (var list in lists)
        {
            if (list.prefab == prefab)
            {
                list.gameObject.SetActive(true);
                list.gameObject.transform.SetAsLastSibling();
                return list.gameObject;
            }
        }

        GameObject obj = Instantiate(prefab, parent);
        lists.Add(new WindowInfo(prefab, obj));
        return obj;
    }

    public void SetInit()
    {
    }
}
