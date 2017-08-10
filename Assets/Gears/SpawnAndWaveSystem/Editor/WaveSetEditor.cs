using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using Gears;

public class WaveSetEditor : EditorWindow
{
    private EnemyWaveManager manager;
    private SerializedProperty managerWaveSet;
    int waveIndex = 0;
    int selectedSetIndex = 0;
    Vector2 slider;

    public void SetMenu(EnemyWaveManager target, SerializedProperty waveSet, int index)
    {
        manager = target;
        managerWaveSet = waveSet;
        waveIndex = index;
    }

    void OnGUI()
    {
        if (manager != null)
        {
            float unit = EditorGUIUtility.singleLineHeight;
            float fix = unit * 1.25f;
            Rect r = new Rect(0, 0, position.width, position.height);
            float elementHeight = r.height - unit;
            if (managerWaveSet.arraySize > 3)
                elementHeight = ((managerWaveSet.arraySize) * fix) + 8f;

            #region StaticToolbar
            GUI.Box(new Rect(0, 0, position.width, position.height), GUIContent.none);
            GUI.Box(new Rect(0, 0, position.width, unit), GUIContent.none, EditorStyles.toolbar);
            if (GUI.Button(new Rect(position.width - 26, 0, 26, unit), EditorGUIUtility.FindTexture("d_Toolbar Plus"), EditorStyles.toolbarButton))
            {
                managerWaveSet.InsertArrayElementAtIndex(managerWaveSet.arraySize);
            }
            #endregion

            #region ScrollView
            slider = GUI.BeginScrollView(new Rect(0, unit + 2, r.width, r.height), slider, new Rect(0, 0, r.width, elementHeight), false, true);
            r.y += 4;
            for (int l = 0; l < managerWaveSet.arraySize; l++)
            {
                float y = r.y + (l * fix);
                if (l % 2 == 0) EditorGUI.DrawRect(new Rect(r.x, y - 2, r.width, fix), new Color(0.23f, 0.56f, 0.5f, 1f));
                else EditorGUI.DrawRect(new Rect(r.x, y - 2, r.width, fix), new Color(0.23f, 0.56f, 0.5f, .8f));
                GUI.BeginGroup(new Rect(r.x, y, r.width, unit));
                #region Menu
                if (new Rect(0, 0, r.width, unit).Contains(Event.current.mousePosition) && Event.current.type == EventType.mouseDown && Event.current.button == 1)
                {
                    selectedSetIndex = l;
                    GenericMenu menu = new GenericMenu();
                    menu.AddItem(new GUIContent("Edit"), false, EditSet, manager.waveSequence[waveIndex].waveSet[l]);
                    menu.AddSeparator(string.Empty);
                    menu.AddItem(new GUIContent("Add"), false, AddSet, managerWaveSet);
                    menu.AddItem(new GUIContent("Remove"), false, RemoveSet, managerWaveSet);
                    menu.ShowAsContext();
                }
                #endregion
                if (GUI.Button(new Rect(5, 0, 40, unit), "Edit", EditorStyles.miniButton))
                {
                    EnemyWaveSetEditor w = EditorWindow.GetWindow(typeof(EnemyWaveSetEditor), true) as EnemyWaveSetEditor;
                    //w.minSize = new Vector2(300, 300);
                    w.SetEditor(manager.waveSequence[waveIndex].waveSet[l]);
                }
                if (managerWaveSet.GetArrayElementAtIndex(l).FindPropertyRelative("name").stringValue == string.Empty)
                    managerWaveSet.GetArrayElementAtIndex(l).FindPropertyRelative("name").stringValue = "New Set";
                EditorGUI.LabelField(new Rect(50, 0, r.width * .4f + 10, unit), managerWaveSet.GetArrayElementAtIndex(l).FindPropertyRelative("name").stringValue, EditorStyles.miniBoldLabel);
                GUI.color = Color.red;
                if (GUI.Button(new Rect(r.width - 44, 0, 24, unit), EditorGUIUtility.FindTexture("d_Toolbar Minus"), EditorStyles.miniButton))
                {
                    managerWaveSet.DeleteArrayElementAtIndex(l);
                    return;
                }
                GUI.color = Color.white;
                GUI.EndGroup();
            }
            GUI.EndScrollView();
            #endregion
            managerWaveSet.serializedObject.ApplyModifiedProperties();
        }
    }

    void OnLostFocus()
    {
        managerWaveSet.serializedObject.ApplyModifiedProperties();
    }

    void OnDestroy()
    {
        managerWaveSet.serializedObject.ApplyModifiedProperties();
    }
    #region GenericMenu Function
    void EditSet(object obj)
    {
        EnemyWaveSet set = (EnemyWaveSet)obj;
        EnemyWaveSetEditor w = EditorWindow.GetWindow(typeof(EnemyWaveSetEditor), true) as EnemyWaveSetEditor;
        w.SetEditor(set);
    }
    void AddSet(object obj)
    {
        SerializedProperty s = (SerializedProperty)obj;
        s.InsertArrayElementAtIndex(selectedSetIndex);
        s.serializedObject.ApplyModifiedProperties();
    }
    void RemoveSet(object obj)
    {
        SerializedProperty s = (SerializedProperty)obj;
        s.DeleteArrayElementAtIndex(selectedSetIndex);
        s.serializedObject.ApplyModifiedProperties();
    }
    #endregion
}
