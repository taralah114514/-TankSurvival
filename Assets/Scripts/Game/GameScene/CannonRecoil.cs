using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class CannonRecoil : MonoBehaviour
{//不用了
    [Header("炮管设置")]
    public Transform cannonPivot;      // 拖入CannonPivot空物体
    public float recoilAngle = 15f;    // 上翘角度
    public float recoilSpeed = 60f;    // 上翘速度
    public float returnSpeed = 20f;     // 复位速度

    private float currentAngle = 0f;
    private bool isRecoiling = false;
    public bool CanFire => currentAngle == 0f && !isRecoiling;

    void Update()
    {
        if (isRecoiling)
        {
            // 快速上翘
            currentAngle = Mathf.MoveTowards(currentAngle, recoilAngle, recoilSpeed * Time.deltaTime);
            if (currentAngle >= recoilAngle) isRecoiling = false;
        }
        else
        {
            // 缓慢复位
            currentAngle = Mathf.MoveTowards(currentAngle, 0f, returnSpeed * Time.deltaTime);
        }

        cannonPivot.localRotation = Quaternion.Euler(-currentAngle, 0, 0);
    }

    // 开炮时调用这个方法
    public void Fire()
    {
        if (!CanFire) return;
        isRecoiling = true;
        currentAngle = 0f;
    }
}