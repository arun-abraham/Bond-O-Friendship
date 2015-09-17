﻿using UnityEngine;
using System.Collections;

public class CameraSplitter : MonoBehaviour {
	private static CameraSplitter instance;
	public static CameraSplitter Instance
	{
		get
		{
 			if (instance == null)
			{
				instance = GameObject.FindGameObjectWithTag("CameraSystem").GetComponent<CameraSplitter>();
			}
			return instance;
		}
	}

	public enum ZoomState{ZoomedIn, ZoomingIn, ZoomedOut, ZoomingOut};

	public ZoomState zoomState = ZoomState.ZoomedOut;

    //[HideInInspector]
	public bool splittable = true;
    //[HideInInspector]
    public bool followPlayers = true;

    [HideInInspector]
	public bool split = false;
	private bool wasSplit = false;

    private bool onStart = true;

    public float splitterDistance = .5f;
    [HideInInspector]
    public float splitterDistanceInWorldSpace = 0.0f;
    
    public float splitLineFadeDistance = .5f;

    [HideInInspector]
    public float splitLineFadeDistanceInWorldSpace = 0.0f;

	public CameraFollow mainCameraFollow;
	public CameraFollow splitCameraFollow;

	[HideInInspector]
	public Camera splitCamera1;
	[HideInInspector]
	public Camera splitCamera2;

	public GameObject player1;
	public GameObject player2;
	public AudioListener audioListener;

    [HideInInspector]
    public float playerDistance;
    private float zViewPortDistance = 0.0f;

    [HideInInspector]
    public bool zoom = false;
    [HideInInspector]
    public bool zoomIn = false;

    [HideInInspector]
    public bool movePlayers = false;

	public Vector3 startPos = new Vector3(28.1f, 22.1f, -70f);
    private Vector3 zoomPos = new Vector3(28.1f, 22.1f, -500f);

    [HideInInspector]
    public float duration = 5f;
    [HideInInspector]
    private float t = 1.0f;

	private float f = 0f;

	public bool zoomOutToggle = false;
	public bool zoomInToggle = false;
	
    private bool toggle = false;
	[HideInInspector]
	public bool startZoomComplete = false;

	public bool camerasCentered = false;
	public bool zoomedOut = false;
	public bool playersMoved = false;

	public GameObject player1Target;
	public GameObject player2Target;

	public float zCameraOffset = -500.0f;

	void Start()
	{
		if (Globals.Instance != null)
		{
			player1 = Globals.Instance.player1.gameObject;
			player2 = Globals.Instance.player2.gameObject;

            mainCameraFollow.player1 = splitCameraFollow.player2 = player1.transform;
            mainCameraFollow.player2 = splitCameraFollow.player1 = player2.transform;

			splitCamera1 = mainCameraFollow.GetComponentInChildren<Camera>();
			splitCamera2 = splitCameraFollow.GetComponentInChildren<Camera>();

			splitCamera1.transparencySortMode = TransparencySortMode.Orthographic;
			splitCamera2.transparencySortMode = TransparencySortMode.Orthographic;
		}
		wasSplit = split;
		CheckSplit(true);
	}

	void Update()
	{

		if (splittable)
			CheckSplit(false);	

        if(Input.GetKeyDown(KeyCode.A))
        {
            //Debug.Log(transform.position);
        }

		audioListener.transform.position = (player1.transform.position + player2.transform.position) / 2;
	}

    void SetSplitDistanceInWorldSpace()
    {
        Vector3 viewportCornerPoint = mainCameraFollow.childMainCamera.ViewportToWorldPoint(new Vector3(0.0f, 0.0f, zViewPortDistance));
        Vector3 viewportSplitPoint = mainCameraFollow.childMainCamera.ViewportToWorldPoint(new Vector3(0.0f, splitterDistance, zViewPortDistance));
        Vector3 fadeLineWorldPoint = mainCameraFollow.childMainCamera.ViewportToWorldPoint(new Vector3(0.0f, splitLineFadeDistance, zViewPortDistance));

        splitterDistanceInWorldSpace = (Mathf.Abs(viewportSplitPoint.y - viewportCornerPoint.y));
        splitLineFadeDistanceInWorldSpace = (Mathf.Abs(fadeLineWorldPoint.y - viewportCornerPoint.y));
    }

