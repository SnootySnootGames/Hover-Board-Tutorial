using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource hoverBoardAS;
    [SerializeField] private AudioClip[] hoverBoardAC; //0 = jump, 1 = landing


    public void PlayerJumpSFX()
    {
        hoverBoardAS.PlayOneShot(hoverBoardAC[0], 0.75F);
    }

    public void PlayerLandJumpSFX()
    {
        hoverBoardAS.PlayOneShot(hoverBoardAC[1], 0.5F);
    }
}
