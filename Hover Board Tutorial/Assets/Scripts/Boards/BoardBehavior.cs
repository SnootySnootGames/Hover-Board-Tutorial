using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardBehavior : MonoBehaviour
{
    //store raycast origins, use to calculate angle of board for animation
    [Header("Player Variables")]
    [SerializeField] private Transform board;

    [Header("Raycast Variables")]
    [SerializeField] private LayerMask groundLayerMask;
    [SerializeField] private float castDistance;
    [SerializeField] private Transform centerOrigin;

    [Header("Player Rotation Variables")]
    [SerializeField] private float rotationSpeed;

    private RaycastHit hit;

    private void FixedUpdate()
    {
        rayCastMethod();   
    }

    private void rayCastMethod()
    {
        Physics.Raycast(centerOrigin.position, Vector3.down, out hit, castDistance, groundLayerMask);
        if (hit.collider != null)
        {
            Quaternion targetRotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
            board.rotation = Quaternion.Slerp(board.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }
}
