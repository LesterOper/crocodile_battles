using Library.Common.Purchases;
using Library.Data;
using UnityEngine;

namespace Library.Common
{
    [RequireComponent(typeof(MusicPlayer))]
    [RequireComponent(typeof(SoundPlayer))]
    [RequireComponent(typeof(ServicesHelper))]
    [RequireComponent(typeof(SavingHelper))]
    [RequireComponent(typeof(AdsHelper))]
    [RequireComponent(typeof(PurchaseTransactionController))]
    [RequireComponent(typeof(DataHelper))]
    [RequireComponent(typeof(ThreadHelper))]
    [RequireComponent(typeof(SceneData))]
    [DisallowMultipleComponent]
    public class Context : MonoBehaviour
    {
        public struct userAttributes { }
        public struct appAtributes { }
        private static Context instance;
        public static bool GDPRAccepted { get; set; }
        public static bool GDPRGeoData { get; set; }
        public static MusicPlayer MusicPlayer { get; private set; }
        public static SoundPlayer SoundPlayer { get; private set; }
        public static AudioSource AudioSource { get; set; }
        public static ServicesHelper ServicesHelper { get; private set; }
        public static SavingHelper SavingHelper { get; private set; }
        public static PurchaseTransactionController PremPurchaseHelper { get; private set; }
        public static AdsHelper AdsHelper { get; private set; }
        public static DataHelper DataHelper { get; private set; }
        public static ThreadHelper ThreadHelper { get; private set; }
        public static SceneData SceneData { get; private set; }
        
        public static bool BonusesTimersUpdated { get; set; }

        public bool Redundant { get; private set; }

        protected virtual void Awake()
        {
            if (instance != null && instance != this)
            {
                Redundant = true;
                Destroy(gameObject);
            }
            else
            {
                Redundant = false;
                instance = this;
                GDPRAccepted = false;
                MusicPlayer = GetComponent<MusicPlayer>();
                SoundPlayer = GetComponent<SoundPlayer>();
                AudioSource = GetComponent<AudioSource>();
                ServicesHelper = GetComponent<ServicesHelper>();
                SavingHelper = GetComponent<SavingHelper>();
                PremPurchaseHelper = GetComponent<PurchaseTransactionController>();
                AdsHelper = GetComponent<AdsHelper>();
                DataHelper = GetComponent<DataHelper>();
                ThreadHelper = GetComponent<ThreadHelper>();
                SceneData = GetComponent<SceneData>();
                
                DontDestroyOnLoad(gameObject);
            }
        }

        protected virtual void Start()
        {
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
        }

        protected virtual void OnSignedChanged(bool signed)
        {
        }
    }
}