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

namespace TwoDTools
{
    public class PlayerController2DInput : MonoBehaviour
    {
        public bool ActionButtonDown()
        {

            if (Input.GetKeyDown(KeyCode.Space))
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        public bool LeftButton()
        {
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool RightButton()
        {
            if (Input.GetKey(KeyCode.RightArrow))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}