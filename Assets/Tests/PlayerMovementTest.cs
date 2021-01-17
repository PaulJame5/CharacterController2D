using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class PlayerMovementTest
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
        public void PlayerMovementTestSimplePasses()
        {
            // Use the Assert class to test conditions
        }

        // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
        // `yield return null;` to skip a frame.
        [UnityTest]
        public IEnumerator PlayerAcceleratRight()
        {
            GameObject player = game.GetPlayer();
            float previousX = player.GetComponent<TwoDTools.PlayerController2D>().currentVelocity.x;
            player.GetComponent<TwoDTools.PlayerMovement>().AccelerateRight();
            Assert.Greater(player.GetComponent<TwoDTools.PlayerController2D>().currentVelocity.x, previousX);
            // Use the Assert class to test conditions.
            // Use yield to skip a frame.
            yield return new WaitForSeconds(1f);
        }

        [UnityTest]
        public IEnumerator PlayerAcceleratLeft()
        {
            GameObject player = game.GetPlayer();
            float previousX = player.GetComponent<TwoDTools.PlayerController2D>().currentVelocity.x;
            player.GetComponent<TwoDTools.PlayerMovement>().AccelerateLeft();
            Assert.Greater(previousX, player.GetComponent<TwoDTools.PlayerController2D>().currentVelocity.x);
            // Use the Assert class to test conditions.
            // Use yield to skip a frame.
            yield return new WaitForSeconds(1f);
        }
        [UnityTest]
        public IEnumerator SlopeTest()
        {
            GameObject player = game.GetPlayer();
            player.GetComponent<TwoDTools.PlayerState>().ResetAllStates();
            player.GetComponent<TwoDTools.PlayerState>().SetIsTouchingFloor(true);
            player.GetComponent<TwoDTools.PlayerController2D>().currentVelocity.y = 0;

            player.GetComponent<TwoDTools.PlayerMovement>().AccelerateRight();
            float previousX = player.GetComponent<TwoDTools.PlayerController2D>().currentVelocity.x;
            float previousY = player.GetComponent<TwoDTools.PlayerController2D>().currentVelocity.y;

            player.GetComponent<TwoDTools.PlayerState>().SetIsTouchingSlopeFront(true);
            player.GetComponent<TwoDTools.PlayerState>().slopeAngleFront = 50;
            player.GetComponent<TwoDTools.PlayerController2D>().currentVelocity = player.GetComponent<TwoDTools.PlayerMovement>().MoveOnSlope();

            Assert.Greater(previousX, player.GetComponent<TwoDTools.PlayerController2D>().currentVelocity.x);
            Assert.Less(previousY, player.GetComponent<TwoDTools.PlayerController2D>().currentVelocity.y);

            yield return new WaitForSeconds(1f);
        }

        [UnityTest]
        public IEnumerator PlayerDecellerateLeft()
        {
            GameObject player = game.GetPlayer();
            float previousX = player.GetComponent<TwoDTools.PlayerController2D>().currentVelocity.x;
            player.GetComponent<TwoDTools.PlayerMovement>().AccelerateLeft();
            Assert.Greater(previousX, player.GetComponent<TwoDTools.PlayerController2D>().currentVelocity.x);

            previousX = player.GetComponent<TwoDTools.PlayerController2D>().currentVelocity.x;
            player.GetComponent<TwoDTools.PlayerMovement>().Decelerate();
            Assert.Greater(player.GetComponent<TwoDTools.PlayerController2D>().currentVelocity.x, previousX);

            yield return new WaitForSeconds(1f);
        }

        [UnityTest]
        public IEnumerator PlayerDecellerateRight()
        {
            GameObject player = game.GetPlayer();
            float previousX = player.GetComponent<TwoDTools.PlayerController2D>().currentVelocity.x;
            player.GetComponent<TwoDTools.PlayerMovement>().AccelerateRight();
            Assert.Greater(player.GetComponent<TwoDTools.PlayerController2D>().currentVelocity.x, previousX);

            previousX = player.GetComponent<TwoDTools.PlayerController2D>().currentVelocity.x;
            player.GetComponent<TwoDTools.PlayerMovement>().Decelerate();
            Assert.Greater(previousX, player.GetComponent<TwoDTools.PlayerController2D>().currentVelocity.x);

            yield return new WaitForSeconds(1f);
        }



    }
}
