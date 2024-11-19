using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class AnimationController : MonoBehaviour
{

    [Header("Animation Variables")]
    [SerializeField] private Animator boardAnim;
    [SerializeField] private Animator playerAnim;
    [SerializeField] private AnimationClip playerJumpClip;
    [SerializeField] private Rig playerIdle;
    [SerializeField] private Rig playerJump;

    [Header("Audio Variables")]
    [SerializeField] private PlayerAudioManager playerAudioManager;

    private bool playerIsJumping = false;
    private float jumpTimer;

    // Update is called once per frame
    void Update()
    {
        PlayerMovementAnimation();
    }

    private void PlayerMovementAnimation()
    {
        if (PlayerMovement.isMoving)
        {
            boardAnim.SetFloat("Move", 1);
        }
        else 
        {
            boardAnim.SetFloat("Move", 0);
        }

        if (PlayerMovement.isJumping)
        {
            playerIsJumping = true;
            jumpTimer = playerJumpClip.length;
            PlayerMovement.isJumping = false;
        }

        if (jumpTimer > 0)
        {
            jumpTimer -= Time.deltaTime;
            playerJump.weight = 1;
            playerIdle.weight = 0;
            if (playerIsJumping)
            {
                playerIsJumping = false;
                playerAnim.SetTrigger("isJumping");
                playerAudioManager.PlayerJumpSFX();
            }
        }
        else
        {
            playerJump.weight = 0;
            playerIdle.weight = 1;
        }
    }
}
