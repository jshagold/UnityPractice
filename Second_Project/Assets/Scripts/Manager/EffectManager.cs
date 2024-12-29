using System.Collections;
using UnityEngine;

public class EffectManager : ManagerBase
{
    [SerializeField] private GameObject[] effectPrefabs;
    [SerializeField] private float[] destoryTime;

    public void SetInit()
    {
    }
    private void Awake()
    {
        DontDestroy<EffectManager>();
    }


    public void PlayEffect(int index, Transform parent)
    {
        if (effectPrefabs.Length > index)
        {
            GameObject obj = Instantiate(effectPrefabs[index], parent);
            obj.transform.localPosition = Vector3.zero;
            StartCoroutine(C_DestoryEffect(obj, destoryTime[index]));
        }
    }

    private IEnumerator  C_DestoryEffect(GameObject obj, float destoryTime)
    {
        yield return new WaitForSeconds(destoryTime);
        Destroy(obj);
    }
}
