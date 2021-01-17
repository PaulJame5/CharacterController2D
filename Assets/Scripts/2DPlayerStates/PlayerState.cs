///<summary>
/// Created by Paul O'Callaghan
/// PlayerState Will handle all types of situations a player can be in such as touching the floor walls.
/// Scale to use assumes transform.localScale.x is always 1(facing right) or -1(facing left) for sprite facing direction
/// and placement of raycasts
/// 
/// 
/// ToDo:
/// Add in other terrain types such as Ice or touching water
/// 
/// 
/// </summary>


using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace TwoDTools
{
    public class PlayerState : MonoBehaviour
    {
        private bool touchingWall;
        private bool touchingFloor;
        private bool touchingCeiling;
        private bool touchingWallBehind;
        private bool touchingSlope;


        // Player Game State
        private bool isDead = false;
        private bool isFinishedLevel = false;
        private bool isSprinting;

        private SpriteRenderer spriteRenderer;
        private TwoDTools.PlayerController2D playerController;

        // Mini optimisation
        Transform myTransform;

        // used by TouchingTerrain()
        Vector2 offset;
        float scaleToUse;

        public Vector2 point;
        public float slopeAngleFront;
        public const float MAX_SLOPE_LIMIT = 80;

        enum CheckType
        {
            Horizontal,
            Vertical
        }

        public void Awake()
        {
            ResetAllStates();
            myTransform = transform;
            playerController = GetComponent<TwoDTools.PlayerController2D>();
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        public void UpdatePlayerState()
        {
            // Horizontal Checks
            TouchingTerrain(myTransform.right, CheckType.Horizontal, playerController.horizontalRaycasts, playerController.raycastLengthHorizontal);
            touchingWallBehind = TouchingTerrain(-myTransform.right, CheckType.Horizontal, playerController.horizontalRaycasts, playerController.raycastLengthHorizontal);
            
            // Vertical Checks
            touchingCeiling = TouchingTerrain(myTransform.up, CheckType.Vertical, playerController.verticalRaycasts, playerController.raycastLengthVertical);
            touchingFloor = TouchingTerrain(-myTransform.up, CheckType.Vertical, playerController.verticalRaycasts, playerController.raycastLengthVertical);

        }




        public void CheckTouchingWall()
        {
            touchingWall = TouchingTerrain(myTransform.right, CheckType.Horizontal, playerController.horizontalRaycasts, playerController.raycastLengthHorizontal);
        }
       
        public void CheckTouchingWallBehind()
        {
            touchingWallBehind = TouchingTerrain(-myTransform.right, CheckType.Horizontal, playerController.horizontalRaycasts, playerController.raycastLengthHorizontal);
        }

        public void ResetTouchWalls()
        {
            touchingWall = false;
            touchingWallBehind = false;
        }

        public void CheckTouchingCeiling()
        {
            touchingCeiling = TouchingTerrain(myTransform.up, CheckType.Vertical, playerController.verticalRaycasts, playerController.raycastLengthVertical);
        }

        public void CheckTouchingFloor()
        {
            touchingFloor = TouchingTerrain(-myTransform.up, CheckType.Vertical, playerController.verticalRaycasts, playerController.raycastLengthVertical);
        }

        public bool IsTouchingWall()
        {
            return touchingWall;
        }
        public bool IsTouchingWallBehind()
        {
            return touchingWallBehind;
        }


        public bool IsTouchingCeiling()
        {
            return touchingCeiling;
        }

        public bool IsTouchingFloor()
        {
            return touchingFloor;
        }

        public void SetIsTouchingFloor(bool touchingFloor)
        {
            this.touchingFloor = touchingFloor;
        }

        public bool IsMovingHorizontal()
        {
            return ((playerController.input.RightButton()  && playerController.currentVelocity.x > 0) || (playerController.input.LeftButton() && playerController.currentVelocity.x < 0));
        }

        public bool IsMovingVerticalUp()
        {
            if (touchingFloor)
            {
                return false;
            }
            return (playerController.currentVelocity.y > 0);
        }
        public bool IsMovingVerticalDown()
        {
            if (touchingFloor)
            {
                return false;
            }
            return (playerController.currentVelocity.y < 0);
        }

        public void ResetAllStates()
        {
            touchingWall = false;
            touchingFloor = false;
            touchingCeiling = false;
            touchingWallBehind = false;
            isDead = false;
            isSprinting = false;
            touchingSlope = false;
        }

        public bool IsTouchingSlope()
        {
            return touchingSlope;
        }

        public void SetIsTouchingSlope(bool isTouchingSlope)
        {
            touchingSlope = isTouchingSlope;
        }

        public void ResetTouchingSlope()
        {
            touchingSlope = false;
        }

        public bool GetIsDead()
        {
            return isDead;
        }

        public bool GetIsFinishedLevel()
        {
            return isFinishedLevel;
        }

        public void SetDeadAs(bool state)
        {
            isDead = state;
        }
        public void Kill()
        {
            isDead = true;
        }

        void CheckSlopeAngle()
        {
            if(slopeAngleFront <= MAX_SLOPE_LIMIT)
            {
                touchingSlope = true;
                return;
            }
            touchingSlope = false;
        }

        private bool TouchingTerrain(Vector2 direction, CheckType checkType, int numberOfRaycasts, float length)
        {

#if UNITY_EDITOR
            // Only used with debug setup for displaying hit raycasts
            bool hit = false;
#endif

            switch (checkType)
            {
                case CheckType.Vertical:
                    offset = new Vector2(spriteRenderer.size.x, 0);
                    scaleToUse = 1;
                    break;
                case CheckType.Horizontal:
                    offset = new Vector2(0, spriteRenderer.size.y);
                    scaleToUse = transform.localScale.x;
                    break;
            }

            Vector2 minimumBounds = -offset / 2.21f;
            Vector2 maximumBounds = offset / 2.21f;

            switch (checkType)
            {
                case CheckType.Vertical:
                    minimumBounds.x -= playerController.raycastSpreadAmountVertical;
                    maximumBounds.x += playerController.raycastSpreadAmountVertical;
                    break;
                case CheckType.Horizontal:
                    minimumBounds.y -= playerController.raycastSpreadAmountHorizontal;
                    maximumBounds.y += playerController.raycastSpreadAmountHorizontal;
                    break;
            }

            for (int i = 0; i < numberOfRaycasts; i++)
            {
                Vector2 pos = Vector2.Lerp(minimumBounds, maximumBounds, (float)i / (float)(numberOfRaycasts - 1));
                var hitRay = Physics2D.Raycast(pos + (Vector2)transform.position, direction * scaleToUse, length, playerController.terrainLayer);
                
                if (hitRay)
                {
                    switch (checkType)
                    {
                        case CheckType.Horizontal:
                            point.x = hitRay.point.x;
                            if (i == 0 && (Vector3)direction == myTransform.right)
                            {
                                slopeAngleFront = Vector2.Angle(hitRay.normal, Vector2.up);
                                CheckSlopeAngle();
                            }
                            else if (i > 0 && (Vector3)direction == myTransform.right)
                            {
                                
                                touchingWall = true;
                            }
                            break;
                        case CheckType.Vertical:
                            point.y = hitRay.point.y;
                            break;
                    }
#if UNITY_EDITOR
                    if (i == 0)
                    {
                        Debug.DrawRay(pos + (Vector2)transform.position, direction * length * scaleToUse, Color.green);
                        hit = true;
                        continue;
                    }
                    Debug.DrawRay(pos + (Vector2)transform.position, direction * length * scaleToUse, Color.red);
                    hit = true;
                    continue;
#endif
                    return true;
                }
                else
                {
                    if (checkType == CheckType.Horizontal && i == 0 && (Vector3)direction == myTransform.right)
                    {
                        touchingSlope = false;
                    }
                }
#if UNITY_EDITOR
                Debug.DrawRay(pos + (Vector2)transform.position, direction * length * scaleToUse, Color.blue);
#endif
            }
#if UNITY_EDITOR
            return hit;
#endif
            return false;
        } // End Touch Wall
    }
}