using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
//using static UnityEditor.Progress;

public class Inventory_Controller : MonoBehaviour
{
    // g_fCharacterSpeed -> g는 글로벌(public) m은 멤버(private) 뒤의 f(float)/i(int)/s(string)
    [Header("인벤토리 온오프를 위한 부모 변수 받아옴")]
    public GameObject g_gin_V;
    [Header("실제 인벤토리")]
    public GameObject g_ginventory;
    public static Inventory_Controller g_ICinstance;

    [Header("인벤토리 슬롯")]
    public List<Slot> g_Sslot; // 인벤토리 안에 있는 각 슬롯들
    public List<int> item_Num_Sort; // 인벤토리 안에 있는 각 슬롯들


    //public Slot[] g_Sslot; // 인벤토리 안에 있는 각 슬롯들
    public Slot g_Sclick_Slot; // 버릴 슬롯
    public ItemEntity g_Iget_Item; // 획득한 아이템

    public Slot g_Sselect_Item; // 현재 인벤토리에서 선택한 오브젝트 정보
    //public Miri_Slot select_Item_Miri; // 미리보기 인벤토리에서 선택한 오브젝트 정보
    public GameObject g_gselect_Item_Ob;

    public ItemEntity g_Iclick_Item; // 클릭한 아이템s
    public int g_iclick_Item_Count; // 클릭한 아이템 갯수

    public bool invent_On_Off_Check; // 인벤토리가 켜져있는지 꺼져있는지 확인해주는 변수
    public bool lock_UI; // 특수한 UI가 켜져있을때 다른것 건들지 못하게 함
    public GameObject discard_value_View; // 버릴 개수 입력 창 띄우기

    [Header("인벤토리 오른쪽 표시 UI")]
    public Image Img_View;
    public TextMeshProUGUI name_View;
    public TextMeshProUGUI Des_View;
    public GameObject UseButton;
    // Start is called before the first frame update

