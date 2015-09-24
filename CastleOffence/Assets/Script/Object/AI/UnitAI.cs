using UnityEngine;
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

    public UnitFSM      state       = UnitFSM.IDLE;
    public float        stateTime   = 0.0f;

    Dictionary<UnitFSM, Action> _dicState       = new Dictionary<UnitFSM, Action>();
    ObjectStatus                _objInfo        = null;
    Rigidbody2D                 _body           = null;
    Animator                    _anim           = null;
    GameObject                  _target         = null;
    float                       _hitDelay       = 0.5f;
    float                       _idleDelay      = 0.1f;
    float                       _moveDelay      = 0.3f;
    float                       _backDelayTime  = 0.0f;


    void OnDisable()
    {
        StopAllCoroutines();
        state = UnitFSM.IDLE;
        _backDelayTime = 0.0f;
        _target = null;
        GameManager.instance.playerObjList.Remove(gameObject);
    }
    void Start()
    {
        _objInfo = GetComponent<ObjectStatus>();
        _body = GetComponent<Rigidbody2D>();
        _anim = GetComponent<Animator>();
        
        _dicState[UnitFSM.IDLE]     = Idle;
        _dicState[UnitFSM.MOVE]     = Move;
        _dicState[UnitFSM.ATTACK]   = Attack;
        _dicState[UnitFSM.HIT]      = Hit;
        _dicState[UnitFSM.DEAD]     = Dead;
    }
    void Update()
    {
        if (_backDelayTime > 0)
            _backDelayTime -= Time.deltaTime;

        _dicState[state]();
    }


    void Idle()
    {
        stateTime += Time.deltaTime;
        if (stateTime > _idleDelay)
        {
            stateTime = 0.0f;
            if (_target && !_target.GetComponent<ObjectStatus>().IsDead())
            {
                LookEnemy();
                var dist = Vector2.Distance(_target.transform.position, transform.position);
                if (dist < _objInfo.attackRange)
                {
                    stateTime = -_backDelayTime;
                    state = UnitFSM.ATTACK;
                    StartCoroutine("AttackDelay", _backDelayTime);
                }
                else
                {
                    state = UnitFSM.MOVE;
                    _anim.SetTrigger("move");
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
        if (_target && !_target.GetComponent<ObjectStatus>().IsDead())
        {
            stateTime += Time.deltaTime;
            if (stateTime > _moveDelay)
            {
                stateTime = 0.0f;
                SearchEnemy();
                LookEnemy();
                var dist = Vector2.Distance(_target.transform.position, transform.position);
                if (dist < _objInfo.attackRange)
                {
                    stateTime = -_backDelayTime;
                    state = UnitFSM.ATTACK;
                    StartCoroutine("AttackDelay", _backDelayTime);
                    return;
                }
                if (_anim.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.walk") == false)
                {
                    _anim.SetTrigger("move");
                }
                float speed = _objInfo.moveSpeed * (float)_objInfo.dir;
                _body.velocity = new Vector2(speed, _body.velocity.y);
            }
        }
        else
        {
            stateTime = UnityEngine.Random.Range(-0.1f, 0.1f);
            state = UnitFSM.IDLE;
            _anim.SetTrigger("idle");
            _target = null;
        }
    }
    void Attack()
    {
        if (_target && !_target.GetComponent<ObjectStatus>().IsDead())
        {
            var dist = Vector2.Distance(_target.transform.position, transform.position);
            if (dist > _objInfo.attackRange)
            {
                StopCoroutine("AttackDelay");
                stateTime = 0.0f;
                state = UnitFSM.IDLE;
                _anim.SetTrigger("idle");
                return;
            }
            stateTime += Time.deltaTime;
            if (stateTime > _objInfo.attackFrontDelay)
            {
                LookEnemy();
                AttackProcess();
                _backDelayTime = _objInfo.attackBackDelay + UnityEngine.Random.Range(0.0f, 0.2f);
                stateTime = -(_backDelayTime);
                StartCoroutine("AttackDelay", _backDelayTime);
            }
        }
        else
        {
            stateTime = UnityEngine.Random.Range(-0.1f, 0.1f);
            state = UnitFSM.IDLE;
            _anim.SetTrigger("idle");
            _target = null;
        }
    }
    void Hit()
    {
        stateTime += Time.deltaTime;
        if (stateTime > _hitDelay)
        {
            stateTime = UnityEngine.Random.Range(-0.1f, 0.1f);
            state = UnitFSM.IDLE;
        }
    }
    void Dead()
    {
    }


    public void KnockBack()
    {
        _body.velocity = new Vector2(-2.0f * (float)_objInfo.dir, _body.velocity.y);
    }

    public void Attacked()
    {
        StopCoroutine("AttackDelay");
        _body.velocity = Vector2.zero;
        stateTime = 0.0f;
        state = UnitFSM.HIT;
        _anim.SetTrigger("hit");
    }
    public void Death()
    {
        StopCoroutine("AttackDelay");
        stateTime = 0.0f;
        state = UnitFSM.DEAD;
        _anim.SetTrigger("death");
    }


    void LookEnemy()
    {
        var dir = ObjectStatus.Direction.RIGHT;
        var displacement = _target.transform.position.x - transform.position.x;
        if (displacement < 0)
            dir = ObjectStatus.Direction.LEFT;

        _objInfo.ChangeDir(dir);
    }
    void AttackProcess()
    {
        if (_objInfo.attackSounds.Count > 0)
        {
            int rand = UnityEngine.Random.Range(0, _objInfo.attackSounds.Count);
            AudioManager.instance.PlaySfx(_objInfo.attackSounds[rand], 3.0f);
        }
        var targetInfo = _target.GetComponent<ObjectStatus>();
        if (targetInfo.type == ObjectStatus.ObjectType.UNIT)
        {
            var ai = _target.GetComponent<UnitAI>();
            ai.Attacked();
            ai.KnockBack();
        }
        else
        {
            var power = _objInfo.damage * 10;
            var dir = Vector3.Normalize(_target.transform.position - transform.position);
            _target.GetComponent<Rigidbody2D>().AddForce(new Vector2(dir.x * power * 2, dir.y * power));
        }
        targetInfo.Damaged(_objInfo.damage);
    }
    void SearchEnemy()
    {
        var enemyList = GameManager.instance.enemyObjList;
        if (_objInfo.owner == PlayerStatus.PlayerType.ENEMY)
            enemyList = GameManager.instance.playerObjList;

        var closeEnemyDist = float.MaxValue;
        for (int i = 0; i < enemyList.Count; ++i)
        {
            var enemy = enemyList[i];
            var dist = Vector2.Distance(enemy.transform.position, transform.position);
            if (dist < closeEnemyDist)
            {
                closeEnemyDist = dist;
                _target = enemy;
            }
        }
    }


    IEnumerator AttackDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (state == UnitFSM.ATTACK)
        {
            _anim.SetTrigger("attack");
        }
    }
}
