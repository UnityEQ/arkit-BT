using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.EventSystems;

public class PlaceObjectsOnPlane : MonoBehaviour
{
	[SerializeField]
	[Tooltip("Instantiates this prefab on a plane at the touch location.")]
	GameObject m_PlacedPrefab;
	public TrackingManager tManager;

	/// <summary>
	/// The prefab to instantiate on touch.
	/// </summary>
	public GameObject placedPrefab
	{
		get { return m_PlacedPrefab; }
		set { m_PlacedPrefab = value; }
	}

	/// <summary>
	/// The object instantiated as a result of a successful raycast intersection with a plane.
	/// </summary>
	public GameObject spawnedObject { get; private set; }

	/// <summary>
	/// Invoked whenever an object is placed in on a plane.
	/// </summary>
	public static event Action onPlacedObject;

	ARRaycastManager m_RaycastManager;

	static List<ARRaycastHit> s_Hits = new List<ARRaycastHit>();
	
	[SerializeField]
	int m_MaxNumberOfObjectsToPlace = 1;

	int m_NumberOfPlacedObjects = 0;
	
	public float initialFingersDistance;
	public Vector3 initialScale;
	public static Transform ScaleTransform;
	
	private float _sensitivity = 1f;
	private Vector3 _mouseReference;
	private Vector3 _mouseOffset;
	private Vector3 _rotation = Vector3.zero;
	public bool _isRotating;
	public bool _isScaling;
	public bool _isTranslating;
	public GameObject playPanel;
	public GameObject moveObj;
	public GameObject scaleObj;
	public GameObject rotateObj;
	
	
	//static int lastTouchCount;
	bool isFirstFrameWithTwoTouches;
	float cachedTouchAngle;
	float cachedTouchDistance;
	public float cachedAugmentationScale;
	Vector3 cachedAugmentationRotation;
	const float scaleRangeMin = .1f;
	const float scaleRangeMax = 5.0f;

	void Awake()
	{
		m_RaycastManager = GetComponent<ARRaycastManager>();
	}
	
	void Start()
	{
	}

	void OnDisable()
	{
		// reset objects when component is disabled
		m_NumberOfPlacedObjects = 0;
	}
	
	
	public void DoMovement(int picker)
	{
		switch(picker)
		{
			case 0:
			_isTranslating = true;
			_isScaling = false;
			_isRotating = false;
			moveObj.GetComponent<Image>().color = new Color32(226,106,106,255);
			scaleObj.GetComponent<Image>().color = new Color32(255,255,255,255);
			rotateObj.GetComponent<Image>().color = new Color32(255,255,255,255);
			break;
			
			case 1:
			_isTranslating = false;
			_isScaling = true;
			_isRotating = false;
			moveObj.GetComponent<Image>().color = new Color32(255,255,255,255);
			scaleObj.GetComponent<Image>().color = new Color32(226,106,106,255);
			rotateObj.GetComponent<Image>().color = new Color32(255,255,255,255);
			break;
			
			case 2:
			_isTranslating = false;
			_isScaling = false;
			_isRotating = true;
			moveObj.GetComponent<Image>().color = new Color32(226,255,255,255);
			scaleObj.GetComponent<Image>().color = new Color32(255,255,255,255);
			rotateObj.GetComponent<Image>().color = new Color32(226,106,106,255);
			break;
			
			default:
			break;
		}
	}
	
	bool IsPointOverUIObject(Vector2 pos)
	{
		if (EventSystem.current.IsPointerOverGameObject())
			return false;

		PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
		eventDataCurrentPosition.position = new Vector2(pos.x, pos.y);
		List<RaycastResult> results = new List<RaycastResult>();
		EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
		return results.Count > 0;
	}
	
	public void CacheIt()
	{
		cachedAugmentationScale = spawnedObject.transform.localScale.x;
		cachedAugmentationRotation = spawnedObject.transform.localEulerAngles;
	}
	
	void Update()
	{
		if (Input.touchCount > 0)
		{
			Touch touch = Input.GetTouch(0);
			Touch[] touches = Input.touches;
			
            if (touch.phase == TouchPhase.Began)
			{
				isFirstFrameWithTwoTouches = true;
				if (!IsPointOverUIObject(touch.position) && m_RaycastManager.Raycast(touch.position, s_Hits, TrackableType.PlaneEstimated))
				{
					Pose hitPose = s_Hits[0].pose;

					if (m_NumberOfPlacedObjects < m_MaxNumberOfObjectsToPlace)
					{
						spawnedObject = Instantiate(m_PlacedPrefab, hitPose.position, hitPose.rotation);
						tManager.arux.FadeOffCurrentUI();
						playPanel.SetActive(true);
						DoMovement(0);
						m_NumberOfPlacedObjects++;
					}
					else
					{
						if(_isTranslating){spawnedObject.transform.SetPositionAndRotation(hitPose.position, spawnedObject.transform.rotation);}
					}
					
					if (onPlacedObject != null)
					{
						onPlacedObject();
					}
				}
				CacheIt();
			}
			else
			{
				if(_isTranslating){
					if(touch.phase == TouchPhase.Moved){
						if (!IsPointOverUIObject(touch.position) && m_RaycastManager.Raycast(touch.position, s_Hits, TrackableType.PlaneEstimated))
						{
							Pose hitPose = s_Hits[0].pose;
							spawnedObject.transform.SetPositionAndRotation(hitPose.position, spawnedObject.transform.rotation);
						}
					}
				}

				if (Input.touchCount == 2 && _isScaling)
				{	
					float currentTouchDistance = Vector2.Distance(touches[0].position, touches[1].position);
					float diff_y = touches[0].position.y - touches[1].position.y;
					float diff_x = touches[0].position.x - touches[1].position.x;
					float currentTouchAngle = Mathf.Atan2(diff_y, diff_x) * Mathf.Rad2Deg;

					if (isFirstFrameWithTwoTouches)
					{
						cachedTouchDistance = currentTouchDistance;
						cachedTouchAngle = currentTouchAngle;
						isFirstFrameWithTwoTouches = false;
					}
					float scaleMultiplier = (currentTouchDistance / cachedTouchDistance);
					float scaleAmount = cachedAugmentationScale * scaleMultiplier;
					float scaleAmountClamped = Mathf.Clamp(scaleAmount, scaleRangeMin, scaleRangeMax);
					spawnedObject.transform.localScale = new Vector3(scaleAmountClamped, scaleAmountClamped, scaleAmountClamped);
				}
				
				if (Input.touchCount == 2 && _isRotating)
				{
					float currentTouchDistance = Vector2.Distance(touches[0].position, touches[1].position);
					float diff_y = touches[0].position.y - touches[1].position.y;
					float diff_x = touches[0].position.x - touches[1].position.x;
					float currentTouchAngle = Mathf.Atan2(diff_y, diff_x) * Mathf.Rad2Deg;

					if (isFirstFrameWithTwoTouches)
					{
						cachedTouchDistance = currentTouchDistance;
						cachedTouchAngle = currentTouchAngle;
						isFirstFrameWithTwoTouches = false;
					}

					float angleDelta = currentTouchAngle - cachedTouchAngle;
					spawnedObject.transform.localEulerAngles = cachedAugmentationRotation - new Vector3(0, angleDelta * 3f, 0);
				}
			}
			//if(spawnedObject != null){CacheIt();}
		}
	}
}
