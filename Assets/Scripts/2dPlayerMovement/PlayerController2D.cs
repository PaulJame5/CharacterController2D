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
    [RequireComponent(typeof(TwoDTools.PlayerState))]
    public class PlayerController2D : MonoBehaviour
    {
        public static float GRAVITY = 11f;
        public static float MAX_FALL_SPEED = 15f;
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
        public TwoDTools.PlayerState playerState;
        #endregion


        #region Player Movement 

#if UNITY_EDITOR
        public bool showMovementGUI = true;
#endif

        // We use Vector3 to avoid any casting operations
        public Vector3 currentVelocity;
        public Vector3 externalForceVelocity;
        public Vector3 normalisedVelocity;
        TwoDTools.PlayerMovement playerMovement;
        public float maximumHorizontalVelocity = 12f; // minimumSpeed is zero opposite direction is negative max

        #endregion // Player Movement


        #region Acceleration and Momentum Variables
        public bool useAcceleration = false;
        public float acceleration = 5.0f;

        public bool useSprintAcceleration;
        public float sprintAcceleration = 7.5f;

        public float deceleration = 5.0f;

        public bool useAirMomentum = false;
        public float airDeceleration = 1f;
        public float airAcceleration = 1.5f;

        #endregion //Acceleration and momentum


        /// <summary>
        /// Everything Jumping Goes Here
        /// </summary>
        #region Jumping

#if UNITY_EDITOR
        public bool showJumpGUI = true;
#endif
        TwoDTools.PlayerJump playerJump;

        public enum JumpType
        {
            MeatSquare,
            BlueHedgehog,
            ItalianPlumber,
            PreItalianPlumber
        }
        public JumpType jumpType = JumpType.PreItalianPlumber;

        public float initialBurstJump = 8f;

        public float jumpVelocityDegradation = 1f;
        public float jumpVelocityDegradationWall = 1f;

        #endregion // Jumping



        #region Raycast Setup

#if UNITY_EDITOR
        public bool showRaycastGUI = true;
#endif
        // Horizontal
        public int horizontalRaycasts = 5;
        public float raycastSpreadAmountHorizontal = 0f;
        public float raycastLengthHorizontal = .31f;
        // Vertical
        public int verticalRaycasts = 5;
        public float raycastSpreadAmountVertical = 0f;
        public float raycastLengthVertical = .61f;
        // Layer Mask Setup
        public LayerMask terrainLayer;
        //public string[] collsionOptions = { "Default" };
        #endregion // Raycast Setup

        #region Falling
        public float maximumFallVelocity;
        #endregion


        private float initialXScale;
        private SpriteRenderer spriteRenderer;
        private BoxCollider2D boxCollider;

        private Rigidbody2D rb;


        // WallJump
        TwoDTools.PlayerWallJump playerWallJump;
        public void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = 60;
            initialXScale = transform.localScale.x;
            jumpType = JumpType.MeatSquare;
            playerState = GetComponent<TwoDTools.PlayerState>();

            playerMovement = GetComponent<TwoDTools.PlayerMovement>();

            playerJump = GetComponent<TwoDTools.PlayerJump>();

            spriteRenderer = GetComponent<SpriteRenderer>();

            boxCollider = GetComponent<BoxCollider2D>();

            playerWallJump = gameObject.AddComponent<TwoDTools.PlayerWallJump>();

        }

        // Update is called once per frame
        public void Update()
        {
            // Detect Input , Then Set Facing Direction before we  
            // use Raycasts for checking player state
            input.InputUpdate();
            SetFacingDirection();
            playerState.ResetAllStates();
            playerState.UpdatePlayerState();

            // Jump Update is a single button press adjustable  
            playerJump.JumpUpdate();
            playerWallJump.WallJumpUpdate();
        }

        public void FixedUpdate()
        {
            ApplyGravtiy();
            playerJump.JumpFixedUpdate();
            playerMovement.MovementUpdate();
            ApplyExternalForces();

            //ApplyGravityCalculation();

            NormaliseVelocity();
            rb.velocity = normalisedVelocity;

        }

        private void NormaliseVelocity()
        {
            if (playerState.IsTouchingSlope())
            {
                normalisedVelocity = playerMovement.MoveOnSlope();
                return;
            }
            normalisedVelocity = currentVelocity;
        }

        public TwoDTools.PlayerController2D Get()
        {
            return this;
        }

        void ApplyExternalForces()
        {
            currentVelocity += externalForceVelocity * Time.fixedDeltaTime;
        }


        public void ApplyGravtiy()
        {
            currentVelocity.y -= GRAVITY * Time.fixedDeltaTime;
            if (currentVelocity.y > MAX_FALL_SPEED)
            {
                currentVelocity.y = MAX_FALL_SPEED;
            }

            else if (currentVelocity.y < -MAX_FALL_SPEED)
            {
                currentVelocity.y = -MAX_FALL_SPEED;
            }
            if (playerState.IsTouchingCeiling() && currentVelocity.y > 0)
            {
                currentVelocity.y = 0;
                externalForceVelocity.y = 0;
                return;
            }
            if (playerState.IsJumping())
            {
                return;
            }
            if (playerState.IsTouchingFloor())
            {
                if (currentVelocity.y <= 0.0f)
                {
                    currentVelocity.y = 0;
                    return;
                }

            }

            //if (playerState.IsTouchingSlope())
            //{
            //    if (currentVelocity.y <= 0.0f)
            //    {
            //        currentVelocity.y = 0;
            //    }
            //}
        } // End Apply Gravity

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

        public void AddPlayerState()
        {
            if (!GetComponent<TwoDTools.PlayerState>())
            {
                gameObject.AddComponent<TwoDTools.PlayerState>();
            }
        }

        public void RemovePlayerMovement()
        {
            if(GetComponent<TwoDTools.PlayerMovement>())
            {
                DestroyImmediate(GetComponent<TwoDTools.PlayerMovement>());
            }
        }

        private void SetFacingDirection()
        {
            if(input.LeftButton())
            {
                FaceLeft();
                return;
            }
            if(input.RightButton())
            {
                FaceRight();
            }
        }

        // Used for flipping the sprites in a specific direction on the x scale
        private void FaceRight()
        {
            Vector3 scale = transform.localScale;
            scale.x = initialXScale;
            transform.localScale = scale;
        }

        private void FaceLeft()
        {
            Vector3 scale = transform.localScale;
            scale.x = -initialXScale;
            transform.localScale = scale;
        }

        public void ResetVelocity()
        {
            currentVelocity.x = 0;
            currentVelocity.y = 0;
        }

        // Used for unit testing
