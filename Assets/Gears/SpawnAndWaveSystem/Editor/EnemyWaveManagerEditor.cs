#define DEBUG
#undef DEBUG
//#undef UNITY_5_3
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Collections.Generic;
using System;
using System.Linq;
using Gears;

[CustomEditor(typeof(EnemyWaveManager))]
public class EnemyWaveManagerEditor : Editor
{
    private EnemyWaveManager me;
    //State
    private SerializedProperty currentWave;
    private SerializedProperty waveState;
    //Gizmo
    private SerializedProperty debugToggle;
    private SerializedProperty gizmosColor;
    private SerializedProperty gizmosSize;
    //Setting
    private SerializedProperty settingToggle;
    private SerializedProperty autoStart;
    private SerializedProperty autoProceed;
    private SerializedProperty waveDelay;
    private SerializedProperty separatedPattern;
    private SerializedProperty waveSequence;
    //Editor
    private SerializedProperty waveEventToggle;
    private SerializedProperty onWaveStart;
    private SerializedProperty onWaveFinish;
    private SerializedProperty onWaveClear;
    private SerializedProperty waveProcessingEventToggle;
    private SerializedProperty onWaveProceedInt;
    private SerializedProperty onWaveProceedString;
    private SerializedProperty sequenceEventToggle;
    private SerializedProperty onAllWaveClear;

    private ReorderableList rolist;
    private int onSelectedIndex;
    private bool drawHandles;

#if UNITY_5_3
    private int selectedSetIndex;
#endif
    void OnEnable()
    {
        me = (EnemyWaveManager)target;
        //State
        currentWave = serializedObject.FindProperty("currentWave");
        waveState = serializedObject.FindProperty("waveState");
        //Gizmo
        debugToggle = serializedObject.FindProperty("debugToggle");
        gizmosColor = serializedObject.FindProperty("gizmosColor");
        gizmosSize = serializedObject.FindProperty("gizmosSize");
        //Setting
        settingToggle = serializedObject.FindProperty("settingToggle");
        autoStart = serializedObject.FindProperty("autoStart");
        autoProceed = serializedObject.FindProperty("autoProceed");
        waveDelay = serializedObject.FindProperty("waveDelay");
        separatedPattern = serializedObject.FindProperty("separatedPattern");
        waveSequence = serializedObject.FindProperty("waveSequence");
        //Event
        waveEventToggle = serializedObject.FindProperty("waveEventToggle");
        onWaveStart = serializedObject.FindProperty("onWaveStart");
        onWaveFinish = serializedObject.FindProperty("onWaveFinish");
        onWaveClear = serializedObject.FindProperty("onWaveClear");
        waveProcessingEventToggle = serializedObject.FindProperty("waveProcessingEventToggle");
        onWaveProceedInt = serializedObject.FindProperty("onWaveProceedInt");
        onWaveProceedString = serializedObject.FindProperty("onWaveProceedString");
        sequenceEventToggle = serializedObject.FindProperty("sequenceEventToggle");
        onAllWaveClear = serializedObject.FindProperty("onAllWaveClear");

        rolist = new ReorderableList(serializedObject, waveSequence, true, true, true, true);
    }

    void OnDisable()
    {
        onSelectedIndex = -1;
    }

