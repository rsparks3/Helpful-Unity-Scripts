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
    public ARSession aRSession;
    public CanvasGroup videoControls;
    private VideoPlayer videoPlayer;

    private bool videoControlsFadeIn = false;
    private bool videoControlsFadeOut = false;
    private bool videoMuted = false;
    public float fadeSpeed;
    public int fastForwardFrames, rewindFrames;
    public Sprite pauseSprite, playSprite, muteSprite, unmuteSprite;
    public Button playPauseButton, fastForwardButton, rewindButton, clearImagesButton, muteButton;

    private void Awake()
    {
        videoControls.alpha = 0;
        videoControls.interactable = false;
    }

    // Bind the ImageChanged function to the trackedImagesChanged function called by ARTrackedImageManager
    //    the ARTrackedImageManager.trackedImagesChanged requires ARTrackedImagesChangedEventsArgs as argument
    //    also idk why this doesn't go in awake (maybe it can?)
    private void OnEnable()
    {
        imageManager.trackedImagesChanged += ImageChanged;
        playPauseButton.onClick.AddListener(PlayPause);
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
        } else if (videoControlsFadeOut)
        {
            VideoControlsFadeOut();
        }
    }

    public void PlayPause()
    {
        videoPlayer = FindObjectOfType<VideoPlayer>();

        if (playSprite != null && pauseSprite != null)
        {
            if (videoPlayer.isPlaying)
            {
                videoPlayer.Pause();
                playPauseButton.transform.GetChild(0).GetComponent<Image>().sprite = playSprite;
            }
            else
            {
                videoPlayer.Play();
                playPauseButton.transform.GetChild(0).GetComponent<Image>().sprite = pauseSprite;
            }
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
        videoPlayer.time = videoPlayer.time - (numberOfFrames/videoPlayer.frameRate)*videoPlayer.playbackSpeed;
    }

    public void ClearARVideos()
    {
        aRSession.Reset();
        Debug.Log("Resetting AR Session");
    }

    public void MuteARVideo()
    {
        videoPlayer = FindObjectOfType<VideoPlayer>();
        videoMuted = !videoMuted;
        videoPlayer.SetDirectAudioMute(0, videoMuted);
        if(videoMuted)
        {
            muteButton.transform.GetChild(0).GetComponent<Image>().sprite = unmuteSprite;
        } else
        {
            muteButton.transform.GetChild(0).GetComponent<Image>().sprite = muteSprite;
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
            videoControls.interactable = true;
            playPauseButton.transform.GetChild(0).GetComponent<Image>().sprite = pauseSprite;
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
                videoControls.alpha = 1;
                videoControlsFadeIn = false;
            }
        }
    }

    private void VideoControlsFadeOut()
    {
        if (videoControls.alpha > 0)
        {
            videoControls.interactable = false;
            videoControls.alpha -= Time.deltaTime * fadeSpeed;
            if (videoControls.alpha <=0)
            {
                videoControls.alpha = 0;
                videoControlsFadeOut = false;
            }
        }
    }

    private void PlayPauseListener()
    {
        PlayPause();
    }

    private void FastForwardListener()
    {
        FastForward(fastForwardFrames);
    }

    private void RewindListener()
    {
        Rewind(rewindFrames);
    }

    private void ClearTrackedImageListener()
    {
        ClearARVideos();
    }

    private void MuteListener()
    {
        MuteARVideo();
    }

}
