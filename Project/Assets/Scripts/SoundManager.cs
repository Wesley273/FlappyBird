using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour {

    public static SoundManager instance = null;

    public AudioClip flyClip;
    public AudioClip dieClip;
    public AudioClip pointClip;

    private AudioSource audioSource;

    void Awake() {
        if (instance == null) {
            instance = this;
        } else if (instance != this) {
            Destroy(gameObject);
        }

        audioSource = GetComponent<AudioSource>();
    }

    public void PlayFly() {
        audioSource.clip = flyClip;
        audioSource.Play();
    }

    public void PlayDie() {
        audioSource.clip = dieClip;
        audioSource.Play();
    }

    public void PlayPass() {
        audioSource.clip = pointClip;
        audioSource.Play();
    }
}