    private void SetzViewPortDistance()
    {
       zViewPortDistance = mainCameraFollow.childMainCamera.WorldToViewportPoint(player1.transform.position).z;
    }

	private void CheckSplit(bool forceCheck)
	{
        SetzViewPortDistance();
        SetSplitDistanceInWorldSpace();
        playerDistance = Vector3.Distance(player1.transform.position, player2.transform.position);
        split = playerDistance > splitterDistanceInWorldSpace;

        if(onStart)
        {
            split = false;
            onStart = false;
        }

        //Toggle split screen on and off
		if (split != wasSplit || forceCheck)
		{
            mainCameraFollow.pivot.transform.FindChild("Mask").gameObject.SetActive(split);
			for (int i = 0; i < splitCameraFollow.transform.childCount; i++)
			{
				splitCameraFollow.transform.GetChild(i).gameObject.SetActive(split);
			}

		}
		wasSplit = split;
	}

	public Camera GetFollowingCamera(GameObject player)
	{
        if (split && player == player2)
            return splitCameraFollow.GetComponentInChildren<Camera>();
        else
            return mainCameraFollow.GetComponentInChildren<Camera>();
	}

	public void JumpToPlayers()
	{
		if (Globals.Instance != null && Globals.Instance.player1 != null && Globals.Instance.player2 != null)
		{
			Vector3 oldCamPos = transform.position;
			Vector3 newCamPos = ((player1.transform.position + player2.transform.position) / 2);
			CameraSplitter.Instance.transform.position = new Vector3(newCamPos.x, newCamPos.y, oldCamPos.z);
		}
	}


	public void MovePlayers(Vector3 player1StartPos, Vector3 player2StartPos, bool paused = true)
	{
		if (paused) 
		{
			if (f < 1) 
			{
				f = Mathf.Clamp (f + Time.deltaTime / duration, 0.0f, 1.0f);
				player1.transform.position = Vector3.Lerp (player1StartPos, player1Target.transform.position, f);
				player2.transform.position = Vector3.Lerp (player2StartPos, player2Target.transform.position, f);
			}
		}
		else
		{
			if (f > 0) 
			{
				f = Mathf.Clamp (f - Time.deltaTime / duration, 0.0f, 1.0f);
				player1.transform.position = Vector3.Lerp (player1StartPos, player1Target.transform.position, f);
				player2.transform.position = Vector3.Lerp (player2StartPos, player2Target.transform.position, f);
			}
		}
	}

	public void SetZoomTarget(bool moveCamera = true)
	{
		//Center CameraSplitter gameobject
		Vector3 newCenterPos = ((player1.transform.position + player2.transform.position) / 2);
		newCenterPos.z = startPos.z;

		if(moveCamera)
		transform.position = newCenterPos;

        startPos = newCenterPos;
		
        if(!moveCamera)
        zoomPos = transform.position;
		//zoomPos.z = -200f;
	}

	public void Zoom(bool zoomingOut, bool isNewGameZoom = false)
	{
		Vector3 zoomPosition = startPos;
		zoomPosition.z = zCameraOffset;

        if (isNewGameZoom)
            zoomPosition = zoomPos;

		if (zoomingOut)
		{			
			if (t != 1)
			{
				zoomState = ZoomState.ZoomingOut;
				t = Mathf.Clamp(t + Time.deltaTime / duration, 0.0f, 1.0f);
				transform.position = Vector3.Lerp(startPos, zoomPosition, t);
			}
			else
			{
				zoomState = ZoomState.ZoomedOut;
			}
		} 
		else 
		{			
			if (t != 0)
			{
				zoomState = ZoomState.ZoomingIn;
				t = Mathf.Clamp(t - Time.deltaTime / duration, 0.0f, 1.0f);
				transform.position = Vector3.Lerp(startPos, zoomPosition, t);
			}
			else
			{
				zoomState = ZoomState.ZoomedIn;
			}
		}
	}





  
}
