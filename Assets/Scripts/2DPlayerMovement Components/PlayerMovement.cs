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
/// Notes:
/// PlayerMovement calculates Movement of player when pressing left or right keys.
/// 
/// 
/// </summary>


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TwoDTools
{
    [RequireComponent(typeof(TwoDTools.PlayerController2D))]
    public class PlayerMovement : MonoBehaviour
    {
        private TwoDTools.PlayerController2D playerController;
        private TwoDTools.PlayerController2DInput input;

        // Called before first frame
        public void Start()
        {
            playerController = GetComponent<TwoDTools.PlayerController2D>();
            input = playerController.GetInput();
        }

        // Called from PlayerController2D Update()
        public void MovementUpdate()
        {
            CalculateAcceleration();
        }

        void CalculateAcceleration()
        {
            
            if (!input.RightButton() && !input.LeftButton())
            {
                if (playerController.playerState.IsTouchingWall())
                {
                    if (playerController.playerState.IsTouchingSlope())
                    {
                        Decelerate();
                        MoveUpSlope();
                        return;
                    }
                    playerController.currentVelocity.x = 0;
                    return;
                }
                // If we aren't using acceleration values then we aren't using Decelerate
                if (!playerController.useAcceleration)
                {
                    playerController.currentVelocity.x = 0;
                    return;
                }
                if(playerController.playerState.IsTouchingWallBehind())
                {
                    playerController.currentVelocity.x = 0;
                    return;
                }
                Decelerate();
                return;
            }
            if (input.LeftButton())
            {
                AccelerateLeft();
                return;
            }
            if (input.RightButton())
            {
                AccelerateRight();
            }
        }

        public void MoveUpSlope()
        {
            ClimbUpSlope();
        }

        public void Decelerate()
        {
            if(NotUsingAccelerationCalculation())
            {
                return;
            }

            if (AirDecellerate())
            {
                return;
            }

            if(DecelerateMovingRight())
            {
                return;
            }

            if(DecelerateMovingLeft())
            {
                return;
            }
        }

        bool DecelerateMovingRight()
        {
            // Moving to the left
            if (playerController.currentVelocity.x < 0)
            {
                return false;
            }
            playerController.currentVelocity.x -= playerController.deceleration * Time.fixedDeltaTime;

            if (playerController.currentVelocity.x < 0)
            {
                playerController.currentVelocity.x = 0;
            }
            return true;
        }

        bool DecelerateMovingLeft()
        {
            // Moving to the Right
            if (playerController.currentVelocity.x > 0)
            {
                return false;
            }
            playerController.currentVelocity.x += playerController.deceleration * Time.fixedDeltaTime;

            if (playerController.currentVelocity.x > 0)
            {
                playerController.currentVelocity.x = 0;
            }
            return true;
        }

        bool NotUsingAccelerationCalculation()
        {
            if (playerController.useAcceleration)
            {
                return false;
            }
            if (playerController.playerState.IsTouchingSlope())
            {
                if (!(playerController.input.JumpButtonHeld() || playerController.input.JumpButtonPressed()))
                {
                    playerController.currentVelocity.y = 0;
                }
            }
            playerController.currentVelocity.x = 0;
            return true;
        }

        private bool AirDecellerate()
        {
            if (!playerController.useAirMomentum)
            {
                return false;
            }
            if (!playerController.playerState.IsTouchingFloor())
            {
                // Moving to the right
                if (playerController.currentVelocity.x > 0)
                {
                    playerController.currentVelocity.x -= playerController.airDeceleration * Time.fixedDeltaTime;

                    if (playerController.currentVelocity.x < 0)
                    {
                        playerController.currentVelocity.x = 0;
                    }
                    return true;
                }
                // Moving to the left
                if (playerController.currentVelocity.x < 0)
                {
                    playerController.currentVelocity.x += playerController.airDeceleration * Time.fixedDeltaTime;

                    if (playerController.currentVelocity.x > 0)
                    {
                        playerController.currentVelocity.x = 0;
                    }
                    return true;
                }
            }
            return false;
        }

        public void AccelerateRight()
        {
            if (playerController.playerState.IsTouchingWall())
            {
                if (playerController.playerState.IsTouchingSlope() == false)
                {
                    playerController.currentVelocity.x = 0;
                    return;
                }
            }
            if (!playerController.useAcceleration)
            {
                playerController.currentVelocity.x = playerController.maximumHorizontalVelocity;
                return;
            }

            // Max Acceration reached
            if (playerController.currentVelocity.x >= playerController.maximumHorizontalVelocity)
            {
                playerController.currentVelocity.x = playerController.maximumHorizontalVelocity;
                return;
            }

            if (!playerController.useAirMomentum)
            {
                if (!playerController.useSprintAcceleration || playerController.input.SprintButtonHeld())
                {
                    playerController.currentVelocity.x += playerController.acceleration * Time.fixedDeltaTime;
                    return;
                }
                playerController.currentVelocity.x += playerController.sprintAcceleration * Time.fixedDeltaTime;
                return;
            }

            if (!playerController.playerState.IsTouchingFloor())
            {
                playerController.currentVelocity.x += playerController.airAcceleration * Time.fixedDeltaTime;
                return;
            }

            if (!playerController.useSprintAcceleration && !playerController.input.SprintButtonHeld())
            {
                playerController.currentVelocity.x += playerController.acceleration * Time.fixedDeltaTime;
                return;
            }
            playerController.currentVelocity.x += playerController.sprintAcceleration * Time.fixedDeltaTime;
            return;

        }

        public void AccelerateLeft()
        {
            if (playerController.playerState.IsTouchingWall())
            {
                if (playerController.playerState.IsTouchingSlope() == false)
                {
                    playerController.currentVelocity.x = 0;
                    return;
                }
            }
            if (!playerController.useAcceleration)
            {
                playerController.currentVelocity.x = -playerController.maximumHorizontalVelocity;
                return;
            }
            if (playerController.currentVelocity.x <= -playerController.maximumHorizontalVelocity)
            {
                playerController.currentVelocity.x = -playerController.maximumHorizontalVelocity;
                return;
            }
            if (!playerController.useAirMomentum)
            {
                if (!playerController.useSprintAcceleration || playerController.input.SprintButtonHeld())
                {
                    playerController.currentVelocity.x += -playerController.acceleration * Time.fixedDeltaTime;
                    return;
                }
                playerController.currentVelocity.x += -playerController.sprintAcceleration * Time.fixedDeltaTime;
                return;
            }

            if (!playerController.playerState.IsTouchingFloor())
            {
                playerController.currentVelocity.x += -playerController.airAcceleration * Time.fixedDeltaTime;
                return;
            }

            if (!playerController.useSprintAcceleration || playerController.input.SprintButtonHeld())
            {
                playerController.currentVelocity.x += -playerController.acceleration * Time.fixedDeltaTime;
                return;
            }
            playerController.currentVelocity.x += -playerController.sprintAcceleration * Time.fixedDeltaTime;
            return;
        }

        public void ClimbUpSlope()
        {
            if (playerController.currentVelocity.x == 0)
            {
                return;
            }
            if (playerController.currentVelocity.y < 0)
            {
                return;
            }
            float movementAmount = Mathf.Abs(playerController.currentVelocity.magnitude);
            playerController.currentVelocity.y = Mathf.Sin(playerController.playerState.slopeAngleFront * Mathf.Deg2Rad) * movementAmount;

            playerController.currentVelocity.x =
                (Mathf.Cos(playerController.playerState.slopeAngleFront * Mathf.Deg2Rad) *  movementAmount * Mathf.Sign(playerController.currentVelocity.x));
        }

        public void ClimbDownSlope()
        {

        }

    }

}