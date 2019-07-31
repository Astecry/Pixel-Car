using UnityEngine;


[CreateAssetMenu]
public class Car : ScriptableObject
{
    public Sprite img;
    public int price;
    public bool purchased,selected;
    public int heartCount;
}
