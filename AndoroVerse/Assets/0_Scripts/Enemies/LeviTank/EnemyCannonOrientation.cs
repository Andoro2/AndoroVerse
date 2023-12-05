using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCannonOrientation : MonoBehaviour
{
    public Transform m_Player;
    public float m_RotSpeed = 5f;
    public float minDistanceToUpdate = 5f;

    private Transform cannonTransform;

    private void Start()
    {
        m_Player = GameObject.Find("Player").transform;
        cannonTransform = transform;
    }

    private void Update()
    {
        Vector3 PlayerDir = m_Player.position - cannonTransform.position;

        float PlayerDist = PlayerDir.magnitude;

        if (PlayerDist > minDistanceToUpdate)
        {
            Quaternion PlayerRot = Quaternion.LookRotation(PlayerDir);
            cannonTransform.rotation = Quaternion.Slerp(cannonTransform.rotation, PlayerRot, m_RotSpeed * Time.deltaTime);

            cannonTransform.localEulerAngles = new Vector3(-90f, cannonTransform.localEulerAngles.y, 0f);
        }
    }
}