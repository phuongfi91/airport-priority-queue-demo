using System;

namespace AirportDEMO
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (AirportMain game = new AirportMain())
            {
                game.Run();
            }
        }
    }
#endif
}

