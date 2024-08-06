using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;


namespace Enneas.MatchingGame
{
    public class MatchingGameSettings : MonoBehaviour, IPausable, IRestart
    {

        [HideInInspector] public Image panelToShadow;

        [SerializeField] private GameObject pausePanel;
        [SerializeField] private GameObject closePanel;
        public static bool isPaused;

        public static event Action OnExitGame;

        private void OnEnable()
        {
            MatchingGameController.OnReset += ResetGame;
        }

        private void OnDisable()
        {
            MatchingGameController.OnReset -= ResetGame;
        }

        public void PauseContainer()
        {
            pausePanel.SetActive(true);

            StartCoroutine(FadeToBlack());

            isPaused = true;
        }

        public void Resume()
        {
            pausePanel.SetActive(false);
            closePanel.SetActive(false);
            StartCoroutine(FadeOut());
            isPaused = false;
        }


        public void CloseContainer()
        {
            StartCoroutine(FadeToBlack());
            closePanel.SetActive(true);
            isPaused = true;
        }

        public void ResetGame()
        {
            {
                // Ensure pause and close panels are hidden
                if (pausePanel != null)
                {
                    pausePanel.SetActive(false);
                }
                if (closePanel != null)
                {
                    closePanel.SetActive(false);
                }

                // Ensure the panel to shadow is hidden and reset
                if (panelToShadow != null)
                {
                    panelToShadow.gameObject.SetActive(false);
                    Color color = panelToShadow.color;
                    color.a = 0f; // Ensure alpha is reset to 0
                    panelToShadow.color = color;
                }

                isPaused = false;

            }
        }

        public void ExitGame()
        {
            //We need to deactive first the overlay, otherwise
            //when we reopen the game, it will still be active with 0 alpha;
            panelToShadow.gameObject.SetActive(false);

            MatchingGameController gameController = FindObjectOfType<MatchingGameController>();

            OnExitGame?.Invoke();



            Destroy(transform.root.gameObject);
        }

        //Make fading animations when we enter pause & when we exit pause;
        private IEnumerator FadeToBlack()
        {
            panelToShadow.gameObject.SetActive(true);
            float elapsedTime = 0f;
            Color color = panelToShadow.color;
            while (elapsedTime < 1f)
            {
                elapsedTime += Time.unscaledDeltaTime; // Use unscaledDeltaTime to ignore time scale
                color.a = Mathf.Clamp01(elapsedTime / 1f) * 0.5f; // Target alpha value of 0.5
                panelToShadow.color = color;
                yield return null;
            }
            color.a = 0.5f; // Ensure the final alpha is 0.5
            panelToShadow.color = color;
        }

        private IEnumerator FadeOut()
        {
            float elapsedTime = 0f;
            Color color = panelToShadow.color;
            while (elapsedTime < 1f)
            {
                elapsedTime += Time.unscaledDeltaTime; // Use unscaledDeltaTime to ignore time scale
                color.a = Mathf.Clamp01(0.5f - (elapsedTime / 1f) * 0.5f); // Start at 0.5 and fade to 0
                panelToShadow.color = color;
                yield return null;
            }
            color.a = 0f; // Ensure the final alpha is 0
            panelToShadow.color = color;
            panelToShadow.gameObject.SetActive(false);
        }
    }

}
