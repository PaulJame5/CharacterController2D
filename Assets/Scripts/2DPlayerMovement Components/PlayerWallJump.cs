using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TwoDTools
{
    public class PlayerWallJump : MonoBehaviour
    {

        private TwoDTools.PlayerController2D playerController;
        private TwoDTools.PlayerController2DInput input;

        private bool walljumpPressed = false;

        private float inputDelay = 0.2f;
        private float timePreviousWallJumpMade;

        private const float OFFSET_AMOUNT = 0.1f; 

        public void Start()
        {
            playerController = GetComponent<TwoDTools.PlayerController2D>();
            input = playerController.GetInput();
        }

        public void WallJumpUpdate()
        {

            if (InputDelayInEffect())
            {
                return;
            }

            if (playerController.playerState.IsTouchingFloor())
            {
                return;
            }

            if (!playerController.playerState.IsTouchingWall() && !playerController.playerState.IsTouchingWallBehind())
            {
                return;
            }
            if (!input.JumpButtonPressed())
            {
                if(!playerController.playerControllerData.usePreEmptiveCoyoteTime)
                {
                    return;
                }
                if (playerController.pressedAt + playerController.playerControllerData.preEmptiveCoyoteTime < Time.timeSinceLevelLoad)
                {
                    return;
                }
            }

            CalculateWallJump();
            return;

        }

        private bool InputDelayInEffect()
        {

            if (walljumpPressed)
            {
                if (inputDelay + timePreviousWallJumpMade > Time.time)
                {
                    return true;
                }
                walljumpPressed = false;

            }
            return false;
        }


        void CalculateWallJump()
        {
            Vector3 pos = transform.position;
            switch (playerController.playerControllerData.jumpType)
            {
                case TwoDTools.PlayerController2DData.JumpType.ItalianPlumber:
                    break;
                case TwoDTools.PlayerController2DData.JumpType.MeatSquare:
                    if (playerController.playerState.IsTouchingWall())
                    {
                        // facing left
                        if (transform.localScale.x < 0)
                        {
                            pos.x += OFFSET_AMOUNT;
                            playerController.currentVelocity.x = playerController.playerControllerData.maximumHorizontalVelocity/2;
                            playerController.currentVelocity.y = playerController.playerControllerData.initialBurstJump;
                            walljumpPressed = true;
                            playerController.playerState.ResetTouchWalls();
                            break;
                        }
                        //facing right
                        if (transform.localScale.x > 0)
                        {
                            pos.x -= OFFSET_AMOUNT;
                            playerController.currentVelocity.x = -playerController.playerControllerData.maximumHorizontalVelocity/2;
                            playerController.currentVelocity.y = playerController.playerControllerData.initialBurstJump;
                            walljumpPressed = true;
                            playerController.playerState.ResetTouchWalls();
                            break;
                        }

                        break;
                    }
                    if (playerController.playerState.IsTouchingWallBehind())
                    {
                        // facing left
                        if (transform.localScale.x < 0)
                        {
                            pos.x -= OFFSET_AMOUNT;
                            playerController.currentVelocity.x = -playerController.playerControllerData.maximumHorizontalVelocity / 1.5f;
                            playerController.currentVelocity.y = playerController.playerControllerData.initialBurstJump;
                            walljumpPressed = true;
                            playerController.playerState.ResetTouchWalls();
                            break;
                        }
                        //facing right
                        if (transform.localScale.x > 0)
                        {
                            pos.x += OFFSET_AMOUNT;
                            playerController.currentVelocity.x = playerController.playerControllerData.maximumHorizontalVelocity / 1.5f;
                            playerController.currentVelocity.y = playerController.playerControllerData.initialBurstJump;
                            walljumpPressed = true;
                            playerController.playerState.ResetTouchWalls();
                            break;
                        }

                        break;
                    }
                    break;
            } // end switch
            transform.position = pos;
            timePreviousWallJumpMade = Time.time;
        }


        public bool WallJumpPressed()
        {
            return walljumpPressed;
        }

        public void Reset()
        {
            walljumpPressed = false;
        }
    }

}