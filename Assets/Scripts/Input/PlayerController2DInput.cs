///<summary>
/// Created By Paul O'Callaghan
/// 
/// This is used to get Player Input States. We use a generic input manager class called 
/// InputManager_2D here which can be subbed out for your own input manager. This is 
/// done this way to make the transition smoother with other projects.
/// 
/// An example would be as follows:
///     void InputUpdate()
///     {
///         jumpKeyPressed = InputManager_2D.ActionButtonDown();
///     }
/// 
/// Here jumpKeyPressed is tied to InputManager_2D class function called 
/// ActionButtonDown()
/// 
/// If you were to create your own class called GreatestInputManagerEver
/// and you and you wanted to use your own function called ActionPressed();
/// then you would change the InputUpdate to:
/// 
/// void InputUpdate()
/// {
///     jumpKeyPressed = GreatestInputManagerEver.ActionPressed();
/// }
/// 
/// This will save headaches in getting your project working with other setups.
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

        private bool jumpButtonPressed;
        private bool jumpButtonHeld;
        private bool jumpButtonLetGo;

        private bool leftInputKeyPressed;
        private bool leftInputKeyHeld;

        private bool rightInputKeyPressed;
        private bool rightInputKeyHeld;

        private bool sprintButtonHeld;

        // you will want to use your own Input Manager possibly and that's totally fine
        // just be careful not to change variable and function names unless you know 
        // what you're doing as other classes rely on this
        public void InputUpdate()
        {
            jumpButtonHeld = InputManager_2D.ActionButton();
            jumpButtonPressed = InputManager_2D.ActionButtonDown();

            leftInputKeyHeld = InputManager_2D.LeftInput();
            leftInputKeyPressed = InputManager_2D.LeftInputDown();

            rightInputKeyHeld = InputManager_2D.RightInput();
            rightInputKeyPressed = InputManager_2D.RightInputDown();

            sprintButtonHeld = InputManager_2D.LeftShiftInput();
            jumpButtonLetGo = InputManager_2D.ActionButtonUp();
        }

        public void ResetInput()
        {
            jumpButtonPressed = false;
            jumpButtonHeld = false;
            jumpButtonLetGo = false;

            leftInputKeyHeld = false;
            leftInputKeyPressed = false;

            rightInputKeyHeld = false;
            rightInputKeyPressed = false;

            sprintButtonHeld = false;
        }

        public bool JumpButtonPressed()
        {
            return jumpButtonPressed;
        }

        public void SetJumpButtonPressed()
        {
            jumpButtonPressed = true;
        }

        public bool JumpButtonHeld()
        {
            return jumpButtonHeld;
        }
        public void SetJumpButtonHeld()
        {
            jumpButtonHeld = true;
        }

        public bool JumpButtonLetGo()
        {
            return jumpButtonLetGo;
        }


        public void SetJumpButtonLetGo()
        {
            jumpButtonLetGo = true;
        }

        public bool LeftButton()
        {
            return leftInputKeyHeld;
        }

        public void SetLeftButtonHeld()
        {
            leftInputKeyHeld = true;
        }
        public bool RightButton()
        {
            return rightInputKeyHeld;
        }


        public void SetRightButtonHeld()
        {
            rightInputKeyHeld = true;
        }

        public bool SprintButtonHeld()
        {
            return sprintButtonHeld;
        }



        public void SetSprintButtonHeld()
        {
            sprintButtonHeld = true;
        }


    }
}