using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;

using MoPubReward = MoPubManager.MoPubReward;

public class MoPubDemoGUI : MonoBehaviour
{
	private int _selectedToggleIndex;
	private string[] _bannerAdUnits;
	private string[] _interstitialAdUnits;
	private string[] _rewardedVideoAdUnits;
	private string[] _rewardedRichMediaAdUnits;
	private Dictionary<string, List<MoPubReward>> _adUnitToRewardsMapping =
		new Dictionary<string, List<MoPubReward>> ();
	private Dictionary<string, bool> _adUnitToLoadedMapping =
		new Dictionary<string, bool> ();
	private Dictionary<string, bool> _bannerAdUnitToShownMapping =
		new Dictionary<string, bool> ();

	// Workaround for lacking adUnit from onAdLoadedEvent for Banners
	private Queue<string> _requestedBannerAdUnits = new Queue<string> ();

	private string[] _networkList = new string[] {
		"MoPub",
		"Millennial",
		"AdMob",
		"Chartboost",
		"Vungle",
		"Facebook",
		"AdColony",
		"Unity Ads"
	};

	#if UNITY_ANDROID
	private Dictionary<string, string[]> _bannerDict = new Dictionary<string, string[]> () {
		{ "AdMob", new string[] { "a57e987d40ae4d84b9a731b008ad52ee" } },
		{ "Facebook", new string[] { "b40a96dd275e4ce5be2cdf5faa92007d" } },
		{ "Millennial", new string[] { "1aa442709c9f11e281c11231392559e4" } },
		{ "MoPub", new string[] { "23b49916add211e281c11231392559e4", "0ac59b0996d947309c33f59d6676399f" } },
	};

	private Dictionary<string, string[]> _interstitialDict = new Dictionary<string, string[]> () {
		{ "AdColony", new string[] { "953d93d50ae343168ad44c3c536cdb1a" } },
		{ "AdMob", new string[] { "554e8baff8d84137941b5a55354105fc" } },
		{ "Chartboost", new string[] { "376366b49d324dedae3d5edb360c27b4" } },
		{ "Facebook", new string[] { "9792d876011f4359887d2d26380e8a84" } },
		{ "Millennial", new string[] { "c6566f7bd85c40afb7afc4232a1cd463" } },
		{ "MoPub", new string[] { "3aba0056add211e281c11231392559e4", "b0482b17a8e64a2c842624d23539ced4" } },
		{ "Unity Ads", new string[] { "6e442143d674437e9a417ae36aa93241" } },
		{ "Vungle", new string[] { "4f5e1e97f87c406cb7878b9eff1d2a77" } }
	};

	private Dictionary<string, string[]> _rewardedVideoDict = new Dictionary<string, string[]> () {
		{ "AdColony", new string[] { "339929134a55413d9c0a85496b250057" } },
		{ "MoPub", new string[] { "fdd35fb5d55b4ccf9ceb27c7a3926b7d", "8f000bd5e00246de9c789eed39ff6096" } },
		{ "Facebook", new string[] { "a8d5f5fff87c49b2a60c5df86ab4f447" } },
		{ "Unity Ads", new string[] { "facae35b91a1451c87b2d6dcb9776873" } },
		{ "Vungle", new string[] { "2d38f4e6881341369e9fc2c2d01ddc9d" } }
	};

	private Dictionary<string, string[]> _rewardedRichMediaDict = new Dictionary<string, string[]> () {
		{ "AdMob", new string[] { "49a4a502ed2945fd92bc5798c9421a57" } },
		{ "Chartboost", new string[] { "df605ab15b56400285c99e521ecc2cb1" } },
		{ "MoPub", new string[] { "db2ef0eb1600433a8cdc31c75549c6b1" } }
	};

	#elif UNITY_IPHONE
	private Dictionary<string, string[]> _bannerDict = new Dictionary<string, string[]> () {
		{ "AdMob", new string[] { "41151815470f4833a867e3e005b832b0" } },
		{ "Facebook", new string[] { "fb759131fd7a40e6b9d324e637a4b299" } },
		{ "Millennial", new string[] { "1b282680106246aa83036892b32ec7cc" } },
		{ "MoPub", new string[] { "23b49916add211e281c11231392559e4",
				"0ac59b0996d947309c33f59d6676399f"} },
	};