#if UNITY_EDITOR
        public void SetPlayerJump(TwoDTools.PlayerJump playerJump)
        {
            this.playerJump = playerJump;
        }
        public void SetControllerInput(TwoDTools.PlayerController2DInput input)
        {
            this.input = input;
        }

#endif

        void OnCollision2DStay(Collider2D col)
        {
            if (col.CompareTag("Terrain"))
            {
                currentVelocity.y = 0;
            }
        }

    }
}

#if UNITY_EDITOR
/// <summary>
/// Custom Editor Code
/// </summary>
[CustomEditor(typeof(TwoDTools.PlayerController2D))]
public class PlayerController2DEditor : UnityEditor.Editor
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


        TwoDTools.PlayerController2D.GRAVITY = EditorGUILayout.FloatField("Gravity", TwoDTools.PlayerController2D.GRAVITY);

        playerController.AddPlayerState();
        ///====================================================================================================================
        /// Movement Inspector
        ///====================================================================================================================
        playerController.showMovementGUI = EditorGUILayout.ToggleLeft("Show Movement Settings", playerController.showMovementGUI);
        switch (playerController.showMovementGUI)
        {
            case true:
                PlayerMovementGUI(playerController);
                break;

        }
        ///====================================================================================================================


        EditorGUILayout.Space(10);
        ///====================================================================================================================
        /// Jump Inspector
        ///====================================================================================================================
        playerController.showJumpGUI = EditorGUILayout.ToggleLeft("Show Jump Settings", playerController.showJumpGUI);
        switch (playerController.showJumpGUI)
        {
            case true:
                PlayerJumpGUI(playerController);
                break;

        }
        ///====================================================================================================================


        EditorGUILayout.Space(10);
        ///====================================================================================================================
        /// Raycast Inspector
        ///====================================================================================================================
        playerController.showRaycastGUI = EditorGUILayout.ToggleLeft("Show Raycast Settings", playerController.showRaycastGUI);
        switch (playerController.showRaycastGUI)
        {
            case true:
                RaycastGUI(playerController);
                break;

        }
        ///====================================================================================================================


        if (GUI.changed)
        {
            EditorUtility.SetDirty(playerController);
        }

    }



    void PlayerMovementGUI(TwoDTools.PlayerController2D playerController)
    {
        GUILayout.Label("Player Movement", titles);
        playerController.AddPlayerMovement();
        EditorGUI.BeginDisabledGroup(true);
        // We only want the current velocity as a readonly but still accessible by other classes/components for writing to
        playerController.currentVelocity = EditorGUILayout.Vector2Field("Current Speed", playerController.currentVelocity);
        playerController.normalisedVelocity = EditorGUILayout.Vector2Field("Normalised Speed", playerController.normalisedVelocity);
        EditorGUI.EndDisabledGroup();

        playerController.maximumHorizontalVelocity = EditorGUILayout.FloatField("Maximum Speed", playerController.maximumHorizontalVelocity);

        AccelerationGUIDisplay(playerController);
    }


    void RaycastGUI(TwoDTools.PlayerController2D playerController)
    {
        GUILayout.Label("Horizontal Raycast Setup", miniTitles);
        RaycastHorizontalGUISetup(playerController);
        GUILayout.Label("Vertical Raycast Setup", miniTitles);
        RaycastVerticalGUISetup(playerController);
        playerController.terrainLayer = TwoDTools.TwoDEditor.LayerMaskField("Collision Flags", playerController.terrainLayer);
    }


    void RaycastHorizontalGUISetup(TwoDTools.PlayerController2D playerController)
    {
        playerController.horizontalRaycasts = EditorGUILayout.IntField("Amount of Horizontal Raycasts", playerController.horizontalRaycasts);

        playerController.raycastSpreadAmountHorizontal = EditorGUILayout.FloatField("Spread Amount", playerController.raycastSpreadAmountHorizontal);

        playerController.raycastLengthHorizontal = EditorGUILayout.FloatField("Length", playerController.raycastLengthHorizontal);

    }
    void RaycastVerticalGUISetup(TwoDTools.PlayerController2D playerController)
    {
        playerController.verticalRaycasts = EditorGUILayout.IntField("Amount of Raycasts", playerController.verticalRaycasts);

        playerController.raycastSpreadAmountVertical = EditorGUILayout.FloatField("Spread Amount", playerController.raycastSpreadAmountVertical);

        playerController.raycastLengthVertical = EditorGUILayout.FloatField("Length", playerController.raycastLengthVertical);
    }

    void AccelerationGUIDisplay(TwoDTools.PlayerController2D playerController)
    {
        playerController.useAcceleration = EditorGUILayout.Toggle("Use Acceleration", playerController.useAcceleration);

        switch (playerController.useAcceleration)
        {
            case true:
                playerController.acceleration = EditorGUILayout.FloatField("Acceleration", playerController.acceleration);
                playerController.deceleration = EditorGUILayout.FloatField("Deceleration", playerController.deceleration);

                playerController.useSprintAcceleration = EditorGUILayout.Toggle("Use Sprint", playerController.useSprintAcceleration);
                switch (playerController.useSprintAcceleration)
                {
                    case true:
                        playerController.sprintAcceleration = EditorGUILayout.FloatField("Sprint Acceleration", playerController.sprintAcceleration);
                        break;
                }//end sprint

                playerController.useAirMomentum = EditorGUILayout.Toggle("Use Air Momentum", playerController.useAirMomentum);
                switch (playerController.useAirMomentum)
                {
                    case true:
                        playerController.airAcceleration = EditorGUILayout.FloatField("Air Acceleration", playerController.airAcceleration);
                        playerController.airDeceleration = EditorGUILayout.FloatField("Air Deceleration", playerController.airDeceleration);
                        break;
                }// end airMomentum

                break;
        }//end acceleration

    }

    void PlayerJumpGUI(TwoDTools.PlayerController2D playerController)
    {

        GUILayout.Label("Player Jump", titles);
        //public JumpType jumpType = JumpType.PreItalianPlumber;
        playerController.initialBurstJump = EditorGUILayout.FloatField("Initial Jump Velocity", playerController.initialBurstJump);
        playerController.jumpVelocityDegradation = EditorGUILayout.FloatField("Jump Velocity Degradation", playerController.jumpVelocityDegradation);
        
        playerController.jumpVelocityDegradationWall = EditorGUILayout.FloatField(new GUIContent("Gravity Friction", "Friction Multiplier when against terrain"), 
            playerController.jumpVelocityDegradationWall);

    }

}
#endif