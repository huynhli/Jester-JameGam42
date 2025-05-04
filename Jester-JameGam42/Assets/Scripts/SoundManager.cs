using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;
    private AudioSource activeAudioSource;

    [SerializeField] private AudioSource soundFXObject;

    private void Awake() {
        if (instance == null) {
            instance = this;
        }
    }

    public void PlaySoundFXClip(AudioClip audioClip, Transform spawnTransform, float volume) {
        AudioSource audioSource = Instantiate(soundFXObject, spawnTransform.position, Quaternion.identity);

        audioSource.clip = audioClip;
        audioSource.volume = volume;
        audioSource.Play();

        float clipLength = audioClip.length;
        Destroy(audioSource.gameObject, clipLength);
    }

    public void PlayLoopMusic(AudioClip audioClip, Transform spawnTransform, float volume) {
    if (activeAudioSource != null) {
        activeAudioSource.Stop();
        Destroy(activeAudioSource.gameObject);
    }

    activeAudioSource = Instantiate(soundFXObject, spawnTransform.position, Quaternion.identity);

    activeAudioSource.clip = audioClip;
    activeAudioSource.volume = volume;
    activeAudioSource.loop = true;
    activeAudioSource.Play();
}

    public void StopLoopingMusic() {
        if (activeAudioSource != null) {
            activeAudioSource.Stop();
            Destroy(activeAudioSource.gameObject);
            activeAudioSource = null;
        }
    }


}