	private Dictionary<string, string[]> _interstitialDict = new Dictionary<string, string[]> () {
		{ "AdColony", new string[] { "09fed773d1e34cba968d910b4fbdc850" } },
		{ "AdMob", new string[] { "4f9d8fb8521f4420b2429184f720f42b" } },
		{ "Chartboost", new string[] { "a97fa010d9c24d06ae267be2a1487af1",
				"bb5403245ad14dc3817f81f4018477ec" } },
		{ "Facebook", new string[] { "27614fde27df488493327f2b952f9d21" } },
		{ "Millennial", new string[] { "0da9e2762f1a48bab695887fb7798b66",
				"47bf0f3adf094486a5fc61abda26cf84" } },
		{ "MoPub", new string[] { "b0482b17a8e64a2c842624d23539ced4", "3aba0056add211e281c11231392559e4" } },
		{ "Unity Ads", new string[] { "4fab4888caa048e085a1dc5c78816061",
				"1923c923be1f4793b07f1bd8c3a2fd93" } },
		{ "Vungle", new string[] { "c87b1701e1084507bf8be89cd13b890c" } }
	};

	private Dictionary<string, string[]> _rewardedVideoDict = new Dictionary<string, string[]> () {
		{ "AdColony", new string[] { "52aa460767374250a5aa5174c2345be3" } },
		{ "Chartboost", new string[] { "2942576082c24e0f80c6172703572870" } },
		{ "Facebook", new string[] { "9204bbf695b24f26a9a8b7066e712c10" } },
		{ "MoPub", new string[] { "fdd35fb5d55b4ccf9ceb27c7a3926b7d",
				"8f000bd5e00246de9c789eed39ff6096" } },
		{ "Unity Ads", new string[] { "676a0fa97aca48cbbe489de5b2fa4cd1" } },
		{ "Vungle", new string[] { "19a24d282ecb49c5bb43c65f501e33bf" } }
	};

	private Dictionary<string, string[]> _rewardedRichMediaDict = new Dictionary<string, string[]> () {
	};
	#endif

	// Label style for no ad unit messages
	private GUIStyle _smallerFont;

	// Buffer space between sections
	private int _sectionMarginSize;

	// Currently selected network
	private string _network;

	// Default text for custom data fields
	private static string _customDataDefaultText = "Optional custom data";

	// String to fill with custom data for Rewarded Videos
	private string _rvCustomData = _customDataDefaultText;

	// String to fill with custom data for Rewarded Rich Media
	private string _rrmCustomData = _customDataDefaultText;


	private static bool IsAdUnitArrayNullOrEmpty (string[] adUnitArray) {
		return (adUnitArray == null || adUnitArray.Length == 0);
	}


	private void addAdUnitsToStateMaps (string[] adUnits) {
		foreach (string adUnit in adUnits) {
			_adUnitToLoadedMapping.Add (adUnit, false);
			// Only banners need this map, but init for all to keep it simple
			_bannerAdUnitToShownMapping.Add (adUnit, false);
		}
	}


	public void loadAvailableRewards (string adUnitId, List<MoPubReward> availableRewards) {
		// Remove any existing available rewards associated with this AdUnit from previous ad requests
		_adUnitToRewardsMapping.Remove (adUnitId);

		if (availableRewards != null) {
			_adUnitToRewardsMapping[adUnitId] = availableRewards;
		}
	}


	public void bannerLoaded () {
		string firstRequestedBannerAdUnit = _requestedBannerAdUnits.Dequeue ();
		_adUnitToLoadedMapping[firstRequestedBannerAdUnit] = true;
		_bannerAdUnitToShownMapping[firstRequestedBannerAdUnit] = true;
	}


	public void adLoaded (string adUnit) {
		_adUnitToLoadedMapping[adUnit] = true;
	}


	public void adDismissed (string adUnit) {
		_adUnitToLoadedMapping[adUnit] = false;
	}


