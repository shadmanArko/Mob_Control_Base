using UnityEngine;

namespace DefaultNamespace
{
    public interface ICloneableObject
    {
        GameObject GetCloneSource();
        void SetCloneSource(GameObject source);
    }
}