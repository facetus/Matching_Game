using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UICollisionDetection : MonoBehaviour
{
    public string targetTag = "MatchingTag"; // Tag to filter specific game objects for collision detection

    private RectTransform rectTransform; // RectTransform of this gameobject

    [SerializeField] private MatchingGameController matchingGameController; // Reference to the matching game controller script

    private void Awake()
    {
        //This image's rectTransform;
        rectTransform = GetComponent<RectTransform>();
    }

    private void Update()
    {
        CheckCollisions();
    }

    private void CheckCollisions()
    {
        RectTransform[] allRectTransforms = transform.root.GetComponentsInChildren<RectTransform>();

        foreach (RectTransform other in allRectTransforms)
        {
            //The Architect's rectTransforms;
            if (other == rectTransform || !other.gameObject.CompareTag(targetTag))
                continue;

            //If our image is overlappping with an architect;
            if (IsOverlapping(rectTransform, other))
            {
                //Debug.Log("Collided with: "+other.name);
                other.GetComponent<MatchingGame_TextScript>().isOverlapping = true;

                //We check if the image and the arcitect are together;
                if (AreObjectsLinked(this.gameObject, other.gameObject))
                {
                    CallSetCorrectPair(this.gameObject, other.gameObject);
                }
                else
                {
                    ResetValues(other.gameObject);
                }
            }
        }
    }

    private bool AreObjectsLinked(GameObject obj1, GameObject obj2)
    {
        if (matchingGameController.matchingDictionary.ContainsKey(obj1) && matchingGameController.matchingDictionary[obj1].Contains(obj2))
        {
            return true;
        }

        if (matchingGameController.matchingDictionary.ContainsKey(obj2) && matchingGameController.matchingDictionary[obj2].Contains(obj1))
        {
            return true;
        }

        return false;
    }

    private bool IsOverlapping(RectTransform rect1, RectTransform rect2)
    {
        Rect rect1World = GetWorldRect(rect1);
        Rect rect2World = GetWorldRect(rect2);

        return rect1World.Overlaps(rect2World);
    }

    private Rect GetWorldRect(RectTransform rt)
    {
        Vector3[] corners = new Vector3[4];
        rt.GetWorldCorners(corners);
        Vector2 size = new Vector2(corners[2].x - corners[0].x, corners[2].y - corners[0].y);
        return new Rect(corners[0], size);
    }

    public void CallSetCorrectPair(GameObject image, GameObject architect)
    {
        architect.GetComponent<MatchingGame_TextScript>().SetCorrectPair();
        architect.GetComponent<MatchingGame_TextScript>().pair1 = image.gameObject;
        architect.GetComponent<MatchingGame_TextScript>().pair2 = architect.gameObject;
    }

    public void ResetValues(GameObject gameobject)
    {
        gameobject.GetComponent<MatchingGame_TextScript>().SetWrongPair();
        gameobject.GetComponent<MatchingGame_TextScript>().pair1 = null;
        gameobject.GetComponent<MatchingGame_TextScript>().pair2 = null;
    }
}
