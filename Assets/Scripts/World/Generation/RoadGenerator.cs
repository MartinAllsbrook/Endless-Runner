using UnityEngine;
using UnityEngine.U2D;

class RoadGenerator : MonoBehaviour
{
    [SerializeField] SpriteShapeController spriteShape;
    
    [Header("Road Generation Settings")]
    [SerializeField] float pointSpacing = 50f;
    [SerializeField] float tangentStrength = 100f;
    [SerializeField] float curviness = 20f;
    [SerializeField] float curveFrequency = 0.05f;
    [SerializeField] int randomSeed = 0;

    void Start()
    {
        GenerateRoad(Vector3.zero, Vector3.right, new Vector3(300f, 200f, 0f), -Vector3.right);
    }

    void GenerateRoad(Vector3 startPoint, Vector3 startTangentDirection, Vector3 endPoint, Vector3 endTangentDirection)
    {
        if (spriteShape == null)
        {
            Debug.LogError("SpriteShapeController reference is missing!");
            return;
        }

        // Calculate how many points we'll need
        float distance = Vector3.Distance(startPoint, endPoint);
        int pointCount = Mathf.Max(2, Mathf.CeilToInt(distance / pointSpacing));
        
        // Initialize random with seed for consistent results
        Random.InitState(randomSeed);
        
        // Find the points based on a bezier curve and add them to an array
        Vector3[] points = new Vector3[pointCount];
        
        Vector3 p0 = startPoint;
        Vector3 p1 = startPoint + startTangentDirection.normalized * 100f;
        Vector3 p2 = endPoint + endTangentDirection.normalized * 100f;
        Vector3 p3 = endPoint;
        
        for (int i = 0; i < pointCount; i++)
        {
            float t = i / (float)(pointCount - 1);
            Vector3 position = CubicBezier(p0, p1, p2, p3, t);
            
            // Add perpendicular variation for curviness (except at start and end)
            if (i > 0 && i < pointCount - 1)
            {
                Vector3 direction = (endPoint - startPoint).normalized;
                Vector3 perpendicular = new Vector3(-direction.y, direction.x, 0f);
                float offset = Mathf.PerlinNoise(t * curveFrequency * 100f, randomSeed) * curviness;
                offset = (offset - curviness * 0.5f); // Center around 0
                position += perpendicular * offset;
            }
            
            points[i] = position;
        }
        
        // For each point in the array, add it with appropriate tangents
        for (int i = 0; i < points.Length; i++)
        {
            Vector3 tangentDirection;
            
            if (i == 0)
            {
                // Start point: use start tangent
                tangentDirection = startTangentDirection;
            }
            else if (i == points.Length - 1)
            {
                // End point: use end tangent
                tangentDirection = -endTangentDirection;
            }
            else
            {
                // Middle point: tangent parallel to vector from previous to next point
                tangentDirection = (points[i + 1] - points[i - 1]).normalized;
            }
            
            SetOrAddPoint(i, points[i], tangentDirection, tangentStrength);
        }
    }
    
    void SetOrAddPoint(int index, Vector3 position, Vector3 tangentDirection, float tangentLength)
    {
        if (index < spriteShape.spline.GetPointCount())
            spriteShape.spline.SetPosition(index, position);
        else
            spriteShape.spline.InsertPointAt(index, position);

        spriteShape.spline.SetTangentMode(index, ShapeTangentMode.Continuous);
        spriteShape.spline.SetRightTangent(index, tangentDirection.normalized * tangentLength);
        spriteShape.spline.SetLeftTangent(index, -tangentDirection.normalized * tangentLength);
    }
    
    Vector3 CubicBezier(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {
        float u = 1 - t;
        float uu = u * u;
        float uuu = uu * u;
        float tt = t * t;
        float ttt = tt * t;
        
        Vector3 result = uuu * p0;
        result += 3 * uu * t * p1;
        result += 3 * u * tt * p2;
        result += ttt * p3;
        
        return result;
    }
}