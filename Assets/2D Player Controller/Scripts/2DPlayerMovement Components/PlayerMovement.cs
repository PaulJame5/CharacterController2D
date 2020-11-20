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
/// It contains a Raycast check for collision against terrain types set by the user.
/// In the editor TouchWall() will have each of the raycasts go red when touching 
/// terrain but in release the TouchWall() will return
/// true on first instance of a Raycast touching terrain
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
        private float initialXscale;
        private SpriteRenderer spriteRenderer;
        // Called before first frame
        void Start()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            initialXscale = transform.localScale.x;
            playerController = GetComponent<TwoDTools.PlayerController2D>();
            input = playerController.GetInput();
        }

        // Called from PlayerController2D Update()
        public void RunUpdate()
        {

            CalculateAcceleration();

            // Apply Movement Calculation Results Here will be removed in a future update
           
            Vector2 pos = transform.position;
            pos.x += playerController.currentSpeed * Time.deltaTime;
            transform.position = pos;
        }

        void CalculateAcceleration()
        {
            
            if (!input.RightButton() && !input.LeftButton())
            {
                // If we aren't using acceleration values then we aren't using Decelerate
                if (TouchingWall() || !playerController.useAcceleration)
                {
                    playerController.currentSpeed = 0;
                    return;
                }
                Decelerate();
                return;
            }

            if (input.RightButton())
            {
                AccelerateRight();
                return;
            }
            if (input.LeftButton())
            {
                AccelerateLeft();
                return;
            }
        }

        void Decelerate()
        {
            if(!playerController.useDeceleration)
            {
                playerController.currentSpeed = 0;
                return;
            }

            // Moving to the right
            if(playerController.currentSpeed > 0)
            {
                playerController.currentSpeed -= playerController.deceleration * Time.deltaTime;

                if(playerController.currentSpeed < 0)
                {
                    playerController.currentSpeed = 0;
                }
                return;
            }

            // Moving to the left
            if (playerController.currentSpeed < 0)
            {
                playerController.currentSpeed += playerController.deceleration * Time.deltaTime;

                if (playerController.currentSpeed > 0)
                {
                    playerController.currentSpeed = 0;
                }
                return;
            }
        }

        bool TouchingWall()
        {

#if UNITY_EDITOR
            // Only used with debug setup for displaying hit raycasts
            bool hit = false;
#endif

            Vector2 offset = new Vector2(0,spriteRenderer.size.y);
            Vector2 bottom = -offset / 2.1f; 
            Vector2 top = offset / 2.1f;
            top.y += playerController.raycastSpreadAmountHorizontal;
            bottom.y -= playerController.raycastSpreadAmountHorizontal;

            for (int i = 0; i < playerController.horizontalRaycasts; i++)
            {
                Vector2 pos = Vector2.Lerp(bottom, top, (float)i / (float)(playerController.horizontalRaycasts - 1));

                if (Physics2D.Raycast(pos + (Vector2)transform.position, transform.right * transform.localScale.x, playerController.raycastLengthHorizontal, playerController.terrainLayer.value))
                {

#if UNITY_EDITOR
                    Debug.DrawRay(pos + (Vector2)transform.position, transform.right * playerController.raycastLengthHorizontal * transform.localScale.x, Color.red);
                    hit = true;
                    continue;
#endif
                    return true;
                }

#if UNITY_EDITOR
                Debug.DrawRay(pos + (Vector2)transform.position, transform.right * playerController.raycastLengthHorizontal * transform.localScale.x, Color.blue);
#endif
            }

#if UNITY_EDITOR
            if (hit)
            {
                return true;
            }
#endif
            return false;
        }

        void AccelerateRight()
        {
            FaceRight();
            if (TouchingWall())
            {
                playerController.currentSpeed = 0;
                return;
            }
            if (!playerController.useAcceleration)
            {
                playerController.currentSpeed = playerController.maximumSpeed;
                return;
            }
            playerController.currentSpeed += playerController.acceleration * Time.deltaTime;
        }

        void AccelerateLeft()
        {
            FaceLeft();
            if (TouchingWall())
            {
                playerController.currentSpeed = 0;
                return;
            }
            if (!playerController.useAcceleration)
            {
                playerController.currentSpeed = -playerController.maximumSpeed;
                return;
            }
            playerController.currentSpeed += -playerController.acceleration * Time.deltaTime;
        }


        void FaceRight()
        {
            Vector3 scale = transform.localScale;
            scale.x = initialXscale;
            transform.localScale = scale;
        }

        void FaceLeft()
        {
            Vector3 scale = transform.localScale;
            scale.x = -initialXscale;
            transform.localScale = scale;
        }
    }




}