using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CannonAI : MonoBehaviour
{
    public GameObject   effectObj       = null;
    public float        explosionRange  = 0.0f;
    public float        explosionPower  = 0.0f;

    ObjectStatus    _objInfo = null;
    Rigidbody2D     _body = null;

    void Awake()
    {
        _objInfo = GetComponent<ObjectStatus>();
        _body = GetComponent<Rigidbody2D>();
    }
    void OnEnable()
    {
        _body.AddTorque(-1000.0f);
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (_objInfo.IsDead())
            return;

        var other = collider.gameObject;
        var otherObjInfo = other.GetComponent<ObjectStatus>();
        if (otherObjInfo != null && otherObjInfo.owner != _objInfo.owner && !otherObjInfo.IsDead()
            || other.gameObject.name == "Ground")
        {
            StartCoroutine("Explosion");
            _objInfo.InstantlyDeath();
        }
    }

    IEnumerator Explosion()
    {
        List<GameObject> enemies = null;
        if (_objInfo.owner == PlayerType.PLAYER)
            enemies = GameManager.instance.enemyObjList;
        else
            enemies = GameManager.instance.playerObjList;

        for(int i = 0; i < enemies.Count; ++i)
        {
            var enemy = enemies[i];
            var enemyPos = enemy.transform.position;
            var dist = Vector3.Distance(transform.position, enemyPos);
            if (dist < explosionRange)
            {
                var enemyStatus = enemy.GetComponent<ObjectStatus>();
                if (enemyStatus.type == ObjectStatus.ObjectType.UNIT)
                {
                    enemy.GetComponent<UnitAI>().Attacked();
                }
                var gap = (explosionRange - dist) / explosionRange;
                var dir = Vector3.Normalize(enemyPos - transform.position);
                var power = explosionPower * gap * 50.0f;
                enemy.GetComponent<Rigidbody2D>().AddForce(new Vector2(dir.x * power * 2, dir.y * power));
                enemyStatus.Damaged((int)(_objInfo.damage * gap));
            } 
        }
        Camera.main.GetComponent<CameraMove>().Shake(1.0f, 0.2f);

        var effect = ObjectManager.instance.Assign(effectObj.name);
        effect.transform.position = transform.position;
        effect.GetComponent<ParticleSystem>().Play();
        yield return new WaitForSeconds(1.5f);
        ObjectManager.instance.Free(effect);
    }
}
