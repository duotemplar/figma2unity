using UnityEngine;
using DummyUI;

public class InspectorPanelExample : MonoBehaviour
{
    [Header("Inspector Panel Reference")]
    public InspectorPanel inspectorPanel;
    
    [Header("Demo Settings")]
    public bool enableAutoDemo = true;
    public float demoUpdateInterval = 2f;
    
    private float demoTimer;
    private bool isDemoRunning;
    
    void Start()
    {
        // Find InspectorPanel if not assigned
        if (inspectorPanel == null)
        {
            inspectorPanel = FindObjectOfType<InspectorPanel>();
        }
        
        if (inspectorPanel == null)
        {
            Debug.LogWarning("InspectorPanel not found! Please create a GameObject with InspectorPanel component.");
            return;
        }
        
        // Bind to inspector events
        inspectorPanel.OnStartBake.AddListener(OnStartBakeClicked);
        inspectorPanel.OnCancelBake.AddListener(OnCancelBakeClicked);
        
        // Start demo if enabled
        if (enableAutoDemo)
        {
            StartDemo();
        }
        
        Debug.Log("InspectorPanelExample initialized. Inspector panel is ready for interaction.");
    }
    
    void Update()
    {
        if (isDemoRunning && enableAutoDemo)
        {
            demoTimer += Time.deltaTime;
            if (demoTimer >= demoUpdateInterval)
            {
                UpdateDemoValues();
                demoTimer = 0f;
            }
        }
    }
    
    private void OnStartBakeClicked()
    {
        Debug.Log("=== Start BakeAsync Clicked ===");
        Debug.Log($"Current Position: {inspectorPanel.GetPosition()}");
        Debug.Log($"Current Rotation: {inspectorPanel.GetRotation()}");
        Debug.Log($"Current Scale: {inspectorPanel.GetScale()}");
        
        // Simulate bake process
        SimulateBakeProcess();
    }
    
    private void OnCancelBakeClicked()
    {
        Debug.Log("=== Cancel Bake Clicked ===");
        Debug.Log("Bake process cancelled by user.");
        
        // Stop demo temporarily
        StopDemo();
        Invoke(nameof(StartDemo), 1f); // Restart after 1 second
    }
    
    private void SimulateBakeProcess()
    {
        // Simulate some baking work
        Debug.Log("Starting bake process...");
        Debug.Log("Processing probe data...");
        Debug.Log("Calculating occlusion...");
        Debug.Log("Bake completed successfully!");
    }
    
    public void StartDemo()
    {
        isDemoRunning = true;
        demoTimer = 0f;
        Debug.Log("Inspector Panel demo started. Watch the values change automatically.");
    }
    
    public void StopDemo()
    {
        isDemoRunning = false;
        Debug.Log("Inspector Panel demo stopped.");
    }
    
    private void UpdateDemoValues()
    {
        if (inspectorPanel == null) return;
        
        // Create some animated demo values
        float time = Time.time;
        
        // Animate position
        Vector3 newPosition = new Vector3(
            Mathf.Sin(time * 0.5f) * 2f,
            Mathf.Cos(time * 0.3f) * 1f,
            Mathf.Sin(time * 0.7f) * 3f
        );
        inspectorPanel.SetPosition(newPosition);
        
        // Animate rotation
        Vector3 newRotation = new Vector3(
            time * 10f % 360f,
            time * 15f % 360f,
            time * 20f % 360f
        );
        inspectorPanel.SetRotation(newRotation);
        
        // Animate scale (keep it reasonable)
        float scaleValue = 1f + Mathf.Sin(time * 0.8f) * 0.2f;
        Vector3 newScale = Vector3.one * scaleValue;
        inspectorPanel.SetScale(newScale);
        
        Debug.Log($"Demo Update - Pos: {newPosition:F2}, Rot: {newRotation:F1}, Scale: {scaleValue:F2}");
    }
    
    // Public methods for testing via Inspector or other scripts
    [ContextMenu("Test Start Bake")]
    public void TestStartBake()
    {
        OnStartBakeClicked();
    }
    
    [ContextMenu("Test Cancel Bake")]
    public void TestCancelBake()
    {
        OnCancelBakeClicked();
    }
    
    [ContextMenu("Reset Inspector Values")]
    public void ResetInspectorValues()
    {
        if (inspectorPanel != null)
        {
            inspectorPanel.SetPosition(Vector3.zero);
            inspectorPanel.SetRotation(Vector3.zero);
            inspectorPanel.SetScale(Vector3.one);
            Debug.Log("Inspector values reset to defaults.");
        }
    }
    
    [ContextMenu("Set Random Values")]
    public void SetRandomValues()
    {
        if (inspectorPanel != null)
        {
            Vector3 randomPos = new Vector3(
                Random.Range(-5f, 5f),
                Random.Range(-2f, 2f),
                Random.Range(-5f, 5f)
            );
            Vector3 randomRot = new Vector3(
                Random.Range(0f, 360f),
                Random.Range(0f, 360f),
                Random.Range(0f, 360f)
            );
            Vector3 randomScale = Vector3.one * Random.Range(0.5f, 2f);
            
            inspectorPanel.SetPosition(randomPos);
            inspectorPanel.SetRotation(randomRot);
            inspectorPanel.SetScale(randomScale);
            
            Debug.Log($"Set random values - Pos: {randomPos:F2}, Rot: {randomRot:F1}, Scale: {randomScale:F2}");
        }
    }
    
    void OnDestroy()
    {
        // Clean up event listeners
        if (inspectorPanel != null)
        {
            inspectorPanel.OnStartBake.RemoveListener(OnStartBakeClicked);
            inspectorPanel.OnCancelBake.RemoveListener(OnCancelBakeClicked);
        }
    }
}