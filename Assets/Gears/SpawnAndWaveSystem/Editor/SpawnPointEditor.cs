using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Events;
using Gears;

[CustomEditor(typeof(SpawnPoint)), CanEditMultipleObjects]
public class EnemySpawnerEditor : Editor
{
    #region OverallDefiner
    private SpawnPoint me;
    private SerializedProperty controlled;
    private SerializedProperty mode;
    private SerializedProperty spawnAreaType;
    private SerializedProperty spawnPointType;
    #endregion
    private SerializedProperty auto;
    #region AutoRespawn
    private SerializedProperty autoRespawn;
    private SerializedProperty recoverTime;
    #endregion
    #region HordeLOD
    private SerializedProperty hordeLod;
    private SerializedProperty player;
    private SerializedProperty activeDistance;
    private SerializedProperty hordeActiveRefreshTime;
    #endregion
    #region Basic
    private SerializedProperty hordeTag;
    private SerializedProperty enemys;
    private SerializedProperty radius;
    private SerializedProperty spawnEffect;
    private SerializedProperty separateDistance;
    private SerializedProperty spawnBuffer;
    private SerializedProperty spawnState;
    #endregion
    #region Horde
    #endregion
    #region Editor
    private SerializedProperty selfPoints;
    private SerializedProperty worldPoints;
    private SerializedProperty closePoints;
    #endregion
    #region Event
    private SerializedProperty onSequenceStart;
    private SerializedProperty onSequenceFinish;
    #endregion
    private ReorderableList rolist;

    private bool placing = false;
    private bool editing = false;

    bool CheckManager(Transform target, ref EnemyWaveManager manager)
    {
        bool check;
        if (target.GetComponentInParent<EnemyWaveManager>())
        {
            manager = target.GetComponentInParent<EnemyWaveManager>();
            check = true;
        }
        else
        {
            check = false;
        }
        serializedObject.ApplyModifiedProperties();
        return check;
    }

    void OnEnable()
    {
        me = (SpawnPoint)target;
        me.transform.hasChanged = false;
        controlled = serializedObject.FindProperty("controlled");
        controlled.boolValue = CheckManager(me.transform, ref me.manager);

        #region OverallDefiner
        mode = serializedObject.FindProperty("mode");
        spawnAreaType = serializedObject.FindProperty("spawnAreaShape");
        spawnPointType = serializedObject.FindProperty("spawnPointType");
        #endregion
        auto = serializedObject.FindProperty("auto");
        #region AutoRespawn
        autoRespawn = serializedObject.FindProperty("autoRespawn");
        recoverTime = serializedObject.FindProperty("recoverTime");
        #endregion
        #region HordeLOD
        hordeLod = serializedObject.FindProperty("hordeLod");
        player = serializedObject.FindProperty("player");
        activeDistance = serializedObject.FindProperty("activeDistance");
        hordeActiveRefreshTime = serializedObject.FindProperty("hordeActiveRefreshTime");
        #endregion
        #region Basic
        hordeTag = serializedObject.FindProperty("hordeTag");
        enemys = serializedObject.FindProperty("enemys");
        radius = serializedObject.FindProperty("radius");
        spawnEffect = serializedObject.FindProperty("spawnEffect");
        separateDistance = serializedObject.FindProperty("separateDistance");
        spawnBuffer = serializedObject.FindProperty("spawnBuffer");
        spawnState = serializedObject.FindProperty("spawnState");
        #endregion
        #region Horde
        #endregion
        #region Editor
        selfPoints = serializedObject.FindProperty("selfPoints");
        worldPoints = serializedObject.FindProperty("worldPoints");
        closePoints = serializedObject.FindProperty("closePoints");
        #endregion
        #region Event
        onSequenceStart = serializedObject.FindProperty("onSequenceStart");
        onSequenceFinish = serializedObject.FindProperty("onSequenceFinish");
        #endregion
        rolist = new ReorderableList(serializedObject, enemys, true, true, true, true);

        if (me && serializedObject != null && selfPoints.arraySize == 0)
        {
            UpdatePointsProperty(CreateBox(2f), selfPoints);
            SceneView.RepaintAll();
        }

        if (controlled.boolValue)
        {
            auto.boolValue = false;
            autoRespawn.boolValue = false;
        }
    }

