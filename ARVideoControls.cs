using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.XR.ARFoundation;

public class ARVideoControls : MonoBehaviour
{
    public ARTrackedImageManager imageManager;
    public CanvasGroup videoControls;
    private VideoPlayer videoPlayer;

    private bool videoControlsFadeIn = false;
    public float fadeSpeed;
    public Sprite pauseSprite, playSprite;

    private void Awake()
    {
        
    }

    // Bind the ImageChanged function to the trackedImagesChanged function called by ARTrackedImageManager
    //    the ARTrackedImageManager.trackedImagesChanged requires ARTrackedImagesChangedEventsArgs as argument
    //    also idk why this doesn't go in awake (maybe it can?)
    private void OnEnable()
    {
        imageManager.trackedImagesChanged += ImageChanged;
    }

    private void OnDisable()
    {
        imageManager.trackedImagesChanged -= ImageChanged;
    }

    private void Update()
    {
        if(videoControlsFadeIn)
        {
            VideoControlsFadeIn();
        }
    }

    public void PlayPause()
    {
        videoPlayer = FindObjectOfType<VideoPlayer>();

        if(videoPlayer.isPlaying)
        {
            videoPlayer.Pause();
            GameObject.Find("Play/Pause").transform.GetChild(0).GetComponent<Image>().sprite = playSprite;
        } else
        {
            videoPlayer.Play();
            GameObject.Find("Play/Pause").transform.GetChild(0).GetComponent<Image>().sprite = pauseSprite;
        }
    }

    public void FastForward(int numberOfFrames)
    {
        videoPlayer = FindObjectOfType<VideoPlayer>();
        for (int i=0;i<numberOfFrames;i++)
        {
            videoPlayer.StepForward();
        }
    }

    public void Rewind(int numberOfFrames)
    {
        videoPlayer = FindObjectOfType<VideoPlayer>();
        for (int i=0;i<numberOfFrames;i++)
        {
            videoPlayer.time = videoPlayer.time - numberOfFrames*videoPlayer.playbackSpeed;
        }
    }

    // this function will be called any time a  tracked image is changed.  
    private void ImageChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
        if(eventArgs.added.Count > 0)
        {
            Debug.Log("Turning on video controls");
            //videoControls.SetActive(true);
            videoControlsFadeIn = true;
            GameObject.Find("Play/Pause").transform.GetChild(0).GetComponent<Image>().sprite = pauseSprite;
        }
        if(eventArgs.removed.Count > 0)
        {
            Debug.Log("Turning off video controls");
            //videoControls.SetActive(false);
            videoControls.alpha = 0;
            videoControls.interactable = false;
        }
    }

    private void VideoControlsFadeIn()
    {
        if(videoControls.alpha < 1)
        {
            videoControls.interactable = true;
            videoControls.alpha += Time.deltaTime * fadeSpeed;
            if(videoControls.alpha >= 1)
            {
                videoControlsFadeIn = false;
            }
        }
    }
}
