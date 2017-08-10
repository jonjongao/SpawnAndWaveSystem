using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class RoomEncounter : MonoBehaviour
{
    public List<SpawnPoint> spawnPoints;

    public List<Collider> walls;

    public bool isActive;

    private void Start()
    {
        walls.ForEach(i => i.gameObject.SetActive(false));
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag != "Player") return;
        if (!isActive)
        {
            walls.ForEach(i => i.gameObject.SetActive(true));

            spawnPoints.ForEach(i => i.StartSequence());

            isActive = true;
        }
    }
}
