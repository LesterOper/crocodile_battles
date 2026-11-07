using App;
using App.Audio;
using App.Crocodile.Interfaces;
using App.PrototypeBattle;
using Battle.Equipment;
using DG.Tweening;
using Effects;
using Library;
using Library.Common;
using UnityEngine;
using UnityEditor;
using Zenject;


[ExecuteInEditMode]
public class MetaballParticleClass : MonoBehaviour
{
    private float _damage;
    public GameObject MObject;
    public float LifeTime;
    public Water2D.Water2D_Spawner SpawnerParent;    
    [SerializeField] private GameObject pngSequenceEffectCroco;
    [SerializeField] private GameObject pngSequenceEffectWater;
    [SerializeField] private GameObject pngSequenceEffectBath;
    private SoundPlayer _soundPlayer;

    [Inject]
    public void Construct(SoundPlayer soundPlayer)
    {
        _soundPlayer = soundPlayer;
    }
    public bool Active
    {
        get { return _active; }
        set
        {
            _active = value;
            if (MObject)
            {
                MObject.SetActive(value);

                if (tr)
                    tr.Clear();

            }

            if (value)
            {
                delta *= 0;
                wakeUpTime = Time.time;
                //RENDERER.color = new Color(RENDERER.color.r, RENDERER.color.g, RENDERER.color.b, 1);
                if (glowSP)
                    glowSP.enabled = true;
            }

            if (!value)
            {
                //substrac 1 unit of particle using.
                if(SpawnerParent)
                    SpawnerParent.DropsUsed--;

                if (rb != null) // reset speed simulation in Editor
                    rb.velocity *= 0f;
                //RENDERER.color = new Color(RENDERER.color.r, RENDERER.color.g, RENDERER.color.b, 0);
                if (_isFreeze)
                {
                    rb.constraints = RigidbodyConstraints2D.None;
                    rb.sharedMaterial = null;
                    _isFreeze = false;
                }
                   

                delta *= 0;

            }
            ScaleDownIsPerforming = false;

        }
    }
    public bool witinTarget;

    public Vector2 Editor_Velocity; // velocity used within editor simulation
    public Vector2 Velocity_Limiter_X; // Limiter of speed in X in simulation
    public Vector2 Velocity_Limiter_Y; // Limiter of speed in X in simulation

    public bool ScaleDown = false;
    public float endSize = 0f;
    public bool BlendingColor;
    public SpriteRenderer glowSP;


    public void SetColor(Color _color)
    {
        sr.color = _color;
    }

    public void SetFreeze(bool _val = true)
    {
        _isFreeze = _val;
        rb.constraints = RigidbodyConstraints2D.FreezeAll;
       //rb.gravityScale = .5f;
    }

    public bool GetFreeze()
    { return _isFreeze; }

    public void SetHighDensity(bool _val = true)
    {
        _isFreeze = _val;
        //rb.drag = 25f;
        rb.angularDrag = 25f;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    public void removeGlow()
    {
        glowSP.enabled = false;
    }

    bool _active;
    bool _isFreeze;
    float delta;
    [SerializeField] private Rigidbody2D rb;
    public Rigidbody2D RB => rb;
    [SerializeField] private CircleCollider2D cc;
    public CircleCollider2D COLL => cc;
    [SerializeField] private TrailRenderer tr;
    public TrailRenderer TRAIL => tr;
    [SerializeField] private SpriteRenderer sr;
    public SpriteRenderer RENDERER => sr;
    Collider2D[] Contacts;
    float deltaSimul;
    float fixedDeltaSimul = 0f;
    float wakeUpTime;

#if UNITY_EDITOR

    private void OnEnable()
    {
        EditorApplication.update += FixedUpdate;
    }

    private void OnDisable()
    {
        EditorApplication.update -= FixedUpdate;
    }

#endif

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        tr = GetComponent<TrailRenderer>();
        cc = GetComponent<CircleCollider2D>();
        sr = GetComponent <SpriteRenderer>();

        Contacts = new Collider2D[4];
    }

    void FixedUpdate()
    {
        if (Active == true)
        {
            if (SpawnerParent.GlowEffect && glowSP == null)
            {
                Transform t = MObject.transform.Find("_glow");
                if(t != null)
                    glowSP = t.GetComponent<SpriteRenderer>();
            } 

            if(sr != null)
                if(sr.isVisible)
                    ResizeQuadEffectController.setMinMaxParticlePosition(transform.position, transform.localScale.x);



            if (ScaleDown)
                ScaleItDown();

            VelocityLimiter();

            if (BlendingColor) {
               
                Blend();
            }

            if(SpawnerParent.Water2DEmissionType == Water2D.Water2D_Spawner.EmissionType.FillerCollider)
                return;

            if (LifeTime < 0)
                return;

            if (delta > LifeTime)
            {
                delta *= 0;
                Active = false;
            }
            else
            {
                delta += Time.deltaTime;
            }



            // Handle collisions in fixedtime only editor
            if (deltaSimul > fixedDeltaSimul)
            {
                deltaSimul *= 0;
                OnCollisionEnter2DEditor();
            }
            else
            {
                deltaSimul += Time.deltaTime;
            }
        }

    }

