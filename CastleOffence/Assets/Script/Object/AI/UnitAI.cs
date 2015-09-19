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
    ObjectStatus                _objInfo    = null;
    Rigidbody2D                 _body       = null;
    Animator                    _anim       = null;
    GameObject                  _target     = null;
    float                       _hitDelay   = 0.5f;


    void OnEnable()
    {
        StartCoroutine("SearchEnemy");
    }
    void OnDisable()
    {
        StopAllCoroutines();
        state = UnitFSM.IDLE;
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
        _dicState[state]();
    }


    void Idle()
    {
        if (_target && !_target.GetComponent<ObjectStatus>().IsDead())
        {
            LookEnemyAndCalcPos();
            state = UnitFSM.MOVE;
            _anim.SetTrigger("move");
        }
        else
            _target = null;
    }
    void Move()
    {
        if (_target && !_target.GetComponent<ObjectStatus>().IsDead())
        {
            var dist = Math.Abs(LookEnemyAndCalcPos());
            if (dist < _objInfo.attackRange)
            {
                stateTime = 0.0f;
                state = UnitFSM.ATTACK;
                _anim.SetTrigger("attack");
                return;
            }
        }
        else
            _target = null;

        float speed = _objInfo.moveSpeed * (float)_objInfo.dir * Time.deltaTime;
        _body.velocity = new Vector2(speed, _body.velocity.y);
    }
    void Attack()
    {
        if (_target && !_target.GetComponent<ObjectStatus>().IsDead())
        {
            var dist = Math.Abs(LookEnemyAndCalcPos());
            if (dist > _objInfo.attackRange)
            {
                state = UnitFSM.IDLE;
                return;
            }
            stateTime += Time.deltaTime;
            if (stateTime > _objInfo.attackFrontDelay)
            {
                AttackProcess();
                StartCoroutine("AttackDelay");
                stateTime = -(_objInfo.attackBackDelay);
            }
        }
        else
        {
            state = UnitFSM.IDLE;
            _target = null;
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



    IEnumerator AttackDelay()
    {
        yield return new WaitForSeconds(_objInfo.attackBackDelay);

        if (state == UnitFSM.ATTACK)
            _anim.SetTrigger("attack");
    }
    IEnumerator SearchEnemy()
    {
        while(true)
        {
            yield return new WaitForSeconds(0.1f);

            var enemyList = GameManager.instance.enemyObjList;
            if (_objInfo.owner == PlayerType.ENEMY)
                enemyList = GameManager.instance.playerObjList;

            var unitPos = transform.localPosition.x;
            var closeEnemyDist = float.MaxValue;

            for (int i = 0; i < enemyList.Count; ++i)
            {
                var dist = Math.Abs(enemyList[i].transform.localPosition.x - unitPos);
                if (dist < closeEnemyDist)
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


    float LookEnemyAndCalcPos()
    {
        var dir = ObjectStatus.Direction.RIGHT;
        var displacement = _target.transform.localPosition.x - transform.localPosition.x;
        if (displacement < 0)
            dir = ObjectStatus.Direction.LEFT;

        _objInfo.ChangeDir(dir);
        return displacement;
    }
    void AttackProcess()
    {
        var targetInfo = _target.GetComponent<ObjectStatus>();
        if (targetInfo.type == ObjectStatus.ObjectType.UNIT)
        {
            var ai = _target.GetComponent<UnitAI>();
            ai.Attacked();
            ai.KnockBack();
        }
        targetInfo.Damaged(_objInfo.damage);
    }

    public void KnockBack()
    {
        _body.velocity = new Vector2(-2.0f * (float)_objInfo.dir, _body.velocity.y);
    }

    public void Attacked()
    {
        _body.velocity = Vector2.zero;
        stateTime = 0.0f;
        state = UnitFSM.HIT;
        _anim.SetTrigger("hit");
    }
    public void Death()
    {
        state = UnitFSM.DEAD;
        _anim.SetTrigger("death");
    }
}
