using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Api;
using System;

public class Ad_Manager : MonoBehaviour
{
    private GameMaster gameMaster;
    private SaveLoad saveLoad;
    private int tries;
    private RewardedInterstitialAd rewardedInterstitialAd;

    private string rewardedAd_ID;

    void Start()
    {
        tries = 0;
        gameMaster = GameMaster.Instance;
        saveLoad = FindObjectOfType<SaveLoad>();

        // Reward Interstitial Android test ad units:
        rewardedAd_ID = "ca-app-pub-3940256099942544/5354046379";

        MobileAds.Initialize(initStatus => { });
        LoadRewardedInterstitialAd();
    }

    private void LoadRewardedInterstitialAd()
    {
        // Clean up the old ad before loading a new one.
        if (rewardedInterstitialAd != null)
        {
            rewardedInterstitialAd.Destroy();
            rewardedInterstitialAd = null;
        }

        Debug.Log("Loading the rewarded interstitial ad.");

        // Create an empty ad request.
        var adRequest = new AdRequest.Builder().Build();

        // send the request to load the ad.
        RewardedInterstitialAd.Load(rewardedAd_ID, adRequest,
            (RewardedInterstitialAd ad, LoadAdError error) =>
            {
                // if error is not null, the load request failed.
                if (error != null || ad == null)
                {
                    Debug.LogError("(Tries: " + tries + ") " + "rewarded interstitial ad failed to load an ad " +
                                   "with error : " + error);
                    if (tries >= 2)
                    {
                        return;
                    }
                    else
                    {
                        tries++;
                        LoadRewardedInterstitialAd();
                    }
                }

                tries = 0;
                Debug.Log("Rewarded interstitial ad loaded with response : "
                          + ad.GetResponseInfo());
                RegisterReloadHandler(ad);
                rewardedInterstitialAd = ad;
            });
    }

    public void ShowRewardedInterstitialAd()
    {
        const string rewardMsg =
        "Rewarded interstitial ad rewarded the user. Type: {0}, amount: {1}.";

        if (rewardedInterstitialAd != null && rewardedInterstitialAd.CanShowAd())
        {
            rewardedInterstitialAd.Show((Reward reward) =>
            {
                // Reward the user.
                Debug.Log(String.Format(rewardMsg, reward.Type, reward.Amount));
                UserEarnedReward();
            });
        }
    }

    private void UserEarnedReward()
    {
        // https://stackoverflow.com/questions/53916533/setactive-can-only-be-called-from-the-main-thread 
        // UI changes and setActive changes must be run on main thread. Apis with callbacks run on multiple threads.
        UnityMainThread.wkr.AddJob(() =>
        {
            gameMaster.AdRewardEarnedContinueGame();
        });
        rewardedInterstitialAd.Destroy(); // clean up to avoid memory leaks
        Debug.Log("User Earned Ad Reward. Continue Game.");
    }

    private void RegisterReloadHandler(RewardedInterstitialAd ad)
    {
        // Raised when the ad closed full screen content.
        ad.OnAdFullScreenContentClosed += () =>
        {
            Debug.Log("Rewarded interstitial ad full screen content closed.");

            // Reload the ad so that we can show another as soon as possible.
            LoadRewardedInterstitialAd();
        };
        // Raised when the ad failed to open full screen content.
        ad.OnAdFullScreenContentFailed += (AdError error) =>
        {
            Debug.LogError("Rewarded interstitial ad failed to open full screen content " +
                           "with error : " + error);

            // Reload the ad so that we can show another as soon as possible.
            LoadRewardedInterstitialAd();
        };
    }
}
