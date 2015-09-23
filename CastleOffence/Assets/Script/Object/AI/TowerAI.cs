using UnityEngine;
using System.Collections;
using System.Collections.Generic;
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
            LookEnemy();
            var dist = Vector2.Distance(_target.transform.position, transform.position);
            if (dist < _objInfo.attackRange)
            {
                state = TowerFSM.ATTACK;
            }
        }
        else
            _target = null;
    }
    void Attack()
    {
        if (_target && !_target.GetComponent<ObjectStatus>().IsDead())
        {
            var dist = Vector2.Distance(_target.transform.position, transform.position);
            if (dist > _objInfo.attackRange)
            {
                state = TowerFSM.IDLE;
                return;
            }
            stateTime += Time.deltaTime;
            if (stateTime > _objInfo.attackFrontDelay)
            {
                LookEnemy();
                AttackProcess();
                stateTime = -(_objInfo.attackBackDelay + UnityEngine.Random.Range(0.0f, 0.2f));
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


    IEnumerator SearchEnemy()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.2f);

            var enemyList = GameManager.instance.enemyObjList;
            if (_objInfo.owner == PlayerStatus.PlayerType.ENEMY)
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


    void LookEnemy()
    {
        if (_objInfo.type == ObjectStatus.ObjectType.CASTLE)
            return;

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
            AudioManager.instance.PlaySfx(_objInfo.attackSounds[rand], 1.5f);
        }
        var pivotGap = 2.0f;
        if (_objInfo.type == ObjectStatus.ObjectType.CASTLE)
            pivotGap += 3.0f;

        var missile = ObjectManager.instance.Assign(missileObj.name);
        missile.transform.localPosition = transform.localPosition + Vector3.up * pivotGap;

        var info = missile.GetComponent<ObjectStatus>();
        info.owner = _objInfo.owner;
        info.damage = _objInfo.damage;

        var forceRatio = 25.0f;
        var forceGap = UnityEngine.Random.Range(450.0f, 550.0f) - pivotGap * forceRatio;
        var displacement = _target.transform.localPosition - transform.localPosition;

        var fireForce = new Vector2(displacement.x * forceRatio, displacement.y * forceRatio + forceGap);
        var body = missile.GetComponent<Rigidbody2D>();
        body.AddForce(fireForce);

//         var dirVector = fireForce.normalized;
//         var angle = Vector2.Angle(Vector2.right, dirVector);
//         if (dirVector.y < 0)
//             angle = -angle;
//         missile.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, angle));
    }

    public void Death()
    {
        state = TowerFSM.DEAD;
    }
}
