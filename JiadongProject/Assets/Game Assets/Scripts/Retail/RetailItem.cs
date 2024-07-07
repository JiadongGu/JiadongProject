using NaughtyAttributes;
using UnityEngine;

[CreateAssetMenu()]
public class RetailItem : ScriptableObject
{
    public string itemName;
    public float orderTimeSeconds;
    public Vector2 cogsRange = new Vector2(15, 40);
    [ShowAssetPreview] public Sprite icon;
}


