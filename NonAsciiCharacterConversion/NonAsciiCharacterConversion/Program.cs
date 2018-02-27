using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenFAST.UnitTests;
using System.IO;
using OpenFAST.Template.Loader;
using OpenFAST;
using OpenFAST.Template;


namespace NonAsciiCharacterConversion
{
    class Program : TemplateTest
    {

        private static IMessageTemplateLoader _loader;
        static void Main(string[] args)
        {
            string myString = string.Empty;
            byte[] data = new byte[] {
                215,255,148,50,48,49,56,48,50,49,50,45,48,54,58,51,56,58,51,55,46,54,53,185,177,49,27,154,128,128,128,129,95,127,127,192,128,243,83,89,66,69,65,78,73,68,82,50,48,70,69,66,50,48,49,184,50,56,53,54,183,248,11,25,38,62,14,253,128,128,128,128,128,128,128,2,29,156,128,128,128,128,128,128,128,128,128,128
            };




            string s = Encoding.ASCII.GetString(data);


            //TemplateTest objTempTest = new TemplateTest();
            //objTempTest.SetUp();
            //objTempTest.TestTemplates();


            //EncodeDecodeTest objEncodeDecodeTest = new EncodeDecodeTest();
            //objEncodeDecodeTest.TestComplexMessage();


            MessageTemplate[] objMessageTemplate = null;
            _loader = new XmlMessageTemplateLoader();

            using (FileStream stream = File.OpenRead("NCEDX.xml"))
                objMessageTemplate = _loader.Load(stream);


            var outStream = new MemoryStream();
          
            MessageTemplate template = _loader.TemplateRegistry[new QName("IncRefreshMDEntries")];
       


            var input = new MessageInputStream(new MemoryStream(data));
            input.RegisterTemplate(7, template);

            Message message = input.ReadMessage();
    



            Console.WriteLine(myString);
            Console.ReadLine();
        }


        public static string PreTradeNs { get; set; }








        public int u008A { get; set; }
    }
}
