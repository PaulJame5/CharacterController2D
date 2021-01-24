using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TwoDTools
{
    [System.Serializable]
    public class PlayerController2DData
    {
        // Gravity Affectors
        public float gravityForce = 11f;
        public float maximumFallSpeed = 15f;
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


        #region Player Movement 

#if UNITY_EDITOR
        public bool showMovementGUI = true;
#endif

        public float maximumHorizontalVelocity = 12f; // minimumSpeed is zero opposite direction is negative max

        #endregion // Player Movement


        #region Acceleration and Momentum Variables
        public bool useAcceleration = false;
        public float acceleration = 5.0f;

        public bool useSprint;
        public float sprintSpeedMultiplier = 1.5f;

        public float deceleration = 5.0f;

        public bool useSlopeMovement = true;
        public float maximumSlopeAngle = 60;

        public bool useSlopeSlideMovement = true;
        public float maximumSlopeSlide = 80;
        public float slideDecellerationDivisor = 4;

        public bool useAirMomentum = false;
        public float airDeceleration = 1f;
        public float airAcceleration = 1.5f;

        #endregion //Acceleration and momentum

        #region Jumping

#if UNITY_EDITOR
        public bool showJumpGUI = true;
#endif


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

        public bool useCoyoteTime;
        public float coyoteTime = .25f;

        public bool usePreEmptiveCoyoteTime;
        public float preEmptiveCoyoteTime = 0.16f;

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
        #endregion // Raycast Setup


    }
}