    private void DrawWaveManager()
    {
        rolist.DoLayoutList();
        rolist.drawHeaderCallback = (Rect r) =>
        {
            EditorGUI.LabelField(r, "Wave Sequence Manager");
        };

        rolist.drawElementCallback = (Rect r, int i, bool a, bool f) =>
        {
            r.y += 2f;
            var unit = EditorGUIUtility.singleLineHeight;
            var e = rolist.serializedProperty.GetArrayElementAtIndex(i);
            var managerName = e.FindPropertyRelative("name");
            //var managerSpawnPointID = e.FindPropertyRelative("spawnPointID");
            var managerWaveSet = e.FindPropertyRelative("waveSet");
            //var waveSetCount = managerWaveSet.arraySize;
            bool error = false;
            if (!CheckSpawnPointArray(me.waveSequence[i].selectedSpawnPoints))
            {
#if (DEBUG)
                Debug.LogError("Error at index " + i);
#endif
                error = true;
                //drawHandles = false;
                //me.waveSequence[i].selectedSpawnPoints = ReconstructSpawnPointArray(me.waveSequence[i].selectedSpawnPoints);
            }

            float margin = unit * .1f;
            //r.y += margin;

            GUI.BeginGroup(new Rect(r.x, r.y - 1, r.width, unit), EditorStyles.toolbar);

            #region DrawWaveSequence
            EditorGUI.LabelField(new Rect(3f, 0, 30, unit), "#" + i, EditorStyles.miniLabel);
            managerName.stringValue = EditorGUI.TextField(new Rect(3f + 30, 2f, r.width * .4f - 30, unit), managerName.stringValue, EditorStyles.toolbarTextField);
            //managerSpawnPointID.intValue = EditorGUI.Popup(new Rect(5f + r.width * .4f + unit, 0, r.width * .4f, unit), managerSpawnPointID.intValue, spawnerName, EditorStyles.toolbarPopup);

            if (error) GUI.color = Color.red;
            if (GUI.Button(new Rect(5f + r.width * .4f, 0, 80, unit), "Spwan Point", EditorStyles.toolbarDropDown))
            {
                Vector2 gsp = GUIUtility.GUIToScreenPoint(new Vector2(r.x + 5f + (r.width * .4f) - unit, 0));
                if (error)
                    me.waveSequence[i].selectedSpawnPoints = ReconstructSpawnPointArray(me.waveSequence[i].selectedSpawnPoints);

                SpawnPointDropDownMenu w = CreateInstance<SpawnPointDropDownMenu>();
                w.SetMenu(me.waveSequence[i], me.childSpawnPoints, me.waveSequence[i].selectedSpawnPoints);
                float height = 45;
                float width = r.width * .4f;
                if (me.childSpawnPoints.Length > 0)
                    height = (me.childSpawnPoints.Length * unit) * 1.25f;
                if (me.childSpawnPoints.Length == 0)
                {
                    width = r.width * .5f;
                    height = 47;
                }
                w.ShowAsDropDown(new Rect(gsp.x, gsp.y, r.width * .4f, unit), new Vector2(width, height));
            }
            if (error)
            {
                GUI.color = Color.white;
                GUI.DrawTexture(new Rect(7f + r.width * .4f, 1f, unit, unit), EditorGUIUtility.FindTexture("console.warnicon"));
            }
#if UNITY_5_3
            if (GUI.Button(new Rect(r.width - 26, 0, 26, unit), EditorGUIUtility.FindTexture("d_Toolbar Plus"), EditorStyles.toolbarButton))
            {
                managerWaveSet.InsertArrayElementAtIndex(managerWaveSet.arraySize);
                //me.waveSequence[i].waveSet.Add(new EnemyWaveSet());
            }
#else
            if (GUI.Button(new Rect(5f + r.width * .4f + 100, 0, 90, unit), "Edit Wave Set", EditorStyles.toolbarDropDown))
            {
                Vector2 gsp = GUIUtility.GUIToScreenPoint(new Vector2(r.x + 5f + (r.width * .4f) - unit, 0));
                WaveSetEditor w = CreateInstance<WaveSetEditor>();
                w.SetMenu(me, managerWaveSet, i);
                float height = 4 * (unit * 1.25f);
                //if (managerWaveSet.arraySize > 3)
                //    height = (managerWaveSet.arraySize + 1) * (unit * 1.25f);
                //w.ShowUtility();
                //w.minSize = new Vector2(r.width * .5f, height + 10);
                //w.position = new Rect(gsp.x, gsp.y+ height + 10, r.width * .5f, height + 10);
                w.ShowAsDropDown(new Rect(gsp.x, gsp.y, r.width * .5f, unit), new Vector2(r.width * .5f, height + 10));
            }
#endif
            #endregion

            GUI.EndGroup();
            r.y += unit + (margin * 2);

            #region DrawEachWaveSet
#if UNITY_5_3
            for (int l = 0; l < managerWaveSet.arraySize; l++)
            {
                float y = r.y + (l * (unit * 1.25f));
                GUI.BeginGroup(new Rect(r.x, y, r.width, unit));
                GUI.color = Color.red;
                //GUIStyle remove = new GUIStyle(EditorStyles.miniButtonLeft);
                //remove.normal.textColor = Color.white;
                if (new Rect(0, 0, r.width, unit).Contains(Event.current.mousePosition) && Event.current.type == EventType.mouseDown && Event.current.button == 1)
                {
                    selectedSetIndex = l;
                    GenericMenu menu = new GenericMenu();
                    menu.AddItem(new GUIContent("Edit"), false, EditSet, me.waveSequence[i].waveSet[l]);
                    menu.AddSeparator(string.Empty);
                    menu.AddItem(new GUIContent("Add"), false, AddSet, managerWaveSet);
                    menu.AddItem(new GUIContent("Remove"), false, RemoveSet, managerWaveSet);
                    menu.ShowAsContext();
                }
                if (GUI.Button(new Rect(r.width - 24, 0, 24, unit), EditorGUIUtility.FindTexture("d_Toolbar Minus"), EditorStyles.miniButton))
                {
                    managerWaveSet.DeleteArrayElementAtIndex(l);
                    return;
                }
                GUI.color = Color.white;
                if (GUI.Button(new Rect(0, 0, 40, unit), "Edit", EditorStyles.miniButton))
                {
                    EnemyWaveSetEditor w = EditorWindow.GetWindow(typeof(EnemyWaveSetEditor), true) as EnemyWaveSetEditor;
                    //w.minSize = new Vector2(300, 300);
                    w.SetEditor(me.waveSequence[i].waveSet[l]);
                }
                EditorGUI.LabelField(new Rect(45, 0, r.width * .4f, unit), managerWaveSet.GetArrayElementAtIndex(l).FindPropertyRelative("name").stringValue, EditorStyles.miniLabel);
                //GUI.Box(new Rect(r.width * .5f + 35f, 0, 30, unit), Resources.Load("Dice") as Texture, EditorStyles.miniButtonLeft);
                //GUI.Box(new Rect(r.width * .5f + 65f, 0, r.width - (r.width * .5f) - 70, unit), GUIContent.none, EditorStyles.miniButtonRight);
                //EditorGUI.IntSlider(new Rect(r.width * .4f + 65, 0, r.width - (r.width * .4f) - 65, unit), managerWaveSet.GetArrayElementAtIndex(l).FindPropertyRelative("chance"), 0, 100, GUIContent.none);
                GUI.EndGroup();
            }
#endif
            #endregion
        };
#if UNITY_5_3
        rolist.elementHeightCallback = (int i) =>
        {
            Repaint();
            float height;
            var c = rolist.serializedProperty.GetArrayElementAtIndex(i).FindPropertyRelative("waveSet").arraySize;
            height = (c + 1) * (EditorGUIUtility.singleLineHeight * 1.25f);
            return height;
        };

        rolist.drawElementBackgroundCallback = (Rect r, int i, bool a, bool f) =>
        {
            var c = rolist.serializedProperty.GetArrayElementAtIndex(i).FindPropertyRelative("waveSet").arraySize;
            //r.height = (c + 1) * (EditorGUIUtility.singleLineHeight * 1.25f);
            r.x += 1f;
            r.width -= 3f;
            for (int l = 0; l <= c; l++)
            {
                if (a)
                {
                    if (l % 2 == 0)
                        EditorGUI.DrawRect(new Rect(r.x, r.y + (l * (EditorGUIUtility.singleLineHeight * 1.25f)), r.width, EditorGUIUtility.singleLineHeight * 1.25f), new Color(0.23f, 0.56f, 0.5f, 1f));
                    else
                        EditorGUI.DrawRect(new Rect(r.x, r.y + (l * (EditorGUIUtility.singleLineHeight * 1.25f)), r.width, EditorGUIUtility.singleLineHeight * 1.25f), new Color(0.23f, 0.56f, 0.5f, .8f));
                }
            }
            //if (a)
            //    EditorGUI.DrawRect(r, new Color(0.23f, 0.56f, 0.5f, 1f));
        };
#endif
        rolist.onSelectCallback = (ReorderableList l) =>
        {
            onSelectedIndex = l.index;
            if (!CheckSpawnPointArray(me.waveSequence[onSelectedIndex].selectedSpawnPoints))
                drawHandles = false;
            else
                drawHandles = true;
        };
        rolist.onAddDropdownCallback = (Rect r, ReorderableList l) =>
        {
            GenericMenu menu = new GenericMenu();
            menu.AddItem(new GUIContent("Create New"), false, CreateWaveSequence, null);

            if (l.index >= 0)
            {
                menu.AddSeparator(string.Empty);
                menu.AddItem(new GUIContent("Duplicate"), false, DuplicateWaveSequence, l.index);
            }
            menu.ShowAsContext();
        };
    }

