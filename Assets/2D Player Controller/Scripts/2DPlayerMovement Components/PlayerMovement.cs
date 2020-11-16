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
    public class PlayerMovement : MonoBehaviour
    {
        private TwoDTools.PlayerController2D playerController;
        private TwoDTools.PlayerController2DInput input;

        // Called before first frame
        void Awake()
        {
            playerController = GetComponent<TwoDTools.PlayerController2D>();
            input = playerController.GetInput();
        }

        // Called from PlayerController2D Update()
        public void RunUpdate()
        {
            Vector2 pos = transform.position;

            if(input.RightButton())
            {
                pos.x += playerController.maximumSpeed * Time.deltaTime;
            }
            else if(input.LeftButton())
            {
                pos.x += -playerController.maximumSpeed * Time.deltaTime;
            }
            transform.position = pos;
        }
    }
}