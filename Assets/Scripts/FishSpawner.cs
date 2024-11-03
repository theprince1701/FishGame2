using System;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using Random = UnityEngine.Random;

public class FishSpawner : MonoBehaviour
{ 
    public GameObject[] fishPrefabs;
    public int fishCount = 5;
    public float spawnRate = 0.25f;
    public float yRandom = 1;

    private int _currentFishCount;
    private List<GameObject> _fishList = new List<GameObject>();
    private void Awake()
    {
        InvokeRepeating(nameof(SpawnFish), spawnRate, spawnRate);
    }

    void SpawnFish()
    {
        if (_currentFishCount < fishCount)
        {
            GameObject fish = Instantiate(fishPrefabs[Random.Range(0, fishPrefabs.Length)], transform.position + new Vector3(0, Random.Range(-yRandom, yRandom)), Quaternion.identity);
            _currentFishCount++;
            _fishList.Add(fish);
        }
    }
}
