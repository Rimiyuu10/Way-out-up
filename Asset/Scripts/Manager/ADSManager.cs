using UnityEngine;
using GoogleMobileAds.Api;

public class ADSManager : MonoBehaviour
{
    public static ADSManager instance;

    public string appId = "ca-app-pub-4566741325318960~5980484445";

#if UNITY_ANDROID
    string bannerId = "ca-app-pub-4566741325318960/9321264979";
    string interId = "ca-app-pub-4566741325318960~5980484445";
    string rewardedId = "ca-app-pub-4566741325318960/8315606387";

#elif UNITY_IPHONE
    string bannerId = "ca-app-pub-3940256099942544/2934735716";
    string interId = "ca-app-pub-3940256099942544/4411468910";
    string rewardedId = "ca-app-pub-3940256099942544/1712485313";

#endif

    BannerView bannerView;
    InterstitialAd interstitialAd;
    RewardedAd rewardedAd;

    private void Start()
    {
        instance = this;

        MobileAds.RaiseAdEventsOnUnityMainThread = true;
        MobileAds.Initialize(initStatus => {

            print("Ads Initialised !!");

        });
    }

    #region Banner

    public void LoadBannerAd()
    {
        // Tạo 1 banner
        CreateBannerView();

        // Lắng nghe các sự kiện của banner
        ListenToBannerEvents();

        // Tải banner
        if (bannerView == null)
        {
            CreateBannerView();
        }

        var adRequest = new AdRequest();
        adRequest.Keywords.Add("unity-admob-sample");

        print("Loading banner Ad !!");
        bannerView.LoadAd(adRequest);// Hiển thị banner trên màn hình
    }

    void CreateBannerView()
    {
        if (bannerView != null)
        {
            DestroyBannerAd();
        }
        bannerView = new BannerView(bannerId, AdSize.Banner, AdPosition.Bottom);
    }

    void ListenToBannerEvents()
    {
        bannerView.OnBannerAdLoaded += () =>
        {
            Debug.Log("Banner view loaded an ad with response : "
                + bannerView.GetResponseInfo());
        };
        // Được gọi khi banner view tải một quảng cáo với phản hồi
        bannerView.OnBannerAdLoadFailed += (LoadAdError error) =>
        {
            Debug.LogError("Banner view failed to load an ad with error : "
                + error);
        };
        // Được gọi khi quảng cáo không tải được vào banner view
        bannerView.OnAdPaid += (AdValue adValue) =>
        {
            Debug.Log("Banner view paid {0} {1}." +
                adValue.Value +
                adValue.CurrencyCode);
        };
        // Được gọi khi quảng cáo được ước tính đã kiếm được tiền
        bannerView.OnAdImpressionRecorded += () =>
        {
            Debug.Log("Banner view recorded an impression.");
        };
        // Được gọi khi có một lần hiển thị được ghi lại cho quảng cáo
        bannerView.OnAdClicked += () =>
        {
            Debug.Log("Banner view was clicked.");
        };
        // Được gọi khi có một lần click được ghi lại cho quảng cáo
        bannerView.OnAdFullScreenContentOpened += () =>
        {
            Debug.Log("Banner view full screen content opened.");
        };
        // Được gọi khi quảng cáo mở nội dung toàn màn hình
        bannerView.OnAdFullScreenContentClosed += () =>
        {
            Debug.Log("Banner view full screen content closed.");
        };
        // Được gọi khi quảng cáo đóng nội dung toàn màn hình
    }

    public void DestroyBannerAd()
    {
        if (bannerView != null)
        {
            print("Destroying banner Ad");
            bannerView.Destroy();
            bannerView = null; // Hủy banner ad nếu nó tồn tại
        }
    }
    #endregion

    #region Interstitial

    public void LoadInterstitialAd()
    {
        if (interstitialAd != null)
        {
            interstitialAd.Destroy();
            interstitialAd = null;
        }
        var adRequest = new AdRequest();
        adRequest.Keywords.Add("unity-admob-sample");

        
        InterstitialAd.Load(interId, adRequest, (InterstitialAd ad, LoadAdError error) =>
        {
            if (error != null || ad == null)
            {
                print("Interstitial ad failed to load" + error);
                return;
            }

            print("Interstitial ad loaded !!" + ad.GetResponseInfo());

            interstitialAd = ad;
            InterstitialEvent(interstitialAd);
        });
        
    }