    #region GenericMenu Function
#if UNITY_5_3
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
#endif
    void CreateWaveSequence(object obj)
    {
        if (me.waveSequence == null) return;
        me.waveSequence.Add(new EnemyWaveSequence());
        EditorUtility.SetDirty(target);
    }

    void DuplicateWaveSequence(object obj)
    {
        if (me.waveSequence == null) return;
        me.waveSequence.Add(new EnemyWaveSequence(me.waveSequence[(int)obj]));
    }
    #endregion

    public override void OnInspectorGUI()
    {
        GUILayout.Space(5);
        serializedObject.Update();
        me.childSpawnPoints = me.transform.GetComponentsInChildren<SpawnPoint>();

        if (debugToggle.boolValue = EditorInspector.ToggleLayout("Debug", debugToggle.boolValue))
        {
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("StartWave"))
            {
                if (Application.isPlaying)
                    me.StartWave();
            }
            if (GUILayout.Button("N.NextWave"))
            {
                if (Application.isPlaying)
                    me.NowNextWave();
            }
            if (GUILayout.Button("T.NextWave"))
            {
                if (Application.isPlaying)
                    me.TimeNextWave(waveDelay.intValue);
            }
            GUILayout.EndHorizontal();
            EditorGUILayout.PropertyField(gizmosColor, new GUIContent("Gizmos Color"));
            EditorGUILayout.PropertyField(gizmosSize, new GUIContent("Gizmos Size"));
        }

