using UnityEngine;
using UnityEditor;
using System.Collections;

public class WaveEditor : EditorWindow
{
    string myString = "Hello World";
    bool groupEnabled;
    bool myBool = true;
    float myFloat = 1.23f;

    //static void Init()
    //{
    //    // Get existing open window or if none, make a new one:
    //    WaveEditor window = (WaveEditor)EditorWindow.GetWindow(typeof(WaveEditor));
    //    window.Show();
    //}

    void OnGUI()
    {
        GUILayout.Label("Base Settings", EditorStyles.boldLabel);
        myString = EditorGUILayout.TextField("Text Field", myString);

        groupEnabled = EditorGUILayout.BeginToggleGroup("Optional Settings", groupEnabled);
        myBool = EditorGUILayout.Toggle("Toggle", myBool);
        myFloat = EditorGUILayout.Slider("Slider", myFloat, -3, 3);
        EditorGUILayout.EndToggleGroup();
    }
}