    public void ShowInterstitialAd()
    {
        if (interstitialAd != null && interstitialAd.CanShowAd())
        {
            interstitialAd.Show();
        }
        else
        {
            print("Intersititial ad not ready!!");
        }
    }

    public void InterstitialEvent(InterstitialAd ad)
    {
        // Được gọi khi quảng cáo được ước tính đã kiếm được tiền
        ad.OnAdPaid += (AdValue adValue) =>
        {
            Debug.Log("Interstitial ad paid {0} {1}." +
                adValue.Value +
                adValue.CurrencyCode);
        };
        // Được gọi khi có một lần hiển thị được ghi lại cho quảng cáo
        ad.OnAdImpressionRecorded += () =>
        {
            Debug.Log("Interstitial ad recorded an impression.");
        };
        // Được gọi khi có một lần click được ghi lại cho quảng cáo
        ad.OnAdClicked += () =>
        {
            Debug.Log("Interstitial ad was clicked.");
        };
        // Được gọi khi quảng cáo mở nội dung toàn màn hình
        ad.OnAdFullScreenContentOpened += () =>
        {
            Debug.Log("Interstitial ad full screen content opened.");
        };
        // Được gọi khi quảng cáo đóng nội dung toàn màn hình
        ad.OnAdFullScreenContentClosed += () =>
        {
            Debug.Log("Interstitial ad full screen content closed.");
        };
        // Được gọi khi quảng cáo thất bại khi mở nội dung toàn màn hình
        ad.OnAdFullScreenContentFailed += (AdError error) =>
        {
            Debug.LogError("Interstitial ad failed to open full screen content " +
                           "with error : " + error);
        };
    }

    #endregion

    #region Rewarded

    public void LoadRewardedAd()
    {
        if (rewardedAd != null)
        {
            rewardedAd.Destroy();
            rewardedAd = null;
        }
        var adRequest = new AdRequest();
        adRequest.Keywords.Add("unity-admob-sample");

        RewardedAd.Load(rewardedId, adRequest, (RewardedAd ad, LoadAdError error) =>
        {
            if (error != null || ad == null)
            {
                print("Rewarded failed to load" + error);
                return;
            }

            print("Rewarded ad loaded !!");
            rewardedAd = ad;
            RewardedAdEvents(rewardedAd);
        });
    }

    public void ShowRewardedAd()
    {
        if (rewardedAd != null && rewardedAd.CanShowAd())
        {
            rewardedAd.Show((Reward reward) =>
            {
                print("Give reward to player !!");

                GameManager.instance.SkipLv();

            });
        }
        else
        {
            print("Rewarded ad not ready");
        }
    }

    public void RewardedAdEvents(RewardedAd ad)
    {
        // Được gọi khi quảng cáo được ước tính đã kiếm được tiền
        ad.OnAdPaid += (AdValue adValue) =>
        {
            Debug.Log("Rewarded ad paid {0} {1}." +
                adValue.Value +
                adValue.CurrencyCode);
        };
        // Được gọi khi có một lần hiển thị được ghi lại cho quảng cáo
        ad.OnAdImpressionRecorded += () =>
        {
            Debug.Log("Rewarded ad recorded an impression.");
        };
        // Được gọi khi có một lần click được ghi lại cho quảng cáo
        ad.OnAdClicked += () =>
        {
            Debug.Log("Rewarded ad was clicked.");
        };
        // Được gọi khi quảng cáo mở nội dung toàn màn hình
        ad.OnAdFullScreenContentOpened += () =>
        {
            Debug.Log("Rewarded ad full screen content opened.");
        };
        // Được gọi khi quảng cáo đóng nội dung toàn màn hình
        ad.OnAdFullScreenContentClosed += () =>
        {
            Debug.Log("Rewarded ad full screen content closed.");
        };
        // Được gọi khi quảng cáo thất bại khi mở nội dung toàn màn hình
        ad.OnAdFullScreenContentFailed += (AdError error) =>
        {
            Debug.LogError("Rewarded ad failed to open full screen content " +
                           "with error : " + error);
        };
    }

    #endregion
}
