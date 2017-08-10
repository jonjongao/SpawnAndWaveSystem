#define DEBUG
#undef DEBUG
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Collections;
using Gears;

public class EnemyWaveSetEditor : EditorWindow
{
    private EnemyWaveSet target;
    private EnemyWaveSet refTarget;
    private ReorderableList rolist;

    public void SetEditor(EnemyWaveSet waveSet)
    {
        this.titleContent = new GUIContent("Enemy Wave Set Editor");
        target = waveSet;
        refTarget = new EnemyWaveSet(waveSet);
        rolist = new ReorderableList(refTarget.wave.enemys, typeof(SpawnSequence), true, true, true, true);
#if(DEBUG)
        Debug.Log("Open");
#endif
    }

    void OnGUI()
    {
        GUILayout.BeginArea(new Rect(5, 5, position.width - 10, position.height - 10));
        if (target != null && refTarget != null)
        {
            EditorGUILayout.LabelField(target.name, (GUIStyle)"sv_label_2");
            refTarget.name = EditorGUILayout.TextField("Name", refTarget.name);
            refTarget.chance = EditorGUILayout.IntSlider("Chance", refTarget.chance, 0, 100);
            refTarget.wave.tag = EditorGUILayout.TextField("Tag", refTarget.wave.tag);
            refTarget.wave.delay = EditorGUILayout.FloatField("Delay (s)", refTarget.wave.delay);
            refTarget.wave.effect = EditorGUILayout.ObjectField("Spawn Effect", refTarget.wave.effect, typeof(GameObject), false) as GameObject;
            rolist.DoLayoutList();
            this.position = new Rect(position.x, position.y, position.width, 280 + (rolist.count - 3) * (EditorGUIUtility.singleLineHeight * 1.25f));
            rolist.drawHeaderCallback = (Rect r) =>
            {
                EditorGUI.LabelField(r, "Enemys");
            };
            rolist.drawElementCallback = (Rect r, int i, bool a, bool f) =>
            {
                r.y += 2;
                refTarget.wave.enemys[i].prefab = EditorGUI.ObjectField(new Rect(r.x, r.y, r.width * .85f, EditorGUIUtility.singleLineHeight), refTarget.wave.enemys[i].prefab, typeof(GameObject), false) as GameObject;
                refTarget.wave.enemys[i].number = EditorGUI.IntField(new Rect(r.x + r.width * .85f + 5f, r.y, r.width * .15f - 5f, EditorGUIUtility.singleLineHeight), refTarget.wave.enemys[i].number);
            };
            if (GUI.Button(new Rect(position.width * .5f - 50, position.height - 50, 100, 40), "Save"))
            {
#if(DEBUG)
                Debug.Log("Save");
#endif
                target.Override(refTarget);
                this.Close();
            }
        }
        GUILayout.EndArea();
    }

    void OnFocus()
    {
    }

    void OnLostFocus()
    {
    }

    void OnDestroy()
    {
#if (DEBUG)
        Debug.Log("Close");
#endif
        target.Override(refTarget);
    }
}
