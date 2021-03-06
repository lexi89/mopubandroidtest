﻿using UnityEngine;
using System;
using System.Collections.Generic;


#if UNITY_IPHONE || UNITY_ANDROID
using MoPubReward = MoPubManager.MoPubReward;
#if UNITY_IPHONE
using MP = MoPubBinding;
#elif UNITY_ANDROID
using MPBanner = MoPubAndroidBanner;
using MPInterstitial = MoPubAndroidInterstitial;
using MPRewardedVideo = MoPubAndroidRewardedVideo;
#endif


public class MoPubMediationSetting : Dictionary<string,object>
{
	public MoPubMediationSetting (string adVendor)
	{
		this.Add ("adVendor", adVendor);
	}
}

public class MoPubRewardedNetwork 
{
	private readonly string name;

	private MoPubRewardedNetwork(string name) {
        this.name = name;
    }

    public override string ToString() {
        return name;
    }

	#if UNITY_IPHONE
	public static readonly MoPubRewardedNetwork AdColony = new MoPubRewardedNetwork("AdColonyRewardedVideoCustomEvent");
	public static readonly MoPubRewardedNetwork AdMob = new MoPubRewardedNetwork("MPGoogleAdMobRewardedVideoCustomEvent");
	public static readonly MoPubRewardedNetwork Chartboost = new MoPubRewardedNetwork("ChartboostRewardedVideoCustomEvent");
	public static readonly MoPubRewardedNetwork Facebook = new MoPubRewardedNetwork("FacebookRewardedVideoCustomEvent");
	public static readonly MoPubRewardedNetwork Tapjoy = new MoPubRewardedNetwork("TapjoyRewardedVideoCustomEvent");
	public static readonly MoPubRewardedNetwork Unity = new MoPubRewardedNetwork("UnityAdsRewardedVideoCustomEvent");
	public static readonly MoPubRewardedNetwork Vungle = new MoPubRewardedNetwork("VungleRewardedVideoCustomEvent");
	#elif UNITY_ANDROID
	public static readonly MoPubRewardedNetwork AdColony =
		new MoPubRewardedNetwork("com.mopub.mobileads.AdColonyRewardedVideo");
	public static readonly MoPubRewardedNetwork AdMob =
		new MoPubRewardedNetwork("com.mopub.mobileads.GooglePlayServicesRewardedVideo");
	public static readonly MoPubRewardedNetwork Chartboost =
		new MoPubRewardedNetwork("com.mopub.mobileads.ChartboostRewardedVideo");
	public static readonly MoPubRewardedNetwork Facebook =
		new MoPubRewardedNetwork("com.mopub.mobileads.FacebookRewardedVideo");
	public static readonly MoPubRewardedNetwork Tapjoy =
		new MoPubRewardedNetwork("com.mopub.mobileads.TapjoyRewardedVideo");
	public static readonly MoPubRewardedNetwork Unity =
		new MoPubRewardedNetwork("com.mopub.mobileads.UnityRewardedVideo");
	public static readonly MoPubRewardedNetwork Vungle =
		new MoPubRewardedNetwork("com.mopub.mobileads.VungleRewardedVideo");
	#endif	
}

public static class MoPub
{
	public const string PLUGIN_VERSION = "4.17.0";
	public const double LAT_LONG_SENTINEL = 99999.0;
	public const string ADUNIT_NOT_FOUND_MSG = "AdUnit {0} not found: no plugin was initialized";

	private static string _sdkName;

	#if UNITY_IPHONE
	private static Dictionary<string, MP> _pluginsDict = new Dictionary<string, MP> ();

	public static void loadPluginsForAdUnits (string[] adUnitIds)
	{
		foreach (string adUnitId in adUnitIds) {
			_pluginsDict.Add (adUnitId, new MP (adUnitId));
		}
		Debug.Log (adUnitIds.Length + " AdUnits loaded for plugins:\n" + string.Join (", ", adUnitIds));
	}
	#elif UNITY_ANDROID
	private static Dictionary<string, MPBanner> _bannerPluginsDict =
		new Dictionary<string, MPBanner> ();
	private static Dictionary<string, MPInterstitial> _interstitialPluginsDict =
		new Dictionary<string, MPInterstitial> ();
	private static Dictionary<string, MPRewardedVideo> _rewardedVideoPluginsDict =
		new Dictionary<string, MPRewardedVideo> ();

