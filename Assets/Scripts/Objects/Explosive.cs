using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosive : MonoBehaviour
{
    public new Rigidbody rigidbody;
    public ParticleSystem BoomParticle;
    public float explosionRadius = 100f;
    public float explosionForce = 2000f;
    public float explodeDelay = 0.07f;
    public new Collider collider;

    private bool boomed = false;
    private int boomedDetonate = 0;
    private Dictionary<float, Explosive> knifeDic = new Dictionary<float, Explosive>();
    private List<float> KnifeList = new  List<float>();

    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        collider = GetComponent<Collider>();
    }

    private void DetonateExplosive()
    {
        KnifeList.Clear();
        knifeDic.Clear();

        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius / 4);
        foreach (Collider collider in colliders)
        {
            if (collider.tag == "BlocksExplosive" || collider.tag == "BulletExplosive")
            {
                float dis = Vector3.Distance(collider.transform.position, transform.position);
                Explosive explosive = collider.gameObject.GetComponent<Explosive>();
                if (explosive != null)
                {
                    knifeDic.Add(dis, explosive);
                    if (!KnifeList.Contains(dis))
                        KnifeList.Add(dis);
                }
            }
        }
        if (KnifeList.Count > 0)
        {
            KnifeList.Sort(); //对距离进行排序
            Explosive obj;
            if (knifeDic.TryGetValue(KnifeList[0], out obj))
                obj.Boom();
        }

    }
    private void HideBase()
    {
        MeshRenderer m = GetComponent<MeshRenderer>();
        if (m != null) m.enabled = false;
        else
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                MeshRenderer m1 = transform.GetChild(i).gameObject.GetComponent<MeshRenderer>();
                if (m1 != null) m1.enabled = false;
            }
        }
    }
    private void ExplodeForce()
    {
        //得到圆心为 collision.contacts[0]  半径为R的圆中间所有的碰撞体
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (Collider collider in colliders)
        {
            //添加爆炸力
            if (collider.tag == "Blocks" || collider.tag == "BulletExplosive" || collider.tag == "BlocksExplosive"
                || collider.tag == "Floor" || collider.tag == "Bullet" || collider.tag == "Ball")
            {
                Rigidbody r = collider.GetComponent<Rigidbody>();
                if (r != null) r.AddExplosionForce(explosionForce, transform.position, explosionRadius);
            }
        }
    }

    public void Boom()
    {
        if (!boomed)
        {
            boomedDetonate = 0;
            boomed = true;

            StartCoroutine(PlayBoomLoop());
        }
    }
    private IEnumerator PlayBoomLoop()
    {
        yield return new WaitForSeconds(explodeDelay);

        ExplodeForce();
        rigidbody.Sleep();
        rigidbody.isKinematic = true;
        collider.enabled = false;

        StartCoroutine(PlayBoomParticleLate());
    }
    private IEnumerator PlayBoomParticleLate()
    {
        if (CameraMain.PlayingBombParticle < 3)
            BoomParticle.gameObject.SetActive(true);
        CameraMain.PlayingBombParticle++;
        yield return new WaitForSeconds(0.2f);
        HideBase();
        DetonateExplosive();
        CameraMain.PlayingBombParticle--;
        yield return new WaitForSeconds(3f);
        BoomParticle.gameObject.SetActive(false);
        gameObject.SetActive(false);
        Destroy(gameObject);
        
    }
}