    Vector3 pos_aux;
    void AnimInEditor()
    {
        Editor_Velocity += Physics2D.gravity * .0001f;
       // VelocityLimiterEditor();
        pos_aux = transform.position;
        pos_aux = (Vector2)pos_aux + Editor_Velocity;
        transform.position = pos_aux;
        //print(Time.deltaTime);
    }

    void VelocityLimiter()
    {
        if (rb == null)
            return;

        Vector2 _vel = rb.velocity;
        _vel = rb.velocity;

        if (_vel.x < Velocity_Limiter_X.x)
        {
            _vel.x = Velocity_Limiter_X.x;
        }

        if (_vel.x > Velocity_Limiter_X.y)
        {
            _vel.x = Velocity_Limiter_X.y;
        }

        if (_vel.y < Velocity_Limiter_Y.x)
        {
            _vel.y = Velocity_Limiter_Y.x;
        }

        if (_vel.y > Velocity_Limiter_Y.y)
        {
            _vel.y = Velocity_Limiter_Y.y;
        }

        rb.velocity = _vel;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        var collisionPoint = collision.contacts[0].point;
        CreateEffect(collision.gameObject, collisionPoint);
        
        gameObject.SetActive(false);
        SpawnerParent.InvokeOnCollisionEnter2D(gameObject, collision.contacts[0].collider.gameObject);
        
        ApplyDamage(collision.gameObject);
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        CreateEffect(other.gameObject, other.ClosestPoint(transform.position));
        
        gameObject.SetActive(false);
        SpawnerParent.InvokeOnCollisionEnter2D(gameObject, other.gameObject);

        ApplyDamage(other.gameObject);
    }

    private void ApplyDamage(GameObject target)
    {
        if (!target.CompareTag("WaterSurface"))
        {
            if (target.TryGetComponent(out IDamagable damageHandler))
            {
                if(_equipment.GetWeaponType() != WeaponType.SHOWER)
                    damageHandler.ApplyDamage(_damage, true, false);
                else 
                    damageHandler.ApplyDamage(_damage, true, true);
            }
        }
        else
        {
            if(_soundPlayer != null )
                _soundPlayer.PlaySound("SFX_WaterDrop", AudioMixerController.MasterMixerGroup.SFX, 0.3f);
        }
    }
    
    private void CreateEffect(GameObject collision, Vector2 collisionPoint)
    {
        GameObject effectPrefab;
        GameObject gm;
        Quaternion rotation;
        if (collision.gameObject.CompareTag("WaterSurface"))
        {
            effectPrefab = pngSequenceEffectWater;
            rotation = effectPrefab.transform.rotation;
            gm = EffectPool.Instance.GetEffectWithWaterPool().Obtain();
            gm.transform.position = new Vector3(collisionPoint.x, collisionPoint.y, 0);
            gm.transform.rotation = rotation;
            gm.SetActive(true);
            DOVirtual.DelayedCall(0.35f, () => { 
                gm.SetActive(false);
                EffectPool.Instance.GetEffectWithWaterPool().Free(gm);
            });
        }
        else
        {
            effectPrefab = pngSequenceEffectCroco;
            rotation = Quaternion.Euler(0,0, Vector2.Angle(rb.velocity, collisionPoint));
            gm = EffectPool.Instance.GetEffectWithCrocoPool().Obtain();
            gm.transform.position = new Vector3(collisionPoint.x, collisionPoint.y, 0);
            gm.transform.rotation = rotation;
            gm.SetActive(true);
            DOVirtual.DelayedCall(0.35f, () => { gm.SetActive(false);
                EffectPool.Instance.GetEffectWithCrocoPool().Free(gm);
            });
        }

        //gm = Instantiate(effectPrefab, new Vector3(collisionPoint.x, collisionPoint.y, 0), rotation);
        //gm.transform.SetParent(collision.transform);
        //Destroy(gm, 0.35f);
    }
    
    private void OnCollisionEnter2DEditor()
    {
        if (Application.isPlaying)
            return;

        if (cc == null)
            return;

        if (Contacts == null)
            return;


        int i = Physics2D.OverlapCircleNonAlloc(rb.position, cc.radius * .9f, Contacts);

        if (i > 0)
        {
            for (int j = 0; j < Contacts.Length; j++)
            {
                if (Contacts[j] == null)
                    continue;

                if (Contacts[j].GetInstanceID() == gameObject.GetInstanceID())
                    continue;

                SpawnerParent.InvokeOnCollisionEnter2D(gameObject, Contacts[j].gameObject);
            }
           
        }

        
    }

