using UnityEngine;

public class AnimationSound : MonoBehaviour
{
    [SerializeField]AudioClip[] _audios;
    [SerializeField]AudioSource _audioSource;

    public void Play(int index)
    {
        _audioSource.clip = _audios[index];
        _audioSource.Play();
    }
}
