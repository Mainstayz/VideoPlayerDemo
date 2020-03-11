using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class Alert_IOS : MonoBehaviour
{
    // Start is called before the first frame update
    [DllImport("__Internal")]
    private static extern void ShowAlert(string msg);

    public static void Alert(string msg)
    {
        ShowAlert(msg);
    }
}