	public static void loadBannerPluginsForAdUnits (string[] bannerAdUnitIds)
	{
		foreach (string bannerAdUnitId in bannerAdUnitIds) {
			_bannerPluginsDict.Add (bannerAdUnitId, new MPBanner (bannerAdUnitId));
		}
		Debug.Log (bannerAdUnitIds.Length + " banner AdUnits loaded for plugins:\n" +
			string.Join (", ", bannerAdUnitIds));
	}

	public static void loadInterstitialPluginsForAdUnits (string[] interstitialAdUnitIds)
	{
		foreach (string interstitialAdUnitId in interstitialAdUnitIds) {
			_interstitialPluginsDict.Add (interstitialAdUnitId, new MPInterstitial (interstitialAdUnitId));
		}
		Debug.Log (interstitialAdUnitIds.Length + " interstitial AdUnits loaded for plugins:\n" +
			string.Join (", ", interstitialAdUnitIds));
	}

	public static void loadRewardedVideoPluginsForAdUnits (string[] rewardedVideoAdUnitIds)
	{
		foreach (string rewardedVideoAdUnitId in rewardedVideoAdUnitIds) {
			_rewardedVideoPluginsDict.Add (rewardedVideoAdUnitId, new MPRewardedVideo (rewardedVideoAdUnitId));
		}
		Debug.Log (rewardedVideoAdUnitIds.Length + " rewarded video AdUnits loaded for plugins:\n" +
			string.Join (", ", rewardedVideoAdUnitIds));
	}
	#endif

	public static string getPluginName ()
	{
		return "MoPub Unity Plugin v" + PLUGIN_VERSION;
	}

	public static string getSDKName ()
	{
		if (_sdkName == null) {
			#if UNITY_EDITOR
			_sdkName = "no SDK loaded (not on a mobile device)";
			#elif UNITY_ANDROID
			AndroidJavaClass _pluginClass = new AndroidJavaClass ("com.mopub.unity.MoPubUnityPlugin");
			string sdkVersion = _pluginClass.CallStatic<string> ("getSDKVersion");
			_sdkName = "Android SDK v" + sdkVersion;
			#elif UNITY_IPHONE
			_sdkName = "iOS SDK v" + MoPubBinding.getSDKVersion();
			#endif

			_sdkName = _sdkName.Replace("+unity", "");
		}
		return _sdkName;
	}


	// Enables/disables location support for banners and interstitials
	public static void enableLocationSupport (bool shouldUseLocation)
	{
		#if UNITY_IPHONE
		MoPubBinding.enableLocationSupport (true);
		#elif UNITY_ANDROID
		MoPubAndroid.setLocationAwareness (MoPubLocationAwareness.NORMAL);
		#endif
	}


	// Reports an app download to MoPub. iTunesAppId is iOS only.
	public static void reportApplicationOpen (string iTunesAppId = null)
	{
		#if UNITY_IPHONE
		MoPubBinding.reportApplicationOpen (iTunesAppId);
		#elif UNITY_ANDROID
		MoPubAndroid.reportApplicationOpen ();
		#endif
	}

	#if UNITY_IPHONE
	// Forces the usage of WKWebView if able.
	public static void forceWKWebView(bool shouldForce)
	{
		MoPubBinding.forceWKWebView(shouldForce);
	}
	#endif

	/*
	 * Banner API
	 */


	#if UNITY_IPHONE
	public static void createBanner (string adUnitId, MoPubAdPosition position,
		MoPubBannerType bannerType = MoPubBannerType.Size320x50)
	{
		MP plugin;
		if (_pluginsDict.TryGetValue (adUnitId, out plugin)) {
			plugin.createBanner (bannerType, position);
		} else {
			Debug.LogWarning (String.Format (ADUNIT_NOT_FOUND_MSG, adUnitId));
		}
	}
	#elif UNITY_ANDROID
	public static void createBanner (string adUnitId, MoPubAdPosition position)
	{
		MPBanner plugin;
		if (_bannerPluginsDict.TryGetValue (adUnitId, out plugin)) {
			plugin.createBanner (position);
		} else {
			Debug.LogWarning (String.Format (ADUNIT_NOT_FOUND_MSG, adUnitId));
		}
	}
	#endif


