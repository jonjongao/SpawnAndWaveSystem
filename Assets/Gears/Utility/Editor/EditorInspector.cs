using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Gears
{
    public class EditorInspector
    {
        public static bool ToggleLayout(string label, bool toggle)
        {
            Rect rect = GetRect(EditorGUIUtility.singleLineHeight);
            rect.x += 2;
            rect.width -= 4;

            var click = DrawShurikenStyleToggleButton(rect, label, toggle);
            return click;
        }

        public static bool ToggleLayoutWithCheckBox(string label, bool toggle, ref bool checkBox)
        {
            Rect rect = GetRect(EditorGUIUtility.singleLineHeight);
            rect.x += 2;
            rect.width -= 4;

            var click = DrawShurikenStyleToggleButton(rect, label, toggle);
            checkBox = DrawShurikenStyleCheckBox(rect, checkBox);
            return click;
        }

        public static void WrapperLine()
        {
            Rect rect = GetRect(EditorGUIUtility.singleLineHeight);
            GUI.Box(new Rect(rect.x - 15f, rect.y + (rect.height * .5f), rect.width + 30f, rect.height), GUIContent.none, (GUIStyle)"IN Title");
        }

        public static void LayoutLine()
        {
            GUILayout.Box(GUIContent.none, (GUIStyle)"IN Title", GUILayout.Height(5));
        }

        public static bool IsMouseClick(Rect rect, int button)
        {
            bool isMouseDown = (Event.current.type == EventType.mouseDown);
            bool isCorrectButton = (Event.current.button == button);
            bool isInRect = rect.Contains(Event.current.mousePosition);
            return isMouseDown && isCorrectButton && isInRect;
        }

        private static bool DrawShurikenStyleToggleButton(Rect rect, string label, bool toggle)
        {
            GUI.Box(rect, GUIContent.none, (GUIStyle)"ShurikenModuleTitle");
            var click = GUI.Button(new Rect(rect.x + 20, rect.y, rect.width - 30, rect.height), new GUIContent(label), (GUIStyle)"ShurikenLabel");

            return click ? !toggle : toggle;
        }

        private static bool DrawShurikenStyleCheckBox(Rect rect, bool value)
        {
            return EditorGUI.Toggle(new Rect(rect.x + 2, rect.y + 2, 20, rect.height), value, (GUIStyle)"ShurikenCheckMark");
        }

        static Rect GetRect(float height)
        {
            return GUILayoutUtility.GetRect(EditorGUIUtility.fieldWidth, height);
        }
    }
}