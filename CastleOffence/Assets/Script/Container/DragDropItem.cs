using UnityEngine;

public class DragDropItem : UIDragDropItem
{
    //-----------------------------------------------------------------------------------
    // inspector field
    public GameObject   Prefab  = null;
    public float        XSize   = 1.0f;
    public float        YSize   = 1.0f;
    public int          Amount  = 1;

    //-----------------------------------------------------------------------------------
    // handler functions
    protected override void Start()
    {
        base.Start();
        mRoot = NGUITools.FindInParents<UIRoot>(mTrans.parent);
        mGrid = NGUITools.FindInParents<UIGrid>(mTrans.parent);
        if (mGrid != null) mGrid.repositionNow = true;

        costLabel = transform.FindChild("Cost").GetComponent<UILabel>();
        costLabel.text = (Prefab.GetComponent<ObjectStatus>().Cost * XSize * YSize).ToString();
        costLabel.depth = 2;

        amountLabel = transform.FindChild("Amount").GetComponent<UILabel>();
        amountLabel.text = Amount.ToString();
        amountLabel.depth = 2;
    }

    protected override void OnDragDropStart()
    {
        if (Amount < 1) return;

        if (mDragScrollView != null)
            mDragScrollView.enabled = false;

        obj = ObjectManager.instance.Assign(Prefab.name);
        obj.transform.localScale = new Vector3(XSize, YSize, 1.0f);
        obj.transform.localRotation = Quaternion.identity;

        var info = obj.GetComponent<ObjectStatus>();
        switch (info.Type)
        {
            case ObjectStatus.ObjectType.BARRIER:
            {
                var mat = obj.GetComponent<MeshRenderer>().material;
                mat.mainTextureScale = new Vector2(XSize, YSize);

                info.MaxHpFix(Prefab.GetComponent<ObjectStatus>().MaxHp);
                break;
            }
            case ObjectStatus.ObjectType.TOWER:
            {
                obj.GetComponent<TowerAI>().State = TowerAI.TowerFSM.DEAD;
                break;
            }
        }
        var body = obj.GetComponent<Rigidbody2D>();
        body.mass = Prefab.GetComponent<Rigidbody2D>().mass;
        body.simulated = false;
    }

    protected override void OnDragDropMove(Vector2 delta)
    {
        if (Amount < 1) return;

        Vector3 pos;
#if UNITY_EDITOR
        pos = Input.mousePosition;
#elif UNITY_IPHONE || UNITY_ANDROID || UNITY_WP8
        pos = Input.GetTouch(0).position;
#endif
        pos = Camera.main.ScreenToWorldPoint(pos);
        pos.z = 1.0f;
        obj.transform.position = pos;
    }

    protected override void OnDragDropRelease(GameObject surface)
    {
        if (Amount < 1) return;

        if (mDragScrollView != null)
            StartCoroutine(EnableDragScrollView());

        if (NGUITools.FindInParents<UIDragDropContainer>(surface))
            ObjectManager.instance.Free(obj);
        else
        {
            amountLabel.text = (--Amount).ToString();

            var info = obj.GetComponent<ObjectStatus>();
            info.Owner = PlayerStatus.PlayerType.PLAYER;
            switch(info.Type)
            {
                case ObjectStatus.ObjectType.BARRIER:
                {
                    info.MaxHpFix(info.MaxHp * (int)(XSize * YSize));
                    break;
                }
                case ObjectStatus.ObjectType.TOWER:
                {
                    obj.GetComponent<TowerAI>().State = TowerAI.TowerFSM.IDLE;
                    break;
                }
            }
            var body = obj.GetComponent<Rigidbody2D>();
            body.mass *= (1.0f + (XSize * YSize - 1.0f) * 0.5f);
            body.simulated = true;

            AudioManager.instance.PlayBuild();
            GameManager.instance.playerObjList.Add(obj);
        }
        obj = null;
    }

    //-----------------------------------------------------------------------------------
    // public functions
    public void Purchase()
    {
        amountLabel.text = (++Amount).ToString();
    }

    //-----------------------------------------------------------------------------------
    // private field
    GameObject obj         = null;
    UILabel     costLabel   = null;
    UILabel     amountLabel = null;
}
