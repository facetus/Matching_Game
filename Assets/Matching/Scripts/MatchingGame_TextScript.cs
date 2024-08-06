using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;

public class MatchingGame_TextScript : MonoBehaviour, IEndDragHandler
{
    [SerializeField] private MatchingGameController matchingGameController;
    [SerializeField] private Canvas canvas;

    private RectTransform rectTransform;
    private Vector2 originalPosition;

    public bool correctPair;

    public GameObject pair1;
    public GameObject pair2;

    public bool isOverlapping;

    [SerializeField] private GameObject centerPos;
    public bool canDrag;
    [SerializeField] private GameObject trailElementPrefab;
    private List<GameObject> trailElements = new List<GameObject>();
    public bool isDragging;




    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        originalPosition = rectTransform.anchoredPosition;
        
    }

    private void Start()
    {
        canDrag = true;
        Debug.Log("Rect Transform POS: " + originalPosition);
    }

    public void DragHandler(BaseEventData data)
    {
        if (canDrag && !MatchingGameSettings.isPaused)
        {
            isDragging = true;
            PointerEventData pointerEventData = (PointerEventData)data;

            Vector2 position;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                (RectTransform)canvas.transform,
                pointerEventData.position,
                canvas.worldCamera,
                out position);

            transform.position = canvas.transform.TransformPoint(position);

            this.transform.parent.SetAsLastSibling();
            CreateTrailElement(transform.position);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        foreach (GameObject trailElement in trailElements)
        {
            //We return the trailElement back to the Queue in our object pooling
            ObjectPoolTrailRenderer.SharedInstance.ReturnPooledObject(trailElement);
            //trailElement.SetActive(false);
        }

        if (correctPair)
        {
            PairLink(pair1, pair2);
        }
        else
        {
            StartCoroutine(WrongLink());
        }

        isDragging = false;
        isOverlapping = false;
        correctPair = false;
    }

    public void SetCorrectPair()
    {
        correctPair = true;
    }

    public void SetWrongPair()
    {
        correctPair = false;
    }

    IEnumerator WrongLink()
    {
        matchingGameController.totalMoves++;

        if (isOverlapping)
        {
            this.transform.DOShakePosition(1f, 50);
            yield return new WaitForSeconds(1f);
        }

        ResetToOriginalPosition();
        isOverlapping = false;
    }

    public void PairLink(GameObject image, GameObject title)
    {
        StartCoroutine(FadeAndDestroyTrailElements(0.5f));

        matchingGameController.correctMatches++;

        var center = new Vector3(centerPos.transform.position.x, centerPos.transform.position.y, 0f);

        image.GetComponent<MatchingGame_ImageScript>().Match(center, image);

        rectTransform.anchoredPosition = originalPosition;
    }

    public void ResetToOriginalPosition()
    {
        rectTransform.anchoredPosition = originalPosition;
    }




    private void CreateTrailElement(Vector3 position)
    {
        // Get a pooled object from the shared instance of ObjectPoolTrailRenderer
        GameObject trailElement = ObjectPoolTrailRenderer.SharedInstance.GetPooledObject();

        if (trailElement != null)
        {
            // Set the position and rotation of the pooled object
            trailElement.transform.position = position;
            trailElement.transform.rotation = Quaternion.identity;

            // Set the parent if needed (though this is generally handled by the pool)
            // If parent was set on instantiation, this might not be needed
            //trailElement.transform.SetParent(canvas.transform, false);

            // Activate the pooled object
            trailElement.SetActive(true);

            // Add the trail element to the list for further processing
            trailElements.Add(trailElement);

            // Start the coroutine to fade and deactivate the trail element
            StartCoroutine(FadeAndDestroyTrailElement(trailElement, 0.5f));
        }
    }



    private IEnumerator FadeAndDestroyTrailElements(float lifetime)
    {
        foreach (GameObject trailElement in trailElements)
        {
            yield return StartCoroutine(FadeAndDestroyTrailElement(trailElement, lifetime));
        }
        trailElements.Clear();
    }



    private IEnumerator FadeAndDestroyTrailElement(GameObject trailElement, float lifetime)
    {
        if (trailElement != null) {
            Image trailImage = trailElement.GetComponent<Image>();

            if (trailImage != null) {
                Color originalColor = trailImage.color;

                float elapsedTime = 0f;
                while (elapsedTime < lifetime)
                {
                    elapsedTime += Time.deltaTime;
                    float alpha = Mathf.Lerp(1f, 0f, elapsedTime / lifetime);
                    trailImage.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
                    yield return null;
                }


                ObjectPoolTrailRenderer.SharedInstance.ReturnPooledObject(trailElement);
                //trailElement.SetActive(false);

            }


        }
        

    }
}
