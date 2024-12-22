using UnityEngine;
using UnityEngine.UI;

public class DebugSystem : MonoBehaviour
{
    // 인풋을 화면에 나타낼 것인지
    public bool EnableInputPrint;

    public Text DebugUI;

    // Update is called once per frame
    void Update()
    {
        if (EnableInputPrint == false)
        {
            DebugUI.text = "";
            return;
        }

        string NewText = "";

        if (Input.GetKeyDown(KeyCode.Q))
        {
            NewText += KeyCode.Q.ToString();
        }
        else if (Input.GetKeyDown(KeyCode.W))
        {
            NewText += KeyCode.W.ToString();
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            NewText += KeyCode.E.ToString();
        }
        else if (Input.GetKeyDown(KeyCode.R))
        {
            NewText += KeyCode.R.ToString();
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            NewText += KeyCode.A.ToString();
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            NewText += KeyCode.D.ToString();
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            NewText += KeyCode.LeftArrow.ToString();
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            NewText += KeyCode.RightArrow.ToString();
        }
        else if (Input.GetKeyDown(KeyCode.Space))
        {
            NewText += KeyCode.Space.ToString();
        }

        if (NewText.Length > 0)
        {
            DebugUI.text = NewText;
        }
    }
}