    private void UpdatePointsProperty(Vector2[] array, SerializedProperty property)
    {
        property.ClearArray();
        for (int i = 0; i < array.Length; i++)
        {
            property.arraySize++;
            property.GetArrayElementAtIndex(i).vector2Value = array[i];
        }

        property.serializedObject.ApplyModifiedProperties();
    }
    private void UpdatePointsProperty(Vector3[] array, SerializedProperty property)
    {
        property.ClearArray();
        for (int i = 0; i < array.Length; i++)
        {
            property.arraySize++;
            property.GetArrayElementAtIndex(i).vector3Value = array[i];
        }

        property.serializedObject.ApplyModifiedProperties();
    }

    private Vector3[] GetPointsProperty(SerializedProperty property)
    {
        Vector3[] array = new Vector3[property.arraySize];
        for (int i = 0; i < array.Length; i++)
        {
            array[i] = property.GetArrayElementAtIndex(i).vector3Value;
        }

        return array;
    }

    private Vector3[] CreateBox(float size) { return ConvertPoints(CreateGeometry(4, 4, size), Space.Self); }
    private Vector3[] CreateCircle(float size) { return ConvertPoints(CreateGeometry(12, Mathf.PI, size), Space.Self); }
    private Vector3[] CreatePolygon(float size) { return ConvertPoints(CreateGeometry(5, 2, size), Space.Self); }
    private Vector3[] CreateGeometry(int vertices, float angle, float size)
    {
        float delta = Mathf.PI / angle;
        Vector3[] points = new Vector3[vertices];
        for (int i = 0; i < vertices; i++)
        {
            points[i] = new Vector3(
               (1 * Mathf.Cos(2f * i * Mathf.PI / vertices + delta)),
                0,
                -(1 * Mathf.Sin(2f * i * Mathf.PI / vertices + delta))) * size;
        }
        return points;
    }

    private Vector3[] ConvertPoints(Vector3[] reference, Space space)
    {
        int count = reference.Length;
        List<Vector3> points = new List<Vector3>();
        for (int i = 0; i < count; i++)
        {
            if (space == Space.Self)
                points.Add(me.transform.InverseTransformPoint(me.transform.TransformPoint(reference[i])));
            else
                points.Add(me.transform.TransformPoint(reference[i]));
        }
        return points.ToArray();
    }