        if (settingToggle.boolValue = EditorInspector.ToggleLayout("Setting", settingToggle.boolValue))
        {
            autoStart.boolValue = EditorGUILayout.ToggleLeft(new GUIContent("Auto Start"), autoStart.boolValue);
            autoProceed.boolValue = EditorGUILayout.BeginToggleGroup(new GUIContent("Auto Proceed"), autoProceed.boolValue);
            EditorGUILayout.PropertyField(waveDelay, new GUIContent("Wave Delay"));
            EditorGUILayout.EndToggleGroup();
            EditorGUILayout.PropertyField(separatedPattern, new GUIContent("Separated Pattern"));
        }

        if (waveEventToggle.boolValue = EditorInspector.ToggleLayout("Wave Event", waveEventToggle.boolValue))
        {
            EditorGUILayout.PropertyField(onWaveStart, new GUIContent("On Wave Start"));
            EditorGUILayout.PropertyField(onWaveFinish, new GUIContent("On Wave Finish"));
            EditorGUILayout.PropertyField(onWaveClear, new GUIContent("On Wave Clear"));
        }

        if (waveProcessingEventToggle.boolValue = EditorInspector.ToggleLayout("Wave Processing Event", waveProcessingEventToggle.boolValue))
        {
            EditorGUILayout.PropertyField(onWaveProceedInt, new GUIContent("On Wave Proceed"));
            EditorGUILayout.PropertyField(onWaveProceedString, new GUIContent("On Wave Proceed"));
        }

