using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Video;


/*
 * The following script is meant for the matching game for the AR applications;
 * 
 * The logic of the game is the following:
 * 
 * We have X objects that each one has Y connections; Meaning we can have 2 artists and each artist has 3 paintings of theirs;
 * To achive that we use a Dictionary that connects a Gameobject (artists) with a List of Gameobjects(list of paintings);
 * 
 */

namespace Enneas.MatchingGame
{
    public class MatchingGameController : MonoBehaviour, IRestart
    {
        public static event Action OnReset;

        //Arraylist of Texts;
        [SerializeField] public GameObject[] texts;

        //Arraylist of Images;
        [SerializeField] public GameObject[] images;

        //We make a dictionary of gameobject with a list of gameobjects because we want to pair 1 object with multiple
        //since we want to have 1 architect be associated with 3 paintings;
        public Dictionary<GameObject, List<GameObject>> matchingDictionary;

        [SerializeField] private GameObject gameCanva;


        [SerializeField] private GameObject startScreenCanva;

        [SerializeField] public int correctMatches;

        /// <summary>
        /// Requires: Gamehunting Script;
        ///
        /// </summary>
        //private GameHunting gameHunting;

        [SerializeField] private int currentPrefabIndex;

        public int totalMoves;
        private int score;

        /// <summary>
        /// Requires: Achievement Manager Script;
        ///
        /// </summary>

        //Achievement Manager;
        //private AchievementManager achievementManager;
        //[SerializeField] int budgeIndex;

        /// <summary>
        /// Requires: BadgeCaseHandlder Script;
        ///
        /// </summary>
        //private BagdeCaseHandler bagdeCaseHandler;


        /// <summary>
        /// Requires: Score Script;
        ///
        /// </summary>
        //Score System;
        //private ScoreSystem scoreSystem;

        [SerializeField] private int totalPairs;
        [SerializeField] GameObject bubble;

        //EndScreen;
        [SerializeField] private GameObject endScreen;

        [SerializeField] private string achievementKey;

        [SerializeField] private VideoPlayer player;

        /// <summary>
        /// Requires: SoundHanlder Script;
        ///
        /// </summary>
        //[SerializeField] private SoundHandler[] soundHanlder;


        private void OnEnable()
        {
            MatchingGameSettings.OnExitGame += QuitButton;
            if (this.gameObject.name != "pf_matching_game")
            {
                this.gameObject.name = "pf_matching_game";
            }
        }


        private void OnDisable()
        {
            MatchingGameSettings.OnExitGame -= QuitButton;
        }

        private void Start()
        {

            //2 architects * 3 paintings = 6 pairs;
            totalPairs = 6;


            //gameHunting = FindObjectOfType<GameHunting>();

            InitializeLists();

            //Initialization;
            //achievementManager = FindObjectOfType<AchievementManager>();
            //bagdeCaseHandler = FindObjectOfType<BagdeCaseHandler>();
            //scoreSystem = FindObjectOfType<ScoreSystem>();

            ////Audio
            //if (SoundHandler.hasSound)
            //{
            //    player.SetDirectAudioMute(0, false);
            //}
            //else
            //{
            //    player.SetDirectAudioMute(0, true);
            //}
            //soundHanlder[0].AddVideoPlayer(player);
            //soundHanlder[1].AddVideoPlayer(player);


        }




        public void InitializeLists()
        {
            matchingDictionary = new Dictionary<GameObject, List<GameObject>>();

            //Ensure both arrays are initialized and have elements
            if (texts != null && images != null && texts.Length > 0 && images.Length > 0)
            {
                //Number of images each text should have
                int imagesPerText = 3;

                //Ensure there are enough images for the texts
                if (texts.Length * imagesPerText > images.Length)
                {
                    Debug.LogError("Not enough images to pair with texts.");
                    return;
                }

                //Iterate through texts array;
                for (int i = 0; i < texts.Length; i++)
                {
                    //Create a new list for each text object
                    List<GameObject> associatedImages = new List<GameObject>();

                    //Calculate the starting index for images for each text
                    int startIndex = i * imagesPerText;
                    int endIndex = startIndex + imagesPerText;

                    //Add images to the list for the current text
                    for (int j = startIndex; j < endIndex; j++)
                    {
                        associatedImages.Add(images[j]);
                    }

                    //Add the text and its associated images list to the dictionary
                    matchingDictionary.Add(texts[i], associatedImages);
                }

            }
            else
            {
                Debug.LogError("Texts or images arrays are not initialized or empty.");
            }
        }




