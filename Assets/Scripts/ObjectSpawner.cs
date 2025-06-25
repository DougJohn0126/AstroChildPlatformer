using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ObjectSpawner : MonoBehaviour
{
    public enum ObjectType { SmallGem, BigGem, Enemy}

    public Tilemap Tilemap;
    public GameObject[] ObjectPrefabs;
    public float SmallGemProbability;
    public float MediumGemProbability;
    public float BigGemProbability;
    public float EnemyProbability;
    public int MaxObjects = 5;
    public float GemLifeTime = 10f;
    public float SpawnInterval = 0.5f;

    private List<Vector3> mValidSpawnPositions = new List<Vector3>();
    private List<GameObject> mSpawnObjects = new List<GameObject>();
    private bool mIsSpawning = false;

    void Start()
    {
        GatherValidPositions();
        StartCoroutine(SpawnObjectsIfNeeded());
        GameController.OnReset += LevelChange;
    }

    void Update()
    {
        if (!Tilemap.gameObject.activeInHierarchy)
        {
            LevelChange();
        }
        if (!mIsSpawning && ActiveObjectCount() < MaxObjects)
        {
            StartCoroutine(SpawnObjectsIfNeeded());
        }
    }

    private void LevelChange()
    {
        Tilemap = GameObject.Find("Ground").GetComponent<Tilemap>();
        GatherValidPositions();
        DestroyAllSpawnedObjects();
    }

    private int ActiveObjectCount()
    {
        mSpawnObjects.RemoveAll(item => item == null);
        return mSpawnObjects.Count;
    }

    private IEnumerator SpawnObjectsIfNeeded()
    {
        mIsSpawning = true;
        while (ActiveObjectCount() < MaxObjects)
        {
            SpawnObject();
            yield return new WaitForSeconds(SpawnInterval);
        }
        mIsSpawning = false;
    }

    private bool PositionHasObject (Vector3 positionToCheck)
    {
        return mSpawnObjects.Any(checkObj => checkObj && Vector3.Distance(checkObj.transform.position, positionToCheck) < 1.0f);
    }

    private ObjectType RandomObjectType()
    {
        float randomChoice = Random.value;

        if (randomChoice <= EnemyProbability)
        {
            return ObjectType.Enemy;
        }
        else if(randomChoice<= (EnemyProbability + BigGemProbability)) 
        {
            return ObjectType.BigGem;
        } else
        {
            return ObjectType.SmallGem;
        }

    }
    private void SpawnObject()
    {
        if (mValidSpawnPositions.Count == 0) return;
        Vector3 spawnPosition = Vector3.zero;
        bool validPositionFound = false;

        while (!validPositionFound&&mValidSpawnPositions.Count >0)
        {
            int randomIndex = Random.Range(0, mValidSpawnPositions.Count);
            Vector3 potetialPosition = mValidSpawnPositions[randomIndex];
            Vector3 leftPosition = potetialPosition + Vector3.left;
            Vector3 rightPosition = potetialPosition + Vector3.right;

            if (!PositionHasObject(leftPosition) && !PositionHasObject(rightPosition))
            {
                spawnPosition = potetialPosition;
                validPositionFound = true;
            }

            mValidSpawnPositions.RemoveAt(randomIndex);
        }

        if (validPositionFound)
        {
            ObjectType objectType = RandomObjectType();
            GameObject gameObject = Instantiate(ObjectPrefabs[(int)objectType], spawnPosition, Quaternion.identity);
            mSpawnObjects.Add(gameObject);

            if (objectType != ObjectType.Enemy)
            {
                StartCoroutine(DestroyObjectsAfterTime(gameObject, GemLifeTime));
            }
        }
    }

    private void DestroyAllSpawnedObjects()
    {
        foreach(GameObject obj in mSpawnObjects)
        {
            if (obj != null)
            {
                Destroy(obj);
            }
        }
        mSpawnObjects.Clear();
    }

    private IEnumerator DestroyObjectsAfterTime (GameObject gameObject, float time)
    {
        yield return new WaitForSeconds(time);
        if (gameObject)
        {
            mSpawnObjects.Remove(gameObject);
            mValidSpawnPositions.Add(gameObject.transform.position);
            Destroy(gameObject);
        }
    }

    private void GatherValidPositions()
    {
        mValidSpawnPositions.Clear();
        BoundsInt boundsInt = Tilemap.cellBounds;
        TileBase[] allTiles = Tilemap.GetTilesBlock(boundsInt);
        Vector3 start = Tilemap.CellToWorld(new Vector3Int (boundsInt.xMin, boundsInt.yMin, 0));


        for (int x = 0; x <boundsInt.size.x; x++)
        {
            for (int y = 0; y < boundsInt.size.y; y++)
            {
                TileBase tile = allTiles[x + y * boundsInt.size.x];
                if (tile != null)
                {
                    Vector3 place = start + new Vector3(x + 0.5f, y + 2f, 0);
                    mValidSpawnPositions.Add(place);
                }
            }
        }
    }
}
