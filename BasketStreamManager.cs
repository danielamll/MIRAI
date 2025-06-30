using UnityEngine;
using LSL;

public class BasketStreamManager : MonoBehaviour
{
    public static BasketStreamManager Instance;

    private StreamOutlet outlet;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); // Opcional

        var streamInfo = new StreamInfo("BasketMarkers", "Markers", 1, 0, channel_format_t.cf_string, System.Guid.NewGuid().ToString());
        outlet = new StreamOutlet(streamInfo);
    }

    public void SendMarker(string message)
    {
        if (outlet != null)
        {
            string[] sample = new string[] { message };
            outlet.push_sample(sample);
        }
    }
}
