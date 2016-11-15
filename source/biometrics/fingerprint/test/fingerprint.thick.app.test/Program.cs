using System;

using System.Collections.Generic;
using System.Windows.Forms;

//! @cond
namespace NoID.Biometrics
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [MTAThread]
        static void Main()
        {
            #if (!WindowsCE)
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.EnableVisualStyles();
            #endif
            Application.Run(new Form_Main());
        }
    }
}
//! @endcond