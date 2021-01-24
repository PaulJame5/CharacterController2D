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

#if UNITY_EDITOR
using System.IO;
#endif

namespace TwoDTools
{
    [RequireComponent(typeof(TwoDTools.PlayerState))]
    public class PlayerController2D : MonoBehaviour
    {
        public PlayerController2DData playerControllerData;
        public PlayerController2DInput input;

        public TwoDTools.PlayerState playerState;
        TwoDTools.PlayerMovement playerMovement;
        TwoDTools.PlayerJump playerJump;
        TwoDTools.PlayerWallJump playerWallJump;

        // We use Vector3 to avoid any casting operations
        public Vector3 currentVelocity;
        public Vector3 externalForceVelocity;
        public Vector3 normalisedVelocity;

        public float pressedAt;

        public float lastTouchedGround;


        private float initialXScale;

        private SpriteRenderer spriteRenderer;
        private BoxCollider2D boxCollider;
        private Rigidbody2D rb;



#if UNITY_EDITOR

#endif

        public void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = 60;
            initialXScale = transform.localScale.x;
            playerControllerData.jumpType = PlayerController2DData.JumpType.MeatSquare;
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
            playerState.UpdatePlayerState();
            CoyoteTimeSetup();
            PreCoyoteTimeSetup();


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

            NormaliseVelocity();
            rb.velocity = normalisedVelocity;

        }

        void CoyoteTimeSetup()
        {
            if(!playerControllerData.useCoyoteTime)
            {
                return;
            }
            if(playerState.IsTouchingFloor() && playerState.IsJumping() == false)
            {
                lastTouchedGround = Time.timeSinceLevelLoad;
            }
        }

        void PreCoyoteTimeSetup()
        {
            if(!playerControllerData.usePreEmptiveCoyoteTime)
            {
                return;
            }
            if(input.JumpButtonPressed() && !playerState.IsTouchingFloor())
            {
                pressedAt = Time.timeSinceLevelLoad;
            }

        }

        public bool ApplySlopedVelocity()
        {
            if (!playerState.IsTouchingSlope())
            {
                return false;
            }
            normalisedVelocity = playerMovement.MoveOnSlope();
            return true;
        }

