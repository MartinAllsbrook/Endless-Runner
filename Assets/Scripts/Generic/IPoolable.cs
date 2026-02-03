/// <summary>
/// Optional interface for objects that need a reference to their pool
/// </summary>
public interface IPoolable<T> where T : UnityEngine.MonoBehaviour
{
    public void SetPool(ObjectPool<T> pool);
}
