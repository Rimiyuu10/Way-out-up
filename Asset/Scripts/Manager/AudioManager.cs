using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [SerializeField] private AudioSource[] sfx;
    [SerializeField] private AudioSource[] bgm;

    private int bgmIndex;
    private bool isPaused; // Biến kiểm soát trạng thái tạm dừng

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if (!bgm[bgmIndex].isPlaying && !isPaused)
        {
            PlayRandomBGM();
        }
    }

    // Gọi khi ứng dụng tạm dừng hoặc quay lại
    private void OnApplicationPause(bool pauseStatus)
    {
        isPaused = pauseStatus;

        if (isPaused)
        {
            // Dừng BGM khi ứng dụng tạm dừng
            if (bgm[bgmIndex].isPlaying)
            {
                bgm[bgmIndex].Pause();
            }
        }
        else
        {
            // Tiếp tục phát BGM khi ứng dụng quay lại
            bgm[bgmIndex].UnPause();
        }
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        if (hasFocus && !isPaused)
        {
            // Nếu ứng dụng trở lại mà chưa bị pause, tiếp tục phát nhạc
            if (!bgm[bgmIndex].isPlaying)
            {
                bgm[bgmIndex].UnPause();
            }
        }
    }

    public void PlayRandomBGM()
    {
        bgmIndex = Random.Range(0, bgm.Length);
        PlayBGM(bgmIndex);
    }

    public void PlaySFX(int index)
    {
        if (index < sfx.Length)
        {
            sfx[index].pitch = Random.Range(.85f, 1.1f);
            sfx[index].Play();
        }
    }

    public void StopSFX(int index)
    {
        sfx[index].Stop();
    }

    public void PlayBGM(int index)
    {
        StopBGM();
        bgm[index].Play();
    }

    public void StopBGM()
    {
        for (int i = 0; i < bgm.Length; i++)
        {
            bgm[i].Stop();
        }
    }
}
