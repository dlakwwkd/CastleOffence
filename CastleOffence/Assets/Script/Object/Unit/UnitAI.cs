using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;

using System.Linq;

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

    Dictionary<UnitFSM, Action> _dicState   = new Dictionary<UnitFSM, Action>();
    ObjectStatus                _unitInfo   = null;
    Rigidbody2D                 _body       = null;
    Animator                    _anim       = null;
    GameObject                  _target     = null;
    float                       _hitDelay   = 0.3f;
    float                       _idleTime   = 0.3f;


    void Start()
    {
        _unitInfo = GetComponent<ObjectStatus>();
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
        _dicState[state]();
    }
    void OnEnable()
    {
        StartCoroutine("SearchEnemy");
    }
    void OnDisable()
    {
        state = UnitFSM.IDLE;
        GameManager.instance.playerObjList.Remove(gameObject);

        StopCoroutine("SearchEnemy");
    }

    void Idle()
    {
        stateTime += Time.deltaTime;
        if (stateTime > _idleTime)
        {
            stateTime = 0.0f;
            state = UnitFSM.MOVE;
            _anim.SetTrigger("move");
        }
    }
    void Move()
    {
        if (_target && !_target.GetComponent<ObjectStatus>().IsDead())
        {
            float dist = Math.Abs(_target.transform.localPosition.x - transform.localPosition.x);
            if (dist < _unitInfo.attackRange)
            {
                state = UnitFSM.ATTACK;
                _anim.SetTrigger("attack");
                return;
            }
        }
        else
            _target = null;

        float speed = _unitInfo.moveSpeed * (float)_unitInfo.dir * Time.deltaTime;
        _body.velocity = new Vector2(speed, _body.velocity.y);
    }
    void Attack()
    {
        if (_target && !_target.GetComponent<ObjectStatus>().IsDead())
        {
            stateTime += Time.deltaTime;
            if (stateTime > _unitInfo.attackSpeed)
            {
                stateTime = 0.0f;
                _anim.SetTrigger("attack");
                AttackProcess();
            }
            float dist = Math.Abs(_target.transform.localPosition.x - transform.localPosition.x);
            if (dist > _unitInfo.attackRange)
            {
                state = UnitFSM.IDLE;
                stateTime = 0.0f;
            }
        }
        else
        {
            _target = null;
            state = UnitFSM.IDLE;
            stateTime = 0.0f;
        }
    }
    void Hit()
    {
        stateTime += Time.deltaTime;
        if (stateTime > _hitDelay)
        {
            stateTime = 0.0f;
            state = UnitFSM.IDLE;
        }
    }
    void Dead()
    {
    }

    IEnumerator SearchEnemy()
    {
        while(true)
        {
            yield return new WaitForSeconds(0.5f);

            var enemyList = GameManager.instance.enemyObjList;
            if (_unitInfo.owner == PlayerType.ENEMY)
                enemyList = GameManager.instance.playerObjList;

            float unitPos = transform.localPosition.x;
            float closeEnemyDist = float.MaxValue;

            for (int i = 0; i < enemyList.Count; ++i)
            {
                float dist = Math.Abs(enemyList[i].transform.localPosition.x - unitPos);
                if (closeEnemyDist > dist)
                {
                    closeEnemyDist = dist;
                    _target = enemyList[i];
                }
            }
//             var q = enemyList
//                 .Where(m =>
//                 {
//                     return m.transform.localPosition.x > unitPos;
//                 })
//                 .OrderBy(m => m.transform.localPosition.x)
//                 .First();
        }
    }

    void AttackProcess()
    {
        var targetInfo = _target.GetComponent<ObjectStatus>();

        if (targetInfo.type == ObjectStatus.ObjectType.UNIT)
        {
            var ai = _target.GetComponent<UnitAI>();
            ai.state = UnitFSM.HIT;
            ai.stateTime = 0.0f;
            ai.KnockBack();
            _target.GetComponent<Animator>().SetTrigger("hit");
        }
        targetInfo.Damaged(_unitInfo.damage);
    }

    public void KnockBack()
    {
        _body.velocity = new Vector2(-2.0f * (float)_unitInfo.dir, _body.velocity.y);
    }
}
