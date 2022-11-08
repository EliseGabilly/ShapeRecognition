using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using System.IO;

using PDollarGestureRecognizer;

public class Demo : MonoBehaviour {

    #region Variable
	private RuntimePlatform platform;
	private List<Gesture> trainingSet = new List<Gesture>();

    public Transform gestureOnScreenPrefab;
	private List<LineRenderer> gestureLinesRenderer = new List<LineRenderer>();
	private LineRenderer currentGestureLineRenderer;
	private Vector3 virtualKeyPosition = Vector2.zero;

	private List<Point> points = new List<Point>();

	private bool inFrame = false;
	private bool isDrawing = false;
	private int vertexCount = 0;

	// UI
	[SerializeField]
	private InputField inputTxt;
	[SerializeField]
	private Text txt;
	[SerializeField]
	private RectTransform drawArea;
	[SerializeField]
	private Toggle toggleAdd;
    #endregion

    void Start() {
		platform = Application.platform;

		//Load user custom gestures
		string[] filePaths = Directory.GetFiles(Application.persistentDataPath, "*.xml");
		foreach (string filePath in filePaths)
			trainingSet.Add(GestureIO.ReadGestureFromFile(filePath));
	}

	void Update() {
		if (platform == RuntimePlatform.Android || platform == RuntimePlatform.IPhonePlayer) {
			if (Input.touchCount > 0) {
				virtualKeyPosition = new Vector3(Input.GetTouch(0).position.x, Input.GetTouch(0).position.y);
			}
		} else {
			if (Input.GetMouseButton(0)) {
				virtualKeyPosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y);
			}
		}

		if (inFrame) {
			if (Input.GetMouseButtonDown(0)) {
				isDrawing = true;
				txt.text = "";
				points.Clear();
				vertexCount = 0;

				foreach (LineRenderer lineRenderer in gestureLinesRenderer) {
					lineRenderer.positionCount = 0;
					Destroy(lineRenderer.gameObject);
				}

				gestureLinesRenderer.Clear();

				Transform tmpGesture = Instantiate(gestureOnScreenPrefab, transform.position, transform.rotation) as Transform;
				currentGestureLineRenderer = tmpGesture.GetComponent<LineRenderer>();

				gestureLinesRenderer.Add(currentGestureLineRenderer);

			} else if (Input.GetMouseButtonUp(0)) {
				Recognize();

			} else if (Input.GetMouseButton(0)) {
				points.Add(new Point(virtualKeyPosition.x, -virtualKeyPosition.y, 1));

				currentGestureLineRenderer.positionCount = ++vertexCount;
				currentGestureLineRenderer.SetPosition(vertexCount - 1, Camera.main.ScreenToWorldPoint(new Vector3(virtualKeyPosition.x, virtualKeyPosition.y, 10)));
			}
		}
	}

	public void Recognize() {
		isDrawing = false;
		if(points.Count <= 3) { // Not enought point to recognize
			return;
        }

		Gesture candidate = new Gesture(points.ToArray());
		Result gestureResult = PointCloudRecognizer.Classify(candidate, trainingSet.ToArray());

		txt.text = gestureResult.GestureClass + " " + gestureResult.Score;
        if (toggleAdd.isOn) {
			AddShape();
		}
	}

	public void AddShape() {
		string newGestureName = inputTxt.text;
		if (points.Count > 0 && newGestureName != "") {
			string fileName = String.Format("{0}/{1}-{2}.xml", Application.persistentDataPath, newGestureName, DateTime.Now.ToFileTime());

			#if !UNITY_WEBPLAYER
			GestureIO.WriteGesture(points.ToArray(), newGestureName, fileName);
			#endif

			trainingSet.Add(new Gesture(points.ToArray(), newGestureName));
		}
	}

	public void OnPointerEnter(){
		inFrame = true;
    }

	public void OnPointerExit(){
		if (isDrawing) {
			Recognize();
		}
		inFrame = false;
    }

}
