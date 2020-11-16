///<summary>
/// Created By Paul O'Callaghan
/// 
/// 2DPlayerController has been created specifically for this project. 
/// Some changes will be expected to be made to your project to get it to work.
/// This script is without license warranty or liability
/// Use of this script is free to use even commercially.
/// User of script accepts terms that the creator accepts no liability for misrepresentation 
/// or illegal use of script.
/// Script may be modified and distributed without acknowlegement.
/// 
/// </summary>


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace TwoDTools
{
    public class PlayerController2D : MonoBehaviour
    {
        #region Presets

        ///// Enum to be used by custom inspector for filling up variables with setting types to match 
        ///// popular game/control Styles
        //public enum ControlType
        //{
        //    SuperPlumberBro,
        //    FastHedgehog,
        //    Hamguy,
        //    Arcade,
        //    Custom
        //}
        //public ControlType controlType = ControlType.Custom;
        #endregion //Presets
        #region Controller Input Variables
        /// Required Input Source for Updates to Work With Other Components
        /// This should be replaced with your own input manager 
        public PlayerController2DInput input;
        #endregion

        #region Player Movement 
        #region Required Movement Variables
        public bool usePlayerMovement = false;
        TwoDTools.PlayerMovement playerMovement;

        public float currentSpeed = 0;
        public float maximumSpeed = 12f; // minimumSpeed is zero opposite direction is negative max

        [Tooltip("This is the floor walls and ceilings of our game world")]
        public LayerMask terrainLayer;
        #endregion // Required Movement Variables


        #region Ground Acceleration Variables
        public bool useAcceleration = false;
        public float acceleration = 5.0f;

        public bool useDeceleration = false;
        public float deceleration = 5.0f;

        #region Air Acceleration Variables
        public bool useAirDeceleration = false;
        public float airDeceleration = 1f;
        public float airAcceleration = 1.5f;

        #endregion //Air Acceleration Variables
        #endregion //Ground Acceleration Variables
        #endregion // Player Movement

        void Start()
        {
            if (usePlayerMovement)
                playerMovement = GetComponent<TwoDTools.PlayerMovement>();
        }

        // Update is called once per frame
        void Update()
        {
            if (usePlayerMovement)
            {
                playerMovement.RunUpdate();
            }
        }

        public PlayerController2DInput GetInput()
        {
            return input;
        }

        public void AddPlayerMovement()
        {
            if (!GetComponent<TwoDTools.PlayerMovement>())
            {
                gameObject.AddComponent<TwoDTools.PlayerMovement>();
            }
        }

        public void RemovePlayerMovement()
        {
            if(GetComponent<TwoDTools.PlayerMovement>())
            {
                DestroyImmediate(GetComponent<TwoDTools.PlayerMovement>());
            }
        }

    }
}

/// <summary>
/// Custom Editor Code
/// </summary>
[CustomEditor(typeof(TwoDTools.PlayerController2D))]
public class PlayerController2DEditor : Editor
{
    override public void OnInspectorGUI()
    {
        var playerController = target as TwoDTools.PlayerController2D;

        playerController.usePlayerMovement = EditorGUILayout.Toggle("Use Player Controller", playerController.usePlayerMovement);
        switch (playerController.usePlayerMovement)
        {
            case true:
                // You can edit this to take your own Input Manager class
                playerController.input = (TwoDTools.PlayerController2DInput)EditorGUILayout.ObjectField("Input Manager", playerController.input, typeof(TwoDTools.PlayerController2DInput), true);
                playerController.AddPlayerMovement();
                EditorGUI.BeginDisabledGroup(true); 
                // We only want the current speed as a readonly but still accessible by other classes/components for writing to
                playerController.currentSpeed = EditorGUILayout.FloatField("Current Speed", playerController.currentSpeed);
                EditorGUI.EndDisabledGroup();

                playerController.maximumSpeed = EditorGUILayout.FloatField("Maximum Speed", playerController.maximumSpeed);
                playerController.terrainLayer = EditorGUILayout.LayerField("Collision Layers", playerController.terrainLayer);

                //myScript.increaseAmount = EditorGUILayout.BeginToggleGroup("Move in Positive Direction", myScript.increaseAmount);
                //EditorGUILayout.EndToggleGroup();
                //myScript.orderInLayer = EditorGUILayout.IntField("Material Order In Layer", myScript.orderInLayer);
                //GUILayout.BeginHorizontal();
                //myScript.spriteLine = (Material)EditorGUILayout.ObjectField("Attached Material", myScript.spriteLine, typeof(Material), false);
                //GUILayout.EndHorizontal();
                break;

            case false:
                playerController.RemovePlayerMovement();
                break;
        }
        //EditorGUIUtility.ExitGUI();

    }
}