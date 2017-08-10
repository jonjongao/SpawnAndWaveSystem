using UnityEngine;
using UnityEngine.Events;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Animations;
#endif
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace Gears
{
    /*=====================================================
            GearsUtility ver 2.5 build 04142016
    -------------------------------------------------------
    ported from old-GearsPropertyEditor
    WIP
    =====================================================*/
    public static class GearsUtility
    {
        #region Runtime Utility
        public static Quaternion LookRotation(GameObject self, GameObject target, float speed)
        {
            float step = speed * Time.deltaTime;
            Vector3 direction = (target.transform.position - self.transform.position).normalized;
            Vector3 tween = Vector3.RotateTowards(self.transform.forward, new Vector3(direction.x, 0, direction.z), step, 0.0f);
            return Quaternion.LookRotation(tween);
        }

        public static string GetRelativeResourcePath(string assetPath)
        {
            string pathWithExtension = assetPath.Substring(assetPath.IndexOf("Resources") + 10);
            return pathWithExtension.Remove(pathWithExtension.IndexOf(".asset"));
        }

        private static string[] SplitString(string splitCharacter, string value)
        {
            char[] key = splitCharacter.ToCharArray();
            return value.Split(key);
        }

        public static bool CheckEnumMask(int enumTarget, int value)
        {
            if ((value & enumTarget) > 0)
                return true;
            else
                return false;
        }

        public static bool CheckLayerMask(GameObject target, LayerMask value)
        {
            int hitLayerMask = (1 << target.layer);
            if ((value.value & hitLayerMask) > 0)
                return true;
            else
                return false;
        }

        public static void DrawLines(Vector3[] points)
        {
            for (int i = 0; i < points.Length; i++)
            {
                if (i == points.Length - 1)
                    Gizmos.DrawLine(points[i], points[0]);
                else
                    Gizmos.DrawLine(points[i], points[i + 1]);
            }
        }
        #endregion

        #region Editor Utility
#if UNITY_EDITOR
        public static void AssetList(ref string[] names, ref string[] paths, string filter, bool includeNone)
        {
            string[] guid = new string[0];
            AssetList(ref names, ref paths, ref guid, filter, includeNone);
        }

        public static void AssetList(ref string[] names, ref string[] paths, ref string[] guids, string filter, bool includeNone)
        {
            string[] guid = AssetDatabase.FindAssets(filter);
            List<string> n = new List<string>();
            List<string> p = new List<string>();
            List<string> g = guid.ToList();
            if (includeNone)
            {
                n.Add("None");
                p.Add("None");
                g.Add("None");
            }
            if (guid.Length > 0)
            {
                foreach (string id in guid)
                {
                    string path = AssetDatabase.GUIDToAssetPath(id);
                    char[] key = "/.".ToCharArray();
                    string[] split = path.Split(key);
                    string name = split[split.Length - 2];
                    n.Add(name);
                    p.Add(path);
                }
            }
            names = n.ToArray();
            paths = p.ToArray();
            guids = g.ToArray();
        }
#endif
        #endregion

        /*======================================================================
                DrawArrow
        ------------------------------------------------------------------------
        source code from AnomalusUndrdog(@AnomalusUndrdog)
        published post : http://forum.unity3d.com/threads/debug-drawarrow.85980/
        ported to Gears by JonGao(@rosa89n20)
        ======================================================================*/
        public static class DrawArrow
        {
#if UNITY_EDITOR
            public static void ForHandle(Vector3 posA, Vector3 posB, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20.0f)
            {
                Handles.DrawLine(posA, posB);
                Vector3 direction = posB - posA;

                Vector3 right = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 + arrowHeadAngle, 0) * new Vector3(0, 0, 1);
                Vector3 left = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 - arrowHeadAngle, 0) * new Vector3(0, 0, 1);
                Handles.DrawLine(posA + direction, (posA + direction) + right * arrowHeadLength);
                Handles.DrawLine(posA + direction, (posA + direction) + left * arrowHeadLength);
            }
#endif

            public static void ForGizmo(Vector3 pos, Vector3 direction, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20.0f)
            {
                Gizmos.DrawRay(pos, direction);

                Vector3 right = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 + arrowHeadAngle, 0) * new Vector3(0, 0, 1);
                Vector3 left = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 - arrowHeadAngle, 0) * new Vector3(0, 0, 1);
                Gizmos.DrawRay(pos + direction, right * arrowHeadLength);
                Gizmos.DrawRay(pos + direction, left * arrowHeadLength);
            }

            public static void ForGizmo(Vector3 pos, Vector3 direction, Color color, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20.0f)
            {
                Gizmos.color = color;
                Gizmos.DrawRay(pos, direction);

                Vector3 right = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 + arrowHeadAngle, 0) * new Vector3(0, 0, 1);
                Vector3 left = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 - arrowHeadAngle, 0) * new Vector3(0, 0, 1);
                Gizmos.DrawRay(pos + direction, right * arrowHeadLength);
                Gizmos.DrawRay(pos + direction, left * arrowHeadLength);
            }

            public static void ForDebug(Vector3 pos, Vector3 direction, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20.0f)
            {
                Debug.DrawRay(pos, direction);

                Vector3 right = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 + arrowHeadAngle, 0) * new Vector3(0, 0, 1);
                Vector3 left = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 - arrowHeadAngle, 0) * new Vector3(0, 0, 1);
                Debug.DrawRay(pos + direction, right * arrowHeadLength);
                Debug.DrawRay(pos + direction, left * arrowHeadLength);
            }
            public static void ForDebug(Vector3 pos, Vector3 direction, Color color, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20.0f)
            {
                Debug.DrawRay(pos, direction, color);

                Vector3 right = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 + arrowHeadAngle, 0) * new Vector3(0, 0, 1);
                Vector3 left = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 - arrowHeadAngle, 0) * new Vector3(0, 0, 1);
                Debug.DrawRay(pos + direction, right * arrowHeadLength, color);
                Debug.DrawRay(pos + direction, left * arrowHeadLength, color);
            }
        }

    }
}