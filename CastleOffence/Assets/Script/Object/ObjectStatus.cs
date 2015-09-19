using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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
    public float        deathTime           = 0.0f;

    List<SpriteRenderer>    _sprites    = new List<SpriteRenderer>();
    Rigidbody2D             _body       = null;
    GameObject              _hpBar      = null;
    Transform               _hpGauge    = null;
    int                     _curHp      = 0;
    bool                    _isDead     = true;


    void Awake()
    {
        var sprite = GetComponent<SpriteRenderer>();
        if (sprite)
            _sprites.Add(sprite);

        var sprites = GetComponentsInChildren<SpriteRenderer>();
        for (int i = 0; i < sprites.Length; ++i)
            _sprites.Add(sprites[i]);

        _body = GetComponent<Rigidbody2D>();

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
        for (int i = 0; i < _sprites.Count; ++i)
        {
            var color = _sprites[i].color;
            _sprites[i].color = new Color(color.r, color.g, color.b, 1.0f);
        }
        _hpBar.SetActive(true);
        _body.simulated = true;
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
        if (type != ObjectType.MISSILE)
        {
            _hpGauge.localScale = new Vector3(hpRatio, 1.0f, 1.0f);
            var sprite = _hpGauge.GetComponent<SpriteRenderer>();
            sprite.color = new Color(1.0f - hpRatio, hpRatio, 0, sprite.color.a);
        }
        if (_curHp <= 0)
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

    public void InstantlyDeath()
    {
        if (!_isDead)
        {
            _isDead = true;
            StartCoroutine("InstantlyDestroy");
        }
    }

    IEnumerator Destroy()
    {
        yield return new WaitForEndOfFrame();
        if (type == ObjectType.MISSILE)
        {
            _body.simulated = false;
        }
        else
        {
            var sprite = _hpGauge.GetComponent<SpriteRenderer>();
            sprite.color = new Color(0, 1.0f, 0, sprite.color.a);
            _hpGauge.localScale = Vector3.one;
            _hpBar.SetActive(false);

            if (owner == PlayerType.PLAYER)
                GameManager.instance.playerObjList.Remove(gameObject);
            else
                GameManager.instance.enemyObjList.Remove(gameObject);
        }
        yield return new WaitForSeconds(deathTime);

        float time = 0.5f;
        while(time > 0)
        {
            time -= Time.deltaTime;
            for (int i = 0; i < _sprites.Count; ++i)
            {
                var color = _sprites[i].color;
                _sprites[i].color = new Color(color.r, color.g, color.b, time * 2);
            }
            yield return new WaitForEndOfFrame();
        }

        if(type == ObjectType.CASTLE)
        {
            gameObject.SetActive(false);
        }
        owner = PlayerType.NONE;
        ObjectManager.instance.Free(gameObject);
    }


    IEnumerator InstantlyDestroy()
    {
        yield return new WaitForEndOfFrame();
        if (type == ObjectType.MISSILE)
        {
            _body.simulated = false;
        }
        else
        {
            var sprite = _hpGauge.GetComponent<SpriteRenderer>();
            sprite.color = new Color(0, 1.0f, 0, sprite.color.a);
            _hpGauge.localScale = Vector3.one;
            _hpBar.SetActive(false);

            if (owner == PlayerType.PLAYER)
                GameManager.instance.playerObjList.Remove(gameObject);
            else
                GameManager.instance.enemyObjList.Remove(gameObject);
        }
        if (type == ObjectType.CASTLE)
        {
            gameObject.SetActive(false);
        }
        owner = PlayerType.NONE;
        ObjectManager.instance.Free(gameObject);
    }
}
