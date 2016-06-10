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

    public PlayerStatus.PlayerType  owner               = PlayerStatus.PlayerType.NONE;
    public ObjectType               type                = ObjectType.NONE;
    public Direction                dir                 = Direction.RIGHT;
    public GameObject               hpBar               = null;
    public AudioClip                hitSound            = null;
    public List<AudioClip>          attackSounds        = new List<AudioClip>();
    public List<AudioClip>          deathSounds         = new List<AudioClip>();
    public int                      cost                = 0;
    public int                      reward              = 0;
    public int                      maxHp               = 0;
    public int                      damage              = 0;
    public float                    attackRange         = 0.0f;
    public float                    attackFrontDelay    = 0.0f;
    public float                    attackBackDelay     = 0.0f;
    public float                    moveSpeed           = 0.0f;
    public float                    createTime          = 0.0f;
    public float                    deathTime           = 0.0f;

    List<SpriteRenderer>    _sprites    = new List<SpriteRenderer>();
    MeshRenderer            _mash       = null;
    Rigidbody2D             _body       = null;
    GameObject              _hpBar      = null;
    Transform               _hpGauge    = null;
    int                     _curHp      = 0;
    bool                    _isDead     = true;


    void Awake()
    {
        if (type == ObjectType.BARRIER)
        {
            _mash = GetComponent<MeshRenderer>();
        }
        else
        {
            var sprite = GetComponent<SpriteRenderer>();
            if (sprite)
            {
                _sprites.Add(sprite);
            }
            var sprites = GetComponentsInChildren<SpriteRenderer>();
            for (int i = 0; i < sprites.Length; ++i)
            {
                _sprites.Add(sprites[i]);
            }
        }
        _body = GetComponent<Rigidbody2D>();

        var collider = GetComponent<BoxCollider2D>();
        var unitSize = collider.size / 2 + collider.offset;
        _hpBar = Instantiate(hpBar) as GameObject;
        _hpBar.transform.SetParent(transform);
        _hpBar.transform.localPosition = Vector3.up * (unitSize.y + 0.5f);
        _hpBar.name = "HpBar";
        _hpBar.SetActive(false);

        if (type == ObjectType.BARRIER)
            _hpBar.transform.localPosition -= Vector3.up * 0.5f;

        _hpGauge = _hpBar.transform.FindChild("HpGauge");
    }

    void OnEnable()
    {
        _curHp = maxHp;
        _isDead = false;

        if (type == ObjectType.BARRIER)
        {
            var color = _mash.material.GetColor("_TintColor");
            _mash.material.SetColor("_TintColor", new Color(color.r, color.g, color.b, 1.0f));
        }
        else
        {
            for (int i = 0; i < _sprites.Count; ++i)
            {
                var color = _sprites[i].color;
                _sprites[i].color = new Color(color.r, color.g, color.b, 1.0f);
            }
        }
        _hpBar.SetActive(true);
        _body.simulated = true;
    }



    public bool IsDead()
    {
        return _isDead;
    }

    public void MaxHpFix(int hp)
    {
        _curHp = maxHp = hp;
    }

    public void ChangeDir(Direction d)
    {
        if (dir != d)
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
        GameManager.instance.DamageLabelShow(transform.position, dam);
        AudioManager.instance.PlaySfx(hitSound, 1.0f, 0.1f);

        if (_curHp <= 0)
        {
            _isDead = true;
            if (owner == PlayerStatus.PlayerType.PLAYER)
            {
                GameManager.instance.mEnemy.Reward(reward);
            }
            else
            {
                GameManager.instance.RewardLabelShow(transform.position, reward);
                GameManager.instance.mPlayer.Reward(reward);
                AudioManager.instance.PlayReward();
            }

            switch (type)
            {
                case ObjectType.UNIT: GetComponent<UnitAI>().Death(); break;
            }

            if (deathSounds.Count > 0)
            {
                int rand = Random.Range(0, deathSounds.Count);
                AudioManager.instance.PlaySfx(deathSounds[rand], 0.8f, 0.1f);
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

            yield return new WaitForEndOfFrame();

            if (owner == PlayerStatus.PlayerType.PLAYER)
                GameManager.instance.mPlayerObjList.Remove(gameObject);
            else
                GameManager.instance.mEnemyObjList.Remove(gameObject);
        }
        yield return new WaitForSeconds(deathTime);

        float time = 0.5f;
        while (time > 0)
        {
            time -= Time.deltaTime;
            if (type == ObjectType.BARRIER)
            {
                var color = _mash.material.GetColor("_TintColor");
                _mash.material.SetColor("_TintColor", new Color(color.r, color.g, color.b, time * 2));
            }
            else
            {
                for (int i = 0; i < _sprites.Count; ++i)
                {
                    var color = _sprites[i].color;
                    _sprites[i].color = new Color(color.r, color.g, color.b, time * 2);
                }
            }
            yield return new WaitForEndOfFrame();
        }

        if (type == ObjectType.CASTLE)
        {
            gameObject.SetActive(false);
        }
        owner = PlayerStatus.PlayerType.NONE;
        ObjectManager.instance.Free(gameObject);
    }

    IEnumerator InstantlyDestroy()
    {
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

            yield return new WaitForEndOfFrame();

            if (owner == PlayerStatus.PlayerType.PLAYER)
                GameManager.instance.mPlayerObjList.Remove(gameObject);
            else
                GameManager.instance.mEnemyObjList.Remove(gameObject);
        }
        if (type == ObjectType.CASTLE)
        {
            gameObject.SetActive(false);
        }
        owner = PlayerStatus.PlayerType.NONE;
        ObjectManager.instance.Free(gameObject);
    }
}
