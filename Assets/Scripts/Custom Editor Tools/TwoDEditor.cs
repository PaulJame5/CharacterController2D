﻿///<summary>
/// Thanks to GameDeveloper12 for this simple script for solving layerMask issues with custom editor
/// Code can be found here: https://answers.unity.com/questions/1073094/custom-inspector-layer-mask-variable.html
/// 
/// Thanks to PetarJ and andeeee for making a background color possible in the custom editor
/// Code can be found from here: https://forum.unity.com/threads/changing-the-background-color-for-beginhorizontal.66015/
/// </summary>

#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

namespace TwoDTools
{
    public class TwoDEditor
    {

        static List<string> layers;
        static string[] layerNames;

        public static LayerMask LayerMaskField(string label, LayerMask selected)
        {

            if (layers == null)
            {
                layers = new List<string>();
                layerNames = new string[4];
            }
            else
            {
                layers.Clear();
            }

            int emptyLayers = 0;
            for (int i = 0; i < 32; i++)
            {
                string layerName = LayerMask.LayerToName(i);

                if (layerName != "")
                {

                    for (; emptyLayers > 0; emptyLayers--) layers.Add("Layer " + (i - emptyLayers));
                    layers.Add(layerName);
                }
                else
                {
                    emptyLayers++;
                }
            }

            if (layerNames.Length != layers.Count)
            {
                layerNames = new string[layers.Count];
            }
            for (int i = 0; i < layerNames.Length; i++) layerNames[i] = layers[i];

            selected.value = EditorGUILayout.MaskField(label, selected.value, layerNames);

            return selected;
        }

        public static Texture2D MakeTex(int width, int height, Color col)
        {
            Color[] pix = new Color[width * height];

            for (int i = 0; i < pix.Length; i++)
                pix[i] = col;

            Texture2D result = new Texture2D(width, height);
            result.SetPixels(pix);
            result.Apply();

            return result;
        }


    }
}
#endif
