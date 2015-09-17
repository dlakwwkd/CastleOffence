using UnityEngine;
using System.Collections;

public class ObjectStatus : MonoBehaviour
{
    public enum ObjectType
    {
        NONE,
        BARRIER,
        TOWER,
        UNIT,
        CASTLE,
        MISSILE,
    }

    public enum Direction
    {
        LEFT = -1,
        RIGHT = 1,
    }

    public PlayerType   owner       = PlayerType.NONE;
    public ObjectType   type        = ObjectType.NONE;
    public Direction    dir         = Direction.RIGHT;
    public GameObject   hpBar       = null;
    public int          cost        = 0;
    public int          maxHp       = 0;
    public int          damage      = 0;
    public float        attackRange = 0.0f;
    public float        attackSpeed = 0.0f;
    public float        moveSpeed   = 0.0f;
    public float        createTime  = 0.0f;

    GameObject  _hpBar      = null;
    Transform   _hpGauge    = null;
    int         _curHp      = 0;
    bool        _isDead     = true;


    void Awake()
    {
        var unitSize = GetComponent<BoxCollider2D>().size / 2 + GetComponent<BoxCollider2D>().offset;

        _hpBar = Instantiate(hpBar) as GameObject;
        _hpBar.transform.SetParent(transform);
        _hpBar.transform.localPosition = new Vector3(0, unitSize.y + 0.5f, 0);
        _hpBar.name = "HpBar";
        _hpBar.SetActive(false);

        _hpGauge = _hpBar.transform.FindChild("HpGauge");
    }
    void OnEnable()
    {
        _curHp = maxHp;
        _isDead = false;

        _hpBar.SetActive(true);
    }
    void OnDisable()
    {
        _hpGauge.localScale = Vector3.one;
        _hpBar.transform.localRotation = Quaternion.identity;
        _hpBar.SetActive(false);
    }

    public bool IsDead()
    {
        return _isDead;
    }

    public void Damaged(int dam)
    {
        _curHp -= dam;

        _hpGauge.localScale = new Vector3((float)_curHp / maxHp, 1.0f, 1.0f);

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
