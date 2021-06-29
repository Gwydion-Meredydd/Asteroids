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
    public AudioSource SFXShieldAudioSource;
    public AudioSource SFXEnemyAudioSource;
    [Header("Audio Clips")]
    public AudioClip[] RockDestoryed;
    public AudioClip[] PlayerShoot;
    public AudioClip PlayerExplosion;
    public AudioClip[] EnemyShoot;
    public AudioClip[] MetalImpact;
    public AudioClip ShipThrusters;
    public AudioClip ShipShield;
    public AudioClip ShipTeleport;

    private void Awake()
    {
        Instance = this;
    }

    public void PlayPlayerTeleport()
    {
        SFXWorldAudioSource.PlayOneShot(ShipTeleport);
    }
    public void PlayShieldSfx() 
    {
        //check is needed to stop overlapping audio
        if (!GameManager.Instance.InMenu)
        {
            if (!SFXShieldAudioSource.isPlaying)
            {
                SFXShieldAudioSource.enabled = true;
            }
        }
    }
    public void StopShieldSfx()
    {
        //check is needed to stop overlapping audio
        if (!GameManager.Instance.InMenu)
        {
            if (SFXShieldAudioSource.isPlaying)
            {
                SFXShieldAudioSource.enabled = false;
            }
        }
    }
    public void PlayRockDestory() 
    {
        SFXWorldAudioSource.PlayOneShot(RockDestoryed[Random.Range(0, RockDestoryed.Length)]);
    }
    public void PlayPlayerShoot()
    {
        SFXWorldAudioSource.PlayOneShot(PlayerShoot[Random.Range(0, PlayerShoot.Length)]);
    }
    public void PlayEnemyShoot()
    {
        SFXEnemyAudioSource.PlayOneShot(EnemyShoot[Random.Range(0, EnemyShoot.Length)]);
    }
    public void PlayMetalImpact()
    {
        SFXEnemyAudioSource.PlayOneShot(MetalImpact[Random.Range(0, MetalImpact.Length)]);
    }
    public void PlayPlayerExplsoion() 
    {
        SFXWorldAudioSource.PlayOneShot(PlayerExplosion);
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
