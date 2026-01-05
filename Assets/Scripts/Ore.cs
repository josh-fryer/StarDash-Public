using System;

public class Ore
{
    // 1 = common
    // 2 = uncommon
    // 3 = rare
    public int type { get; set; } 
    public int points
    {
        get
        {
            if (type <= 0)
            {
                throw new ArgumentException("Ore type must have a value");
            }

            switch (type)
            {
                case 1:
                    return 50;
                case 2:
                    return 150;
                case 3:
                    return 250;
                default:
                    throw new ArgumentException("Ore type did not match any switch case.");
            }
        }
    }
}