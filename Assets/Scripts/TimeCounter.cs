using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class TimeCounter : MonoBehaviourPunCallbacks
{
    public const string timer = "Timer";
    public const string duration = "Duration";
    public static TimeCounter Instance;
    public delegate void OnCountDownTimerExpire(Tile tile);
    public static event OnCountDownTimerExpire onCountDownTimerExpire;

    public bool timerRunning = false;
    private float startTime;
    
    [Header("Reference to a Text component for visualizing the countdown")]
    public Text timerText;

    [Header("Countdown time in seconds")]
    public float Countdown = 30f;
    public float countdownSpeed = 1f;
    float length;
    public RectTransform countdownRectTransform;
    double valueToShow;
    

    void Awake()
    {
        Instance = this;
    }



    void Update()
    {
        if (GameManager.Instance.blockersList.Count > 0)
        {
            if (!timerRunning)
            {              
                return;
            }

            float timer = (float)PhotonNetwork.Time - startTime;
            float countdown = Countdown - timer;

            Image blockerImg = GameManager.Instance.blockersList[0].timerImage;
            Debug.Log("the gamoeobject:::" + GameManager.Instance.blockersList[0].gameObject+":::"+ blockerImg.fillAmount);
            blockerImg.enabled = true;
            blockerImg.fillAmount = countdown / Countdown;

            //Text.text = string.Format("Game starts in {0} seconds", countdown.ToString("n0"));

            if (countdown > 0.0f)
            {
                return;
            }

            timerRunning = false;
            Debug.Log("the timer is false:::");
            blockerImg.fillAmount = 1;
            blockerImg.enabled = false;
            onCountDownTimerExpire?.Invoke(GameManager.Instance.blockersList[0]);
        }


        //double previousValue = valueToShow;
        //double timer = (double)PhotonNetwork.Time - startTime;
        //if (valueToShow==0)
        //{
        //    previousValue= timer - Mathf.Floor((float)timer);
        //}
        //valueToShow = timer - Mathf.Floor((float)timer);
        //Debug.Log("the value::" +(float)previousValue+":::"+(float)valueToShow);
        //if ((float)valueToShow <(float)previousValue)
        //{            
        //    countdown -=1;         
        //}
   
        //if (countdown > 0)
        //{
        //    timerText.gameObject.SetActive(true);
        //    timerText.text = Mathf.Ceil((float)countdown).ToString();
        //    countdownRectTransform.localScale = Vector3.one * (1.0f - ((float)valueToShow - Mathf.Floor((float)valueToShow)));
        //}
        //else
        //{
        //    countdownRectTransform.localScale = Vector3.zero;
        //}
        //if (countdown > 0)
        //    return;
        //onCountDownTimerExpire?.Invoke();
        //timerRunning = false;
    }

    public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
    {
        Debug.Log("the value for propertiesThatChanged" + propertiesThatChanged);
        object startTimeFromProps;
        

        if (propertiesThatChanged.TryGetValue(timer, out startTimeFromProps))
        {
            Debug.Log("the timer is truee:::");
            timerRunning = true;
            startTime = (float)startTimeFromProps;           
        }      
    }
}
