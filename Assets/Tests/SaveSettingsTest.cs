using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEditor;
using UnityEngine.TestTools;
using System.IO;

namespace Tests
{
    public class SaveSettingsTest
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
        public void SaveSettingsTestSimplePasses()
        {
            // Use the Assert class to test conditions
            bool saved = game.GetPlayer().GetComponent<TwoDTools.PlayerController2D>().SaveSettings("akjshdakjdhaskjhdaslkdhasljkd");
            Assert.AreEqual(saved, true);
            FileAssert.Exists(Application.dataPath + "/2DCharacterController Saved Settings/" + "akjshdakjdhaskjhdaslkdhasljkd" + ".txt");
            File.Delete(Application.dataPath + "/2DCharacterController Saved Settings/" + "akjshdakjdhaskjhdaslkdhasljkd" + ".txt");
            Assert.AreEqual(File.Exists(Application.dataPath + "/2DCharacterController Saved Settings/" + "akjshdakjdhaskjhdaslkdhasljkd" + ".txt"), false);
            AssetDatabase.Refresh();
        }

        // A Test behaves as an ordinary method
        [Test]
        public void SaveSettingsLoads()
        {
            // Use the Assert class to test conditions
            game.GetPlayer().GetComponent<TwoDTools.PlayerController2D>().playerControllerData.acceleration = -1;
            Assert.AreEqual(-1, game.GetPlayer().GetComponent<TwoDTools.PlayerController2D>().playerControllerData.acceleration);
            bool saved = game.GetPlayer().GetComponent<TwoDTools.PlayerController2D>().SaveSettings("test1");
            Assert.AreEqual(saved, true);
            FileAssert.Exists(Application.dataPath + "/2DCharacterController Saved Settings/" + "test1" + ".txt");
            game.GetPlayer().GetComponent<TwoDTools.PlayerController2D>().playerControllerData.acceleration = -9999;
            Assert.AreEqual(-9999, game.GetPlayer().GetComponent<TwoDTools.PlayerController2D>().playerControllerData.acceleration);

            bool loaded = game.GetPlayer().GetComponent<TwoDTools.PlayerController2D>().LoadSettings("test1");
            Assert.AreEqual(loaded, true);
            Assert.AreEqual(-1, game.GetPlayer().GetComponent<TwoDTools.PlayerController2D>().playerControllerData.acceleration);

            // Fail a load
            game.GetPlayer().GetComponent<TwoDTools.PlayerController2D>().playerControllerData.acceleration = -2;
            Assert.AreEqual(-2, game.GetPlayer().GetComponent<TwoDTools.PlayerController2D>().playerControllerData.acceleration);
            bool failedLoad = game.GetPlayer().GetComponent<TwoDTools.PlayerController2D>().LoadSettings("doesntExist");
            Assert.AreEqual(failedLoad, false);
            Assert.AreEqual(-2, game.GetPlayer().GetComponent<TwoDTools.PlayerController2D>().playerControllerData.acceleration);


            File.Delete(Application.dataPath + "/2DCharacterController Saved Settings/" + "test1" + ".txt");
            Assert.AreEqual(File.Exists(Application.dataPath + "/2DCharacterController Saved Settings/" + "test1" + ".txt"), false);
            AssetDatabase.Refresh();
        }

    }
}
