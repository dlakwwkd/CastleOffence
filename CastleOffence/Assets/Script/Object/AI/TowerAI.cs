﻿using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;

public class TowerAI : MonoBehaviour
{
    public enum TowerFSM
    {
        IDLE,
        ATTACK,
        DEAD,
    }

    public GameObject   missile     = null;
    public TowerFSM     state       = TowerFSM.IDLE;
    public float        stateTime   = 0.0f;

    Dictionary<TowerFSM, Action>    _dicState   = new Dictionary<TowerFSM, Action>();
    ObjectStatus                    _objInfo    = null;
    Rigidbody2D                     _body       = null;
    Animator                        _anim       = null;
    GameObject                      _target     = null;


    void OnEnable()
    {
        StartCoroutine("SearchEnemy");
    }
    void OnDisable()
    {
        StopAllCoroutines();
        state = TowerFSM.IDLE;
        GameManager.instance.playerObjList.Remove(gameObject);
    }
    void Start()
    {
        _objInfo = GetComponent<ObjectStatus>();
        _body = GetComponent<Rigidbody2D>();
        _anim = GetComponent<Animator>();

        _dicState[TowerFSM.IDLE]    = Idle;
        _dicState[TowerFSM.ATTACK]  = Attack;
        _dicState[TowerFSM.DEAD]    = Dead;
    }
    void Update()
    {
        _dicState[state]();
    }


    void Idle()
    {
        if (_target && !_target.GetComponent<ObjectStatus>().IsDead())
        {
            var dist = Math.Abs(LookEnemyAndCalcPos());
            if (dist < _objInfo.attackRange)
            {
                stateTime = 0.0f;
                state = TowerFSM.ATTACK;
                //_anim.SetTrigger("attack");
            }
        }
        else
            _target = null;
    }
    void Attack()
    {
        if (_target && !_target.GetComponent<ObjectStatus>().IsDead())
        {
            var dist = Math.Abs(LookEnemyAndCalcPos());
            if (dist > _objInfo.attackRange)
            {
                state = TowerFSM.IDLE;
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
            state = TowerFSM.IDLE;
            _target = null;
        }
    }
    void Dead()
    {
    }



    IEnumerator AttackDelay()
    {
        yield return new WaitForSeconds(_objInfo.attackBackDelay);

        if (state == TowerFSM.ATTACK) ;
            //_anim.SetTrigger("attack");
    }
    IEnumerator SearchEnemy()
    {
        while (true)
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
        var m = ObjectManager.instance.Assign(missile.name);
        m.GetComponent<ObjectStatus>().owner = _objInfo.owner;
        m.transform.localPosition = transform.localPosition;

        var displacement = _target.transform.localPosition - transform.localPosition;
        var fireForce = new Vector2(displacement.x * 25.0f, displacement.x * 15.0f + displacement.y * 15.0f + 350.0f);
        m.GetComponent<Rigidbody2D>().AddForce(fireForce);
    }

    public void Death()
    {
        state = TowerFSM.DEAD;
        //_anim.SetTrigger("death");
    }
}
