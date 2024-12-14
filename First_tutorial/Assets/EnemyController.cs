using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;

public class EnemyController : MonoBehaviour
{
    // [Coroutine]
    //private int indexMove;
    //private bool isFinished = false;

    // [·ÎÄÃ, ±Û·Î¹ú ÁÂÇ¥°è]
    //[SerializeField] GameObject objChild;

    // [NavMesh] 
    NavMeshAgent agent; // cache
    PlayerController playerController; // cache

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // [Coroutine]
        //yield return StartCoroutine(C_MoveRight());
        //yield return new WaitForSeconds(2f);
        //yield return StartCoroutine(C_MoveDown());

        // [·ÎÄÃÁÂÇ¥°è, ±Û·Î¹úÁÂÇ¥°è]
        //transform.position = new Vector3(5,0,0);
        //transform.position = transform.TransformPoint(5, 0, 0);
        
        // ·ÎÄÃ ÁÂÇ¥°è ÀÌµ¿ (·ÎÄÃÁÂÇ¥°è -> ±Û·Î¹úÁÂÇ¥°è·Î º¯È¯ÇØ¼­ ÀÌµ¿)
        //objChild.transform.position = transform.TransformPoint(5, 0, 0);
        // ±Û·Î¹ú ÁÂÇ¥°è ÀÌµ¿
        //objChild.transform.position = new Vector3(5, 0, 0);
    

        //[NavMesh]
        agent = GetComponent<NavMeshAgent>();
        playerController = FindAnyObjectByType<PlayerController>();

    }


    // Update is called once per frame
    void Update()
    {

        //if(indexMove <= 3)
        //{
        //    transform.Translate(1, 0, 0);
        //    indexMove++;
        //}
        //else
        //{
        //    isFinished = true;
        //} 

        //if(isFinished)
        //{
        //    transform.Translate(0, 0, 1);
        //}


        agent.SetDestination(playerController.transform.position);

    }

    public void Attack()
    {
        Debug.Log($"Enemy Attack ()");
    }


    // [Coroutine] - yield
    //IEnumerator C_MoveRight()
    //{
    //    yield return new WaitForSeconds(0.5f);
    //    transform.Translate(1, 0, 0);
    //    yield return new WaitForSeconds(1f);
    //    transform.Translate(1, 0, 0);
    //    yield return new WaitForSeconds(1.5f);
    //    transform.Translate(1, 0, 0);

    //    int index = 0;
    //    while(true)
    //    {
    //        if(index >= 5)
    //        {
    //            yield break;
    //        }
    //        yield return null;
    //        transform.Translate(0, 0, 1);
    //        index++;
    //    }
    //}
    //IEnumerator C_MoveDown()
    //{
    //    yield return null;
    //    transform.Translate(0, 0, -1);
    //    yield return null;
    //    transform.Translate(0, 0, -1);
    //    yield return null;
    //    transform.Translate(0, 0, -1);
    //}
}
