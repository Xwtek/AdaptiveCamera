using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(EnemyEvent))]
public class EnemyController : MonoBehaviour
{
    public float maximumDistance;
    public bool shouldReturn;
    public bool destroyNow;
    public GameObject dyingEffect;

    NavMeshAgent nav;
    Transform playerPos;
    Animator anim;
    EnemyEvent evt;
    public bool dying;
    Vector3? returnTo;
    // Start is called before the first frame update
    void Start()
    {
        nav = GetComponent<NavMeshAgent>();
        playerPos = NoahController.Instance.transform;
        returnTo = transform.position;
        anim = GetComponent<Animator>();
        evt = GetComponent<EnemyEvent>();
        evt.OnDie.AddListener(Dying);
        evt.OnRevive.AddListener(Revive);
    }
    private void OnDestroy() {
        evt.OnDie.RemoveListener(Dying);
        evt.OnRevive.AddListener(Revive);
    }

    // Update is called once per frame
    void Update()
    {
        if(dying) return;
        destroyNow = false;
        if ((transform.position - playerPos.position).sqrMagnitude < maximumDistance * maximumDistance)
        {
            nav.destination = playerPos.position;
            nav.isStopped = false;
        }
        else if (returnTo.HasValue)
        {
            nav.destination = returnTo.Value;
            nav.isStopped = false;
        }
        else
        {
            nav.isStopped = true;
        }
    }

    private IEnumerator Die(){
        while(!destroyNow) yield return null;
        if(dyingEffect != null)Instantiate(dyingEffect, transform.position, transform.rotation);
        gameObject.SetActive(false);
    }
    private void Dying(){
        nav.enabled = false;
        dying = true;
        anim.SetTrigger("Die");
        StartCoroutine(Die());
    }
    private void Revive(){
        nav.enabled = true;
        dying = false;
        anim.SetTrigger("Revive");
    }
}
