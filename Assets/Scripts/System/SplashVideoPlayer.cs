using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class SplashVideoPlayer : MonoBehaviour
{
    public VideoPlayer VideoPlayer;
    public string NextSceneName;

    void Start()
    {
        VideoPlayer.loopPointReached += OnVideoEnd;
    }

    void OnVideoEnd(VideoPlayer vp)
    {
        SceneManager.LoadScene(NextSceneName);
    }
}
