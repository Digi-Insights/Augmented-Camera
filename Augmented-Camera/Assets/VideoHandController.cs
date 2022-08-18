
using System;
using System.Collections;
using System.Collections.Generic;
using Mediapipe;
using UnityEngine;
using Mediapipe.Unity;
using Mediapipe.Unity.HandTracking;
using UnityEngine.Serialization;
using UnityEngine.UI;
using UnityEngine.Video;

public class VideoHandController : MonoBehaviour
{
  public int videoIndex = 1;
  public bool controlTime, controlBrightness, controlVolume;
  
  public VideoClip[] videos;
  
  public RawImage videoImage;
  public VideoPlayer player;
  public RawImage videoCover;
  public ButtonBar buttonBar;

  private PointAnnotation[] _leftHandPointAnnotations;
  private PointAnnotation[] _rightHandPointAnnotations;

  private long _videoTime = 0;

  private void Awake()
  {
    player.clip = videos[videoIndex - 1];
  }

  private void OnEnable()
  {
    videoCover.texture =
      Resources.Load("Videos/Video1/Images/Capture02626") as Texture2D;
  }

  private void Update()
  {

    if(!SetupHands())
      return;

    var leftHandBegin = _leftHandPointAnnotations[0].transform.position;
    var leftHandEnd = _leftHandPointAnnotations[12].transform.position;
    
    
    var closePoint = GetClosestPointOnInfiniteLine(_rightHandPointAnnotations[8].transform.position,
      leftHandBegin, leftHandEnd);
    
    var yRatio = Vector3.Distance(closePoint, leftHandBegin) / Vector3.Distance(leftHandBegin, leftHandEnd);

    var distance = Vector3.Distance(_rightHandPointAnnotations[8].transform.position, closePoint);
    print("Distance: " +distance);
    
    if(controlTime){
      if (distance < 10)
      {
        ControlTimeInTimeLine(yRatio, true);
        //ControlTimeInSpeed(yRatio, true);
        return;
      }
      else
      {
        ControlTimeInTimeLine(yRatio, false);
        //ControlTimeInSpeed(yRatio, false);
      }
    }

    if (controlBrightness && controlVolume)
      if(CheckIfOneFinger())
        ControlBrightness(yRatio);
      else
        ControlVolume(yRatio);
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

  private bool CheckIfOneFinger()
  {
    var distanceTwoTips = Vector3.Distance(_rightHandPointAnnotations[8].transform.position,
      _rightHandPointAnnotations[12].transform.position);
    var distanceIndexToRoot = Vector3.Distance(_rightHandPointAnnotations[8].transform.position,
      _rightHandPointAnnotations[6].transform.position);
    return distanceIndexToRoot < distanceTwoTips;
  }

  private void ControlBrightness(float ratio)
  {
    var temp = videoImage.color;
    temp.a = ratio;
    videoImage.color = temp;
    print("Bright: " + ratio);
  }
  
  private void ControlVolume(float ratio)
  {
    player.SetDirectAudioVolume(0,ratio); 
    print("Volume: " + ratio);
  }

  private void ControlTimeInTimeLine(float ratio, bool on)
  {    
    var videoRawTime = (long) (ratio * player.frameCount);
    const int sensitivity = 200;
    // ReSharper disable once UselessBinaryOperation
    print(videoRawTime - _videoTime);
    var timeGap = Mathf.Abs(videoRawTime - _videoTime);
    if (videoRawTime - _videoTime > sensitivity)
      _videoTime += (long)(timeGap*Time.deltaTime*3);
    else if (_videoTime - videoRawTime > sensitivity)
      _videoTime -= (long)(timeGap*Time.deltaTime*3);
    
    if(on)
    {
      buttonBar.SetTime(_videoTime);
      videoCover.gameObject.SetActive(true);
      var index = (_videoTime - _videoTime % 125 + 1).ToString("00000");
      print("Set time: " + index);
      videoCover.texture =
        Resources.Load($"Videos/Video{videoIndex}/Images/Capture" + index ) as Texture2D;
      if(!player.isPaused)
        player.Pause();
    }
    else
    {
      if (player.isPlaying) return;
      videoCover.gameObject.SetActive(false);
      print("Apply time: " + _videoTime);
      player.frame = _videoTime;
      StartCoroutine(WaitVideoLoad(_videoTime));
      player.Play();
    }
  }

  IEnumerator WaitVideoLoad(Int64 videoTime)
  {
    yield return new WaitUntil(() => player.frame == videoTime);
    buttonBar.CanCounting = true;
  }

  private void ControlTimeInSpeed(float ratio, bool on)
  {
    if(on){
      if (ratio > 0.5f)
      {
        player.playbackSpeed = 4;
      }
      else
      {
        player.playbackSpeed = -4;
      }
    }
    else
    {
      player.playbackSpeed = 1;
    }
  }
  
  private static Vector3 GetClosestPointOnInfiniteLine(Vector3 point, Vector3 lineStart, Vector3 lineEnd)
  {
    if (Vector3.Dot(point - lineStart, lineEnd - lineStart) < 0)
      return lineStart;
    else if (Vector3.Dot(point - lineEnd, lineStart - lineEnd) < 0)
      return lineEnd;
    else 
      return lineStart + Vector3.Project(point - lineStart, lineEnd - lineStart);
  }

  
}
