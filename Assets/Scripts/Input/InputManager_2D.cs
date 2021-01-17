///<summary>
/// Generic Input Manager for TwoDTools PlayerController2D
/// Swap this out with your own Input manager or keep using it, it's up to you.
/// 
/// </summary>


using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace TwoDTools
{
    public class InputManager_2D : MonoBehaviour
    {
        private static KeyCode a = (KeyCode)97; // used for our left movement
        private static KeyCode s = (KeyCode)115; // used for down input
        private static KeyCode d = (KeyCode)100; // used for our right movement
        private static KeyCode w = (KeyCode)119; // used for up input
        private static KeyCode upArrow = (KeyCode)273; 
        private static KeyCode downArrow = (KeyCode)274;
        private static KeyCode rightArrow = (KeyCode)275; // used for right input
        private static KeyCode leftArrow = (KeyCode)276; // used for left input
        private static KeyCode space = (KeyCode)32; // Used for Our Action input
        private static KeyCode leftShift = (KeyCode)304; // Used for Our Sprint input

        /// <summary>
        /// Action Button Pressed Once
        /// </summary>
        /// <returns></returns>
        public static bool ActionButtonDown()
        {
            return Input.GetKeyDown(space);
        }
        public static bool ActionButton()
        {
            return Input.GetKey(space);
        }

        public static bool ActionButtonUp()
        {
            return Input.GetKeyUp(space);
        }

        public static bool LeftInput()
        {
            if (!Input.GetKey(a) && !Input.GetKey(leftArrow))
            {
                return false;
            }
            return true;
        }
        public static bool LeftInputDown()
        {
            if (!Input.GetKeyDown(a) && !Input.GetKeyDown(leftArrow))
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Input for Right Key Held Down
        /// </summary>
        /// <returns></returns>
        public static bool RightInput()
        {
            if (!Input.GetKey(d) && !Input.GetKey(rightArrow))
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Input for Right Key Pressed (Not Held Down)
        /// </summary>
        /// <returns></returns>
        public static bool RightInputDown()
        {
            if (!Input.GetKeyDown(d) && !Input.GetKeyDown(rightArrow))
            {
                return false;
            }
            return true;
        }


        /// <summary>
        /// Input for Up Key Held Down
        /// </summary>
        /// <returns></returns>
        public static bool UpInput()
        {
            if (!Input.GetKey(w) && !Input.GetKey(upArrow))
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Input for Up Key Pressed (Not Held Down)
        /// </summary>
        /// <returns></returns>
        public static bool UpInputDown()
        {
            if (!Input.GetKeyDown(w) && !Input.GetKeyDown(upArrow))
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Input for Down Key Held Down
        /// </summary>
        /// <returns></returns>
        public static bool DownInput()
        {
            if (!Input.GetKey(s) && !Input.GetKey(downArrow))
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Input for Down Key Pressed (Not Held Down)
        /// </summary>
        /// <returns></returns>
        public static bool DownInputDown()
        {
            if (!Input.GetKeyDown(s) && !Input.GetKeyDown(downArrow))
            {
                return false;
            }
            return true;
        }

        public static bool LeftShiftInput()
        {
            if(!Input.GetKey(leftShift))
            {
                return false;
            }
            return true;
        }

    }
}