        public void StartGame()
        {

            //Destroy(startScreenCanva);

            gameCanva.SetActive(true);

            startScreenCanva.SetActive(false);

            startScreenCanva.GetComponent<CanvasGroup>().alpha = 1f;
            //If we want to use the bubble;
            //StartCoroutine(ActivateBubble());

        }

        /*IEnumerator ActivateBubble()
        {
            yield return new WaitForSeconds(1.5f);

            bubble.SetActive(true);
            bubble.transform.DOScale(0.8f, 1f);

            yield return new WaitForSeconds(3f);
            bubble.transform.DOScale(0, 1f);

            yield return new WaitForSeconds(1f);
            Destroy(bubble);
        }*/


        public void EndScreen()
        {
            endScreen.SetActive(true);
        }


        public void GainAchievement()
        {
            //DisplayAchivement
            StartCoroutine(QuitGameCo());
            //achievementManager.Unlock(achievementKey);
            //gameHunting.FinishCurrentGame(gameHunting.FindPrefabIndex(gameObject.name), true);

            //this.gameObject.SetActive(false);
        }

        public void QuitButton()
        {


            //Close the panel;
            GetComponentInChildren<MatchingGameSettings>().panelToShadow.enabled = false;
            MatchingGameSettings.isPaused = false;

            //set game as lost;
            //gameHunting.FinishCurrentGame(gameHunting.FindPrefabIndex(gameObject.name), false);



        }

        IEnumerator QuitGameCo()
        {
            //achievementManager.Unlock(achievementKey);
            //bagdeCaseHandler.UnlockBudge(budgeIndex);
            //gameHunting.FinishCurrentGame(gameHunting.FindPrefabIndex(gameObject.name), true);
            yield return new WaitForSeconds(0.01f);

            if (this.gameObject.activeSelf)
            {
                this.gameObject.SetActive(false);
            }
        }


        public void WinGame()
        {
            if (correctMatches == totalPairs)
            {
                ///Calculate Total Score;
                CalculateScore();

                //We want a small delay to make sure the animation of the last image moving is finished;
                StartCoroutine(DelayForWin());

            }

        }

        //We need a small delay in order to allow the last image to finish its scale + move animation;
        IEnumerator DelayForWin()
        {
            yield return new WaitForSeconds(2.5f);
            Debug.Log("You have won");



            gameCanva.SetActive(false);
            EndScreen();


        }

        public void ResetGame()
        {

            OnReset?.Invoke();
            //gameHunting.FinishCurrentGame(gameHunting.FindPrefabIndex(gameObject.name), false);
            // Reset correct matches and total moves
            correctMatches = 0;
            totalMoves = 0;

            // Clear the matching dictionary
            matchingDictionary.Clear();

            // Reset score
            score = 0;

            // Reset UI elements
            if (gameCanva != null)
            {
                gameCanva.SetActive(false);

            }
            if (startScreenCanva != null)
            {
                startScreenCanva.SetActive(true);



            }
            if (endScreen != null)
            {
                endScreen.SetActive(false);
            }




            // Reinitialize lists
            InitializeLists();


        }

        public void CalculateScore()
        {
            if (totalMoves < 10)
            {
                score = 4;
            }
            else if (totalMoves < 14)
            {
                score = 3;
            }
            else if (totalMoves < 18)
            {
                score = 2;
            }
            else
                score = 1;

            //scoreSystem.AddMinigameScore(score);
        }

    }

}
