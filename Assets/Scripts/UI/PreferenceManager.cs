using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PreferenceManager : MonoBehaviour {

	public Text controlText;
	public Text namesText;
	public Text lightText;

	public static string LASTIP = "last-ip";

	public static string CONTROL = "control";
	public static string NAMES = "names";
	public static string LIGHT = "light";

	public static int JOYSTICK = 0;
	private static string JOYSTICKTEXT = "joystick";
	public static int ACCELERATOR = 1;
	private static string ACCELERATORTEXT = "accelerator";

	private static int COUNTCONTROL = 1;

	public static int DAYLIGHT = 0;
	private static string DAYLIGHTTEXT = "DAY";
	public static int NIGHTLIGHT = 1;
	private static string NIGHTLIGHTTEXT = "NIGHT";

	private static int control;
	private static int lightCount;
	private static bool names;

	void Start() {
		//Load values from preference
		control = PlayerPrefs.GetInt(CONTROL);

		if (PlayerPrefs.GetInt (NAMES) >= 1)
			names = true;
		else
			names = false;

		lightCount = PlayerPrefs.GetInt (LIGHT);

		updateAllButtons ();
	}

	public void ChangeLight() {
		lightCount++;

		if (lightCount > 1) {
			lightCount = 0;
		}

		PlayerPrefs.SetInt (LIGHT, lightCount);
		PlayerPrefs.Save ();

		updateAllButtons ();
	}

	public void ChangeControl() {
		control++;

		if (control > COUNTCONTROL) {
			control = 0;
		}

		PlayerPrefs.SetInt (CONTROL, control);
		PlayerPrefs.Save ();

		updateAllButtons ();
	}

	public void ChangeNames() {
		names = !names;

		if (names)
			PlayerPrefs.SetInt (NAMES, 1);
		else
			PlayerPrefs.SetInt (NAMES, 0);

		updateAllButtons ();
	}

	private void updateAllButtons() {
		//Set text of control button
		if (control == JOYSTICK) {
			controlText.text = "Control: " + JOYSTICKTEXT;
		} else if (control == ACCELERATOR) {
			controlText.text = "Control: " + ACCELERATORTEXT;
		}

		//Set the name button
		if (names) {
			namesText.text = "Show names: yes";
		} else {
			namesText.text = "Show names: no";
		}

		//Set the light
		if (lightCount == DAYLIGHT) {
			lightText.text = "Time: " + DAYLIGHTTEXT;
		} else if (lightCount == NIGHTLIGHT) {
			lightText.text = "Time: " + NIGHTLIGHTTEXT;
		}
	}

	public void goToMainMenu() {
		SceneManager.LoadScene("MainMenu"); 
	}

}