    private void Awake()
    {
        g_ICinstance = this;
        
    }
    private void Start()
    {
        lock_UI = true;
        //g_Sslot = new Slot[g_ginventory.transform.childCount];
        for (int i = 0; i < g_ginventory.transform.childCount; i++)  // 유니티 창에서 슬롯을 넣어주는게 아니고 스크립트에서 넣어주는거
        {
            g_Sslot.Add(g_ginventory.transform.GetChild(i).GetComponent<Slot>()); // 유니티상에서 인벤토리라는 오브젝트 안에 슬롯들이 있기때문에 그 슬롯들을 가져와서 배열에 넣어줌
            g_Sslot[i].g_iitem_Number = 0;
        }
        g_gin_V.transform.localScale = new Vector3(0, 0, 1);
    }
    // Update is called once per frame
    void Update()
    {
        View_Inventory();
        if (invent_On_Off_Check)
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                item_Num_Sort.Clear();

                for (int i = 0; i < g_Sslot.Count; i++)
                {
                    if (g_Sslot[i].g_Ihave_item != null)
                    {
                        item_Num_Sort.Add(g_Sslot[i].g_Ihave_item.number);
                    }
                }
                item_Num_Sort.Sort();



                for (int i = 0; i < item_Num_Sort.Count; i++)
                {
                    for (int g = 0; g < g_Sslot.Count; g++)
                    {
                        if (g_Sslot[g].g_Ihave_item != null)
                        {
                            if (item_Num_Sort[i] == g_Sslot[g].g_Ihave_item.number)
                            {
                                // Slot temp = g_Sslot[i];
                                Slot temp = new Slot();
                                temp.g_iitem_Image = g_Sslot[i].g_iitem_Image;
                                temp.g_snull_item_Image = g_Sslot[i].g_snull_item_Image;
                                temp.g_Ihave_item = g_Sslot[i].g_Ihave_item;
                                temp.g_iitem_Number = g_Sslot[i].g_iitem_Number;

                                g_ginventory.transform.GetChild(i).gameObject.GetComponent<Slot>().g_iitem_Image.sprite = g_Sslot[g].g_iitem_Image.sprite;
                                g_ginventory.transform.GetChild(i).gameObject.GetComponent<Slot>().g_snull_item_Image = g_Sslot[g].g_snull_item_Image;
                                g_ginventory.transform.GetChild(i).gameObject.GetComponent<Slot>().g_Ihave_item = g_Sslot[g].g_Ihave_item;
                                g_ginventory.transform.GetChild(i).gameObject.GetComponent<Slot>().g_iitem_Number = g_Sslot[g].g_iitem_Number;

                                g_Sslot[g].g_iitem_Image.sprite = temp.g_iitem_Image.sprite;
                                g_Sslot[g].g_snull_item_Image = temp.g_snull_item_Image;
                                g_Sslot[g].g_Ihave_item = temp.g_Ihave_item;
                                g_Sslot[g].g_iitem_Number = temp.g_iitem_Number;
                                temp = null;
                                break;
                            }
                        }
                        else
                        {

                        }
                    }
                }
 
            }
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            if (discard_value_View.activeSelf == false)
            {
                {
                    discard_value_View.SetActive(true);
                    lock_UI = false;
                }
            }
            else
            {
                discard_value_View.SetActive(false);
                lock_UI = true;
            }
        }

    }

    public void Check_Slot(int num = 1) // 획득한 아이템을 인벤토리에 넣어주는 함수
    {
        ItemEntity item = null; // 획득한 아이템이 인벤토리에 있는지 없는지를 판단해주는 변수
        foreach (Slot slot_B in g_Sslot)
        {
            if (slot_B != null && slot_B.g_Ihave_item != null)  // 인벤토리가 비어있지 않다면
            {
                if (slot_B.g_Ihave_item.m_sItemName == g_Iget_Item.m_sItemName) // 현재 가지고 있는 아이템과 인벤토리에 있는 아이템이 같다면
                {
                    item = g_Iget_Item; // 변수에 현재 가지고 있는 아이템을 넣어줌
                    break;
                }
            }
        }
        Put_Invent(item, num);
    }

    public void Put_Invent(ItemEntity item, int num = 1)
    {
        for (int i = 0; i <= g_Sslot.Count; i++)
        {
            if (item == null) //인벤토리에 현재 가지고 있는 아이템이 없다면
            {
                if (g_Sslot[i].g_Ihave_item == null) // 인벤토리가 비어있을때
                {
                    g_Sslot[i].Input_Item(g_Iget_Item, num); // 비어있는 슬롯에 현재 아이템을 넣어줌
                    break;
                }
                else
                {
                    continue;
                }
            }
            else
            {
                if (g_Sslot[i].g_Ihave_item != null) // 인벤토리가 비어있지 않다면
                {
                    if (g_Sslot[i].g_Ihave_item.m_sItemName == item.m_sItemName) // 인벤토리에 현재 가지고 있는 아이템이 있다면
                    {
                        g_Sslot[i].Input_Item(g_Iget_Item, num); // 슬롯에 아이템을 넣어줌
                        break;
                    }
                    else
                    {
                        continue;
                    }
                }
                else
                {
                    continue;
                }
            }
        }
    }
    public void View_Inventory() // 인벤토리 온 오프 함수
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            if (GameManager.Instance.g_GameState == GameManager.GameState.BATTLE)
            {
                if(GameObject.Find("BattleManager") == null)
                {
                    GameManager.Instance.g_GameState = GameManager.GameState.INPROGRESS;
                }
                return;
            }
               
            if (g_gin_V.transform.localScale == new Vector3(1, 1, 1)) // 인벤토리가 켜있으면
            {
                Hide_Inv();
            }
            else if (g_gin_V.transform.localScale == new Vector3(0, 0, 1))
            {
                Change_UI.instance.ALL_OFF_UI();
                Show_Inv();
            }
        }
    }

    public void Set_GetItem(GameObject itemEntity)
    {
        GameObject entity = Instantiate(itemEntity,GameObject.Find("Inventory_GOs").transform);
        Destroy(entity.transform.GetComponent<SpriteRenderer>());
        Destroy(entity.transform.GetComponent<Collider2D>());
        g_Iget_Item = entity.GetComponent<ItemEntity>();
    }
    public void Show_Inv()
    {
        Img_View.gameObject.SetActive(false);
        name_View.text = " ";
        Des_View.text = " ";
        invent_On_Off_Check = true;
        g_gin_V.transform.localScale = new Vector3(1, 1, 1);
    }
    public void Hide_Inv()
    {
        for (int i = 0; i < g_ginventory.transform.childCount; i++)  // 유니티 창에서 슬롯을 넣어주는게 아니고 스크립트에서 넣어주는거
        {
            g_Sslot[i].GetComponent<Slot_Button>().Off_Inven();
        }
        g_gin_V.transform.localScale = new Vector3(0, 0, 1); // 꺼줌
        invent_On_Off_Check = false;

        if (GameManager.Instance.g_GameState == GameManager.GameState.BATTLE)
        {
            GameObject.Find("BattleManager").transform.GetComponent<BattleManager>().state = BattleManager.BattleState.ACTION;
        }
        UseButton.SetActive(false);
    }
}