    bool ScaleDownIsPerforming = false;
    Vector2 deltaScale;
    Vector2 initScale;
    Vector2 tmpScale;
    void ScaleItDown()
    {
        // Initializes
        if (!ScaleDownIsPerforming) {
            ScaleDownIsPerforming = true;
        }

        if(ScaleDownIsPerforming)
            ScaleDownPerform();
    }
    void ScaleDownPerform()
    {
         transform.localScale = Vector2.Lerp(transform.localScale, new Vector2(endSize, endSize), (delta /LifeTime) * Time.deltaTime * .8f );
             //RENDERER.color = Color.Lerp(RENDERER.color, new Color(RENDERER.color.r, RENDERER.color.g, RENDERER.color.b, 0),
             //(delta / LifeTime) * Time.deltaTime * .8f);
         //print(Time.deltaTime);
         //elapsedTime += Time.deltaTime;
    }


    int breathFrames = 6;
    int framesCount = 0;


    void Blend()
    {
        if (true)
        {
           
            if (cc == null)
                return;

            if (Contacts == null)
            {
                Contacts = new Collider2D[2];
            }

            if(framesCount < breathFrames)
            {
                framesCount++;
                return;
            }

            framesCount *= 0;
            
            //mix only in movement

            if (rb.velocity.sqrMagnitude < 0.00000001f)
              return;

            int i = Physics2D.OverlapCircleNonAlloc(rb.position, cc.radius * 8f, Contacts, 1 << gameObject.layer);

            if (i > 0)
            {


                for (int j = 0; j < Contacts.Length; j++)
                {
                    if (Contacts[j] == null)
                        continue;

                    if (!Contacts[j].gameObject.activeSelf)
                        continue;

                    if (Contacts[j].GetInstanceID() == gameObject.GetInstanceID())
                        continue;

                    if (Contacts[j].tag != "Metaball_liquid")
                        continue;

                    //print("blending");
                    Color c2 = Contacts[j].GetComponent<SpriteRenderer>().color;
                    //print(j);
                    if (c2 == sr.color)
                        return;

                    //print("mixing");

                    //sr.color = Color.Lerp(sr.color, c2, .045f);
                    if (Contacts[j].GetComponent<MetaballParticleClass>().SpawnerParent.Blending) {
                        Contacts[j].GetComponent<SpriteRenderer>().color = Color.Lerp(c2, sr.color, .045f);
                    }
                    
                }

            }


        }
    }

    void Blend2()
    {
        if (true)
        {

            if (cc == null)
                return;

           

            if (framesCount < breathFrames)
            {
                framesCount++;
                return;
            }

            framesCount *= 0;

            //mix only in movement

            if (rb.velocity.sqrMagnitude < 0.00000001f)
                return;

            int c = 0;
            int ContactsMax = 60;
            MetaballParticleClass []_contacts = new MetaballParticleClass[ContactsMax] ;

           

            if (Water2D.SpawnersManager.instance == null)
                Water2D.SpawnersManager.GetAllParticles();
            

            if (Water2D.SpawnersManager.instance._allparticles == null)
                return;
            

            for (int i = 0; i < Water2D.SpawnersManager.instance._allparticles.Length; i++)
            {
                if (c >= ContactsMax)
                    break;

                if (Water2D.SpawnersManager.instance._allparticles[i].gameObject.GetHashCode() == gameObject.GetHashCode())
                    continue;

                if (!Water2D.SpawnersManager.instance._allparticles[i].gameObject.activeSelf)
                    continue;

                if (Water2D.SpawnersManager.instance._allparticles[i].gameObject.tag != "Metaball_liquid")
                    continue;

                if ((Water2D.SpawnersManager.instance._allparticles[i].transform.position - gameObject.transform.position).sqrMagnitude < (cc.radius +.1f) * (cc.radius + .1f))
                {
                    // Contact!SpawnersManager.instance._allparticles[i];
                    c++;
                   
                }
            }
            

            //int i = Physics2D.OverlapCircleNonAlloc(rb.position, cc.radius * .9f, Contacts, 1 << gameObject.layer);

            if (c > 0)
            {

                for (int j = 0; j < _contacts.Length; j++)
                {
                    if (_contacts[j] == null)
                        continue;

                    
                    Color c2 = _contacts[j].gameObject.GetComponent<SpriteRenderer>().color;

                    if (c2 == sr.color)
                        return;



                    sr.color = Color.LerpUnclamped(sr.color, c2, .02f);
                   // print(sr.color + " " + c2);
                }

            }


        }
    }


    public void SetDamage(float damage) => _damage = damage;

    private IEquipment _equipment;
    public void SetWeapon(IEquipment equipment) => _equipment = equipment;


}