	void Start () {
		var allBannerAdUnits = new string[0];
		var allInterstitialAdUnits = new string[0];
		var allRewardedVideoAdUnits = new string[0];

		foreach (var bannerAdUnits in _bannerDict.Values) {
			allBannerAdUnits = allBannerAdUnits.Union (bannerAdUnits).ToArray ();
		}

		foreach (var interstitialAdUnits in _interstitialDict.Values) {
			allInterstitialAdUnits = allInterstitialAdUnits.Union (interstitialAdUnits).ToArray ();
		}

		foreach (var rewardedVideoAdUnits in _rewardedVideoDict.Values) {
			allRewardedVideoAdUnits = allRewardedVideoAdUnits.Union (rewardedVideoAdUnits).ToArray ();
		}

		foreach (var rewardedRichMediaAdUnits in _rewardedRichMediaDict.Values) {
			allRewardedVideoAdUnits = allRewardedVideoAdUnits.Union (rewardedRichMediaAdUnits).ToArray ();
		}

		addAdUnitsToStateMaps (allBannerAdUnits);
		addAdUnitsToStateMaps (allInterstitialAdUnits);
		addAdUnitsToStateMaps (allRewardedVideoAdUnits);

		#if UNITY_ANDROID && !UNITY_EDITOR
		MoPub.loadBannerPluginsForAdUnits (allBannerAdUnits);
		MoPub.loadInterstitialPluginsForAdUnits (allInterstitialAdUnits);
		MoPub.loadRewardedVideoPluginsForAdUnits (allRewardedVideoAdUnits);
		#elif UNITY_IPHONE && !UNITY_EDITOR
		MoPub.loadPluginsForAdUnits(allBannerAdUnits);
		MoPub.loadPluginsForAdUnits(allInterstitialAdUnits);
		MoPub.loadPluginsForAdUnits(allRewardedVideoAdUnits);
		#endif

		#if !UNITY_EDITOR
		if (!IsAdUnitArrayNullOrEmpty (allRewardedVideoAdUnits)) {
			MoPub.initializeRewardedVideo ();
		}
		#endif
	}


	void OnGUI () {
		ConfigureGUI ();

		CreateNetworksTab ();

		GUILayout.BeginArea (new Rect (20, 0, Screen.width - 40, Screen.height));
		GUILayout.BeginVertical ();

		CreateTitleSection ();
		CreateBannersSection ();
		CreateInterstitialsSection ();
		List<MoPubMediationSetting> mediationSettings = GetMediationSettings ();
		CreateRewardedVideosSection (mediationSettings);
		CreateRewardedRichMediaSection (mediationSettings);
		CreateActionsSection ();

		GUILayout.EndVertical ();
		GUILayout.EndArea ();
	}


	private void ConfigureGUI () {
		// Set default label style
		GUI.skin.label.fontSize = 42;

		// Set default button style
		GUI.skin.button.margin = new RectOffset (0, 0, 10, 0);
		GUI.skin.button.stretchWidth = true;
		GUI.skin.button.fixedHeight = (Screen.width >= 960 || Screen.height >= 960) ? 75 : 50;
		GUI.skin.button.fontSize = 34;

		// Set default text field style
		GUI.skin.textField.stretchWidth = true;
		GUI.skin.textField.fixedHeight = 35;
		GUI.skin.textField.fontSize = 28;

		// Buffer space between sections
		var sectionMargin = 36;
		_smallerFont = new GUIStyle (GUI.skin.label);
		_smallerFont.fontSize = GUI.skin.button.fontSize;

		_sectionMarginSize = GUI.skin.label.fontSize;
	}


	private void CreateNetworksTab () {
		_selectedToggleIndex = GUI.Toolbar (
			new Rect (0, Screen.height - GUI.skin.button.fixedHeight, Screen.width, GUI.skin.button.fixedHeight),
			_selectedToggleIndex,
			_networkList);
		_network = _networkList[_selectedToggleIndex];
		_bannerAdUnits = _bannerDict.ContainsKey (_network) ? _bannerDict[_network] : null;
		_interstitialAdUnits = _interstitialDict.ContainsKey (_network) ? _interstitialDict[_network] : null;
		_rewardedVideoAdUnits = _rewardedVideoDict.ContainsKey (_network) ? _rewardedVideoDict[_network] : null;
		_rewardedRichMediaAdUnits = _rewardedRichMediaDict.ContainsKey (_network) ? _rewardedRichMediaDict[_network] : null;
	}


