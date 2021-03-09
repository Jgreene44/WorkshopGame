using System;

namespace WorkshopGame
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            using (var game = new WorkshopGame())
                game.Run();
        }
    }
}