	// Destroys the banner and removes it from view
	public static void destroyBanner (string adUnitId)
	{
		#if UNITY_IPHONE
		MP plugin;
		if (_pluginsDict.TryGetValue (adUnitId, out plugin)) {
			plugin.destroyBanner ();
		} else {
			Debug.LogWarning (String.Format (ADUNIT_NOT_FOUND_MSG, adUnitId));
		}
		#elif UNITY_ANDROID
		MPBanner plugin;
		if (_bannerPluginsDict.TryGetValue (adUnitId, out plugin)) {
			plugin.destroyBanner ();
		} else {
			Debug.LogWarning (String.Format (ADUNIT_NOT_FOUND_MSG, adUnitId));
		}
		#endif
	}


	// Shows/hides the banner
	public static void showBanner (string adUnitId, bool shouldShow)
	{
		#if UNITY_IPHONE
		MP plugin;
		if (_pluginsDict.TryGetValue (adUnitId, out plugin)) {
			plugin.showBanner (shouldShow);
		} else {
			Debug.LogWarning (String.Format (ADUNIT_NOT_FOUND_MSG, adUnitId));
		}
		#elif UNITY_ANDROID
		MPBanner plugin;
		if (_bannerPluginsDict.TryGetValue (adUnitId, out plugin)) {
			plugin.showBanner (shouldShow);
		} else {
			Debug.LogWarning (String.Format (ADUNIT_NOT_FOUND_MSG, adUnitId));
		}
		#endif
	}



	/*
	 * Interstitial API
	 */


	// Starts loading an interstitial ad
	public static void requestInterstitialAd (string adUnitId, string keywords = "")
	{
		#if UNITY_IPHONE
		MP plugin;
		if (_pluginsDict.TryGetValue (adUnitId, out plugin)) {
			plugin.requestInterstitialAd (keywords);
		} else {
			Debug.LogWarning (String.Format (ADUNIT_NOT_FOUND_MSG, adUnitId));
		}
		#elif UNITY_ANDROID
		MPInterstitial plugin;
		if (_interstitialPluginsDict.TryGetValue (adUnitId, out plugin)) {
			plugin.requestInterstitialAd (keywords);
		} else {
			Debug.LogWarning (String.Format (ADUNIT_NOT_FOUND_MSG, adUnitId));
		}
		#endif
	}


	// If an interstitial ad is loaded this will take over the screen and show the ad
	public static void showInterstitialAd (string adUnitId)
	{
		#if UNITY_IPHONE
		MP plugin;
		if (_pluginsDict.TryGetValue (adUnitId, out plugin)) {
			plugin.showInterstitialAd ();
		} else {
			Debug.LogWarning (String.Format (ADUNIT_NOT_FOUND_MSG, adUnitId));
		}
		#elif UNITY_ANDROID
		MPInterstitial plugin;
		if (_interstitialPluginsDict.TryGetValue (adUnitId, out plugin)) {
			plugin.showInterstitialAd ();
		} else {
			Debug.LogWarning (String.Format (ADUNIT_NOT_FOUND_MSG, adUnitId));
		}
		#endif
	}



	/*
	 * Rewarded Video API
	 */


	// Initializes the rewarded video system
	public static void initializeRewardedVideo ()
	{
		#if UNITY_IPHONE
		MP.initializeRewardedVideo ();
		#elif UNITY_ANDROID
		MPRewardedVideo.initializeRewardedVideo ();
		#endif
	}

	// Initializes the rewarded video system with the specified networks.
	public static void initializeRewardedVideo(MoPubRewardedNetwork[] networks)
	{
		#if UNITY_IPHONE
		MP.initializeRewardedVideoWithNetworks (networks);
		#elif UNITY_ANDROID
		MPRewardedVideo.initializeRewardedVideoWithNetworks (networks);
		#endif
	}