    GUIStyle GetMiniButton(int index, int length)
    {
        GUIStyle style;
        if (index == 0) style = EditorStyles.miniButtonLeft;
        else if (index == length - 1) style = EditorStyles.miniButtonRight;
        else style = EditorStyles.miniButtonMid;
        return style;
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        if (me.transform.hasChanged)
            controlled.boolValue = CheckManager(me.transform, ref me.manager);

        EditorGUILayout.Space();

        GUILayout.Label("Mode", EditorStyles.boldLabel);

        GUILayout.BeginHorizontal();
        for (int i = 0; i < mode.enumDisplayNames.Length; i++)
        {
            if (mode.enumValueIndex == i) GUI.color = Color.green;
            else GUI.color = Color.white;
            if (GUILayout.Button(mode.enumDisplayNames[i], GetMiniButton(i, mode.enumDisplayNames.Length), GUILayout.Height(EditorGUIUtility.singleLineHeight)))
            {
                switch (mode.enumDisplayNames[i])
                {
                    case "Simple":
                        mode.enumValueIndex = 0;
                        break;
                    case "Advanced":
                        mode.enumValueIndex = 1;
                        break;
                }
            }
        }
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        switch (mode.enumValueIndex)
        {
            case 0:
                GUI.color = Color.white;
                for (int i = 0; i < spawnPointType.enumDisplayNames.Length; i++)
                {
                    string name = spawnPointType.enumDisplayNames[i];
                    if (i == spawnPointType.enumValueIndex) GUI.color = Color.green;
                    else GUI.color = Color.white;

                    if (GUILayout.Button(name, GetMiniButton(i, spawnPointType.enumDisplayNames.Length), GUILayout.Height(EditorGUIUtility.singleLineHeight)))
                    {
                        switch (name)
                        {
                            case "Box":
                                spawnPointType.enumValueIndex = 0;
                                break;
                            case "Sphere":
                                spawnPointType.enumValueIndex = 1;
                                break;
                            case "Point":
                                spawnPointType.enumValueIndex = 2;
                                break;
                        }
                        SceneView.RepaintAll();
                    }
                }
                GUI.color = Color.white;
                break;
            case 1:
                GUI.color = Color.white;
                for (int i = 0; i < spawnAreaType.enumDisplayNames.Length; i++)
                {
                    string name = spawnAreaType.enumDisplayNames[i];
                    if (i == spawnAreaType.enumValueIndex) GUI.color = Color.green;
                    else GUI.color = Color.white;

                    if (GUILayout.Button(name, GetMiniButton(i, spawnAreaType.enumDisplayNames.Length), GUILayout.Height(EditorGUIUtility.singleLineHeight)))
                    {
                        switch (name)
                        {
                            case "Rectangle":
                                UpdatePointsProperty(CreateBox(2f), selfPoints);
                                spawnAreaType.enumValueIndex = 0;
                                break;
                            case "Circle":
                                UpdatePointsProperty(CreateCircle(2f), selfPoints);
                                spawnAreaType.enumValueIndex = 1;
                                break;
                            case "Polygon":
                                UpdatePointsProperty(CreatePolygon(2f), selfPoints);
                                spawnAreaType.enumValueIndex = 2;
                                break;
                        }
                        SceneView.RepaintAll();
                    }
                    GUI.color = Color.white;
                }
                if (editing) GUI.color = Color.red;
                else GUI.color = Color.white;
                if (GUILayout.Button("Edit Vertex", EditorStyles.miniButton, GUILayout.Height(EditorGUIUtility.singleLineHeight)))
                {
                    if (editing)
                    {
                        Tools.current = lastUsedTool;
                        editing = false;
                    }
                    else
                    {
                        lastUsedTool = Tools.current;
                        editing = true;
                    }
                }
                GUI.color = Color.white;
                break;
            case 2:
                GUI.color = Color.red;
                GUILayout.Label("Adopt Child Transform", EditorStyles.miniButton, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                GUI.color = Color.white;
                break;
        }
        GUILayout.EndHorizontal();

        if (controlled.boolValue)
            EditorGUILayout.HelpBox("This SpawnPoint is controlled by EnemyWaveManager", MessageType.Warning);

        //GearsUtility.DrawWrapperLine();

        //-------------------------------------------------
        //Only available when not-controlled
        //-------------------------------------------------
        EditorGUI.BeginDisabledGroup(controlled.boolValue);

        auto.boolValue = EditorGUILayout.ToggleLeft("Auto Spawn", auto.boolValue, EditorStyles.boldLabel);
        //GearsUtility.DrawWrapperLine();
        autoRespawn.boolValue = EditorGUILayout.BeginToggleGroup("Auto Respawn", autoRespawn.boolValue);
        if (autoRespawn.boolValue)
        {
            EditorGUILayout.PropertyField(recoverTime, new GUIContent("Recover Time (s)"));
        }
        EditorGUILayout.EndToggleGroup();
        //GearsUtility.DrawWrapperLine();
        hordeLod.boolValue = EditorGUILayout.BeginToggleGroup("Horde LOD", hordeLod.boolValue);
        if (hordeLod.boolValue)
        {
            EditorGUILayout.PropertyField(player);
            EditorGUILayout.PropertyField(activeDistance, new GUIContent("Activate Distance"));
            EditorGUILayout.PropertyField(hordeActiveRefreshTime, new GUIContent("LOD Update Time (s)"));
        }
        EditorGUILayout.EndToggleGroup();

        EditorGUI.EndDisabledGroup();
        //GearsUtility.DrawWrapperLine();
        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(13);
        EditorGUILayout.LabelField("Setting", EditorStyles.boldLabel);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.PropertyField(hordeTag, new GUIContent("Tag"));
        if (mode.enumValueIndex == 0)
        {
            radius.floatValue = me.transform.localScale.x;
            me.transform.localScale = Vector3.one * radius.floatValue;
        }
        //EditorGUILayout.LabelField(new GUIContent("Radius"), new GUIContent("Inherit by LocalScaleX"));
        //EditorGUILayout.PropertyField(radius, new GUIContent("Radius"));
        EditorGUILayout.PropertyField(spawnEffect, new GUIContent("Spawn Effect"));
        EditorGUILayout.PropertyField(separateDistance, new GUIContent("Separate Distance"));
        EditorGUILayout.PropertyField(spawnBuffer, new GUIContent("Spawn Delay (s)"));
        EditorGUILayout.LabelField("Wave State", spawnState.enumDisplayNames[spawnState.enumValueIndex], EditorStyles.boldLabel);
        //GearsUtility.DrawWrapperLine();

        //-------------------------------------------------
        //Only available when not-controlled
        //-------------------------------------------------
        EditorGUI.BeginDisabledGroup(controlled.boolValue);

        GUILayout.Space(3f);
        rolist.DoLayoutList();
        rolist.drawHeaderCallback = (Rect r) =>
        {
            EditorGUI.LabelField(r, "Spawn Pool");
        };
        rolist.drawElementCallback = (Rect r, int i, bool a, bool f) =>
        {
            r.y += 2f;
            var e = rolist.serializedProperty.GetArrayElementAtIndex(i);
            EditorGUI.PropertyField(new Rect(r.x, r.y, r.width * .85f, EditorGUIUtility.singleLineHeight), e.FindPropertyRelative("prefab"), GUIContent.none);
            EditorGUI.PropertyField(new Rect(r.x + r.width * .85f + 5f, r.y, r.width * .15f - 5f, EditorGUIUtility.singleLineHeight), e.FindPropertyRelative("number"), GUIContent.none);
        };
        GUILayout.Space(3f);
        //GearsUtility.DrawWrapperLine();
        EditorGUI.EndDisabledGroup();
        //-------------------------------------------------
        GUILayout.Space(3f);
        EditorGUILayout.PropertyField(onSequenceStart);
        EditorGUILayout.PropertyField(onSequenceFinish);

        if (Selection.objects.Length == 1 && GetPointsProperty(selfPoints).Length > 0)
        {
            UpdatePointsProperty(ConvertPoints(GetPointsProperty(selfPoints), Space.World), worldPoints);
        }

        EditorUtility.SetDirty(me);

        serializedObject.ApplyModifiedProperties();
        //DrawWrapperLine();
        //DrawDefaultInspector();
    }


    private Vector3 mousePosition;
    private int draggingPointIndex = -1;
    private Vector3 mouseDownPosition { get; set; }
    private Tool lastUsedTool;

    Vector2 IsPointOnLine(Vector3 alpha, Vector3 beta, Vector3 pointer)
    {
        Handles.color = Color.green;
        Handles.DrawLine(alpha, beta);
        Handles.color = Color.white;

        Vector2 a = new Vector2(alpha.x, alpha.z);
        Vector2 b = new Vector2(beta.x, beta.z);
        Vector2 c = HandleUtility.WorldToGUIPoint(pointer);

        Vector2 AP = c - a;       //Vector from A to P   
        Vector2 AB = b - a;       //Vector from A to B  

        float magnitudeAB = AB.magnitude;   //Magnitude of AB vector (it's length squared)     
        float ABAPproduct = Vector2.Dot(AP, AB);    //The DOT product of a_to_p and a_to_b     
        float distance = ABAPproduct / magnitudeAB; //The normalized "distance" from a to your closest point  

        if (distance < 0)     //Check if P projection is over vectorAB     
        {
            return a;

        }
        else if (distance > 1)
        {
            return b;
        }
        else
        {
            return a + AB * distance;
        }
    }

    void OnSceneGUI()
    {
        if (Application.isPlaying)
            editing = false;

        if (editing)
        {
            Ray worldRay = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
            RaycastHit hitInfo;
            Event e = Event.current;
            Tools.current = Tool.None;

            if (Physics.Raycast(worldRay, out hitInfo))
            {
                mousePosition = hitInfo.point;

                if (Selection.objects.Length == 1)
                {
                    if (!placing)
                    {
                        draggingPointIndex = FindClosestPoint(GetPointsProperty(worldPoints));
                    }

                    if (draggingPointIndex != -1)
                    {
                        Handles.color = Color.green;
                        Handles.DrawSolidDisc(GetPointsProperty(worldPoints)[draggingPointIndex], Vector3.up, SceneView.lastActiveSceneView.camera.nearClipPlane * 1.5f);
                        Handles.color = Color.white;
                    }
                }

                if (e.type == EventType.MouseDown && e.button == 0)
                {
                    mouseDownPosition = mousePosition;
                    e.Use();
                }
                else if (e.type == EventType.MouseDrag && e.button == 0)
                {
                    placing = true;

                    if (draggingPointIndex != -1)
                    {
                        Vector3 v = me.transform.InverseTransformPoint(mousePosition);
                        selfPoints.GetArrayElementAtIndex(draggingPointIndex).vector3Value = v;
                    }
                    e.Use();
                }
                else if (e.type == EventType.MouseUp && e.button == 0)
                {
                    placing = false;
                    e.Use();
                }
                else
                {
                    mouseDownPosition = Vector3.zero;
                }
            }
        }

        if (me)
        {
            if (Selection.objects.Length == 1)
            {
                switch (mode.enumValueIndex)
                {
                    case 0:
                        switch (spawnPointType.enumValueIndex)
                        {
                            //Box
                            case 0:
                                break;
                            //Sphere
                            case 1:
                                break;
                            //Point
                            case 2:
                                break;
                        }
                        break;
                    case 1:
                        if (GetPointsProperty(selfPoints).Length > 0)
                        {
                            if (editing)
                                UpdatePointsProperty(ConvertPoints(GetPointsProperty(selfPoints), Space.World), worldPoints);

                            List<Vector3> h = GetPointsProperty(worldPoints).ToList();
                            h.Add(h[0]);
                            Vector3[] handlePoints = h.ToArray();
                            Handles.color = new Color(0f, 1f, 0f, 0.2f);
                            Handles.DrawAAConvexPolygon(handlePoints);
                            Handles.color = new Color(0f, 1f, 0f, 0.4f);
                            Handles.DrawAAPolyLine(handlePoints);
                            for (int i = 0; i < h.Count; i++)
                            {
                                Handles.DrawSolidDisc(handlePoints[i], Vector3.up, SceneView.lastActiveSceneView.camera.nearClipPlane * 1.5f);
                            }
                        }
                        break;
                }
            }
            Handles.color = Color.white;
        }
        HandleUtility.AddDefaultControl(-1);
        HandleUtility.Repaint();
    }

    int[] FindMultipleClosestPoint(Vector3[] points, int length)
    {
        List<int> choose = new List<int>();
        Vector2 mouse = Event.current.mousePosition;
        List<Vector2> distances = new List<Vector2>();
        Vector2[] order;

        for (int i = 0; i < points.Length; i++)
        {
            Vector2 screenPoint = HandleUtility.WorldToGUIPoint(points[i]);
            distances.Add(new Vector2(Vector2.Distance(screenPoint, mouse), i));
        }

        order = distances.OrderBy(go => go.x).ToArray();
        for (int c = 0; c < length; c++)
        {
            choose.Add((int)order[c].y);
        }

        UpdatePointsProperty(order, closePoints);
        return choose.ToArray();
    }

    int FindClosestPoint(Vector3[] points)
    {
        List<int> check = new List<int>();
        Vector2 mouse = Event.current.mousePosition;
        int closest = -1;

        for (int i = 0; i < points.Length; i++)
        {
            Vector2 screenPoint = HandleUtility.WorldToGUIPoint(points[i]);
            if (Vector2.Distance(screenPoint, mouse) < 10)
                check.Add(i);
        }

        if (check.Count == 0)
            return closest;
        else if (check.Count == 1)
            return check[0];
        else
        {
            Vector3 sceneCamera = SceneView.lastActiveSceneView.camera.transform.position;
            float record = float.MaxValue;

            for (int i = 0; i < check.Count; i++)
            {
                float distance = Vector3.Distance(points[check[i]], sceneCamera);
                if (distance < record)
                {
                    record = distance;
                    closest = check[i];
                }
            }
            check.Clear();
            return closest;
        }
    }
}