using UnityEngine;
using System.Collections;

public enum ObjectType
{
    NONE,
    BARRIER,
    TOWER,
    UNIT,
    CASTLE,
    MISSILE,
}

public class ObjectStatus : MonoBehaviour
{
    public PlayerType   owner       = PlayerType.NONE;
    public ObjectType   type        = ObjectType.NONE;
    public int          cost        = 0;
    public int          maxHp       = 0;
    public int          damage      = 0;
    public float        attackRange = 0.0f;
    public float        attackSpeed = 0.0f;
    public float        moveSpeed   = 0.0f;
    public float        createTime  = 0.0f;

    int     _curHp  = 0;
    bool    _isDead = true;

    void OnEnable()
    {
        _curHp = maxHp;
        _isDead = false;
    }

    public bool IsDead()
    {
        return _isDead;
    }

    public void Damaged(int dam)
    {
        _curHp -= dam;
        if(_curHp <= 0)
        {
            _isDead = true;
            Destroy();
        }
    }

    public void Destroy()
    {
        if (owner == PlayerType.PLAYER)
            GameManager.instance.playerObjList.Remove(gameObject);
        else
            GameManager.instance.enemyObjList.Remove(gameObject);

        if(type == ObjectType.CASTLE)
        {
            gameObject.SetActive(false);
        }

        owner = PlayerType.NONE;
        ObjectManager.instance.Free(gameObject);
    }
}
