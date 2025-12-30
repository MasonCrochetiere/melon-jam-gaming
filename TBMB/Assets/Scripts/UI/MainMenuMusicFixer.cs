using UnityEngine;

public class MainMenuMusicFixer : MonoBehaviour
{

    private void OnMouseOver()
    {
        GetComponent<Collider2D>().enabled = false;
    }
}
