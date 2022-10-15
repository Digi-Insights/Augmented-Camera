using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Mediapipe.Unity;
using UnityEngine;
using Screen = UnityEngine.Screen;

public class MessageBox : MonoBehaviour
{
    public MessageBoxMovement movement;
    public MessageBoxMovement fullBox;
    public Transform deleteButton;
    private PointAnnotation[] _leftHandPointAnnotations;
    private PointAnnotation[] _rightHandPointAnnotations;

    private void Update()
    {
        if(!SetupHands())
            return;

        if (Vector3.Distance(_rightHandPointAnnotations[8].transform.position, transform.position) < 50)
        {
            fullBox.gameObject.SetActive(true);
        }
        else
        {
            fullBox.MoveBackToDisable();
        }

        if (Camera.main == null) return;
        print("Screen Distance = " +
              (Camera.main.WorldToScreenPoint(_rightHandPointAnnotations[8].transform.position).x -
               Screen.width));

        if (fullBox.gameObject.activeSelf &&
            Mathf.Abs(Camera.main.WorldToScreenPoint(_rightHandPointAnnotations[8].transform.position).x -
                      Screen.width) < 50)
        {
            movement.MoveBackToDisable();
        }

        if (!fullBox.IsTweening() && fullBox.gameObject.activeSelf &&
            Vector3.Distance(_rightHandPointAnnotations[8].transform.position, deleteButton.position) < 20)
        {
            movement.MoveBackToDisable();
        }
    }

    private bool SetupHands()
    {
        if (_leftHandPointAnnotations != null && _rightHandPointAnnotations!= null && _leftHandPointAnnotations.Length > 0 && _rightHandPointAnnotations.Length > 0) 
            return true;
    
        var hands = FindObjectsOfType<HandLandmarkListAnnotation>();
        if(hands.Length != 2) return false;

        var hasLeft = false;
        var hasRight = false;
    
        foreach (var hand in hands)
        {
            var pointList = hand.GetComponentInChildren<PointListAnnotation>();
            var annotations = pointList.GetComponentsInChildren<PointAnnotation>();
            switch (hand.Handedness)
            {
                case HandLandmarkListAnnotation.Hand.Left:
                    _leftHandPointAnnotations = annotations;
                    hasLeft = true;
                    break;
                case HandLandmarkListAnnotation.Hand.Right:
                    _rightHandPointAnnotations = annotations;
                    hasRight = true;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    
        return hasLeft && hasRight;
    }
}
