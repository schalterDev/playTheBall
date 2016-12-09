using UnityEngine;
using System;
using System.Collections;
using UnityEngine.SceneManagement;

using GoogleMobileAds;
using GoogleMobileAds.Api;

public class UIManager : MonoBehaviour {

    //MAIN MENU

    public GameObject overlayPause;

	private static GameObject overlay;

	private string adUnitId;
	private BannerView bannerView;

	void Start() {
		overlay = overlayPause;
	}

	/*
	private void RequestBanner()
	{
		// These ad units are configured to always serve test ads.
		#if UNITY_EDITOR
		string adUnitId = "unused";
		#elif UNITY_ANDROID
		string adUnitId = "ca-app-pub-3940256099942544/6300978111";
		#elif UNITY_IPHONE
		string adUnitId = "ca-app-pub-3940256099942544/2934735716";
		#else
		string adUnitId = "unexpected_platform";
		#endif

		// Create a 320x50 banner at the top of the screen.
		this.bannerView = new BannerView(adUnitId, AdSize.SmartBanner, AdPosition.Top);

		// Register for ad events.
		this.bannerView.OnAdLoaded += this.HandleAdLoaded;
		this.bannerView.OnAdFailedToLoad += this.HandleAdFailedToLoad;
		this.bannerView.OnAdLoaded += this.HandleAdOpened;
		this.bannerView.OnAdClosed += this.HandleAdClosed;
		this.bannerView.OnAdLeavingApplication += this.HandleAdLeftApplication;

		// Load a banner ad.
		this.bannerView.LoadAd(this.CreateAdRequest());
	}

	// Returns an ad request with custom ad targeting.
	private AdRequest CreateAdRequest()
	{
		return new AdRequest.Builder()
			.AddTestDevice(AdRequest.TestDeviceSimulator)
			.AddTestDevice("0123456789ABCDEF0123456789ABCDEF")
			.AddKeyword("game")
			.SetGender(Gender.Male)
			.SetBirthday(new DateTime(1985, 1, 1))
			.TagForChildDirectedTreatment(false)
			.AddExtra("color_bg", "9B30FF")
			.Build();
	}

	#region Banner callback handlers

	public void HandleAdLoaded(object sender, EventArgs args)
	{
		MonoBehaviour.print("HandleAdLoaded event received");
	}

	public void HandleAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
	{
		MonoBehaviour.print("HandleFailedToReceiveAd event received with message: " + args.Message);
	}

	public void HandleAdOpened(object sender, EventArgs args)
	{
		MonoBehaviour.print("HandleAdOpened event received");
	}

	public void HandleAdClosed(object sender, EventArgs args)
	{
		MonoBehaviour.print("HandleAdClosed event received");
	}

	public void HandleAdLeftApplication(object sender, EventArgs args)
	{
		MonoBehaviour.print("HandleAdLeftApplication event received");
	}

	#endregion Banner callback handlers

	
	void OnGUI(){
		if (GUI.Button(new Rect(240, 100, 100, 60), "showbanner"))
		{
			RequestBanner ();	
		}
	} */

	public void GoBackToLobby() {
		GameObject lobby = GameObject.Find ("LobbyManager");
		Prototype.NetworkLobby.LobbyManager lobbyManager = Prototype.NetworkLobby.LobbyManager.s_Singleton;

		lobbyManager.GoBackButton ();

		//lobbyManager.goToMainMenu ();
	}

    public void StartSinglePlayer()
    {
        SceneManager.LoadScene("SinglePlayer"); 
    }

    public void StartMultiPlayer()
    {
        SceneManager.LoadScene("Lobby");
    }

    public void GoToMenu()
    {
		SceneManager.LoadScene("MainMenu"); 
        Time.timeScale = 1;
    }

	public void OpenPreferences() {
		SceneManager.LoadScene ("Settings");
	}

	public static void PauseLocal() {

	}

	public static void ResumeLocal() {

	}

    //SINGLE PLAYER
    public static void PauseGame()
    {
        overlay.SetActive(true);

		Prototype.NetworkLobby.LobbyManager lobbyManager = Prototype.NetworkLobby.LobbyManager.s_Singleton;

		lobbyManager.topPanel.ToggleVisibility (true);
    }

    public static void ResumeGame()
    {
        overlay.SetActive(false);

		Prototype.NetworkLobby.LobbyManager lobbyManager = Prototype.NetworkLobby.LobbyManager.s_Singleton;

		lobbyManager.topPanel.ToggleVisibility (false);
    }
}
