using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Scroll : MonoBehaviour
{
    public RectTransform Content;
    public float SmoothSpeed = 0.125f;
    bool Btn;
    Vector2 SPos;
    Vector2 NewPos;
    private Vector2 startTouchPos, endTouchPos;
    public GameManager GM;
    public Text btnStatus, btnprice;
    public GameObject[] ShopCar;
    int itemIndexID = 0;
    public Text sayi;
    private Car car,selectedCar;
   // public PolygonCollider2D carColider;
 //   private PolygonCollider2D selectColider;
    public Sprite disBtn, enBtn;
    public Image btnimg;

    public void Start()
    {
        Shop();
        CheckData();
        for (int i = 0; i < ShopCar.Length; i++)
        {
            
            if (ShopCar[i].GetComponent<CarProperty>().car.selected)
            {
                selectedCar = ShopCar[i].GetComponent<CarProperty>().car;
                GM.ChangeCar(selectedCar.img);
                GM.HeartStart(selectedCar.heartCount);
                GM.heartCount = selectedCar.heartCount;
            }
        }
    }

    public void BtnNext()
    {
        if (-Content.sizeDelta.x < Content.position.x && !Btn)
        {
            NewPos = Content.position + new Vector3(-1030, 0, 0);
            Btn = true;
            itemIndexID++;
            CheckData();
        }
    }

    public void Pre()
    {
        if (0 > Content.position.x && !Btn)
        {
            NewPos = Content.position + new Vector3(1030, 0, 0);
            Btn = true;
            itemIndexID--;
            CheckData();
        }
    }

    private void Update()
    {
        if (GM.ShopBoolean)
        {
            CheckTouch();
        }

        if (Btn)
        {
            StartCoroutine(BtnLearp());
        }

        sayi.text = itemIndexID.ToString();
    }

    void CheckTouch()
    {
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            startTouchPos = Input.GetTouch(0).position;
        }

        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended)
        {
            endTouchPos = Input.GetTouch(0).position;

            if (endTouchPos.x + 150 < startTouchPos.x)
            {
                Debug.Log("nnnnnnn");
                //left
                BtnNext();
            }

            if (endTouchPos.x - 150 > startTouchPos.x)
            {
                Debug.Log("preee");
                //right
                Pre();
            }
        }
    }

    IEnumerator BtnLearp()
    {
        SPos = Vector2.Lerp(Content.position, NewPos, SmoothSpeed);
        Content.position = SPos;
        Vector2 New = NewPos;
        yield return new WaitForSeconds(0.05f);
        Content.position = New;
        Btn = false;
    }

    void Shop()
    {
        for (int i = 0; i < ShopCar.Length; i++)
        {
            Instantiate(ShopCar[i], transform);
        }
    }

    void BuyState()
    {
        btnStatus.text = "Buy";
    }

    void SelectState()
    {
        btnStatus.text = "Select";
    }

    void CheckData()
    {
        car = ShopCar[itemIndexID].GetComponent<CarProperty>().car;
   //     selectColider = ShopCar[itemIndexID].GetComponent<CarProperty>().colider;
        if (car.selected)
        {
            btnprice.gameObject.SetActive(false);
            btnStatus.gameObject.SetActive(true);
            btnStatus.text = "Selected";
            btnimg.sprite = disBtn;
        }
        else
        {
            if (!car.purchased)
            {
                btnprice.gameObject.SetActive(true);
                btnStatus.gameObject.SetActive(false);
                btnprice.text = car.price.ToString();
                if (car.price <= GM.coin)
                {
                    btnimg.sprite = enBtn;
                }
                else
                {
                    btnimg.sprite = disBtn;
                }
            }
            else
            {
                SelectState();
                btnprice.gameObject.SetActive(false);
                btnStatus.gameObject.SetActive(true);
                btnimg.sprite = enBtn;
            }
        }
    }

    public void Button()
    {
        if (car.purchased)
        {
            Select();
        }
        else if (car.price <= GM.coin)
        {
            car.purchased = true;
            int coinvalue= GM.coin = GM.coin - car.price;
            Select();
            StartCoroutine(GM.AnimTxtCoin(coinvalue));

        }
        CheckData();
    }

    public void Select()
    {
        selectedCar.selected = false;
        car.selected = true;
        selectedCar = car;
        GM.ChangeCar(selectedCar.img);
        GM.HeartStart(selectedCar.heartCount);
        GM.heartCount = selectedCar.heartCount;
    }
}