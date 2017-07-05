using System;
using Ak.DataAccess.XML;

namespace Ak.All
{
    // ReSharper disable InconsistentNaming
    ///<summary>
    ///</summary>
    public class Program
    {
        static void Main()
        {
            Console.Write("Akeon Library. Add the DLLs to your project.");

            var node = new Node(@"D:\Caroline\Handhara\MEAK\_A\00.xml");

            node[0][0].Value += "...";

            node.BackUpAndSave();

            Console.Read();
        }
    }
    // ReSharper restore InconsistentNaming
}
