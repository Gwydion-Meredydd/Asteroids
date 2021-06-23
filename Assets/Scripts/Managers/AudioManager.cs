using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

#region SummarySection
/// <summary>
/// Singleton Class that can be used to random auido clips when called
///  </summary>
/// <param name="AudioManager"></param>

#endregion
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    public AudioMixer Mixer;
    [Header("Audio Sources")]
    public AudioSource SFXWorldAudioSource;
    public AudioSource SFXShipAudioSource;
    [Header("Audio Clips")]
    public AudioClip[] RockDestoryed;
    public AudioClip[] PlayerShoot;
    public AudioClip ShipThrusters;

    private void Awake()
    {
        Instance = this;
    }

    public void PlayRockDestory() 
    {
        SFXWorldAudioSource.PlayOneShot(RockDestoryed[Random.Range(0, RockDestoryed.Length)]);
    }
    public void PlayPlayerShoot()
    {
        SFXWorldAudioSource.PlayOneShot(PlayerShoot[Random.Range(0, PlayerShoot.Length)]);
    }
    public void PlayPlayerMove() 
    {
        //check is needed to stop overlapping audio
        if (!GameManager.Instance.InMenu)
        {
            if (!SFXShipAudioSource.isPlaying)
            {
                SFXShipAudioSource.PlayOneShot(ShipThrusters);
            }
        }
    }
    public void StopPlayerMove()
    {
        SFXShipAudioSource.Stop();
    }

}
