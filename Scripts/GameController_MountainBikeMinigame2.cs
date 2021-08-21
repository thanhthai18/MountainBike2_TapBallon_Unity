using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using DG.Tweening;

public class GameController_MountainBikeMinigame2 : MonoBehaviour
{
    public static GameController_MountainBikeMinigame2 instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
            Destroy(instance);
    }

    public RaycastHit2D[] hit;
    public int score = 0;
    public bool isWin = false;
    public int[] tmp;
    public GameObject mountainBike, theThief, theGuy, idea, rope;
    public Chain_MountainBikeMinigame2 ChainPrefab, theChains;
    public Camera mainCamera;
    public bool isHoldChain, isPlay;
    private float maxXCamera;
    private float maxYCamera;
    public Vector2 mouseCurrentPos;
    public Coroutine myCoroutine, myCoroutineFly;
    public GameObject tutorial;
    public Tween tut;


    private void Start()
    {        
        tutorial.SetActive(false);
        rope.SetActive(false);
        tutorial.transform.position = new Vector3(2.44f, -2.82f, 0);
        rope.transform.position = new Vector3(-2.97f, -2.32f, 0);
        rope.transform.eulerAngles = new Vector3(0, 0, -39.262f);
        rope.transform.localScale = new Vector3(0.39f, 1.5f, 1);
        isHoldChain = false;
        isPlay = false;
        maxXCamera = mainCamera.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height)).x;
        maxYCamera = mainCamera.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height)).y;
        SetSizeCamera();
        mountainBike.transform.DOMoveX(-4.83f, 3).OnComplete(() => { theGuy.transform.DOMoveX(theGuy.transform.position.x + 4, 2).OnComplete(SpawnChain); });
    }

    void SetSizeCamera()
    {
        float f1 = 16.0f / 9;
        float f2 = Screen.width * 1.0f / Screen.height;

        mainCamera.orthographicSize *= f1 / f2;
    }

    void SpawnChain()
    {
        theChains = Instantiate(ChainPrefab, new Vector3(2.4f, -1, 0), Quaternion.identity);
        myCoroutine = StartCoroutine(IdleChain());
        tutorial.SetActive(true);
        tut = tutorial.transform.DOMoveX(-3.65f, 2);
        tut.SetLoops(-1);
    }

    void DelayComebackChainPos()
    {
        if (theChains != null)
        {
            theChains.transform.position = new Vector3(2.4f, -1, 0);
            myCoroutine = StartCoroutine(IdleChain());
        }
    }

    public void VFXChain()
    {
        var VFX = mountainBike.transform.GetChild(0).gameObject;
        VFX.SetActive(true);
        VFX.transform.DOShakeScale(2).OnComplete(() =>
        {
            VFX.GetComponent<SpriteRenderer>().DOFade(0, 1).OnComplete(() =>
            {
                VFX.SetActive(false);
                theGuy.transform.DOScale(2.875f, 2).OnComplete(() =>
                {
                    theGuy.transform.DOMove(new Vector3(theGuy.transform.position.x - 0.6f, theGuy.transform.position.y + 0.8f), 2).SetEase(Ease.Linear).OnComplete(() =>
                    {
                        Destroy(theGuy);
                        ThiefAction();
                    });
                });

            });
            mountainBike.transform.GetChild(1).gameObject.SetActive(true);
        });
    }

    void VFXBalloon()
    {
        var VFX = mountainBike.transform.GetChild(0).gameObject;
        VFX.GetComponent<SpriteRenderer>().DOFade(1, 0.1f);
        VFX.SetActive(true);      
        VFX.transform.DOShakeScale(2).OnComplete(() =>
        {
            mountainBike.transform.GetChild(2).gameObject.SetActive(true);
            rope.SetActive(true);
            VFX.GetComponent<SpriteRenderer>().DOFade(0, 1).OnComplete(() =>
            {
                VFX.SetActive(false);                
                mountainBike.transform.DOMove(new Vector2(-0.1f, 0.2f), 1.5f);
                mountainBike.transform.DOScale(0.45f, 1.5f);
                rope.transform.DOMove(new Vector3(-0.33f, -2.29f, 0), 1.5f);
                rope.transform.DOScale(new Vector3(0.52f, 1.5f, 1), 1.5f);
                rope.transform.DORotate(new Vector3(0, 0, -99.39001f), 1.5f);
                theThief.transform.DOMove(new Vector2(0, -4.5f), 1.5f).OnComplete(()=> 
                {
                    isPlay = true;
                    myCoroutineFly = StartCoroutine(IdleFlying());
                    tutorial.transform.position = new Vector3(0.08f, 3.53f, 0);
                    tutorial.SetActive(true);
                    tutorial.transform.DOScale(1.5f, 1).SetLoops(-1);
                });
            });
        });
    }

    void ThiefAction()
    {
        theThief.transform.DOMoveX(-1.01f, 2.5f).OnComplete(() =>
        {
            theThief.transform.DOMoveY(-0.33f, 1f).OnComplete(() =>
            {
                Invoke(nameof(GetIdea), 1);
            });
        });
    }

    void TheifWhenEndGame()
    {
        theThief.transform.DORotate(new Vector3(0, 0, -90), 1);
    }

    void GetIdea()
    {
        idea.SetActive(true);
        idea.transform.DOScale(1.7f, 0.5f).OnComplete(() =>
        {
            idea.transform.DOScale(1.5f, 0.5f).SetEase(Ease.OutBack).OnComplete(() =>
            {
                Destroy(idea);
                theThief.transform.DOMoveX(theThief.transform.position.x - 1, 1).OnComplete(() =>
                {
                    VFXBalloon();
                    theThief.transform.DOMove(new Vector2(-0.97f, -3.21f), 1);
                });
            });
        });
    }

    IEnumerator IdleFlying()
    {
        while (!isWin)
        {
            mountainBike.transform.DOMoveY(mountainBike.transform.position.y + 0.1f, 0.5f);
            yield return new WaitForSeconds(0.5f);
            mountainBike.transform.DOMoveY(mountainBike.transform.position.y - 0.1f, 0.5f);
            yield return new WaitForSeconds(0.5f);
        }
    }
    IEnumerator IdleChain()
    {
        while (!isHoldChain && theChains != null)
        {
            theChains.transform.DOScale(new Vector3(0.7f, 1.2f, 0), 0.5f);
            yield return new WaitForSeconds(0.5f);
            theChains.transform.DOScale(new Vector3(0.5f, 1f, 0), 0.5f);
            yield return new WaitForSeconds(0.5f);
        }
    }


    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && !isWin)
        {
            mouseCurrentPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            hit = Physics2D.RaycastAll(mouseCurrentPos, Vector2.zero);
            if (hit.Length != 0)
            {
                if (hit[0].collider.gameObject.CompareTag("Balloon") && isPlay)
                {
                    if(tutorial != null)
                    {
                        Destroy(tutorial);
                    }
                    tmp = new int[hit.Length];
                    for (int j = 0; j < hit.Length; j++)
                    {
                        tmp[j] = hit[j].collider.gameObject.GetComponent<SpriteRenderer>().sortingOrder;
                    }
                    for (int i = 0; i < hit.Length; i++)
                    {
                        if (hit[i] && hit[i].collider != null)
                        {
                            if (hit[i].collider.gameObject.GetComponent<SpriteRenderer>().sortingOrder == Mathf.Max(tmp))
                            {
                                score++;
                                Destroy(hit[i].collider.gameObject);
                            }
                        }
                    }
                }

                if (hit[0].collider.gameObject.CompareTag("Path"))
                {
                    StopCoroutine(myCoroutine);
                    isHoldChain = true;
                    if (tutorial.activeSelf == true)
                    {
                        tutorial.SetActive(false);
                        tut.Kill();
                    }
                }
            }

        }
        if (Input.GetMouseButtonUp(0) && !isWin)
        {
            isHoldChain = false;
            Invoke(nameof(DelayComebackChainPos), 0.1f);

        }

        if (isHoldChain)
        {            
            mouseCurrentPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            mouseCurrentPos = new Vector2(Mathf.Clamp(mouseCurrentPos.x, -maxXCamera + 1.8f, maxXCamera - 1.8f), Mathf.Clamp(mouseCurrentPos.y, -maxYCamera + 1, maxYCamera - 1));
            theChains.transform.position = mouseCurrentPos;
        }

        if (score == 6)
        {
            StopCoroutine(myCoroutineFly);
            StartCoroutine(IdleFlying());
            rope.transform.DOMoveY(-2.7f, 1);
            rope.transform.DOScaleX(0.43f, 1);
            mountainBike.transform.DOMoveY(-3.83f/3, 1f).SetEase(Ease.Linear);
        }

        if (score == 10)
        {
            isWin = true;
            StopAllCoroutines();
            rope.transform.DOScaleX(0, 1f);
            mountainBike.transform.DOMoveY(-3.83f, 1f).SetEase(Ease.Linear);
            Invoke(nameof(TheifWhenEndGame), 0.8f);
        }
    }
}