	private void CreateTitleSection () {
		// App title including Plugin and SDK versions
		GUIStyle centeredStyle = new GUIStyle (GUI.skin.label);
		centeredStyle.alignment = TextAnchor.UpperCenter;
		centeredStyle.fontSize = 48;
		GUI.Label (new Rect (0, 10, Screen.width, 60), MoPub.getPluginName (), centeredStyle);
		centeredStyle.fontSize = _smallerFont.fontSize;
		GUI.Label (new Rect (0, 70, Screen.width, 60), "with " + MoPub.getSDKName (), centeredStyle);
	}


	private void CreateBannersSection () {
		int titlePadding = 102;
		GUILayout.Space (titlePadding);
		GUILayout.Label ("Banners");
		if (!IsAdUnitArrayNullOrEmpty (_bannerAdUnits)) {
			foreach (string bannerAdUnit in _bannerAdUnits) {
				GUILayout.BeginHorizontal ();

				GUI.enabled = !_adUnitToLoadedMapping[bannerAdUnit];
				if (GUILayout.Button (CreateRequestButtonLabel (bannerAdUnit))) {
					Debug.Log ("requesting banner with AdUnit: " + bannerAdUnit);
					MoPub.createBanner (bannerAdUnit, MoPubAdPosition.BottomRight);
					_requestedBannerAdUnits.Enqueue (bannerAdUnit);
				}

				GUI.enabled = _adUnitToLoadedMapping[bannerAdUnit];
				if (GUILayout.Button ("Destroy")) {
					MoPub.destroyBanner (bannerAdUnit);
					_adUnitToLoadedMapping[bannerAdUnit] = false;
					_bannerAdUnitToShownMapping[bannerAdUnit] = false;
				}

				GUI.enabled = _adUnitToLoadedMapping[bannerAdUnit] && !_bannerAdUnitToShownMapping[bannerAdUnit];
				if (GUILayout.Button ("Show")) {
					MoPub.showBanner (bannerAdUnit, true);
					_bannerAdUnitToShownMapping[bannerAdUnit] = true;
				}

				GUI.enabled = _adUnitToLoadedMapping[bannerAdUnit] && _bannerAdUnitToShownMapping[bannerAdUnit];
				if (GUILayout.Button ("Hide")) {
					MoPub.showBanner (bannerAdUnit, false);
					_bannerAdUnitToShownMapping[bannerAdUnit] = false;
				}
				GUI.enabled = true;

				GUILayout.EndHorizontal ();
			}
		} else {
			GUILayout.Label ("No banner AdUnits for " + _network, _smallerFont, null);
		}
	}


	private void CreateInterstitialsSection () {
		GUILayout.Space (_sectionMarginSize);
		GUILayout.Label ("Interstitials");
		if (!IsAdUnitArrayNullOrEmpty (_interstitialAdUnits)) {
			foreach (string interstitialAdUnit in _interstitialAdUnits) {
				GUILayout.BeginHorizontal ();

				GUI.enabled = !_adUnitToLoadedMapping[interstitialAdUnit];
				if (GUILayout.Button (CreateRequestButtonLabel (interstitialAdUnit))) {
					Debug.Log ("requesting interstitial with AdUnit: " + interstitialAdUnit);
					MoPub.requestInterstitialAd (interstitialAdUnit);
				}

				GUI.enabled = _adUnitToLoadedMapping[interstitialAdUnit];
				if (GUILayout.Button ("Show")) {
					MoPub.showInterstitialAd (interstitialAdUnit);
				}
				GUI.enabled = true;

				GUILayout.EndHorizontal ();
			}
		} else {
			GUILayout.Label ("No interstitial AdUnits for " + _network, _smallerFont, null);
		}
	}


