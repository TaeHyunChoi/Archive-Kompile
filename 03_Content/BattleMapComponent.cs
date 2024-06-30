using UnityEngine;

public class BattleMapComponent : MonoBehaviour
{
    public byte Code { get => code; }
    public Vector3 Position { get => transform.position; set => transform.position = value; }

    [SerializeField]
    private byte code;

    public void SetActive(bool isOn)
    {
        gameObject.SetActive(isOn);
    }
}
