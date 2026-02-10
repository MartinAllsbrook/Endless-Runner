using UnityEngine;
using UnityEngine.U2D;

class Road : MonoBehaviour
{
    [Header("References")]
    [SerializeField] SpriteShapeController worldSpriteShape;
    [SerializeField] SpriteShapeController mapSpriteShape;
    [SerializeField] PolygonCollider2D roadCollider;
    
    [Header("Road Generation Settings")]
    [SerializeField] float pointSpacing = 50f;
    [SerializeField] float tangentStrength = 100f;
    [SerializeField] float curviness = 20f;
    [SerializeField] float curveFrequency = 0.05f;
    
    [Header("Road Collider Settings")]
    [SerializeField] float roadWidth = 20f;
    [SerializeField] int colliderResolution = 2;
    
    void Start()
    {
        SyncElements();
    }

    public void GenerateRoad(Vector3 startPoint, Vector3 startTangentDirection, Vector3 endPoint, Vector3 endTangentDirection)
    {
        GenerateNewRoad(startPoint, startTangentDirection, endPoint, endTangentDirection);
        SyncElements();
    }

    void SyncElements()
    {
        Spline spline = worldSpriteShape.spline;

        for (int i = 0; i < spline.GetPointCount(); i++)
        {
            Vector3 position = spline.GetPosition(i);
            Vector3 tangentDirection = spline.GetRightTangent(i).normalized;
            SetOrAddPoint(mapSpriteShape, i, position, tangentDirection, tangentStrength);
        }

        GenerateRoadCollider(spline);
    }

    void GenerateNewRoad(Vector3 startPoint, Vector3 startTangentDirection, Vector3 endPoint, Vector3 endTangentDirection)
    {
        Spline spline = worldSpriteShape.spline;
        spline.Clear();

        // Calculate how many points we'll need
        float distance = Vector3.Distance(startPoint, endPoint);
        int pointCount = Mathf.Max(2, Mathf.CeilToInt(distance / pointSpacing));
                
        // Find the points based on a bezier curve and add them to an array
        Vector3[] points = new Vector3[pointCount];
        
        Vector3 p0 = startPoint;
        Vector3 p1 = startPoint + startTangentDirection.normalized * 200f;
        Vector3 p2 = endPoint + endTangentDirection.normalized * 200f;
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
                float offset = Mathf.PerlinNoise(t * curveFrequency * 100f, Random.Range(0, 1000)) * curviness;
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
            
            SetOrAddPoint(spline, i, points[i], tangentDirection, tangentStrength);
        }
    }
    
    void GenerateRoadCollider(Spline spline)
    {
        int pointCount = spline.GetPointCount();
        int segmentCount = Mathf.Max(2, colliderResolution * (pointCount - 1));
        Vector2[] colliderPoints = new Vector2[segmentCount * 2];
        
        // Sample along the spline curve segments
        for (int i = 0; i < segmentCount; i++)
        {
            float t = i / (float)(segmentCount - 1);
            
            // Find which spline segment we're in
            int segmentIndex = Mathf.Min((int)(t * (pointCount - 1)), pointCount - 2);
            float localT = (t * (pointCount - 1)) - segmentIndex;
            
            // Get the bezier control points for this segment
            Vector3 p0 = spline.GetPosition(segmentIndex);
            Vector3 p1 = p0 + spline.GetRightTangent(segmentIndex);
            Vector3 p3 = spline.GetPosition(segmentIndex + 1);
            Vector3 p2 = p3 + spline.GetLeftTangent(segmentIndex + 1);
            
            // Evaluate position on the bezier curve
            Vector3 position = CubicBezier(p0, p1, p2, p3, localT);
            
            // Evaluate tangent using bezier derivative
            Vector3 tangent = CubicBezierDerivative(p0, p1, p2, p3, localT).normalized;
            
            // Calculate perpendicular direction (rotate tangent by 90 degrees)
            Vector3 perpendicular = new Vector3(-tangent.y, tangent.x, 0f);
            
            // Create left and right points to form polygon around the road
            colliderPoints[i] = position + perpendicular * (roadWidth * 0.5f);
            colliderPoints[segmentCount * 2 - 1 - i] = position - perpendicular * (roadWidth * 0.5f);
        }
        
        roadCollider.points = colliderPoints;
    }
    
    void SetOrAddPoint(Spline spline, int index, Vector3 position, Vector3 tangentDirection, float tangentLength)
    {
        if (index < spline.GetPointCount())
            spline.SetPosition(index, position);
        else
            spline.InsertPointAt(index, position);

        spline.SetTangentMode(index, ShapeTangentMode.Continuous);
        spline.SetRightTangent(index, tangentDirection.normalized * tangentLength);
        spline.SetLeftTangent(index, -tangentDirection.normalized * tangentLength);
    }

    void SetOrAddPoint(SpriteShapeController controller, int index, Vector3 position, Vector3 tangentDirection, float tangentLength)
    {
        if (index < controller.spline.GetPointCount())
            controller.spline.SetPosition(index, position);
        else
            controller.spline.InsertPointAt(index, position);

        controller.spline.SetTangentMode(index, ShapeTangentMode.Continuous);
        controller.spline.SetRightTangent(index, tangentDirection.normalized * tangentLength);
        controller.spline.SetLeftTangent(index, -tangentDirection.normalized * tangentLength);
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
    
    Vector3 CubicBezierDerivative(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {
        float u = 1 - t;
        float uu = u * u;
        float tt = t * t;
        
        Vector3 result = -3 * uu * p0;
        result += 3 * (uu - 2 * u * t) * p1;
        result += 3 * (2 * u * t - tt) * p2;
        result += 3 * tt * p3;
        
        return result;
    }
}