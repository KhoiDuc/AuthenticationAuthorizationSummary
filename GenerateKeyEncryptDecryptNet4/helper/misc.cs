using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenerateKeyEncryptDecryptNet4.helper
{
    public static class misc
    {
        private static void Print(string text)
        {
            Console.WriteLine(String.Format("\n[{0}] {1}", DateTime.Now.ToString(), text));
        }
    }
}
