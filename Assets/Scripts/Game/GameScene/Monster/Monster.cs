using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : TankBase
{
    [Header("AI行为设置")]
    public float detectionRange = 50f;      // 侦测玩家的范围
    public float stopDistance = 20f;        // 停止距离（开始攻击的距离）
    public float patrolRange = 30f;         // 巡逻范围
    public float dangerDistance = 15f;      // 危险距离（需要闪避的范围）

    [Header("移动设置")]
    public float moveStopTime = 2f;
    public float moveForwardTime = 3f;
    public float maxTurnSpeed = 90f;  // 每秒最大转向角度（度）← 新增

    private Quaternion currentBodyRotation;  // 当前身体旋转缓存

    [Header("攻击设置")]
    public float shootInterval = 2f;        // 射击间隔
    public float bodyRotateSpeed = 50f;     // 身体转向速度
    public float headRotateSpeed = 80f;     // 炮塔转向速度
    public float shootRange = 60f;          // 射击范围
    public float shootAngleTolerance = 30f; // 射击角度容差

    public float dodgeDistance = 10f;       // 闪避距离
    public float dodgeCooldown = 3f;        // 闪避冷却时间

    [Header("武器")]
    public EnemyWeapon weapon;

    private PlayerObject player;
    private Vector3 patrolTarget;
    private float shootTimer = 0f;
    private bool canDetectPlayer = false;
    private float dodgeTimer = 0f;
    private int healthThreshold = 30;
    public MonsterSpawner spawner;
    private MonsterHealthBar healthBar;
    enum AIState
    {
        Patrol,
        Chase,
        Attack,
        Dodge,
        Dead
    }
    private AIState currentState = AIState.Patrol;

    private void Start()
    {
        healthBar = GetComponentInChildren<MonsterHealthBar>();
        if (healthBar != null)
            healthBar.UpdateHP(HP, MaxHP);
        player = FindObjectOfType<PlayerObject>();
        GeneratePatrolTarget();
        shootTimer = shootInterval;
        dodgeTimer = dodgeCooldown;
        currentBodyRotation = this.transform.rotation;  // 初始化
        // 确保TankHead是子物体
        if (TankHead != null && TankHead.parent != this.transform)
        {
            TankHead.parent = this.transform;
        }
    }

    private void Update()
    {
        if (currentState == AIState.Dead)
            return;

        DetectPlayer();
        UpdateAIState();
        UpdateTankHeadDirection();

        switch (currentState)
        {
            case AIState.Patrol:
                PatrolBehavior();
                break;
            case AIState.Chase:
                ChaseBehavior();
                break;
            case AIState.Attack:
                AttackBehavior();
                break;
            case AIState.Dodge:
                DodgeBehavior();
                break;
        }

        shootTimer -= Time.deltaTime;
        dodgeTimer -= Time.deltaTime;
    }

    /// <summary>
    /// 每帧更新炮塔方向指向玩家
    /// </summary>
    private void SmoothRotateBody(Vector3 targetDirection)
    {
        if (targetDirection.magnitude < 0.01f) return;
        targetDirection.y = 0;
        targetDirection.Normalize();

        Quaternion targetRot = Quaternion.LookRotation(targetDirection);

        this.transform.rotation = Quaternion.RotateTowards(
            this.transform.rotation,
            targetRot,
            maxTurnSpeed * Time.deltaTime  // 每帧最多转maxTurnSpeed度
        );
    }
    private void DetectPlayer()
    {
        if (player == null)
            return;

        float distanceToPlayer = Vector3.Distance(this.transform.position, player.transform.position);
        canDetectPlayer = distanceToPlayer <= detectionRange;
    }

    private void UpdateAIState()
    {
        if (!canDetectPlayer)
        {
            currentState = AIState.Patrol;
            return;
        }
        if (player != null)
        {
            float distanceToPlayer = Vector3.Distance(this.transform.position, player.transform.position);
            float healthPercent = (float)HP / MaxHP;

            if (healthPercent < healthThreshold * 0.01f && distanceToPlayer < dodgeDistance && dodgeTimer <= 0f)
            {
                currentState = AIState.Dodge;
                dodgeTimer = dodgeCooldown;
                return;
            }

            if (distanceToPlayer <= shootRange)
            {
                currentState = AIState.Attack;
            }
            else if (distanceToPlayer <= detectionRange)
            {
                currentState = AIState.Chase;
            }
            else
            {
                currentState = AIState.Patrol;
            }
        }
    }

    private void PatrolBehavior()
    {
        Vector3 directionToPatrol = (patrolTarget - this.transform.position).normalized;
        float distanceToPatrol = Vector3.Distance(this.transform.position, patrolTarget);

        if (distanceToPatrol > 1f)
        {
            SmoothRotateBody(directionToPatrol);  // 替换原来的Slerp

            // 只有基本对齐才前进，避免还没转过来就冲出去
            float alignment = Vector3.Dot(this.transform.forward, directionToPatrol);
            if (alignment > 0.5f)
                this.transform.Translate(Vector3.forward * MoveSpeed * Time.deltaTime);
        }
        else
        {
            GeneratePatrolTarget();
        }
    }

    private void ChaseBehavior()
    {
        if (player == null) return;

        float distanceToPlayer = Vector3.Distance(this.transform.position, player.transform.position);
        if (distanceToPlayer < 3f) return;

        Vector3 directionToPlayer = (player.transform.position - this.transform.position).normalized;
        SmoothRotateBody(directionToPlayer);  // 替换

        float alignmentFactor = Vector3.Dot(this.transform.forward, directionToPlayer);
        if (alignmentFactor > 0.7f)
            this.transform.Translate(Vector3.forward * MoveSpeed * Time.deltaTime);
    }

    private void AttackBehavior()
    {
        if (player == null || TankHead == null) return;

        float distanceToPlayer = Vector3.Distance(this.transform.position, player.transform.position);

        if (distanceToPlayer < shootRange * 0.3f)
        {
            this.transform.Translate(-Vector3.forward * MoveSpeed * 0.5f * Time.deltaTime);
            return;
        }

        Vector3 directionToPlayer = (player.transform.position - TankHead.position).normalized;
        directionToPlayer.y = 0;
        directionToPlayer.Normalize();

        Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
        float angleDifference = Quaternion.Angle(TankHead.rotation, targetRotation);

        if (shootTimer <= 0f && weapon != null && weapon.GetCurrentAmmo() > 0)
        {
            if (angleDifference < shootAngleTolerance)
            {
                weapon.Fire();
                shootTimer = shootInterval;
            }
        }
    }

    private void DodgeBehavior()
    {
        if (player == null) return;

        Vector3 directionAway = (this.transform.position - player.transform.position).normalized;
        SmoothRotateBody(directionAway);  // 替换

        float alignmentFactor = Vector3.Dot(this.transform.forward, directionAway);
        if (alignmentFactor > 0.8f)
            this.transform.Translate(Vector3.forward * MoveSpeed * 1.5f * Time.deltaTime);

        float distanceToPlayer = Vector3.Distance(this.transform.position, player.transform.position);
        if (distanceToPlayer > dangerDistance * 1.5f)
            currentState = AIState.Attack;
    }

    private void GeneratePatrolTarget()
    {
        Vector3 randomDirection = Random.insideUnitSphere * patrolRange;
        randomDirection.y = this.transform.position.y;
        patrolTarget = this.transform.position + randomDirection;
    }
    private void UpdateTankHeadDirection()
    {
        if (player == null || TankHead == null)
            return;

        float distanceToPlayer = Vector3.Distance(TankHead.position, player.transform.position);

        // 太近了不更新方向，避免疯狂旋转
        if (distanceToPlayer < 2f) return;

        Vector3 directionToPlayer = (player.transform.position - TankHead.position).normalized;
        directionToPlayer.y = 0;
        if (directionToPlayer.magnitude < 0.01f) return;
        directionToPlayer.Normalize();

        Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
        TankHead.rotation = Quaternion.Slerp(
            TankHead.rotation,
            targetRotation,
            headRotateSpeed * Time.deltaTime
        );
    }
    public override void Fire()
    {
        if (weapon != null)
        {
            weapon.Fire();
        }
    }

    public override void Wound(TankBase other)
    {
        base.Wound(other);
        if (healthBar != null)
            healthBar.OnHit(HP, MaxHP);  // 用OnHit触发显示计时器
    }

  
    public GameObject deadBodyPrefab;  

    public override void Dead()
    {
        if (deadBodyPrefab != null)
            Instantiate(deadBodyPrefab, transform.position, transform.rotation);
        if (spawner != null) spawner.OnMonsterDead();
       
        base.Dead();
        
    }
}