        private void NormaliseVelocity()
        {
            if(ApplySlopedVelocity())
            {
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
            currentVelocity.y -= playerControllerData.gravityForce * Time.fixedDeltaTime;
            if (currentVelocity.y > playerControllerData.maximumFallSpeed)
            {
                currentVelocity.y = playerControllerData.maximumFallSpeed;
            }

            else if (currentVelocity.y < -playerControllerData.maximumFallSpeed)
            {
                currentVelocity.y = -playerControllerData.maximumFallSpeed;
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


        public bool LoadSettings(string fileName)
        {
            string filePath = Application.dataPath + "/2DCharacterController Saved Settings/" + fileName + ".txt";

            if (File.Exists(filePath))
            {
                string dataAsJson = File.ReadAllText(filePath);
                playerControllerData = JsonUtility.FromJson<TwoDTools.PlayerController2DData>(dataAsJson);
                return true;
            }

            return false;
        }

        public bool SaveSettings(string fileName)
        {
            if (!Directory.Exists(Application.dataPath + "/" + "/2DCharacterController Saved Settings/"))
            {
                Directory.CreateDirectory(Application.dataPath + "/" + "/2DCharacterController Saved Settings/");
            }
            string dataAsJson = JsonUtility.ToJson(playerControllerData);

            using (StreamWriter sw = File.CreateText(Application.dataPath + "/2DCharacterController Saved Settings/" + fileName + ".txt"))
            {
                sw.Write(dataAsJson);
            }

            AssetDatabase.Refresh();
            return true;
        }

#endif

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

    // Saving / Exporting Settinigs
    public string fileName;
    public TextAsset file;
    public bool activateSave;
    public bool save;
    public bool activateLoad;
    public bool load;


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


        playerController.playerControllerData.gravityForce = EditorGUILayout.FloatField("Gravity", playerController.playerControllerData.gravityForce);
        playerController.playerControllerData.maximumFallSpeed = EditorGUILayout.FloatField("Maximum Fall Speed", playerController.playerControllerData.maximumFallSpeed);

        playerController.AddPlayerState();
        ///====================================================================================================================
        /// Movement Inspector
        ///====================================================================================================================
        playerController.playerControllerData.showMovementGUI = EditorGUILayout.ToggleLeft("Show Movement Settings", playerController.playerControllerData.showMovementGUI);
        switch (playerController.playerControllerData.showMovementGUI)
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
        playerController.playerControllerData.showJumpGUI = EditorGUILayout.ToggleLeft("Show Jump Settings", playerController.playerControllerData.showJumpGUI);
        switch (playerController.playerControllerData.showJumpGUI)
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
        playerController.playerControllerData.showRaycastGUI = EditorGUILayout.ToggleLeft("Show Raycast Settings", playerController.playerControllerData.showRaycastGUI);
        switch (playerController.playerControllerData.showRaycastGUI)
        {
            case true:
                RaycastGUI(playerController);
                break;

        }
        ///====================================================================================================================

        ///====================================================================================================================
        /// Save
        ///====================================================================================================================

        SaveSettingsGUI(playerController);
        ///====================================================================================================================
        /// Load
        ///====================================================================================================================
        LoadSettingsGUI(playerController);

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

        playerController.playerControllerData.maximumHorizontalVelocity = EditorGUILayout.FloatField("Maximum Speed", playerController.playerControllerData.maximumHorizontalVelocity);

        AccelerationGUIDisplay(playerController);
        SlopeMovementSettingsGUI(playerController);
    }


    void RaycastGUI(TwoDTools.PlayerController2D playerController)
    {
        GUILayout.Label("Horizontal Raycast Setup", miniTitles);
        RaycastHorizontalGUISetup(playerController);
        GUILayout.Label("Vertical Raycast Setup", miniTitles);
        RaycastVerticalGUISetup(playerController);
        playerController.playerControllerData.terrainLayer = TwoDTools.TwoDEditor.LayerMaskField("Collision Flags", playerController.playerControllerData.terrainLayer);
    }


    void RaycastHorizontalGUISetup(TwoDTools.PlayerController2D playerController)
    {
        playerController.playerControllerData.horizontalRaycasts = EditorGUILayout.IntField("Amount of Horizontal Raycasts", playerController.playerControllerData.horizontalRaycasts);

        playerController.playerControllerData.raycastSpreadAmountHorizontal = EditorGUILayout.FloatField("Spread Amount", playerController.playerControllerData.raycastSpreadAmountHorizontal);

        playerController.playerControllerData.raycastLengthHorizontal = EditorGUILayout.FloatField("Length", playerController.playerControllerData.raycastLengthHorizontal);

        playerController.playerControllerData.raycastHorizontalIgnoreTriggers = EditorGUILayout.Toggle("Ignore Triggers Horizontal", playerController.playerControllerData.raycastHorizontalIgnoreTriggers);

    }
    void RaycastVerticalGUISetup(TwoDTools.PlayerController2D playerController)
    {
        playerController.playerControllerData.verticalRaycasts = EditorGUILayout.IntField("Amount of Raycasts", playerController.playerControllerData.verticalRaycasts);

        playerController.playerControllerData.raycastSpreadAmountVertical = EditorGUILayout.FloatField("Spread Amount", playerController.playerControllerData.raycastSpreadAmountVertical);

        playerController.playerControllerData.raycastLengthVertical = EditorGUILayout.FloatField("Length", playerController.playerControllerData.raycastLengthVertical); 
        
        playerController.playerControllerData.raycastVerticalIgnoreTriggers = EditorGUILayout.Toggle("Ignore Triggers Vertical", playerController.playerControllerData.raycastVerticalIgnoreTriggers);

    }

    void AccelerationGUIDisplay(TwoDTools.PlayerController2D playerController)
    {
        playerController.playerControllerData.useAcceleration = EditorGUILayout.Toggle("Use Acceleration", playerController.playerControllerData.useAcceleration);

        switch (playerController.playerControllerData.useAcceleration)
        {
            case true:
                playerController.playerControllerData.acceleration = EditorGUILayout.FloatField("Acceleration", playerController.playerControllerData.acceleration);
                playerController.playerControllerData.deceleration = EditorGUILayout.FloatField("Deceleration", playerController.playerControllerData.deceleration);

                playerController.playerControllerData.useSprint = EditorGUILayout.Toggle("Use Sprint", playerController.playerControllerData.useSprint);
                switch (playerController.playerControllerData.useSprint)
                {
                    case true:
                        playerController.playerControllerData.sprintSpeedMultiplier = EditorGUILayout.FloatField("Sprint Speed Multiplier", playerController.playerControllerData.sprintSpeedMultiplier);
                        break;
                }//end sprint

                playerController.playerControllerData.useAirMomentum = EditorGUILayout.Toggle("Use Air Momentum", playerController.playerControllerData.useAirMomentum);
                switch (playerController.playerControllerData.useAirMomentum)
                {
                    case true:
                        playerController.playerControllerData.airAcceleration = EditorGUILayout.FloatField("Air Acceleration", playerController.playerControllerData.airAcceleration);
                        playerController.playerControllerData.airDeceleration = EditorGUILayout.FloatField("Air Deceleration", playerController.playerControllerData.airDeceleration);
                        break;
                }// end airMomentum

                break;
        }//end acceleration

    }

    void PlayerJumpGUI(TwoDTools.PlayerController2D playerController)
    {

        GUILayout.Label("Player Jump", titles);
        //public JumpType jumpType = JumpType.PreItalianPlumber;
        playerController.playerControllerData.initialBurstJump = EditorGUILayout.FloatField("Initial Jump Velocity", playerController.playerControllerData.initialBurstJump);
        playerController.playerControllerData.jumpVelocityDegradation = EditorGUILayout.FloatField("Jump Velocity Degradation", playerController.playerControllerData.jumpVelocityDegradation);
        
        playerController.playerControllerData.jumpVelocityDegradationWall = EditorGUILayout.FloatField(new GUIContent("Gravity Friction", "Friction Multiplier when against terrain"), 
            playerController.playerControllerData.jumpVelocityDegradationWall);


        playerController.playerControllerData.useCoyoteTime = EditorGUILayout.Toggle("Use Coyote Time", playerController.playerControllerData.useCoyoteTime);

        switch (playerController.playerControllerData.useCoyoteTime)
        {
            case false:
                break;

            case true:
                playerController.playerControllerData.coyoteTime = EditorGUILayout.FloatField("Coyote Time", playerController.playerControllerData.coyoteTime);
                break;

        }
        playerController.playerControllerData.usePreEmptiveCoyoteTime = EditorGUILayout.Toggle("Use Pre-Emptive Coyote Time", playerController.playerControllerData.usePreEmptiveCoyoteTime);
        switch (playerController.playerControllerData.usePreEmptiveCoyoteTime)
        {
            case false:
                break;

            case true:
                playerController.playerControllerData.preEmptiveCoyoteTime = EditorGUILayout.FloatField("Pre-Emptive Coyote Time", playerController.playerControllerData.preEmptiveCoyoteTime);
                break;
        }
    }

    void SlopeMovementSettingsGUI(TwoDTools.PlayerController2D playerController)
    {

        GUILayout.Label("Slope Settings", titles);
        playerController.playerControllerData.useSlopeMovement = EditorGUILayout.Toggle("Use Slope Movement", playerController.playerControllerData.useSlopeMovement);
        switch (playerController.playerControllerData.useSlopeMovement)
        {
            case false:
                break;
            case true:
                playerController.playerControllerData.maximumSlopeAngle = EditorGUILayout.FloatField("Maximum Travesable Angle", playerController.playerControllerData.maximumSlopeAngle);


                playerController.playerControllerData.useSlopeSlideMovement = EditorGUILayout.Toggle("Use Slope Slide", playerController.playerControllerData.useSlopeSlideMovement);
                switch (playerController.playerControllerData.useSlopeSlideMovement)
                {
                    case false:
                        break;
                    case true:
                        playerController.playerControllerData.maximumSlopeSlide = EditorGUILayout.FloatField("Slide At Angle", playerController.playerControllerData.maximumSlopeSlide);
                        break;
                }
                break;
        }
    }


    void SaveSettingsGUI(TwoDTools.PlayerController2D playerController)
    {
        GUILayout.Label("Save Settings", titles);
        activateSave = EditorGUILayout.Toggle("Show Save Settings", activateSave);
        switch (activateSave)
        {
            case false:
                break;
            case true:
                fileName = EditorGUILayout.TextField("Save File Name As: ", fileName);
                save = EditorGUILayout.Toggle("Save File?", save);
                switch (save)
                {
                    case false:
                        break;
                    case true:
                        playerController.SaveSettings(fileName);
                        save = false;
                        activateSave = false;
                        Debug.Log("Saved " + fileName);
                        break;
                }
                break;
        }

    }


    void LoadSettingsGUI(TwoDTools.PlayerController2D playerController)
    {
        GUILayout.Label("Load Settings", titles);
        activateLoad = EditorGUILayout.Toggle("Show Load Settings", activateLoad);
        switch (activateLoad)
        {
            case false:
                break;
            case true:
                fileName = EditorGUILayout.TextField("Load File With Name: ", fileName);
                load = EditorGUILayout.Toggle("Load File?", load);
                switch (load)
                {
                    case false:
                        break;
                    case true:
                        playerController.LoadSettings(fileName);
                        load = false;
                        activateLoad = false;
                        Debug.Log("Loaded Settings From " + fileName);
                        break;
                }
                break;
        }

    }

}
#endif