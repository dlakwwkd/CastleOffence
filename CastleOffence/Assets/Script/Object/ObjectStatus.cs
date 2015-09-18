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

    public PlayerType   owner               = PlayerType.NONE;
    public ObjectType   type                = ObjectType.NONE;
    public Direction    dir                 = Direction.RIGHT;
    public GameObject   hpBar               = null;
    public int          cost                = 0;
    public int          maxHp               = 0;
    public int          damage              = 0;
    public float        attackRange         = 0.0f;
    public float        attackFrontDelay    = 0.0f;
    public float        attackBackDelay     = 0.0f;
    public float        moveSpeed           = 0.0f;
    public float        createTime          = 0.0f;

    GameObject  _hpBar      = null;
    Transform   _hpGauge    = null;
    int         _curHp      = 0;
    bool        _isDead     = true;


    void Awake()
    {
        var collider = GetComponent<BoxCollider2D>();
        var unitSize = collider.size / 2 + collider.offset;

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
    }

    public bool IsDead()
    {
        return _isDead;
    }

    public void ChangeDir(Direction d)
    {
        if(dir != d)
        {
            dir = d;
            if (dir == Direction.LEFT)
                transform.localRotation = Quaternion.Euler(new Vector3(0, 180.0f, 0));
            else
                transform.localRotation = Quaternion.identity;

            _hpBar.transform.localRotation = transform.localRotation;
        }
    }

    public void Damaged(int dam)
    {
        _curHp -= dam;

        var hpRatio = (float)_curHp / maxHp;
        _hpGauge.localScale = new Vector3(hpRatio, 1.0f, 1.0f);
        _hpGauge.GetComponent<SpriteRenderer>().color = new Color(1.0f - hpRatio, hpRatio, 0);

        if(_curHp <= 0)
        {
            _isDead = true;
            if (type == ObjectType.UNIT)
            {
                GetComponent<UnitAI>().Death();
            }
            StartCoroutine("Destroy");
        }
    }

    public void Death()
    {
        if (!_isDead)
        {
            _isDead = true;
            StartCoroutine("Destroy");
        }
    }

    IEnumerator Destroy()
    {
        _hpGauge.localScale = Vector3.one;
        _hpGauge.GetComponent<SpriteRenderer>().color = Color.green;
        _hpBar.SetActive(false);

        if (owner == PlayerType.PLAYER)
            GameManager.instance.playerObjList.Remove(gameObject);
        else
            GameManager.instance.enemyObjList.Remove(gameObject);

        yield return new WaitForSeconds(1.5f);

        if(type == ObjectType.CASTLE)
        {
            gameObject.SetActive(false);
        }
        owner = PlayerType.NONE;
        ObjectManager.instance.Free(gameObject);
    }
}