	// Starts loading a rewarded video ad
	public static void requestRewardedVideo (string adUnitId,
	                                         List<MoPubMediationSetting> mediationSettings = null,
	                                         string keywords = null,
	                                         double latitude = LAT_LONG_SENTINEL,
	                                         double longitude = LAT_LONG_SENTINEL,
	                                         string customerId = null)
	{
		#if UNITY_IPHONE
		MP plugin;
		if (_pluginsDict.TryGetValue (adUnitId, out plugin)) {
			plugin.requestRewardedVideo (mediationSettings, keywords, latitude, longitude, customerId);
		} else {
			Debug.LogWarning (String.Format (ADUNIT_NOT_FOUND_MSG, adUnitId));
		}
		#elif UNITY_ANDROID
		MPRewardedVideo plugin;
		if (_rewardedVideoPluginsDict.TryGetValue (adUnitId, out plugin)) {
			plugin.requestRewardedVideo (mediationSettings, keywords, latitude, longitude, customerId);
		} else {
			Debug.LogWarning (String.Format (ADUNIT_NOT_FOUND_MSG, adUnitId));
		}
		#endif
	}


	// If a rewarded video ad is loaded this will take over the screen and show the ad
	public static void showRewardedVideo (string adUnitId)
	{
		showRewardedVideo (adUnitId, null);
	}

	// If a rewarded video ad is loaded this will take over the screen and show the ad
	public static void showRewardedVideo (string adUnitId, string customData)
	{
		#if UNITY_IPHONE
		MP plugin;
		if (_pluginsDict.TryGetValue (adUnitId, out plugin)) {
			plugin.showRewardedVideo (customData);
		} else {
			Debug.LogWarning (String.Format (ADUNIT_NOT_FOUND_MSG, adUnitId));
		}
		#elif UNITY_ANDROID
		MPRewardedVideo plugin;
		if (_rewardedVideoPluginsDict.TryGetValue (adUnitId, out plugin)) {
			plugin.showRewardedVideo (customData);
		} else {
			Debug.LogWarning (String.Format (ADUNIT_NOT_FOUND_MSG, adUnitId));
		}
		#endif
	}


	// Whether a rewarded video is ready to play for this AdUnit
	public static bool hasRewardedVideo(string adUnitId)
	{
		#if UNITY_IPHONE
		MP plugin;
		if (_pluginsDict.TryGetValue (adUnitId, out plugin)) {
			return plugin.hasRewardedVideo ();
		} else {
			Debug.LogWarning (String.Format (ADUNIT_NOT_FOUND_MSG, adUnitId));
			return false;
		}
		#elif UNITY_ANDROID
		MPRewardedVideo plugin;
		if (_rewardedVideoPluginsDict.TryGetValue (adUnitId, out plugin)) {
			return plugin.hasRewardedVideo();
		} else {
			Debug.LogWarning (String.Format (ADUNIT_NOT_FOUND_MSG, adUnitId));
			return false;
		}
		#endif
	}


	// Retrieves a list of available rewards for this AdUnit
	public static List<MoPubReward> getAVailableRewards(string adUnitId)
	{
		#if UNITY_IPHONE
		MP plugin;
		if (_pluginsDict.TryGetValue (adUnitId, out plugin)) {
			List<MoPubReward> rewards = plugin.getAvailableRewards ();

			// Logging
			Debug.Log (String.Format ("getAVailableRewards found {0} rewards for ad unit {1}", rewards.Count, adUnitId));

			return rewards;
		} else {
			Debug.LogWarning (String.Format (ADUNIT_NOT_FOUND_MSG, adUnitId));
			return null;
		}
		#elif UNITY_ANDROID
		MPRewardedVideo plugin;
		if (_rewardedVideoPluginsDict.TryGetValue (adUnitId, out plugin)) {
			return plugin.getAVailableRewards();
		} else {
			Debug.LogWarning (String.Format (ADUNIT_NOT_FOUND_MSG, adUnitId));
			return null;
		}
		#endif
	}


	// Selects the reward for this AdUnit
	public static void selectReward(string adUnitId, MoPubReward selectedReward)
	{
		#if UNITY_IPHONE
		MP plugin;
		if (_pluginsDict.TryGetValue (adUnitId, out plugin)) {
			plugin.selectedReward = selectedReward;
		} else {
			Debug.LogWarning (String.Format (ADUNIT_NOT_FOUND_MSG, adUnitId));
		}
		#elif UNITY_ANDROID
		MPRewardedVideo plugin;
		if (_rewardedVideoPluginsDict.TryGetValue (adUnitId, out plugin)) {
			plugin.selectReward(selectedReward);
		} else {
			Debug.LogWarning (String.Format (ADUNIT_NOT_FOUND_MSG, adUnitId));
		}
		#endif
	}
}

#endif