using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TwoDTools
{

    [RequireComponent(typeof(TwoDTools.PlayerController2D))]
    public class PlayerJump : MonoBehaviour
    {

        private TwoDTools.PlayerController2D playerController;
        private TwoDTools.PlayerController2DInput input;

        public void Start()
        {
            playerController = GetComponent<TwoDTools.PlayerController2D>();
            input = playerController.GetInput();
        }
        public void JumpUpdate()
        {
            if(!input.JumpButtonPressed() && !input.JumpButtonLetGo() && !input.JumpButtonHeld())
            {
                if(!playerController.playerState.IsTouchingFloor() && playerController.currentVelocity.y > 0)
                {
                    playerController.currentVelocity.y = 0;
                }
                playerController.playerState.NotJumping();
                return;
            }
            if(input.JumpButtonPressed())
            {
                CalcualteJump();
                playerController.lastTouchedGround = -1;
                return;
            }
            if(input.JumpButtonLetGo())
            {
                CalculateForWhenJumpButtonLetGo();
                playerController.playerState.NotJumping();
                return;
            }
        }

        public void JumpFixedUpdate()
        {
            if (!input.JumpButtonHeld())
            {
                return;
            }
            if (playerController.playerState.IsJumping() == false)
            {
                if (playerController.pressedAt + playerController.playerControllerData.preEmptiveCoyoteTime > Time.timeSinceLevelLoad)
                {
                    CalcualteJump();
                }
                return;
            }
            if (playerController.currentVelocity.y <= 0)
            {
                playerController.playerState.NotJumping();
                return;
            }
            CalculateJumpDegradation();
            return;

        }


        void CalcualteJump()
        {
            if (!playerController.playerState.IsTouchingFloor())
            {
                if (!playerController.playerControllerData.useCoyoteTime && !playerController.playerControllerData.usePreEmptiveCoyoteTime)
                {
                    return;
                }

                if (playerController.playerControllerData.coyoteTime + playerController.lastTouchedGround < Time.timeSinceLevelLoad)
                {
                    return;
                }
            }
            playerController.playerState.StopSliding();


            switch (playerController.playerControllerData.jumpType)
            {
                case PlayerController2DData.JumpType.PreItalianPlumber:
                    playerController.currentVelocity.y = playerController.playerControllerData.initialBurstJump;
                    break;
                case PlayerController2DData.JumpType.MeatSquare:
                    playerController.currentVelocity.y = playerController.playerControllerData.initialBurstJump;
                    playerController.playerState.ResetTouchingSlope();
                    playerController.playerState.Jumping();
                    break;
            }
        }

        void CalculateJumpDegradation()
        {
            if (playerController.playerState.IsTouchingFloor())
            {
                if (playerController.currentVelocity.y < 0)
                {
                    playerController.playerState.NotJumping();
                }
                    return;
            }
            if (playerController.currentVelocity.y <= 0)
            {
                playerController.playerState.NotJumping();
                return;
            }

            switch (playerController.playerControllerData.jumpType)
            {
                case PlayerController2DData.JumpType.PreItalianPlumber:
                    // Let gravity do it's thing.
                    break;
                case PlayerController2DData.JumpType.MeatSquare:
                    if (playerController.playerState.IsTouchingWall() || playerController.playerState.IsTouchingWallBehind())
                    {
                        playerController.currentVelocity.y -= playerController.playerControllerData.jumpVelocityDegradationWall * Time.deltaTime;
                        break;
                    }
                    playerController.currentVelocity.y -= playerController.playerControllerData.jumpVelocityDegradation * Time.deltaTime;
                    break;
            }
        }


        void CalculateForWhenJumpButtonLetGo()
        {
            switch (playerController.playerControllerData.jumpType)
            {
                case PlayerController2DData.JumpType.PreItalianPlumber:
                    // Let gravity do it's thing.
                    break;
                case PlayerController2DData.JumpType.MeatSquare:
                    if (playerController.currentVelocity.y > .1f)
                    {
                        playerController.currentVelocity.y = playerController.playerControllerData.gravityForce * Time.deltaTime;
                    }
                    break;
            }
        }

        // Used for unit testing
#if UNITY_EDITOR
        public void SetPlayerController(TwoDTools.PlayerController2D playerController)
        {
            this.playerController = playerController;
        }
#endif

    }
}