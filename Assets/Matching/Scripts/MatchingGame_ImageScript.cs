using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

namespace Enneas.MatchingGame
{
    public class MatchingGame_ImageScript : MonoBehaviour, IRestart
    {
        MatchingGameController matchingGameController;
        [SerializeField] private GameObject textInfo;

        private static Dictionary<GameObject, Vector3> initialPositions = new Dictionary<GameObject, Vector3>();

        private void Start()
        {
            matchingGameController = FindObjectOfType<MatchingGameController>();

            // Store the initial position of this image
            StoreInitialPosition();
        }

        private void OnEnable()
        {
            MatchingGameController.OnReset += ResetGame;
        }

        private void OnDisable()
        {
            MatchingGameController.OnReset += ResetGame;
        }

        private void StoreInitialPosition()
        {
            if (transform.parent != null)
            {
                initialPositions[transform.parent.gameObject] = transform.parent.localPosition;
            }
        }

        public void Match(Vector3 center, GameObject image)
        {
            // Loop to make all texts not draggable while the image animation is playing;
            for (int i = 0; i < matchingGameController.texts.Length; i++)
            {
                matchingGameController.texts[i].GetComponent<MatchingGame_TextScript>().canDrag = false;
            }

            StartCoroutine(DisabelImage());

            // We want to bring the image on top of everything in the scene
            // so other elements won't be rendered on top of it during the animation;
            image.transform.parent.parent.SetAsLastSibling();
            image.transform.parent.SetAsLastSibling();

            image.transform.parent.DOMove(new Vector3(center.x, center.y, 0), 1.5f);
            image.transform.parent.DOScale(2f, 1.5f);

            // Check if we have won the game;
            matchingGameController.WinGame();
        }

        IEnumerator DisabelImage()
        {
            // We take the "MatchingGame_Textscript" component from all the titles and we can set the
            // canDrag variable to false so that the user cannot drag another text while the
            // image animation plays;

            this.GetComponent<Image>().enabled = false;
            yield return new WaitForSeconds(1.5f);

            textInfo.SetActive(true);
            yield return new WaitForSeconds(1.5f);

            transform.parent.gameObject.SetActive(false);

            for (int i = 0; i < matchingGameController.texts.Length; i++)
            {
                matchingGameController.texts[i].GetComponent<MatchingGame_TextScript>().canDrag = true;
            }
        }

        public void ResetGame()
        {
            // Ensure the image component is visible
            Image imageComponent = GetComponent<Image>();
            if (imageComponent != null)
            {
                imageComponent.enabled = true;
            }

            // Reset the textInfo to its initial state
            if (textInfo != null)
            {
                textInfo.SetActive(false);
            }

            // Ensure the parent object of this transform is active
            if (transform.parent != null && transform.parent.gameObject != null)
            {
                transform.parent.gameObject.SetActive(true);
            }

            // Reset the position and scale of the image
            if (transform.parent != null)
            {
                // Reset position to the stored initial position
                if (initialPositions.TryGetValue(transform.parent.gameObject, out Vector3 initialPosition))
                {
                    transform.parent.localPosition = initialPosition;
                }
                transform.parent.localScale = Vector3.one; // Reset scale
            }

            // Ensure draggable state for all text elements
            if (matchingGameController != null && matchingGameController.texts != null)
            {
                for (int i = 0; i < matchingGameController.texts.Length; i++)
                {
                    var textScript = matchingGameController.texts[i].GetComponent<MatchingGame_TextScript>();
                    if (textScript != null)
                    {
                        textScript.canDrag = true;
                    }
                }
            }
        }
    }

}
