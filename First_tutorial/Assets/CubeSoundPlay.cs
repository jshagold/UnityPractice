using UnityEngine;

public class CubeSoundPlay : MonoBehaviour
{

    private AudioSource audioSource; // cache
    [SerializeField] private AudioClip[] audioClips;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlaySound(int index, bool isSkip)
    {
        if(!isSkip)
        {
            if (audioSource.isPlaying)
            {
                return;
            }
        }

        

        if(audioClips.Length > index)
        {
            audioSource.clip = audioClips[index];
            audioSource.Play();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
