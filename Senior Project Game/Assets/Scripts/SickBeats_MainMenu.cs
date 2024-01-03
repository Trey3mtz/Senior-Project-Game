using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SickBeats_MainMenu : MonoBehaviour
{
    [SerializeField] AudioSource sfx1;
    [SerializeField] AudioSource sfx2;

    private float soundLength;

    // Start is called before the first frame update
    void Start()
    {
        soundLength = sfx1.clip.length;
        StartCoroutine(StartFX());
    }

     IEnumerator StartFX()
    {
        yield return new WaitForSeconds(soundLength*4f);
        StartCoroutine(BeatFX());
    }

    IEnumerator BeatFX()
    {

        sfx1.Play();
        yield return new WaitForSeconds(soundLength);


        yield return new WaitForSeconds(soundLength);
        sfx2.Play();
        yield return new WaitForSeconds(soundLength*3f);

         StartCoroutine(BeatFX());       
    }
}
