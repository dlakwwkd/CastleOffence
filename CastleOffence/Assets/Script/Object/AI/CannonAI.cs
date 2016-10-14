using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CannonAI : MonoBehaviour
{
    //-----------------------------------------------------------------------------------
    // inspector field
    public GameObject   EffectObj       = null;
    public float        ExplosionRange  = 0.0f;
    public float        ExplosionPower  = 0.0f;

    //-----------------------------------------------------------------------------------
    // handler functions
    void Awake()
    {
        objInfo = GetComponent<ObjectStatus>();
        body = GetComponent<Rigidbody2D>();
    }

    void OnEnable()
    {
        body.AddTorque(-1000.0f);
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (objInfo.IsDead())
            return;

        var other = collider.gameObject;
        var otherObjInfo = other.GetComponent<ObjectStatus>();
        if (otherObjInfo != null && otherObjInfo.Owner != objInfo.Owner && !otherObjInfo.IsDead()
            || other.gameObject.name == "Ground")
        {
            StartCoroutine("Explosion");
            objInfo.InstantlyDeath();
        }
    }

    //-----------------------------------------------------------------------------------
    // coroutine functions
    IEnumerator Explosion()
    {
        if(objInfo.AttackSounds.Count > 0)
        {
            int rand = Random.Range(0, objInfo.AttackSounds.Count);
            AudioManager.instance.PlaySfx(objInfo.AttackSounds[rand], 1.2f);
        }
        List<GameObject> enemies = null;
        if (objInfo.Owner == PlayerStatus.PlayerType.PLAYER)
            enemies = GameManager.instance.enemyObjList;
        else
            enemies = GameManager.instance.playerObjList;

        for(int i = 0; i < enemies.Count; ++i)
        {
            var enemy = enemies[i];
            var enemyPos = enemy.transform.position;
            var dist = Vector2.Distance(transform.position, enemyPos);
            if (dist < ExplosionRange)
            {
                var enemyStatus = enemy.GetComponent<ObjectStatus>();
                if (enemyStatus.Type == ObjectStatus.ObjectType.UNIT)
                {
                    enemy.GetComponent<UnitAI>().Attacked();
                }
                var gap = (ExplosionRange - dist) / ExplosionRange;
                var dir = Vector3.Normalize(enemyPos - transform.position);
                var power = ExplosionPower * gap * 50.0f;
                enemy.GetComponent<Rigidbody2D>().AddForce(new Vector2(dir.x * power * 3.0f, dir.y * power));
                enemyStatus.Damaged((int)(objInfo.Damage * gap));
            }
        }
        Camera.main.GetComponent<CameraMove>().Shake(1.0f, 0.2f);

        var effect = ObjectManager.instance.Assign(EffectObj.name);
        effect.transform.position = transform.position;
        effect.GetComponent<ParticleSystem>().Play();
        ObjectManager.instance.FreeAfter(effect, 1.5f);
        yield return null;
    }

    //-----------------------------------------------------------------------------------
    // private field
    ObjectStatus    objInfo = null;
    Rigidbody2D     body    = null;
}
