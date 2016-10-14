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

    //-----------------------------------------------------------------------------------
    // inspector field
    public PlayerStatus.PlayerType  Owner               = PlayerStatus.PlayerType.NONE;
    public ObjectType               Type                = ObjectType.NONE;
    public Direction                Dir                 = Direction.RIGHT;
    public GameObject               HpBar               = null;
    public AudioClip                HitSound            = null;
    public List<AudioClip>          AttackSounds        = new List<AudioClip>();
    public List<AudioClip>          DeathSounds         = new List<AudioClip>();
    public int                      Cost                = 0;
    public int                      Reward              = 0;
    public int                      MaxHp               = 0;
    public int                      Damage              = 0;
    public float                    AttackRange         = 0.0f;
    public float                    AttackFrontDelay    = 0.0f;
    public float                    AttackBackDelay     = 0.0f;
    public float                    MoveSpeed           = 0.0f;
    public float                    CreateTime          = 0.0f;
    public float                    DeathTime           = 0.0f;

    //-----------------------------------------------------------------------------------
    // handler functions
    void Awake()
    {
        if (Type == ObjectType.BARRIER)
        {
            mash = GetComponent<MeshRenderer>();
        }
        else
        {
            var sprite = GetComponent<SpriteRenderer>();
            if (sprite)
            {
                this.sprites.Add(sprite);
            }
            var sprites = GetComponentsInChildren<SpriteRenderer>();
            for (int i = 0; i < sprites.Length; ++i)
            {
                this.sprites.Add(sprites[i]);
            }
        }
        body = GetComponent<Rigidbody2D>();

        var collider = GetComponent<BoxCollider2D>();
        var unitSize = collider.size / 2 + collider.offset;
        hpBar = Instantiate(HpBar) as GameObject;
        hpBar.transform.SetParent(transform);
        hpBar.transform.localPosition = Vector3.up * (unitSize.y + 0.5f);
        hpBar.name = "HpBar";
        hpBar.SetActive(false);

        if (Type == ObjectType.BARRIER)
            hpBar.transform.localPosition -= Vector3.up * 0.5f;

        hpGauge = hpBar.transform.FindChild("HpGauge");
    }

    void OnEnable()
    {
        curHp = MaxHp;
        isDead = false;

        if (Type == ObjectType.BARRIER)
        {
            var color = mash.material.GetColor("_TintColor");
            mash.material.SetColor("_TintColor", new Color(color.r, color.g, color.b, 1.0f));
        }
        else
        {
            for (int i = 0; i < sprites.Count; ++i)
            {
                var color = sprites[i].color;
                sprites[i].color = new Color(color.r, color.g, color.b, 1.0f);
            }
        }
        hpBar.SetActive(true);
        body.simulated = true;
    }

    //-----------------------------------------------------------------------------------
    // public functions
    public bool IsDead()
    {
        return isDead;
    }

    public void MaxHpFix(int hp)
    {
        curHp = MaxHp = hp;
    }

    public void ChangeDir(Direction d)
    {
        if (Dir != d)
        {
            Dir = d;
            if (Dir == Direction.LEFT)
                transform.localRotation = Quaternion.Euler(new Vector3(0, 180.0f, 0));
            else
                transform.localRotation = Quaternion.identity;

            hpBar.transform.localRotation = transform.localRotation;
        }
    }

    public void Damaged(int dam)
    {
        curHp -= dam;

        var hpRatio = (float)curHp / MaxHp;
        if (Type != ObjectType.MISSILE)
        {
            hpGauge.localScale = new Vector3(hpRatio, 1.0f, 1.0f);
            var sprite = hpGauge.GetComponent<SpriteRenderer>();
            sprite.color = new Color(1.0f - hpRatio, hpRatio, 0, sprite.color.a);
        }
        GameManager.instance.DamageLabelShow(transform.position, dam);
        AudioManager.instance.PlaySfx(HitSound, 1.0f, 0.1f);

        if (curHp <= 0)
        {
            isDead = true;
            if (Owner == PlayerStatus.PlayerType.PLAYER)
            {
                GameManager.instance.enemy.Reward(Reward);
            }
            else
            {
                GameManager.instance.RewardLabelShow(transform.position, Reward);
                GameManager.instance.player.Reward(Reward);
                AudioManager.instance.PlayReward();
            }

            switch (Type)
            {
                case ObjectType.UNIT: GetComponent<UnitAI>().Death(); break;
            }

            if (DeathSounds.Count > 0)
            {
                int rand = Random.Range(0, DeathSounds.Count);
                AudioManager.instance.PlaySfx(DeathSounds[rand], 0.8f, 0.1f);
            }
            StartCoroutine("Destroy");
        }
    }

    public void Death()
    {
        if (!isDead)
        {
            isDead = true;
            StartCoroutine("Destroy");
        }
    }

    public void InstantlyDeath()
    {
        if (!isDead)
        {
            isDead = true;
            StartCoroutine("InstantlyDestroy");
        }
    }

    //-----------------------------------------------------------------------------------
    // coroutine functions
    IEnumerator Destroy()
    {
        if (Type == ObjectType.MISSILE)
        {
            body.simulated = false;
        }
        else
        {
            var sprite = hpGauge.GetComponent<SpriteRenderer>();
            sprite.color = new Color(0, 1.0f, 0, sprite.color.a);
            hpGauge.localScale = Vector3.one;
            hpBar.SetActive(false);

            yield return new WaitForEndOfFrame();

            if (Owner == PlayerStatus.PlayerType.PLAYER)
                GameManager.instance.playerObjList.Remove(gameObject);
            else
                GameManager.instance.enemyObjList.Remove(gameObject);
        }
        yield return new WaitForSeconds(DeathTime);

        float time = 0.5f;
        while (time > 0)
        {
            time -= Time.deltaTime;
            if (Type == ObjectType.BARRIER)
            {
                var color = mash.material.GetColor("_TintColor");
                mash.material.SetColor("_TintColor", new Color(color.r, color.g, color.b, time * 2));
            }
            else
            {
                for (int i = 0; i < sprites.Count; ++i)
                {
                    var color = sprites[i].color;
                    sprites[i].color = new Color(color.r, color.g, color.b, time * 2);
                }
            }
            yield return new WaitForEndOfFrame();
        }

        if (Type == ObjectType.CASTLE)
        {
            gameObject.SetActive(false);
        }
        Owner = PlayerStatus.PlayerType.NONE;
        ObjectManager.instance.Free(gameObject);
    }

    IEnumerator InstantlyDestroy()
    {
        if (Type == ObjectType.MISSILE)
        {
            body.simulated = false;
        }
        else
        {
            var sprite = hpGauge.GetComponent<SpriteRenderer>();
            sprite.color = new Color(0, 1.0f, 0, sprite.color.a);
            hpGauge.localScale = Vector3.one;
            hpBar.SetActive(false);

            yield return new WaitForEndOfFrame();

            if (Owner == PlayerStatus.PlayerType.PLAYER)
                GameManager.instance.playerObjList.Remove(gameObject);
            else
                GameManager.instance.enemyObjList.Remove(gameObject);
        }
        if (Type == ObjectType.CASTLE)
        {
            gameObject.SetActive(false);
        }
        Owner = PlayerStatus.PlayerType.NONE;
        ObjectManager.instance.Free(gameObject);
    }

    //-----------------------------------------------------------------------------------
    // private field
    List<SpriteRenderer>    sprites     = new List<SpriteRenderer>();
    MeshRenderer            mash        = null;
    Rigidbody2D             body        = null;
    GameObject              hpBar       = null;
    Transform               hpGauge     = null;
    int                     curHp       = 0;
    bool                    isDead      = true;
}