        if (sequenceEventToggle.boolValue = EditorInspector.ToggleLayout("Sequence Event", sequenceEventToggle.boolValue))
        {
            EditorGUILayout.PropertyField(onAllWaveClear, new GUIContent("On All Wave Clear"));
        }

        EditorGUILayout.LabelField("Wave State", waveState.enumDisplayNames[waveState.enumValueIndex]);
        EditorGUILayout.LabelField(new GUIContent("Current Wave"), new GUIContent(currentWave.intValue.ToString()));

        if (waveSequence.arraySize > 0)
        {
            DrawWaveManager();
        }
        else
        {
            GUILayout.Space(5f);
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Start New Wave Sequence", GUILayout.Width(180)))
                CreateWaveSequence(null);
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
            GUILayout.Space(5f);
        }

        EditorUtility.SetDirty(me);
        serializedObject.ApplyModifiedProperties();
    }

    private bool CheckSpawnPointArray(SpawnPoint[] array)
    {
        for (int i = 0; i < array.Length; i++)
        {
            if (array[i] == null)
                return false;
        }
        return true;
    }

    private SpawnPoint[] ReconstructSpawnPointArray(SpawnPoint[] oldArray)
    {
#if (DEBUG)
        Debug.Log("Rebuild");
#endif
        List<SpawnPoint> list = new List<SpawnPoint>();
        for (int i = 0; i < oldArray.Length; i++)
        {
            if (oldArray[i] != null)
                list.Add(oldArray[i]);
        }
        return list.ToArray();
    }

    private Vector3 GetMeanVector(SpawnPoint[] positions)
    {
        if (positions.Length == 0)
            return Vector3.zero;
        float x = 0f;
        float y = 0f;
        float z = 0f;
        foreach (SpawnPoint pos in positions)
        {
            x += pos.transform.position.x;
            y += pos.transform.position.y;
            z += pos.transform.position.z;
        }
        return new Vector3(x / positions.Length, y / positions.Length, z / positions.Length);
    }

    void OnSceneGUI()
    {
        if (drawHandles)
        {
            if (waveSequence.arraySize > 0 && onSelectedIndex >= 0)
            {
                if (onSelectedIndex > waveSequence.arraySize - 1)
                    onSelectedIndex = waveSequence.arraySize - 1;
                if (me.waveSequence[onSelectedIndex] == null)
                    return;
                if (me.waveSequence[onSelectedIndex].selectedSpawnPoints.Length == 0)
                    return;
                if (me.childSpawnPoints.Length < me.waveSequence[onSelectedIndex].selectedSpawnPoints.Length)
                    return;
                Handles.color = gizmosColor.colorValue;
                for (int i = 0; i < me.waveSequence[onSelectedIndex].selectedSpawnPoints.Length; i++)
                {
                    if (me.waveSequence[onSelectedIndex].selectedSpawnPoints[i])
                    {
                        Handles.DrawLine(me.transform.position, me.waveSequence[onSelectedIndex].selectedSpawnPoints[i].transform.position);
                        Handles.DrawSolidDisc(me.waveSequence[onSelectedIndex].selectedSpawnPoints[i].transform.position, Vector3.up, gizmosSize.floatValue);
                    }
                }
                GUIStyle text = new GUIStyle(EditorStyles.whiteBoldLabel);
                text.normal.textColor = gizmosColor.colorValue;
                Handles.Label(GetMeanVector(me.waveSequence[onSelectedIndex].selectedSpawnPoints), me.waveSequence[onSelectedIndex].name, text);
            }
        }
    }
}