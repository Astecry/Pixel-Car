using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public bool EndGameBool;
    public Material[] BG;
    public float StoneS, RoadS, DirtS;

    public GameObject PanelPause;
    public GameObject PanelGame;
    public GameObject PanelMenu;
    public GameObject PanelAds;

    Animator PauseAnim;
    Animator gameAnim;
    Animator MenuAnim;
    Animator AdsAnim;


    public Text scorNum;
    public Text scorBTxt;
    public Text scorBNum;

    public GameObject scorGo;
    private HeartBeatAnimationEffect heartEffect;

    public CarController CarController;

    public Text ScorText;
    public float Scor = 0;
    float bestScor;

    public Image adsSkipImg;
    bool adsFill = false;
    bool skillFill = false;
    [SerializeField] float fillSpeed;
    float adscurrentAmount;
    float skillcurrentAmount;
    public Transform content;
    public GameObject[] ShopCar;

    public GameObject skill;
    public Image skillImage;

    public bool ShopBoolean = false;

    private int twoX = 1;

    public Text coinTxt;
    public int coin;
    private int gcoin;
    private bool coinbool = true;

    public GameObject mainCarGO;
    private SpriteRenderer mainCarSP;
    private PolygonCollider2D mainCarColider;
    public bool pause = false;

    public GameObject[] menuGo;
    public GameObject[] pauseGo;

    public EnemyControl enemyControl;
    public AudioSource music;
    public bool musicActive;
    public Image musicBtnImg;
    public Sprite musicSpOn, musicSpOff;

    [HideInInspector] public int heartCount;
    public Animator warningAnim;
    public bool noSpawn=false;

    private void Awake()
    {
        PauseAnim = PanelPause.GetComponent<Animator>();
        gameAnim = PanelGame.GetComponent<Animator>();
        MenuAnim = PanelMenu.GetComponent<Animator>();
        AdsAnim = PanelAds.GetComponent<Animator>();
        bestScor = PlayerPrefs.GetInt("BestScor");
        scorBNum.text = bestScor.ToString();
        heartEffect = scorGo.GetComponent<HeartBeatAnimationEffect>();
        coin = PlayerPrefs.GetInt("Coin");
        mainCarSP = mainCarGO.GetComponent<SpriteRenderer>();
        mainCarColider = mainCarGO.GetComponent<PolygonCollider2D>();
        if (PlayerPrefs.GetInt("music") == 1 || PlayerPrefs.GetInt("music") == null)
        {
            musicActive = true;
            music.enabled = true;
            musicBtnImg.sprite = musicSpOn;
        }
        else
        {
            musicActive = false;
            music.enabled = false;
            musicBtnImg.sprite = musicSpOff;
        }
    }

    private void Start()
    {
        DisableMenu(pauseGo);
    }


    public void PauseBtn()
    {
        pause = true;
        MenuAnim.SetBool("durum", true);
        gameAnim.SetBool("durum", false);
        PauseAnim.SetBool("durum", true);
        DisableMenu(menuGo);
        EnableMenu(pauseGo);
    }

    public void PauseBtnB()
    {
        MenuAnim.SetBool("durum", false);
        gameAnim.SetBool("durum", true);
        PauseAnim.SetBool("durum", false);
        StartCoroutine(PauseC());
    }

    IEnumerator PauseC()
    {
        yield return new WaitForSeconds(.5f);
        pause = false;
        DisableMenu(pauseGo);
    }

    public void Resume()
    {
        Time.timeScale = 1;
        PauseAnim.SetBool("Pause", false);
    }

    private void LateUpdate()
    {
        if (!pause)
        {
            Repeating(BG);
        }

        if (CarController.Lives && !pause)
        {
            if (skillFill)
            {
                twoX = 2;
            }

            Scor += twoX * Time.deltaTime * 0.4f;
            ScorText.text = Scor.ToString("0");

            if (skillFill)
            {
                if (skillcurrentAmount < 100)
                {
                    skillcurrentAmount += fillSpeed / 3 * Time.deltaTime;
                    skillImage.fillAmount = skillcurrentAmount / 100;
                }
                else
                {
                    TwoXScorExit();
                }
            }
        }

        if (adsFill)
        {
            if (adscurrentAmount < 100)
            {
                adscurrentAmount += fillSpeed * Time.deltaTime;
                adsSkipImg.fillAmount = adscurrentAmount / 100;
            }
            else
            {
                AdsSkip();
                adsFill = false;
            }
        }


        if (coinbool)
        {
            StartCoroutine(AnimTxtCoin(Mathf.RoundToInt(10 + coin + Scor / 2)));
        }
    }

    void Repeating(Material[] Mat)
    {
        float Speed = RoadS;
        for (int i = 0; i < Mat.Length; i++)
        {
            if (Mat[i].name == "Stone")
            {
                Speed = StoneS;
            }

            if (Mat[i].name == "Dirt" || Mat[i].name == "Dirt 1")
            {
                Speed = DirtS;
            }

            if (Mat[i].name == "Road")
            {
                Speed = RoadS;
            }

            Vector2 SetOffset = new Vector2(0, Speed * Time.time * 0.1f);
            Mat[i].SetTextureOffset("_MainTex", SetOffset);
        }
    }

    void PanelExit()
    {
        gameObject.SetActive(false);
    }

    public void Bonus()
    {
        Scor += 8;
    }

    public void BtnPlay()
    {
        enemyControl.Remove();
        PanelGame.SetActive(true);
        MenuAnim.SetBool("durum", false);
        gameAnim.SetBool("durum", true);
        CarController.Lives = true;
        CarController.PetrolReload();
        Invoke("StartGame", 1.5f);
        heartEffect.enabled = false;
        enemyControl.ResetSpawnS();
        HeartStart(heartCount);
    }

    public void BtnShop()
    {
        MenuAnim.SetBool("Shop", true);
        StartCoroutine(ShopC());
    }

    IEnumerator ShopC()
    {
        yield return new WaitForSeconds(.50f);
        ShopBoolean = true;
    }

    public void BtnShopE()
    {
        MenuAnim.SetBool("Shop", false);
        ShopBoolean = false;
    }

    public void Ads()
    {
        AdsAnim.SetBool("durum", true);
        adsSkipImg.fillAmount = 0f;
        adscurrentAmount = 0;
        StartCoroutine(adsFillStarted());
    }

    IEnumerator adsFillStarted()
    {
        yield return new WaitForSeconds(0.35f);
        adsFill = true;
    }

    public void AdsSkip()
    {
        adsFill = false;
        AdsAnim.SetBool("durum", false);
        gameOver();
    }

    void StartGame()
    {
        EndGameBool = true;
        Scor = 0;
    }

    public void EndGame()
    {
        gameAnim.SetBool("durum", false);
        EndGameBool = false;
        CarController.Lives = false;
        if (Scor > 0)
        {
            scorGo.SetActive(true);
            scorNum.text = Scor.ToString("0");
            if (Scor > bestScor)
            {
                PlayerPrefs.SetInt("BestScor", Mathf.RoundToInt(Scor));
                bestScor = Scor;
                scorBTxt.text = "Best";
                scorBNum.text = bestScor.ToString("0");
                heartEffect.enabled = true;
            }
            else
            {
                heartEffect.enabled = false;
            }
        }
        else
        {
            scorGo.SetActive(false);
            scorBTxt.text = "BestScor";
        }

        TwoXScorExit();
    }

    public void gameOver()
    {
        MenuAnim.SetBool("durum", true);
        EnableMenu(menuGo);
        coinbool = true;
    }

    public bool CheckInternet()
    {
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    public void TwoXScor()
    {
        skill.SetActive(true);
        skillFill = true;
        skillcurrentAmount = 0;
    }

    void TwoXScorExit()
    {
        skill.SetActive(false);
        skillFill = false;
        twoX = 1;
    }

    public IEnumerator AnimTxtCoin(int targetValue)
    {
        yield return new WaitForSeconds(0.8f);
        gcoin = Mathf.RoundToInt(Mathf.Lerp(gcoin, targetValue, 0.15f));
        coinTxt.text = gcoin.ToString("0");
        yield return new WaitForSeconds(.01f);
        coin = targetValue;
        coinTxt.text = coin.ToString("0");
        PlayerPrefs.SetInt("Coin", coin);
        coinbool = false;
    }

    public void ChangeCar(Sprite carSp)
    {
        mainCarSP.sprite = carSp;

        for (int i = 0; i < mainCarColider.pathCount; i++) mainCarColider.SetPath(i, null);
        mainCarColider.pathCount = carSp.GetPhysicsShapeCount();

        List<Vector2> path = new List<Vector2>();
        for (int i = 0; i < mainCarColider.pathCount; i++)
        {
            path.Clear();
            carSp.GetPhysicsShape(i, path);
            mainCarColider.SetPath(i, path.ToArray());
        }
    }

    void DisableMenu(GameObject[] Golist)
    {
        foreach (GameObject GO in Golist)
        {
            GO.SetActive(false);
        }
    }

    void EnableMenu(GameObject[] Golist)
    {
        foreach (GameObject GO in Golist)
        {
            GO.SetActive(true);
        }
    }

    public void RePlayBtn()
    {
        PauseAnim.SetBool("durum", false);
        pause = false;
        BtnPlay();
    }

    public void MusicBtn()
    {
        if (musicActive)
        {
            music.enabled = false;
            musicActive = false;
            PlayerPrefs.SetInt("music", 0);
            musicBtnImg.sprite = musicSpOff;
        }
        else
        {
            music.enabled = true;
            musicActive = true;
            PlayerPrefs.SetInt("music", 1);
            musicBtnImg.sprite = musicSpOn;
        }
    }

    public void ExitBtn()
    {
        MenuAnim.SetBool("exit",true);
    }
    public void ExitBtnYes()
    {
        Application.Quit();
    }
    public void ExitBtnNo()
    {
        MenuAnim.SetBool("exit",false);
    }

    public void HeartStart(int hC)
    {
        CarController.HeartStart(hC);
        CarController.HeartCount = hC;
    }

    public void WarningT()
    {
        warningAnim.SetBool("durum",true);
    }
    
    public void WarningF()
    {
        warningAnim.SetBool("durum",false);
    }
}