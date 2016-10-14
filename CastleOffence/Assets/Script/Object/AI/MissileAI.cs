using UnityEngine;
using System.Collections;

public class MissileAI : MonoBehaviour
{
    //-----------------------------------------------------------------------------------
    // handler functions
    void Start()
    {
        objInfo = GetComponent<ObjectStatus>();
        body = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (objInfo.IsDead())
            return;

        var dirVector = body.velocity.normalized;
        var angle = Vector2.Angle(Vector2.right, dirVector);
        if (dirVector.y < 0)
            angle = -angle;

        transform.localRotation = Quaternion.Euler(new Vector3(0, 0, angle));
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (objInfo.IsDead())
            return;

        var other = collider.gameObject;
        if(other.gameObject.name == "Ground")
        {
            StartCoroutine("ArrowShaking", other.transform);
            objInfo.Death();
            return;
        }
        var otherObjInfo = other.GetComponent<ObjectStatus>();
        if (otherObjInfo != null && otherObjInfo.Owner != objInfo.Owner && !otherObjInfo.IsDead())
        {
            if (otherObjInfo.Type == ObjectStatus.ObjectType.UNIT)
            {
                other.GetComponent<UnitAI>().Attacked();
            }
            var power = body.velocity * 25.0f;
            other.GetComponent<Rigidbody2D>().AddForce(new Vector2(power.x * 1.5f, power.y));
            otherObjInfo.Damaged(objInfo.Damage);

            StartCoroutine("ArrowShaking", other.transform);
            objInfo.Death();
        }
    }

    //-----------------------------------------------------------------------------------
    // coroutine functions
    IEnumerator ArrowShaking(Transform targetPos)
    {
        if (objInfo.AttackSounds.Count > 0)
        {
            int rand = Random.Range(0, objInfo.AttackSounds.Count);
            AudioManager.instance.PlaySfx(objInfo.AttackSounds[rand]);
        }
        var r = transform.localRotation.eulerAngles;
        var reverse = 1.0f;
        var rotation = 5.0f;
        var time = 1.0f;
        var gap = rotation / time;
        var startPos = targetPos.localPosition;
        while (time > 0)
        {
            if(targetPos.gameObject.activeSelf)
            {
                var deltaPos = targetPos.localPosition - startPos;
                deltaPos.z = 0.0f;
                startPos = targetPos.localPosition;
                transform.localPosition += deltaPos;
            }
            transform.localRotation = Quaternion.Euler(new Vector3(r.x, r.y , r.z + rotation * reverse));
            reverse = -reverse;
            rotation -= Time.deltaTime * gap;
            time -= Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
    }
    
    //-----------------------------------------------------------------------------------
    // private field
    ObjectStatus    objInfo = null;
    Rigidbody2D     body    = null;
}
