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
                    playerController.playerState.NotJumping();
                }
                return;
            }
            if(input.JumpButtonPressed())
            {
                CalcualteJump();
                playerController.lastTouchedGround = -1;
                playerController.playerState.Jumping();
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
            if (input.JumpButtonHeld())
            {
                if (playerController.pressedAt + playerController.preEmptiveCoyoteTime > Time.timeSinceLevelLoad)
                {
                    CalcualteJump();
                    return;
                }
                CalculateJumpDegradation();
                return;
            }
        }


        void CalcualteJump()
        {
            if (!playerController.playerState.IsTouchingFloor())
            {
                if (!playerController.useCoyoteTime && !playerController.usePreEmptiveCoyoteTime)
                {
                    return;
                }

                if (playerController.coyoteTime + playerController.lastTouchedGround < Time.timeSinceLevelLoad)
                {
                    return;
                }
            }


            switch (playerController.jumpType)
            {
                case PlayerController2D.JumpType.PreItalianPlumber:
                    playerController.currentVelocity.y = playerController.initialBurstJump;
                    break;
                case PlayerController2D.JumpType.MeatSquare:
                    playerController.currentVelocity.y = playerController.initialBurstJump;
                    playerController.playerState.ResetTouchingSlope();
                    break;
            }
        }

        void CalculateJumpDegradation()
        {
            if (playerController.playerState.IsTouchingFloor())
            {
                return;
            }
            if (playerController.currentVelocity.y <= 0)
            {
                playerController.playerState.NotJumping();
                return;
            }

            switch (playerController.jumpType)
            {
                case PlayerController2D.JumpType.PreItalianPlumber:
                    // Let gravity do it's thing.
                    break;
                case PlayerController2D.JumpType.MeatSquare:
                    if (playerController.playerState.IsTouchingWall() || playerController.playerState.IsTouchingWallBehind())
                    {
                        playerController.currentVelocity.y -= playerController.jumpVelocityDegradationWall * Time.deltaTime;
                        break;
                    }
                    playerController.currentVelocity.y -= playerController.jumpVelocityDegradation * Time.deltaTime;
                    break;
            }
        }


        void CalculateForWhenJumpButtonLetGo()
        {
            switch (playerController.jumpType)
            {
                case PlayerController2D.JumpType.PreItalianPlumber:
                    // Let gravity do it's thing.
                    break;
                case PlayerController2D.JumpType.MeatSquare:
                    if (playerController.currentVelocity.y > .1f)
                    {
                        playerController.currentVelocity.y = PlayerController2D.GRAVITY * Time.deltaTime;
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