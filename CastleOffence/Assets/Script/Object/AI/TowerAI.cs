using UnityEngine;
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

    public GameObject   missileObj  = null;
    public TowerFSM     state       = TowerFSM.IDLE;
    public float        stateTime   = 0.0f;

    Dictionary<TowerFSM, Action>    _dicState   = new Dictionary<TowerFSM, Action>();
    ObjectStatus                    _objInfo    = null;
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
        var missile = ObjectManager.instance.Assign(missileObj.name);
        missile.transform.localPosition = transform.localPosition + Vector3.up * GetComponent<BoxCollider2D>().size.y;

        var info = missile.GetComponent<ObjectStatus>();
        info.owner = _objInfo.owner;
        info.damage = _objInfo.damage;

        var body = missile.GetComponent<Rigidbody2D>();
        var displacement = _target.transform.localPosition - transform.localPosition;
        var fireForce = new Vector2(displacement.x * 25.0f, displacement.y * 25.0f + 500.0f);
        body.AddForce(fireForce);
    }

    public void Death()
    {
        state = TowerFSM.DEAD;
        //_anim.SetTrigger("death");
    }
}
