using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Aspect;

public class PlayerController : MonoBehaviour
{
    private float moveSpeed = 5f;
    private Vector3 moveDirection;

    private void Start()
    {
        Aspect aspect = gameObject.AddComponent<Aspect>();
        aspect.affiliation = Aspect.Affiliation.Player;
    }

    void Update()
    {
        MovePlayer();
    }

    void MovePlayer()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        moveDirection = new Vector3(moveX, 0f, moveZ).normalized;

        if (moveDirection != Vector3.zero)
        {
            transform.Translate(moveDirection * moveSpeed * Time.deltaTime, Space.World);
        }
    }
}
