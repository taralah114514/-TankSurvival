using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticlesController : MonoBehaviour
{
    public ParticleSystem smokeParticles;  // 拖入你的烟雾粒子
    public float stopAfterSeconds = 3f;    // 几秒后停止生成

    private float timer = 0f;
    private bool isStopped = false;

    void Start()
    {
        if (smokeParticles == null)
            smokeParticles = GetComponent<ParticleSystem>();
    }

    void Update()
    {
        if (!isStopped)
        {
            timer += Time.deltaTime;
            if (timer >= stopAfterSeconds)
            {
                // 停止发射新粒子，但已有的继续消散
                var emission = smokeParticles.emission;
                emission.enabled = false;
                isStopped = true;
            }
        }
    }
}