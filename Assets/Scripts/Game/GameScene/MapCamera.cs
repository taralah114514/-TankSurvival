using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapCamera : MonoBehaviour
{
    public GameObject player;

    private void Update()
    {
        // 获取当前位置
        Vector3 newPosition = transform.position;

        // 只修改 X 和 Z 轴
        if (player != null)
        {
            newPosition.x = player.transform.position.x;
            newPosition.z = player.transform.position.z;
        }

        // 应用新位置
        transform.position = newPosition;
    }
}
