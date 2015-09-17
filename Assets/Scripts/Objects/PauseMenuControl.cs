﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using InControl;

public class PauseMenuControl : MonoBehaviour {

    public ClusterNodePuzzle resumeGameNodePuzzle;

    public InputFill inputFill;


	public GameObject mainMenu;

    public GameObject inputSelect;

	public GameObject options;

	public GameObject exitGameConfirm;

	public enum MenuState{PauseMenu, Options, InputSelect, QuitGame};

	public MenuState menuState = MenuState.PauseMenu;

	public FadePauseMenu fMainMenu;
	public FadeInputSelect fInputSelect;
	public FadeQuitGame fQuitGame;
	public FadeOptions fOptions;


	public ClusterNodePuzzle confirmQuitNodePuzzle;
	
	// Update is called once per frame
    void Update()
    {    

	//Main Menu/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

			if(Globals.Instance.gameState == Globals.GameState.Paused)
			{

				
				if(!mainMenu.activeInHierarchy)
						mainMenu.SetActive(true);
                if (!exitGameConfirm.activeInHierarchy)
                    exitGameConfirm.SetActive(true);
                if (!options.activeInHierarchy)
                    options.SetActive(true);
                if (!inputSelect.activeInHierarchy)
                    inputSelect.SetActive(true);

				if(fMainMenu.f != 1)
					fMainMenu.FadeIn();

                
                if (fQuitGame.f != 1)
                    fQuitGame.FadeIn();

               
                if (fOptions.f != 1)
                    fOptions.FadeIn();


                if (fInputSelect.f != 1)
                    fInputSelect.FadeIn();
                

				if (resumeGameNodePuzzle != null && resumeGameNodePuzzle.solved)
				{
					resumeGameNodePuzzle.solved = false;
					Globals.Instance.gameState = Globals.GameState.Unpausing;
				}

                if (confirmQuitNodePuzzle != null && confirmQuitNodePuzzle.solved)
                {
                    confirmQuitNodePuzzle.solved = false;
                    Globals.Instance.ResetOrExit();
                }

                if (fOptions.f == 1)
                {
                    if (!options.GetComponent<OptionsMenu>().soundChecked)
                        options.GetComponent<OptionsMenu>().CheckSoundSettings();
                }

                if (!inputFill.allowFill)
                    inputFill.allowFill = true;
				
			}
			else
			{
				ToggleFadeMainMenu();
				ToggleFadeInputSelectMenu();
				ToggleFadeExitGameConfirm();
				ToggleFadeOptionsMenu();
			}
					


			

        

    }	

	private void ToggleFadeMainMenu ()
	{
		if(fMainMenu.f == 0)
		{
			if(mainMenu.activeInHierarchy)
				mainMenu.SetActive(false);
		}
		else
		{
			fMainMenu.FadeOut();
		}
	}
	private void ToggleFadeOptionsMenu()
	{
		if(fOptions.f == 0)
		{
			if(options.activeInHierarchy)
				options.SetActive(false);
		}
		else
		{
			fOptions.FadeOut();
		}
	}

	private void ToggleFadeInputSelectMenu()
	{
		if(fInputSelect.f == 0)
		{
			if(inputSelect.activeInHierarchy)
				inputSelect.SetActive(false);
		}
		else
		{
			fInputSelect.FadeOut();
		}
	}
	private void ToggleFadeExitGameConfirm()
	{
		if(fQuitGame.f == 0)
		{
			if(exitGameConfirm.activeInHierarchy)
				exitGameConfirm.SetActive(false);
		}
		else
		{
			fQuitGame.FadeOut();
		}
	}   

}
