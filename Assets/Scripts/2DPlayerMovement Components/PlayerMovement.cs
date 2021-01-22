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

        private float maximumSpeedMultiplier = 1;

        // Called before first frame
        public void Start()
        {
            playerController = GetComponent<TwoDTools.PlayerController2D>();
            input = playerController.GetInput();
        }

        // Called from PlayerController2D Update()
        public void MovementUpdate()
        {
            SprintCheck();
            CalculateAcceleration();
        }

        public void CalculateAcceleration()
        {
            if(NoInput())
            {
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

        void SprintCheck()
        {
            if(!playerController.playerControllerData.useSprint)
            {
                return;
            }
            if (input.SprintButtonHeld())
            {
                maximumSpeedMultiplier = playerController.playerControllerData.sprintSpeedMultiplier;
            }
            else
            {
                maximumSpeedMultiplier = 1;
            }
        }

        bool NoInput()
        {
            if (input.RightButton())
            {
                return false;
            }
            if (input.LeftButton())
            {
                return false;
            }

            if (playerController.playerState.IsTouchingWall())
            {
                playerController.currentVelocity.x = 0;
                return true;
            }
            // If we aren't using acceleration values then we aren't using Decelerate
            if (!playerController.playerControllerData.useAcceleration)
            {
                playerController.currentVelocity.x = 0;
                return true;
            }
            if (playerController.playerState.IsTouchingWallBehind())
            {
                playerController.currentVelocity.x = 0;
                return true;
            }
            Decelerate();
            return true;

        }

        public Vector3 MoveOnSlope()
        {

            if (playerController.playerState.IsTouchingSlopeBack() && playerController.playerState.IsTouchingSlopeFront())
            {
                if(playerController.playerState.FacingDownSlope())
                {
                    return ClimbDownSlope();
                }
                return ClimbUpSlope();

            }

            if (playerController.playerState.FacingDownSlope())
            {
                if (playerController.playerState.IsTouchingSlopeBack())
                {
                    return ClimbDownSlope();
                }
            }

            if (!playerController.playerState.FacingDownSlope())
            {
                if (playerController.playerState.IsTouchingSlopeFront())
                {
                    return ClimbUpSlope();
                }
            }
            return playerController.currentVelocity;
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

            DecelerateMovingRight();
            DecelerateMovingLeft();
        }

        bool DecelerateMovingRight()
        {
            // Moving to the left
            if (playerController.currentVelocity.x < 0)
            {
                return false;
            }
            playerController.currentVelocity.x -= playerController.playerControllerData.deceleration * Time.fixedDeltaTime;

            if (playerController.currentVelocity.x <= 0)
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
            playerController.currentVelocity.x += playerController.playerControllerData.deceleration * Time.fixedDeltaTime;

            if (playerController.currentVelocity.x >= 0)
            {
                playerController.currentVelocity.x = 0;
            }
            return true;
        }

        bool NotUsingAccelerationCalculation()
        {
            if (playerController.playerControllerData.useAcceleration)
            {
                return false;
            }

            playerController.currentVelocity.x = 0;
            return true;
        }

        private bool AirDecellerate()
        {
            if (!playerController.playerControllerData.useAirMomentum)
            {
                return false;
            }
            if (!playerController.playerState.IsTouchingFloor() && !playerController.playerState.IsTouchingSlope())
            {
                // Moving to the right
                if (playerController.currentVelocity.x > 0)
                {
                    playerController.currentVelocity.x -= playerController.playerControllerData.airDeceleration * Time.fixedDeltaTime;

                    if (playerController.currentVelocity.x < 0)
                    {
                        playerController.currentVelocity.x = 0;
                    }
                    return true;
                }
                // Moving to the left
                if (playerController.currentVelocity.x < 0)
                {
                    playerController.currentVelocity.x += playerController.playerControllerData.airDeceleration * Time.fixedDeltaTime;

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
                playerController.currentVelocity.x = 0;
                return;
            }

            if (!playerController.playerControllerData.useAcceleration)
            {
                playerController.currentVelocity.x = playerController.playerControllerData.maximumHorizontalVelocity * maximumSpeedMultiplier;
                return;
            }

            // Max Acceration reached
            if (playerController.currentVelocity.x >= playerController.playerControllerData.maximumHorizontalVelocity * maximumSpeedMultiplier)
            {
                playerController.currentVelocity.x = playerController.playerControllerData.maximumHorizontalVelocity * maximumSpeedMultiplier;
                return;
            }

            if (!playerController.playerControllerData.useAirMomentum)
            {
                playerController.currentVelocity.x += playerController.playerControllerData.acceleration * Time.fixedDeltaTime;
                return;
            }

            if (!playerController.playerState.IsTouchingFloor() && !playerController.playerState.IsTouchingSlope())
            {
                playerController.currentVelocity.x += playerController.playerControllerData.airAcceleration * Time.fixedDeltaTime;
                return;
            }

            if (playerController.currentVelocity.x < 0)
            {
                Decelerate();
                return;
            }
            playerController.currentVelocity.x += playerController.playerControllerData.acceleration * Time.fixedDeltaTime;
            return;

        }

        public void AccelerateLeft()
        {
            if (playerController.playerState.IsTouchingWall())
            {
                playerController.currentVelocity.x = 0;
                return;
            }
            if (!playerController.playerControllerData.useAcceleration)
            {
                playerController.currentVelocity.x = -playerController.playerControllerData.maximumHorizontalVelocity * maximumSpeedMultiplier;
                return;
            }
            if (playerController.currentVelocity.x <= -playerController.playerControllerData.maximumHorizontalVelocity * maximumSpeedMultiplier)
            {
                playerController.currentVelocity.x = -playerController.playerControllerData.maximumHorizontalVelocity * maximumSpeedMultiplier;
                return;
            }
            if (!playerController.playerControllerData.useAirMomentum)
            {
                playerController.currentVelocity.x += -playerController.playerControllerData.acceleration * Time.fixedDeltaTime;
                return;
            }

            if (!playerController.playerState.IsTouchingFloor() && !playerController.playerState.IsTouchingSlope())
            {
                playerController.currentVelocity.x += -playerController.playerControllerData.airAcceleration * Time.fixedDeltaTime;
                return;
            }

            if (playerController.currentVelocity.x > 0)
            {
                Decelerate();
                return;
            }

            if (playerController.currentVelocity.x > 0)
            {
                Decelerate();
                return;
            }
            playerController.currentVelocity.x += -playerController.playerControllerData.acceleration * Time.fixedDeltaTime;
            return;
        }

        public Vector3 ClimbUpSlope()
        {
            Vector3 result = playerController.currentVelocity;

            if (playerController.playerState.IsJumping())
            {
                return result;
            }
            if(!playerController.playerState.IsTouchingFloor())
            {
                return result;
            }

            //StickToSlope(false);
            float movementAmount = Mathf.Abs(playerController.currentVelocity.x);
            result.y = Mathf.Sin(playerController.playerState.slopeAngleFront * Mathf.Deg2Rad) * movementAmount;

            result.x =
                (Mathf.Cos(playerController.playerState.slopeAngleFront * Mathf.Deg2Rad) *  movementAmount * Mathf.Sign(playerController.currentVelocity.x));

            return result;
        }

        public Vector3 ClimbDownSlope()
        {
            Vector3 result = playerController.currentVelocity;

            if (playerController.playerState.IsJumping())
            {
                return result;
            }
            if (!playerController.playerState.IsTouchingFloor())
            {
                bool stuck = false;
                Vector2 pos = transform.position;
                if (Vector2.Distance(playerController.playerState.hitPointBack, pos - playerController.playerState.spriteRenderer.size / 2) < .39f)
                {
                    StickToSlope(true);
                    stuck = true;
                }
                if (!stuck)
                {
                    return result;
                }
            }
            float movementAmount = Mathf.Abs(playerController.currentVelocity.x);

            result.y = -Mathf.Sin(playerController.playerState.slopeAngleBack * Mathf.Deg2Rad) * movementAmount;

            result.x =
                (Mathf.Cos(playerController.playerState.slopeAngleBack * Mathf.Deg2Rad) * movementAmount * Mathf.Sign(playerController.currentVelocity.x));

            return result;

        }

        private void StickToSlope(bool goingDown)
        {
            Vector2 pos = transform.position;
            if (goingDown)
            {
                pos.y = playerController.playerState.hitPointBack.y + playerController.playerState.spriteRenderer.size.y / 2;
                transform.position = pos;
                return;
            }
            // Up
            pos.y = playerController.playerState.hitPointFront.y + playerController.playerState.spriteRenderer.size.y / 2;
            transform.position = pos;
        }

    }

}