/*
 * Name: Adrian Norris
 * Date: 
 * Class and Teacher: Games and Virtual Environments, Dr. Clint Jeffery
 * Task: Homework #3 - Mob Duel Game v 1.5
 * */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Drawing;

namespace WindowsFormsApplication1
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}
