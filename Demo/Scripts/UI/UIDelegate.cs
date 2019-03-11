using System;
public class UIDelegate 
{
    public static UIDelegate Delegate
    {
        get
        {
            return Instance ?? (Instance = new UIDelegate());
        }
    }
    private static UIDelegate Instance;

    public Action UpdataBagUI;
}
