using UnityEngine.UI;

public class UIBackground : CustomCanvas
{
    public Image Background_01;
    public Image Background_02;


   public void SetBackground(int level)
    {
        if(level == 1 || level == 3)
        {
            Background_01.enabled = true;
            Background_02.enabled = false;
        }
        else
        {
            Background_01.enabled = false;
            Background_02.enabled = true;
        }
    }
}
