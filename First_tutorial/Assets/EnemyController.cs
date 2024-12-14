using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public class EnemyController : MonoBehaviour
{

    private int indexMove;
    private bool isFinished = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    IEnumerator Start()
    {

        yield return StartCoroutine(C_MoveRight());
        yield return new WaitForSeconds(2f);
        yield return StartCoroutine(C_MoveDown());

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
            



    }

    public void Attack()
    {
        Debug.Log($"Enemy Attack ()");
    }


    // [Coroutine] - yield
    IEnumerator C_MoveRight()
    {
        yield return new WaitForSeconds(0.5f);
        transform.Translate(1, 0, 0);
        yield return new WaitForSeconds(1f);
        transform.Translate(1, 0, 0);
        yield return new WaitForSeconds(1.5f);
        transform.Translate(1, 0, 0);

        int index = 0;
        while(true)
        {
            if(index >= 5)
            {
                yield break;
            }
            yield return null;
            transform.Translate(0, 0, 1);
            index++;
        }
    }

    IEnumerator C_MoveDown()
    {
        yield return null;
        transform.Translate(0, 0, -1);
        yield return null;
        transform.Translate(0, 0, -1);
        yield return null;
        transform.Translate(0, 0, -1);
    }
}
