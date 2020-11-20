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

        #region Raycast Setup
        [Tooltip("This is the floor walls and ceilings of our game world")]
        public int horizontalRaycasts = 5;
        public float raycastSpreadAmountHorizontal = 0f;
        public float raycastLengthHorizontal = .31f;
        public LayerMask terrainLayer;
        public string[] collsionOptions = { "Terrain", "Ice", "Water" };
        #endregion // Raycast Setup
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
    GUIStyle titles = new GUIStyle();
    GUIStyle miniTitles = new GUIStyle();


    void SetupTitlesGUIStyle()
    {
        titles.normal.background = TwoDTools.TwoDEditor.MakeTex(100, 1, Color.black);
        titles.fontStyle = FontStyle.BoldAndItalic;
        titles.fontSize = 14;
        titles.alignment = TextAnchor.UpperCenter;
        titles.normal.textColor = Color.white;
    }
    void SetupMiniTitlesGUIStyle()
    {
        miniTitles.normal.background = TwoDTools.TwoDEditor.MakeTex(70, 1, Color.black);
        miniTitles.fontStyle = FontStyle.BoldAndItalic;
        miniTitles.fontSize = 10;
        miniTitles.alignment = TextAnchor.UpperCenter;
        miniTitles.normal.textColor = Color.white;
    }

    override public void OnInspectorGUI()
    {
        SetupTitlesGUIStyle();
        SetupMiniTitlesGUIStyle();
        var playerController = target as TwoDTools.PlayerController2D;

        ///=======================================================================================================
        //You can edit this to take your own Input Manager class
        playerController.input = (TwoDTools.PlayerController2DInput)EditorGUILayout.ObjectField("Input Manager", playerController.input, typeof(TwoDTools.PlayerController2DInput), true);
        ///========================================================================================================

        switch (playerController.usePlayerMovement)
        {
            case true:
                GUILayout.Label("Player Movement", titles);
                playerController.AddPlayerMovement();
                EditorGUI.BeginDisabledGroup(true); 
                // We only want the current speed as a readonly but still accessible by other classes/components for writing to
                playerController.currentSpeed = EditorGUILayout.FloatField("Current Speed", playerController.currentSpeed);
                EditorGUI.EndDisabledGroup();

                playerController.maximumSpeed = EditorGUILayout.FloatField("Maximum Speed", playerController.maximumSpeed);

                AccelerationGUIDisplay(playerController);


                GUILayout.Label("Raycast Setup", miniTitles);
                RaycastGUISetup(playerController);

               
                if (GUILayout.Button("Remove PlayerMovement Component"))
                {
                    playerController.usePlayerMovement = false;
                    return;
                }
                break;

            case false:
                if (GUILayout.Button("Add PlayerMovement Component"))
                {
                    playerController.usePlayerMovement = true;
                    return;
                }
                playerController.RemovePlayerMovement();
                break;
        }
        //EditorGUIUtility.ExitGUI();

    }

    void RaycastGUISetup(TwoDTools.PlayerController2D playerController)
    {
        playerController.horizontalRaycasts = EditorGUILayout.IntField("Amount of Raycasts", playerController.horizontalRaycasts);

        playerController.raycastSpreadAmountHorizontal = EditorGUILayout.FloatField("Spread Amount", playerController.raycastSpreadAmountHorizontal);
        
        playerController.raycastLengthHorizontal = EditorGUILayout.FloatField("Length", playerController.raycastLengthHorizontal);

        playerController.terrainLayer = TwoDTools.TwoDEditor.LayerMaskField("Collision Flags", playerController.terrainLayer);

    }

    void AccelerationGUIDisplay(TwoDTools.PlayerController2D playerController)
    {
        playerController.useAcceleration = EditorGUILayout.Toggle("Use Acceleration", playerController.useAcceleration);

        switch (playerController.useAcceleration)
        {
            case true:
                playerController.acceleration = EditorGUILayout.FloatField("Acceleration", playerController.acceleration);
                playerController.useDeceleration = EditorGUILayout.Toggle("Use Deceleration", playerController.useDeceleration);

                switch (playerController.useDeceleration)
                {
                    case true:
                        playerController.deceleration = EditorGUILayout.FloatField("Deceleration", playerController.deceleration);

                        break;
                    case false:
                        break;
                }
                break;
            case false:
                break;
        }

    }
}