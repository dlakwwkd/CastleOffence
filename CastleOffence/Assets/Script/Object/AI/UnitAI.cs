using UnityEngine;
using UnityEngine.Serialization;
using System.Collections;
using System.Collections.Generic;
using System;
//using System.Linq;

public class UnitAI : MonoBehaviour
{
    public enum UnitFSM
    {
        IDLE,
        MOVE,
        ATTACK,
        HIT,
        DEAD,
    }

    //-----------------------------------------------------------------------------------
    // inspector field
    [FormerlySerializedAs("state")]
    public UnitFSM      State       = UnitFSM.IDLE;
    [FormerlySerializedAs("stateTime")]
    public float        StateTime   = 0.0f;

    //-----------------------------------------------------------------------------------
    // handler functions
    void OnDisable()
    {
        StopAllCoroutines();
        State = UnitFSM.IDLE;
        backDelayTime = 0.0f;
        target = null;
        GameManager.instance.playerObjList.Remove(gameObject);
    }

    void Start()
    {
        objInfo = GetComponent<ObjectStatus>();
        body = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        
        dicState[UnitFSM.IDLE]     = Idle;
        dicState[UnitFSM.MOVE]     = Move;
        dicState[UnitFSM.ATTACK]   = Attack;
        dicState[UnitFSM.HIT]      = Hit;
        dicState[UnitFSM.DEAD]     = Dead;
    }

    void Update()
    {
        if (backDelayTime > 0)
            backDelayTime -= Time.deltaTime;

        dicState[State]();
    }

    void Idle()
    {
        StateTime += Time.deltaTime;
        if (StateTime > idleDelay)
        {
            StateTime = 0.0f;
            if (target && !target.GetComponent<ObjectStatus>().IsDead())
            {
                LookEnemy();
                var dist = Vector2.Distance(target.transform.position, transform.position);
                if (dist < objInfo.AttackRange)
                {
                    StateTime = -backDelayTime;
                    State = UnitFSM.ATTACK;
                    StartCoroutine("AttackDelay", backDelayTime);
                }
                else
                {
                    State = UnitFSM.MOVE;
                    anim.SetTrigger("move");
                }
            }
            else
            {
                SearchEnemy();
            }
        }
    }

    void Move()
    {
        if (target && !target.GetComponent<ObjectStatus>().IsDead())
        {
            StateTime += Time.deltaTime;
            if (StateTime > moveDelay)
            {
                StateTime = 0.0f;
                SearchEnemy();
                LookEnemy();
                var dist = Vector2.Distance(target.transform.position, transform.position);
                if (dist < objInfo.AttackRange)
                {
                    StateTime = -backDelayTime;
                    State = UnitFSM.ATTACK;
                    StartCoroutine("AttackDelay", backDelayTime);
                    return;
                }
                if (anim.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.walk") == false)
                {
                    anim.SetTrigger("move");
                }
                float speed = objInfo.MoveSpeed * (float)objInfo.Dir;
                body.velocity = new Vector2(speed, body.velocity.y);
            }
        }
        else
        {
            StateTime = UnityEngine.Random.Range(-0.1f, 0.1f);
            State = UnitFSM.IDLE;
            anim.SetTrigger("idle");
            target = null;
        }
    }

    void Attack()
    {
        if (target && !target.GetComponent<ObjectStatus>().IsDead())
        {
            var dist = Vector2.Distance(target.transform.position, transform.position);
            if (dist > objInfo.AttackRange)
            {
                StopCoroutine("AttackDelay");
                StateTime = 0.0f;
                State = UnitFSM.IDLE;
                anim.SetTrigger("idle");
                return;
            }
            StateTime += Time.deltaTime;
            if (StateTime > objInfo.AttackFrontDelay)
            {
                LookEnemy();
                AttackProcess();
                backDelayTime = objInfo.AttackBackDelay + UnityEngine.Random.Range(0.0f, 0.2f);
                StateTime = -(backDelayTime);
                StartCoroutine("AttackDelay", backDelayTime);
            }
        }
        else
        {
            StateTime = UnityEngine.Random.Range(-0.1f, 0.1f);
            State = UnitFSM.IDLE;
            anim.SetTrigger("idle");
            target = null;
        }
    }

    void Hit()
    {
        StateTime += Time.deltaTime;
        if (StateTime > hitDelay)
        {
            StateTime = UnityEngine.Random.Range(-0.1f, 0.1f);
            State = UnitFSM.IDLE;
        }
    }

    void Dead()
    {
    }

    //-----------------------------------------------------------------------------------
    // public functions
    public void KnockBack()
    {
        body.velocity = new Vector2(-2.0f * (float)objInfo.Dir, body.velocity.y);
    }

    public void Attacked()
    {
        StopCoroutine("AttackDelay");
        body.velocity = Vector2.zero;
        StateTime = 0.0f;
        State = UnitFSM.HIT;
        anim.SetTrigger("hit");
    }

    public void Death()
    {
        StopCoroutine("AttackDelay");
        StateTime = 0.0f;
        State = UnitFSM.DEAD;
        anim.SetTrigger("death");
    }

    //-----------------------------------------------------------------------------------
    // private functions
    void LookEnemy()
    {
        var dir = ObjectStatus.Direction.RIGHT;
        var displacement = target.transform.position.x - transform.position.x;
        if (displacement < 0)
            dir = ObjectStatus.Direction.LEFT;

        objInfo.ChangeDir(dir);
    }

    void AttackProcess()
    {
        if (objInfo.AttackSounds.Count > 0)
        {
            int rand = UnityEngine.Random.Range(0, objInfo.AttackSounds.Count);
            AudioManager.instance.PlaySfx(objInfo.AttackSounds[rand], 3.0f);
        }
        var targetInfo = target.GetComponent<ObjectStatus>();
        if (targetInfo.Type == ObjectStatus.ObjectType.UNIT)
        {
            var ai = target.GetComponent<UnitAI>();
            ai.Attacked();
            ai.KnockBack();
        }
        else
        {
            var power = objInfo.Damage * 10;
            var dir = Vector3.Normalize(target.transform.position - transform.position);
            target.GetComponent<Rigidbody2D>().AddForce(new Vector2(dir.x * power * 2, dir.y * power));
        }
        targetInfo.Damaged(objInfo.Damage);
    }

    void SearchEnemy()
    {
        var enemyList = GameManager.instance.enemyObjList;
        if (objInfo.Owner == PlayerStatus.PlayerType.ENEMY)
            enemyList = GameManager.instance.playerObjList;

        var closeEnemyDist = float.MaxValue;
        for (int i = 0; i < enemyList.Count; ++i)
        {
            var enemy = enemyList[i];
            var dist = Vector2.Distance(enemy.transform.position, transform.position);
            if (dist < closeEnemyDist)
            {
                closeEnemyDist = dist;
                target = enemy;
            }
        }
    }

    //-----------------------------------------------------------------------------------
    // coroutine functions
    IEnumerator AttackDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (State == UnitFSM.ATTACK)
        {
            anim.SetTrigger("attack");
        }
    }

    //-----------------------------------------------------------------------------------
    // private field
    Dictionary<UnitFSM, Action> dicState        = new Dictionary<UnitFSM, Action>();
    ObjectStatus                objInfo         = null;
    Rigidbody2D                 body            = null;
    Animator                    anim            = null;
    GameObject                  target          = null;
    float                       hitDelay        = 0.5f;
    float                       idleDelay       = 0.1f;
    float                       moveDelay       = 0.3f;
    float                       backDelayTime   = 0.0f;
}
