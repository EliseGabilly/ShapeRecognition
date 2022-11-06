using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ZeppelinGames.GlyphRecogniser;

public class LineManager : Singleton<LineManager> {

    #region Variables
    [SerializeField]
    private float lineSeparationDistance = .2f;
    [SerializeField]
    private float lineWidth = .1f;
    [SerializeField]
    private Color lineColorDraw = Color.black;
    [SerializeField]
    private Color lineColor = Color.black;
    [SerializeField]
    private int lineCapVertices = 5;
    [SerializeField]
    private Transform linesContainer;

    List<Vector2> currentLine;
    LineRenderer currentLinerRenderer;
    [SerializeField]
    private Material material;

    private bool isDrawing = false;
    public int LineCount { get; set; } = 0;

    private Camera mainCamera;

    public float maxPointDist = 0.1f;
    public bool debugMode = false;

    //private List<Vector2> glyphPoints = new List<Vector2>();
    private List<GlyphSO> glyphs = new List<GlyphSO>();
    #endregion

    protected override void Awake() {
        base.Awake();
        lineColorDraw.a = 0.5f;
        mainCamera = Camera.main;

        GlyphSO[] loadedGlyphs = Resources.LoadAll<GlyphSO>("Glyphs/");
        Debug.Log("Loaded " + loadedGlyphs.Length + " glyphs");
        glyphs.AddRange(loadedGlyphs);
    }

    private void Update() {
        if ((Input.touchCount > 0 && !isDrawing) || Input.GetMouseButtonDown(0)) {
            StartCoroutine(nameof(Drawing));
        } else if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended || Input.GetMouseButtonUp(0)) {
            isDrawing = false;
        }
    }

    private IEnumerator Drawing() {
        isDrawing = true;
        LineCount++;
        StartLine();
        while (isDrawing) {
            AddPoint(GetCurrentWorldPoitn());
            yield return null;
        }
        EndLine();
    }

    private void StartLine() {
        //Init line
        currentLine = new List<Vector2>();
        GameObject goLine = new GameObject { name = "Line", tag = "Obstacle" };
        goLine.transform.parent = linesContainer;
        currentLinerRenderer = goLine.AddComponent<LineRenderer>();

        //Set values
        currentLinerRenderer.positionCount = 0;
        currentLinerRenderer.startWidth = lineWidth;
        currentLinerRenderer.endWidth = lineWidth;
        currentLinerRenderer.numCapVertices = lineCapVertices;
        currentLinerRenderer.material = material;
        currentLinerRenderer.startColor = lineColorDraw;
        currentLinerRenderer.endColor = lineColorDraw;
        currentLinerRenderer.sortingOrder = 20;
    }

    private Vector2 GetCurrentWorldPoitn() {
        Vector3 screenPosDepth = Input.mousePosition;
        screenPosDepth.z = -mainCamera.transform.position.z; // Give it camera depth
        Vector3 mousePos = mainCamera.ScreenToWorldPoint(screenPosDepth);
        return mousePos;
    }

    private void AddPoint(Vector2 point) {
        if (PlacePoint(point)) {
            currentLine.Add(point);
            currentLinerRenderer.positionCount++;
            currentLinerRenderer.SetPosition(currentLinerRenderer.positionCount - 1, point);
        }
    }

    private bool PlacePoint(Vector2 point) {
        return currentLine.Count == 0 || Vector2.Distance(point, currentLine[currentLine.Count - 1]) > lineSeparationDistance;
    }

    private void EndLine() {
        if (currentLine.Count > 1) {
            currentLinerRenderer.startColor = lineColor;
            currentLinerRenderer.endColor = lineColor;
            EdgeCollider2D lineEdgeTrigger = currentLinerRenderer.gameObject.AddComponent<EdgeCollider2D>();
            lineEdgeTrigger.edgeRadius = lineWidth / 2;
            lineEdgeTrigger.isTrigger = true;
            lineEdgeTrigger.SetPoints(currentLine);
            EdgeCollider2D lineEdgeCollider = currentLinerRenderer.gameObject.AddComponent<EdgeCollider2D>();
            lineEdgeCollider.edgeRadius = lineWidth / 2;
            lineEdgeCollider.SetPoints(currentLine);

            Reconize();
        }
    }

    private void Reconize() {
        List<GlyphReturnData> allMatchData = new List<GlyphReturnData>();
        foreach (GlyphSO matchGlyph in glyphs) {
            GlyphReturnData glyphData = MatchGlyph(currentLine.ToArray(), matchGlyph);
            Debug.Log(glyphData.glyphName + " : " +
                "\n     " + glyphData.matchPercent +
                " + " + glyphData.keyPointsPercent +
                " > " + matchGlyph.minMatchPercent +
                " = " + (glyphData.keyPointsPercent + glyphData.matchPercent > matchGlyph.minMatchPercent));
            /*   if(glyphData.keyPointsPercent >= matchGlyph.minKeyPointMatchPercentage && glyphData.matchPercent >= matchGlyph.minMatchPercentage)
               {
                   allMatchData.Add(glyphData);
               }*/
            if (glyphData.keyPointsPercent + glyphData.matchPercent > matchGlyph.minMatchPercent) {
                allMatchData.Add(glyphData);
            }
        }

        GlyphReturnData bestMatch = null;
        if (allMatchData.Count > 0) {
            float highestPercent = 0;
            foreach (GlyphReturnData data in allMatchData) {
                float avgPer = (data.keyPointsPercent + data.matchPercent) / 2;
                if (avgPer > highestPercent) {
                    highestPercent = avgPer;
                    bestMatch = data;
                }
            }
        }

        if (bestMatch != null) {
            //Run commmands
            Debug.Log("Best match : " + bestMatch.glyphName);
        }

        currentLine.Clear();
    }

}
