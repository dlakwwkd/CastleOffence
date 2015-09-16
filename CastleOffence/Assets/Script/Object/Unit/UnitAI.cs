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
    public enum Direction
    {
        LEFT = -1,
        RIGHT = 1,
    }

    public UnitFSM  state       = UnitFSM.IDLE;
    public float    stateTime   = 0.0f;

    Dictionary<UnitFSM, Action> _dicState   = new Dictionary<UnitFSM, Action>();
    ObjectStatus                _unitInfo   = null;
    Rigidbody2D                 _body       = null;
    Animator                    _anim       = null;
    GameObject                  _target     = null;
    Direction                   _dir        = Direction.LEFT;
    float                       _hitDelay   = 0.5f;


    void Start()
    {
        _unitInfo = GetComponent<ObjectStatus>();
        _body = GetComponent<Rigidbody2D>();
        _anim = GetComponent<Animator>();
        if (_unitInfo.owner == PlayerType.PLAYER)
            _dir = Direction.RIGHT;

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
        state = UnitFSM.MOVE;
        _anim.SetBool("move", true);
    }
    void Move()
    {
        float speed = _unitInfo.moveSpeed * (float)_dir * Time.deltaTime;
        _body.velocity = new Vector2(speed, _body.velocity.y);

        if (_target)
        {
            if(_target.GetComponent<ObjectStatus>().IsDead())
            {
                _target = null;
                return;
            }

            float dist = Math.Abs(_target.transform.localPosition.x - transform.localPosition.x);
            if (dist < _unitInfo.attackRange)
            {
                state = UnitFSM.ATTACK;
                _anim.SetBool("move", false);
                _anim.SetTrigger("attack");
            }
        }
    }
    void Attack()
    {
        if(_target)
        {
            if (_target.GetComponent<ObjectStatus>().IsDead())
            {
                _target = null;
                return;
            }

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
            }
        }
        else
        {
            state = UnitFSM.IDLE;
        }
    }
    void Hit()
    {
        stateTime += Time.deltaTime;
        if (stateTime > _hitDelay)
        {
            stateTime = 0.0f;
            state = UnitFSM.IDLE;
            _anim.SetBool("move", false);
        }
    }
    void Dead()
    {
    }

    IEnumerator SearchEnemy()
    {
        while(true)
        {
            yield return new WaitForSeconds(1.0f);

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

        if (targetInfo.type == ObjectType.UNIT)
        {
            _target.GetComponent<UnitAI>().state = UnitFSM.HIT;
            _target.GetComponent<UnitAI>().stateTime = 0.0f;
            _target.GetComponent<Animator>().SetTrigger("hit");
        }
        targetInfo.Damaged(_unitInfo.damage);
    }
}
