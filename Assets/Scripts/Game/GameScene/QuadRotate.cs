using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemFloat : MonoBehaviour
{
    [Header("玩家")]
    public Transform player;

    [Header("距离设置")]
    public float pickupRange = 15f;

    [Header("漂浮旋转设置")]
    public float floatSpeed = 2f;
    public float floatHeight = 0.3f;
    public float rotateSpeed = 90f;

    [Header("朝向玩家平滑设置")]
    public float turnSmoothSpeed = 3f;  // 转向玩家的平滑速度，越大越快
    //public float fixedRotZ = 0f;

    private Vector3 startPos;

    void Start()
    {
        startPos = transform.position;

        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) player = p.transform;
        }
    }

    void Update()
    {
        // 上下浮动
        float newY = startPos.y + Mathf.Sin(Time.time * floatSpeed) * floatHeight;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);

        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= pickupRange)
        {
            // 距离内：平滑朝向玩家
            Vector3 dir = player.position - transform.position;
            dir.y = 0;
            if (dir != Vector3.zero)
            {
                Quaternion targetRot = Quaternion.LookRotation(dir);
                // Slerp平滑插值，turnSmoothSpeed控制速度
                transform.rotation = Quaternion.Slerp(transform.rotation,targetRot,turnSmoothSpeed * Time.deltaTime);
                  //transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x,transform.rotation.eulerAngles.y,fixedRotZ);
            }
        }
        else
        {// 距离外：自动旋转
            transform.Rotate(Vector3.up, rotateSpeed * Time.deltaTime);
        }
    }
}