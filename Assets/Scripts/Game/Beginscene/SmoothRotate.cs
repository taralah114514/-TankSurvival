using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SmoothRotate : MonoBehaviour
{
        [Header("旋转设置")]
        [Tooltip("最大旋转角度（总范围，比如90=左右各45度）")]
        public float maxAngle = 90f;

        [Tooltip("旋转速度")]
        public float rotateSpeed = 50f;

        // 旋转轴心（默认Y轴 = 左右旋转，可改成X/Z轴）
        public Vector3 rotateAxis = Vector3.up;

        void Update()
        {
            // 1. 生成往返运动曲线 (0~1之间平滑往返)
            // PingPong：基础往返；SmoothStep：越到边界越慢（核心缓动效果）
            float pingPongValue = Mathf.PingPong(Time.time * rotateSpeed, 1f);
            float smoothValue = Mathf.SmoothStep(0f, 1f, pingPongValue);

            // 2. 计算当前旋转角度（-maxAngle/2 到 maxAngle/2 之间往返）
            float currentAngle = Mathf.Lerp(-maxAngle / 2, maxAngle / 2, smoothValue);

            // 3. 应用旋转到物体
            transform.localEulerAngles = rotateAxis * currentAngle;
        }
}


