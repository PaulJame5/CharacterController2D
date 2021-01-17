using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class PlayerJumpTest
    {
        private Game game;

        [SetUp]
        public void Setup()
        {
            GameObject gameGameObject =
                MonoBehaviour.Instantiate(Resources.Load<GameObject>("Prefabs/Game"));
            game = gameGameObject.GetComponent<Game>();
        }


        [TearDown]
        public void Teardown()
        {
            Object.Destroy(game.gameObject);
        }

        // A Test behaves as an ordinary method
        [Test]
        public void PlayerJumpsOnKeyPress()
        {

            GameObject player = game.GetPlayer();
            player.GetComponent<TwoDTools.PlayerController2D>().Awake();
            player.GetComponent<TwoDTools.PlayerJump>().Start();
            TwoDTools.PlayerController2DInput input = player.GetComponent<TwoDTools.PlayerController2DInput>();

            Assert.AreEqual(input.JumpButtonHeld(), false);
            player.GetComponent<TwoDTools.PlayerState>().SetIsTouchingFloor(true);
            input.SetJumpButtonPressed();
            input.SetJumpButtonHeld();
            Assert.AreEqual(input.JumpButtonHeld(), true);
            player.GetComponent<TwoDTools.PlayerJump>().JumpUpdate();
            //player.GetComponent<TwoDTools.PlayerController2D>().Update();
            Assert.Greater(player.GetComponent<TwoDTools.PlayerController2D>().currentVelocity.y, 0);

        }

        [Test]
        public void PlayerCantJumpWhenNotTouchingGround()
        {

            GameObject player = game.GetPlayer();
            player.GetComponent<TwoDTools.PlayerController2D>().Awake();
            player.GetComponent<TwoDTools.PlayerJump>().Start();
            TwoDTools.PlayerController2DInput input = player.GetComponent<TwoDTools.PlayerController2DInput>();

            Assert.AreEqual(input.JumpButtonHeld(), false);
            player.GetComponent<TwoDTools.PlayerState>().SetIsTouchingFloor(false);
            input.SetJumpButtonPressed();
            input.SetJumpButtonHeld();
            Assert.AreEqual(input.JumpButtonHeld(), true);
            player.GetComponent<TwoDTools.PlayerJump>().JumpUpdate();

            Assert.LessOrEqual(player.GetComponent<TwoDTools.PlayerController2D>().currentVelocity.y, 0);

        }

        // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
        // `yield return null;` to skip a frame.
        //[UnityTest]
        //public IEnumerator NewTestScriptWithEnumeratorPasses()
        //{
        //    // Use the Assert class to test conditions.
        //    // Use yield to skip a frame.
        //    yield return null;
        //}
    }
}
