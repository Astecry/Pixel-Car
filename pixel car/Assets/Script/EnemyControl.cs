using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyControl : MonoBehaviour
{
    public GameObject[] EnemyCars;
    public GameObject[] CoinHeart;
    Vector3 SpawnPos;
    bool CarSpawnBool = true, CHSpawnBoll = true;
    List<GameObject> CarList = new List<GameObject>();
    List<GameObject> CHList = new List<GameObject>();
    public float DestroySecond = 5f;
    public float SpawnSecond = 1.5f;
    public float CHSpawnSecond = 5f;
    public float CarSpeed = 10f, CHSpeed = 1f;
    int CarId = 0;
    public GameManager GM;
    private int repeat;
    private Vector3 newCarPos = Vector3.zero, newpos;

    private void LateUpdate()
    {
        if (!GM.pause)
        {
            if (GM.EndGameBool)
            {
                if (!GM.noSpawn)
                {
                    if (CarSpawnBool)
                    {
                        StartCoroutine(Spawn());
                    }

                    if (CHSpawnBoll)
                    {
                        StartCoroutine(CHSpawn());
                    }
                }
                CarMove();
                if (SpawnSecond>0.65f)
                {
                    SpawnSecond -= Time.deltaTime / 250;
                }
            }
        }
    }


    void CarMove()
    {
        for (int i = 0; i < CarList.Count; i++)
        {
            if (CarList[i] != null)
            {
                CarList[i].transform.Translate(0, CarSpeed * -0.02f, 0);
            }
        }

        for (int i = 0; i < CHList.Count; i++)
        {
            if (CHList[i] != null)
            {
                CHList[i].transform.Translate(0, CHSpeed * -0.02f, 0);
            }
        }
    }

    IEnumerator CHSpawn()
    {
        CHSpawnBoll = false;
        SpawnPos = new Vector3(Random.Range(-1.86f, 1.77f), 6, 0);
        int R = Random.Range(0, 60);
        int id = -1;
        if (R < 12)
        {
            id = 0;
        }
        else if (R < 24)
        {
            id = 1;
        }
        else if (R < 48)
        {
            id = 2;
        }

        if (id >= 0)
        {
            GameObject GO = Instantiate(CoinHeart[id], SpawnPos, Quaternion.Euler(0, 0, 0));
            CHList.Add(GO);
        }
        yield return new WaitForSeconds(CHSpawnSecond);
        CHSpawnBoll = true;
    }

    IEnumerator Spawn()
    {
        if (GM.Scor < 100)
        {
            repeat = 1;
        }
        else
        {
            repeat = 2;
        }

        CarSpawnBool = false;
        for (int i = 0; i < repeat; i++)
        {
            newpos = new Vector3(Random.Range(-1.86f, 1.77f), Random.Range(7f, 10f), 0);
            if (newCarPos != Vector3.zero)
            {
              //  newpos = newCarPos;
                if (newCarPos.x - 0.6f < newpos.x && newCarPos.x + 0.6f > newpos.x)
                {
                    float x = .8f;
                    int r=Random.Range(0, 2);
                    if (newCarPos.x+x>1.77f)
                    {
                        r = 1;
                    }
                    else if (newCarPos.x-x<-1.86f)
                    {
                        r = 0;
                    }
                    if (r==1)
                    {
                        x *= -1;
                    }
                    newpos = new Vector3(newCarPos.x+x, newpos.y, 0);
                }
            }
            SpawnPos = newpos;
            GameObject GO = Instantiate(EnemyCars[Random.Range(0, EnemyCars.Length)], SpawnPos,
                Quaternion.Euler(0, 0, 0));
            newCarPos = GO.transform.position;
            GO.name = GO.name + "" + CarId;
            CarList.Add(GO);
            CarId++;
        }

        // SpawnSecond = GM.Scor/GM.Scor;
        yield return new WaitForSeconds(SpawnSecond);
        newCarPos = Vector3.zero;
        CarSpawnBool = true;
    }

    public void Remove()
    {
        for (int i = 0; i < CarList.Count; i++)
        {
            if (CarList[i] != null)
            {
                Destroy(CarList[i]);
            }
        }

        for (int i = 0; i < CHList.Count; i++)
        {
            if (CHList[i] != null)
            {
                Destroy(CHList[i]);
            }
        }
    }

    public void ResetSpawnS()
    {
        SpawnSecond=1.5f;
    }
}