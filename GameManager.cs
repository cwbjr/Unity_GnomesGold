using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//public class GameManager : MonoBehaviour {

// BEGIN 2d_gamemanager
public class GameManager : Singleton<GameManager> {


	public GameObject startingPoint;


	public Rope rope;



	public CameraFollow cameraFollow;


	Gnome currentGnome;


	public GameObject gnomePrefab;


	public RectTransform mainMenu;


	public RectTransform gameplayMenu;


	public RectTransform gameOverMenu;


	public bool gnomeInvincible { get; set; }


	public float delayAfterDeath = 1.0f;


	public AudioClip gnomeDiedSound;


	public AudioClip gameOverSound;

	// BEGIN 2d_gamemanager_start_reset
	void Start() {

		Reset ();
	}


	public void Reset() {


		if (gameOverMenu)
			gameOverMenu.gameObject.SetActive(false);

		if (mainMenu)
			mainMenu.gameObject.SetActive(false);

		if (gameplayMenu)
			gameplayMenu.gameObject.SetActive(true);


		var resetObjects = FindObjectsOfType<Resettable>();

		foreach (Resettable r in resetObjects) {
			r.Reset();
		}


		CreateNewGnome();


		Time.timeScale = 1.0f;
	}
	// END 2d_gamemanager_start_reset


	// BEGIN 2d_gamemanager_createnewgnome
	void CreateNewGnome() {


		RemoveGnome();


		GameObject newGnome = (GameObject)Instantiate(gnomePrefab, 

			startingPoint.transform.position, 

			Quaternion.identity);                                                     
		currentGnome = newGnome.GetComponent<Gnome>();


		rope.gameObject.SetActive(true);



		rope.connectedObject = currentGnome.ropeBody;


		rope.ResetLength();


		cameraFollow.target = currentGnome.cameraFollowTarget;

	}
	// END 2d_gamemanager_createnewgnome

	// BEGIN 2d_gamemanager_removegnome
	void RemoveGnome() {


		if (gnomeInvincible)
			return;


		rope.gameObject.SetActive(false);


		cameraFollow.target = null;


		if (currentGnome != null) {


			currentGnome.holdingTreasure = false;

			currentGnome.gameObject.tag = "Untagged";


			foreach (Transform child in currentGnome.transform) {
				child.gameObject.tag = "Untagged";
			}


			currentGnome = null;
		}
	}
	// END 2d_gamemanager_removegnome

	// BEGIN 2d_gamemanager_killgnome
	void KillGnome(Gnome.DamageType damageType) {


		var audio = GetComponent<AudioSource>();
		if (audio) {
			audio.PlayOneShot(this.gnomeDiedSound);
		}


		currentGnome.ShowDamageEffect(damageType);


		if (gnomeInvincible == false) {


			currentGnome.DestroyGnome(damageType);


			RemoveGnome();


			StartCoroutine(ResetAfterDelay());

		}
	}
	// END 2d_gamemanager_killgnome

	// BEGIN 2d_gamemanager_reset
	IEnumerator ResetAfterDelay() {


		yield return new WaitForSeconds(delayAfterDeath);
		Reset();
	}
	// END 2d_gamemanager_reset

	// BEGIN 2d_gamemanager_ontouch
	public void TrapTouched() {
		KillGnome(Gnome.DamageType.Slicing);
	}


	public void FireTrapTouched() {
		KillGnome(Gnome.DamageType.Burning);
	}


	public void TreasureCollected() {

		currentGnome.holdingTreasure = true;
	}
	// END 2d_gamemanager_ontouch

	// BEGIN 2d_gamemanager_exitreached
	public void ExitReached() {

		if (currentGnome != null && currentGnome.holdingTreasure == 
			true) {


			var audio = GetComponent<AudioSource>();
			if (audio) {
				audio.PlayOneShot(this.gameOverSound);
			}


			Time.timeScale = 0.0f;

			if (gameOverMenu)
				gameOverMenu.gameObject.SetActive(true);

			if (gameplayMenu)
				gameplayMenu.gameObject.SetActive(false);
		}
	}
	// END 2d_gamemanager_exitreached

	// BEGIN 2d_gamemanager_setpaused
	public void SetPaused(bool paused) {

		if (paused) {
			Time.timeScale = 0.0f;
			mainMenu.gameObject.SetActive(true);
			gameplayMenu.gameObject.SetActive(false);
		} else {
	
			Time.timeScale = 1.0f;
			mainMenu.gameObject.SetActive(false);
			gameplayMenu.gameObject.SetActive(true);
		}
	}
	// END 2d_gamemanager_setpaused

	// BEGIN 2d_gamemanager_restartgame
	public void RestartGame() {


		Destroy(currentGnome.gameObject);
		currentGnome = null;


		Reset();
	}
	// END 2d_gamemanager_restartgame

}