	private List<MoPubMediationSetting> GetMediationSettings () {
		#if UNITY_ANDROID
		MoPubMediationSetting adColonySettings = new MoPubMediationSetting ("AdColony");
		adColonySettings.Add ("withConfirmationDialog", true);
		adColonySettings.Add ("withResultsDialog", true);

		MoPubMediationSetting chartboostSettings = new MoPubMediationSetting ("Chartboost");
		chartboostSettings.Add ("customId", "the-user-id");

		MoPubMediationSetting vungleSettings = new MoPubMediationSetting ("Vungle");
		vungleSettings.Add ("userId", "the-user-id");
		vungleSettings.Add ("cancelDialogBody", "Cancel Body");
		vungleSettings.Add ("cancelDialogCloseButton", "Shut it Down");
		vungleSettings.Add ("cancelDialogKeepWatchingButton", "Watch On");
		vungleSettings.Add ("cancelDialogTitle", "Cancel Title");

		List<MoPubMediationSetting> mediationSettings = new List<MoPubMediationSetting> ();
		mediationSettings.Add (adColonySettings);
		mediationSettings.Add (chartboostSettings);
		mediationSettings.Add (vungleSettings);
		#elif UNITY_IPHONE
		MoPubMediationSetting adColonySettings = new MoPubMediationSetting ("AdColony");
		adColonySettings.Add ("showPrePopup", true);
		adColonySettings.Add ("showPostPopup", true);

		MoPubMediationSetting vungleSettings = new MoPubMediationSetting ("Vungle");
		vungleSettings.Add ("userIdentifier", "the-user-id");

		List<MoPubMediationSetting> mediationSettings = new List<MoPubMediationSetting> ();
		mediationSettings.Add (adColonySettings);
		mediationSettings.Add (vungleSettings);
		#endif

		return mediationSettings;
	}


	private void CreateRewardedVideosSection (List<MoPubMediationSetting> mediationSettings) {
		GUILayout.Space (_sectionMarginSize);
		GUILayout.Label ("Rewarded Videos");
		if (!IsAdUnitArrayNullOrEmpty (_rewardedVideoAdUnits)) {
			CreateCustomDataField ("rvCustomDataField", ref _rvCustomData);
			foreach (string rewardedVideoAdUnit in _rewardedVideoAdUnits) {
				GUILayout.BeginHorizontal ();

				GUI.enabled = !_adUnitToLoadedMapping[rewardedVideoAdUnit];
				if (GUILayout.Button (CreateRequestButtonLabel (rewardedVideoAdUnit))) {
					Debug.Log ("requesting rewarded video with AdUnit: " +
						rewardedVideoAdUnit +
						" and mediation settings: " +
						MoPubInternal.ThirdParty.MiniJSON.Json.Serialize (mediationSettings));
					MoPub.requestRewardedVideo (rewardedVideoAdUnit,
						mediationSettings,
						"rewarded, video, mopub",
						37.7833,
						122.4167,
						"customer101");
				}

				GUI.enabled = _adUnitToLoadedMapping[rewardedVideoAdUnit];
				if (GUILayout.Button ("Show")) {
					MoPub.showRewardedVideo (rewardedVideoAdUnit, GetCustomData (_rvCustomData));
				}
				GUI.enabled = true;

				GUILayout.EndHorizontal ();

				#if !UNITY_EDITOR
				// Display rewards if there's a rewarded video loaded and there are multiple rewards available
				if (MoPub.hasRewardedVideo (rewardedVideoAdUnit) &&
					_adUnitToRewardsMapping.ContainsKey (rewardedVideoAdUnit) &&
					_adUnitToRewardsMapping[rewardedVideoAdUnit].Count > 1) {

					GUILayout.BeginVertical ();
					GUILayout.Space (_sectionMarginSize);
					GUILayout.Label ("Select a reward:");

					foreach (MoPubReward reward in _adUnitToRewardsMapping[rewardedVideoAdUnit]) {
						if (GUILayout.Button (reward.ToString ())) {
							MoPub.selectReward (rewardedVideoAdUnit, reward);
						}
					}

					GUILayout.Space (_sectionMarginSize);
					GUILayout.EndVertical ();
				}
				#endif
			}
		} else {
			GUILayout.Label ("No rewarded video AdUnits for " + _network, _smallerFont, null);
		}
	}


