#define DEBUG
#undef DEBUG
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using Gears;

public class SpawnPointDropDownMenu : EditorWindow
{
    private EnemyWaveSequence manager;
    private SpawnPoint[] spawnPoints;
    private SpawnPoint[] refSpawnPoints;

    public void SetMenu(EnemyWaveSequence target, SpawnPoint[] availablePoints, SpawnPoint[] selectedPoints)
    {
        manager = target;
        spawnPoints = availablePoints;
        refSpawnPoints = selectedPoints;
    }

    void OnGUI()
    {
        if (manager != null && spawnPoints != null && spawnPoints.Length > 0)
        {
            List<SpawnPoint> list = refSpawnPoints.ToList();
            for (int i = 0; i < spawnPoints.Length; i++)
            {
                if (EditorGUILayout.ToggleLeft(spawnPoints[i].name, IsSelected(spawnPoints[i], refSpawnPoints)))
                {
#if (DEBUG)
                    Debug.Log("Add");
#endif
                    if (!IsSelected(spawnPoints[i], list.ToArray()))
                        list.Add(spawnPoints[i]);
                }
                else
                {
#if (DEBUG)
                    Debug.Log("Remove");
#endif
                    list.Remove(spawnPoints[i]);
                }
            }
            refSpawnPoints = list.Distinct().ToArray();
        }
        else
        {
            EditorGUILayout.HelpBox("Can't Find Spawn Point in childs", MessageType.Error);
        }
    }

    void OnFocus()
    {
#if (DEBUG)
        Debug.Log("Open Window");
#endif
    }

    void OnLostFocus()
    {
#if (DEBUG)
        Debug.Log("Close Menu");
#endif
        manager.selectedSpawnPoints = refSpawnPoints;
    }

    bool IsSelected(SpawnPoint target, SpawnPoint[] array)
    {
        for (int i = 0; i < array.Length; i++)
        {
            if (array[i].GetHashCode() == target.GetHashCode())
                return true;
        }
        return false;
    }
}
