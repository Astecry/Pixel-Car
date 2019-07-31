using System.Collections;
using UnityEngine;

public class CarController : MonoBehaviour
{
    public bool Lives = false;
    public float CarSpeed = 5f;
    public float SmoothSpeed = 0.125f;
    public bool Left, Right;
    public Animator Vignette;
    Animator CarAnim;

    public GameObject Bar;
    RectTransform BarRect;
    float BarH;
    float Petrol;
    public float PetrolMax;
    public float PetrolP = 0;
    public float PetrolSpeed = 10f;
    public GameManager GM;
    public float distance;
    int EnemyCarIdL = 0;
    int ColiderCarL = 0;
    int EnemyCarIdR = 0;
    int ColiderCarR = 0;
    [HideInInspector] public int HeartCount;
    public Transform HeartB;
    public PolygonCollider2D CarCollider;
    private AudioSource aS;
    private bool warning;
    


    private void Awake()
    {
        aS = GetComponent<AudioSource>();
        CarAnim = GetComponent<Animator>();
        BarRect = Bar.GetComponent<RectTransform>();
        PetrolReload();
    }

    public void PetrolReload()
    {
        Petrol = PetrolMax;
        BarH = PetrolMax;
    }

    void BarConsumption()
    {
        if (BarH<40 && !warning)
        {
            GM.WarningT();
        }
        else
        {
            GM.WarningF();
        }
        if (BarH > 0)
        {
            BarH -= PetrolSpeed * 0.052f;
            Vector2 NewPos = new Vector2(BarRect.sizeDelta.x, BarH);
            BarRect.sizeDelta = NewPos;
        }
        else
        {
            GM.EndGame();
            Invoke("adsOrGo", 0.3f);
        }
    }

    private void LateUpdate()
    {
        if (Lives && !GM.pause)
        {
            BarConsumption();
            RayCast();
        }
    }

    void RayCast()
    {
        RaycastHit2D RayLeft = Physics2D.Raycast(transform.position, -transform.right, distance);

        if (RayLeft.collider != null)
        {
            Debug.DrawLine(transform.position, RayLeft.point, Color.red);
            if (RayLeft.collider.transform.tag == "Enemy")
            {
                EnemyCarIdL =
                    int.Parse(
                        RayLeft.collider.transform.name.Substring(15, RayLeft.collider.transform.name.Length - 15));

                if (EnemyCarIdL != ColiderCarL)
                {
                    GM.Bonus();

                    ColiderCarL = EnemyCarIdL;
                }
            }
        }

        RaycastHit2D RayRight = Physics2D.Raycast(transform.position, transform.right, distance);

        if (RayRight.collider != null)
        {
            Debug.DrawLine(transform.position, RayRight.point, Color.red);
            if (RayRight.collider.transform.tag == "Enemy")
            {
                EnemyCarIdR =
                    int.Parse(RayRight.collider.transform.name.Substring(15,
                        RayRight.collider.transform.name.Length - 15));

                if (EnemyCarIdR != ColiderCarR)
                {
                    GM.Bonus();

                    ColiderCarR = EnemyCarIdR;
                }
            }
        }
    }


    void FixedUpdate()
    {
        if (Lives)
        {
            CarMove();
            // Control();
        }
    }

    void Control()
    {
        Vector3 Move = transform.forward * Time.deltaTime * -10f;
        this.transform.Translate(Move, Space.World);
        transform.position = new Vector3(Mathf.Clamp(transform.position.x, -1.77f, 1.77f), transform.position.y, 0);
        if (Left)
        {
            Vector3 MoveLeft = transform.right * Time.deltaTime * -CarSpeed;
            this.transform.Translate(MoveLeft, Space.World);
        }

        if (Right)
        {
            Vector3 MoveRight = transform.right * Time.deltaTime * CarSpeed;
            this.transform.Translate(MoveRight, Space.World);
        }

        Vector3 _pos = new Vector3(transform.position.x, transform.position.y, 0);
        Vector3 smoothed_pos = Vector3.Lerp(transform.position, _pos, SmoothSpeed);
        transform.position = smoothed_pos;
    }

    public void BtnLEnter()
    {
        Left = true;
    }

    public void BtnLExit()
    {
        Left = false;
    }

    public void BtnREnter()
    {
        Right = true;
    }

    public void BtnRExit()
    {
        Right = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Enemy")
        {
            StartCoroutine(DamageWait());
            StartCoroutine(VignetteOff());
            HeartN();
            Destroy(collision.GetComponent<PolygonCollider2D>());
            Handheld.Vibrate();
        }
        else if (collision.tag == "Heart")
        {
            HeartP(collision.gameObject);
        }
        else if (collision.tag == "Tank")
        {
            PertrolPlus();
            Destroy(collision.gameObject);
            if (GM.musicActive)
            {
                aS.Play();
            }
        }
        else if (collision.tag == "2X")
        {
            GM.TwoXScor();
            Destroy(collision.gameObject);
            if (GM.musicActive)
            {
                aS.Play();
            }
        }
    }

    IEnumerator CarDamageAnim()
    {
        CarAnim.SetBool("Damage", true);
        CarCollider.enabled = false;
        yield return new WaitForSeconds(1.3f);
        CarCollider.enabled = true;
        CarAnim.SetBool("Damage", false);
    }

    IEnumerator VignetteOff()
    {
        Vignette.SetBool("Damage", true);
        yield return new WaitForSeconds(0.15f);
        Vignette.SetBool("Damage", false);
    }

    void HeartN()
    {
        if (HeartCount > 0)
        {
            StartCoroutine(CarDamageAnim());
            HeartCount--;
            HeartB.GetChild(HeartCount).gameObject.SetActive(false);
            PositionReset();
        }
        else
        {
            GM.EndGame();
            Invoke("adsOrGo", 0.3f);
        }
    }

    void CarMove()
    {
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved)
        {
            Vector2 touch_pos = Input.GetTouch(0).deltaPosition;
            float _x = touch_pos.x * 0.0055f;
            // float _newx = Mathf.Lerp(transform.position.x,_x,0.99f);
            // Debug.Log(_x+"hahhahahaa"+_newx);
            transform.position += new Vector3(_x, 0, 0);
            transform.position = new Vector3(Mathf.Clamp(transform.position.x, -1.77f, 1.77f), transform.position.y, 0);
        }
    }

    void adsOrGo()
    {
        if (GM.CheckInternet())
        {
            GM.Ads();
        }
        else
        {
            GM.gameOver();
        }
    }

    void EndGame()
    {
        Lives = false;
        GM.EndGameBool = false;
    }

    void HeartP(GameObject GO)
    {
        if (HeartCount < 3)
        {
            HeartB.GetChild(HeartCount).gameObject.SetActive(true);
            HeartCount++;
            Destroy(GO);
            if (GM.musicActive)
            {
                aS.Play();
            }
        }
    }

    void PertrolPlus()
    {
        if (BarH<65)
        {
            BarH += 35;
        }
        else
        {
            BarH = 100;
        }
    }

    void PositionReset()
    {
        transform.position = new Vector3(0, -2.8f, 0);
    }

    public void HeartStart(int hC)
    {
        for (int i = 0; i < HeartB.childCount; i++)
        {
            HeartB.GetChild(i).gameObject.SetActive(false);
        }

        for (int i = 0; i < hC; i++)
        {
            HeartB.GetChild(i).gameObject.SetActive(true);
        }
    }
    IEnumerator DamageWait()
    {
        GM.noSpawn = true;
        yield return new WaitForSeconds(1.5f);
        GM.noSpawn = false;
    }
    
}