	private void CreateRewardedRichMediaSection (List<MoPubMediationSetting> mediationSettings)
	{
		GUILayout.Space (_sectionMarginSize);
		GUILayout.Label ("Rewarded Rich Media");
		if (!IsAdUnitArrayNullOrEmpty (_rewardedRichMediaAdUnits)) {
			CreateCustomDataField ("rrmCustomDataField", ref _rrmCustomData);
			foreach (string rewardedRichMediaAdUnit in _rewardedRichMediaAdUnits) {
				GUILayout.BeginHorizontal ();

				GUI.enabled = !_adUnitToLoadedMapping[rewardedRichMediaAdUnit];
				if (GUILayout.Button (CreateRequestButtonLabel (rewardedRichMediaAdUnit))) {
					Debug.Log ("requesting rewarded rich media with AdUnit: " +
						rewardedRichMediaAdUnit +
						" and mediation settings: " +
						MoPubInternal.ThirdParty.MiniJSON.Json.Serialize (mediationSettings));
					MoPub.requestRewardedVideo (rewardedRichMediaAdUnit,
						mediationSettings,
						"rewarded, video, mopub",
						37.7833,
						122.4167,
						"customer101");
				}

				GUI.enabled = _adUnitToLoadedMapping[rewardedRichMediaAdUnit];
				if (GUILayout.Button ("Show")) {
					MoPub.showRewardedVideo (rewardedRichMediaAdUnit, GetCustomData (_rrmCustomData));
				}
				GUI.enabled = true;

				GUILayout.EndHorizontal ();

				#if !UNITY_EDITOR
				// Display rewards if there's a rewarded rich media ad loaded and there are multiple rewards available
				if (MoPub.hasRewardedVideo (rewardedRichMediaAdUnit) &&
					_adUnitToRewardsMapping.ContainsKey (rewardedRichMediaAdUnit) &&
					_adUnitToRewardsMapping[rewardedRichMediaAdUnit].Count > 1) {

					GUILayout.BeginVertical ();
					GUILayout.Space (_sectionMarginSize);
					GUILayout.Label ("Select a reward:");

					foreach (MoPubReward reward in _adUnitToRewardsMapping[rewardedRichMediaAdUnit]) {
						if (GUILayout.Button (reward.ToString ())) {
							MoPub.selectReward (rewardedRichMediaAdUnit, reward);
						}
					}

					GUILayout.Space (_sectionMarginSize);
					GUILayout.EndVertical ();
				}
				#endif
			}
		} else {
			GUILayout.Label ("No rewarded rich media AdUnits for " + _network, _smallerFont, null);
		}
	}


	private void CreateCustomDataField (string fieldName, ref string customDataValue)
	{
		GUI.SetNextControlName (fieldName);
		customDataValue = GUILayout.TextField (customDataValue, new GUILayoutOption[] { GUILayout.MinWidth(200) });
		if (UnityEngine.Event.current.type == EventType.Repaint) {
			if (GUI.GetNameOfFocusedControl () == fieldName && customDataValue == _customDataDefaultText) {
				// Clear default text when focused
				customDataValue = "";
			} else if (GUI.GetNameOfFocusedControl () != fieldName && customDataValue == "") {
				// Restore default text when unfocused and empty
				customDataValue = _customDataDefaultText;
			}
		}
	}


	private string GetCustomData (string customDataFieldValue)
	{
		return customDataFieldValue != _customDataDefaultText ? customDataFieldValue : null;
	}


	private void CreateActionsSection ()
	{
		GUILayout.Space (_sectionMarginSize);
		GUILayout.Label ("Actions");
		if (GUILayout.Button ("Report App Open")) {
			MoPub.reportApplicationOpen ();
		}
		if (GUILayout.Button ("Enable Location Support")) {
			MoPub.enableLocationSupport (true);
		}
	}


	private string CreateRequestButtonLabel (string adUnit) {
		return "Request " + adUnit.Substring (0, 10) + "...";
	}
}
