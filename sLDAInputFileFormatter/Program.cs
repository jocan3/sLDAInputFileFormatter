using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.IO;
using System.Text.RegularExpressions;
using Microsoft.VisualBasic.FileIO;

namespace sLDAInputFileFormatter
{
    class Program
    {
        /*
         * Parses a csv file containing a text attribure and a class attribute into an text file in the format required for Bei's sLDA algorithm http://www.cs.cmu.edu/~chongw/slda/
         * @author Jose Andres Mena Arias
         */
        static void Main(string[] args)
        {
            if (args.Length < 3)
            {
                throw new Exception("Usage: inputFilePath outputFolder baseName");
            }

            string inputFile = args[0];
            string outputFolder = args[1];
            string baseName = args[2];
            string mode = args[3];

            if (mode == "1")
            {
                executeMode1(inputFile, outputFolder, baseName);
            }
            else if (mode == "2")
            {
                executeMode2(inputFile, outputFolder, baseName);
            }
            else if (mode == "3") {
                string labelsFile = args[4];
                executeMode3(inputFile, outputFolder, baseName, labelsFile);
            }
                
        }

        static void executeMode1(string inputFile, string outputFolder, string baseName) {
            string[] delimiters = { "," };
            string[] fields;
            TextFieldParser tfp;
            tfp = new TextFieldParser(inputFile);
            tfp.HasFieldsEnclosedInQuotes = true;
            tfp.Delimiters = delimiters;

            //HashSet<string> globalTerms = new HashSet<string>();
            Dictionary<string, int> globalTerms = new Dictionary<string, int>();
            List<Dictionary<int, int>> documentTerms = new List<Dictionary<int, int>>();
            List<string> labelOrgs = new List<string>();
            Dictionary<string, int> labelIndex = new Dictionary<string, int>();
            Dictionary<int, string> indexLabel = new Dictionary<int, string>();

            int labelCount = 0;

            fields = tfp.ReadFields();
            int classIndex = 0;
            int textIndex = 0;
            for (int i = 0; i < fields.Length; ++i)
            {
                if (fields[i] == "text")
                {
                    textIndex = i;
                }
                if (fields[i] == "class")
                {
                    classIndex = i;
                }
            }

            string path = outputFolder + @"\" + baseName + "_data.txt";

            StreamWriter swData = new StreamWriter(outputFolder + @"\" + baseName + "_data.txt");
            StreamWriter swLabelsInt = new StreamWriter(outputFolder + @"\" + baseName + "_labels_int.txt");
            StreamWriter swLabelsText = new StreamWriter(outputFolder + @"\" + baseName + "_labels.txt");

            StreamWriter swDataTest = new StreamWriter(outputFolder + @"\" + baseName + "_data_test.txt");
            StreamWriter swLabelsIntTest = new StreamWriter(outputFolder + @"\" + baseName + "_labels_int_test.txt");
            StreamWriter swLabelsTextTest = new StreamWriter(outputFolder + @"\" + baseName + "_labels_test.txt");


            while (!tfp.EndOfData)
            {
                fields = tfp.ReadFields();

                //Process label
                string labelOrg = fields[classIndex];
                labelOrgs.Add(labelOrg);
                string label = labelOrg.Replace("TEST:", "");
                int temp = 0;
                if (!labelIndex.TryGetValue(label, out temp))
                {
                    labelIndex[label] = labelCount;
                    indexLabel[labelCount] = label;
                    ++labelCount;
                }

                if (!labelOrg.Contains("TEST:"))
                {
                    swLabelsText.WriteLine(label);
                    swLabelsInt.WriteLine("" + labelIndex[label]);
                }
                else
                {
                    swLabelsTextTest.WriteLine(label);
                    swLabelsIntTest.WriteLine("" + labelIndex[label]);
                }

                //Process terms
                Dictionary<int, int> docTerms = new Dictionary<int, int>();
                string[] terms = fields[textIndex].Split();
                foreach (string oTerm in terms)
                {
                    string term = oTerm.ToUpper();
                    if (!globalTerms.TryGetValue(term, out temp))
                    {
                        globalTerms[term] = globalTerms.Count;
                    }

                    int termNumber = globalTerms[term];
                    if (docTerms.TryGetValue(termNumber, out temp))
                    {
                        docTerms[termNumber] = (temp + 1);
                    }
                    else
                    {
                        docTerms[termNumber] = 1;
                    }
                }

                documentTerms.Add(docTerms);

                /* string line = "";
                 foreach (var element in docTerms) {
                     line += " " + element.Key + ":" + element.Value;    
                 }

                 if (!labelOrg.Contains("TEST:")) {
                     swData.WriteLine(docTerms.Count + line);
                 }else
                 {
                     swDataTest.WriteLine(docTerms.Count + line);
                 }*/
            }

            int totalTerms = globalTerms.Count;
            int count = 0;
            foreach (var doc in documentTerms)
            {
                string line = "";
                int temp;
                for (int i = 0; i < totalTerms; ++i)
                {
                    if (doc.TryGetValue(i, out temp))
                    {
                        line += " " + i + ":" + temp;
                    }
                }
                if (!labelOrgs[count].Contains("TEST:"))
                {
                    swData.WriteLine(doc.Count + line);
                }
                else
                {
                    swDataTest.WriteLine(doc.Count + line);
                }
                ++count;
            }

            tfp.Close();
            swData.Flush();
            swLabelsInt.Flush();
            swLabelsText.Flush();
            swData.Close();
            swLabelsInt.Close();
            swLabelsText.Close();

            swDataTest.Flush();
            swLabelsIntTest.Flush();
            swLabelsTextTest.Flush();
            swDataTest.Close();
            swLabelsIntTest.Close();
            swLabelsTextTest.Close();

            Console.WriteLine("Total topics: " + labelIndex.Count);
            Console.ReadLine();
        }


        static void executeMode2(string inputFile, string outputFolder, string baseName)
        {
            string[] delimiters = { "," };
            string[] fields;
            TextFieldParser tfp;
            tfp = new TextFieldParser(inputFile);
            tfp.HasFieldsEnclosedInQuotes = true;
            tfp.Delimiters = delimiters;

            //HashSet<string> globalTerms = new HashSet<string>();
            Dictionary<string, int> globalTerms = new Dictionary<string, int>();
            List<Dictionary<int, int>> documentTerms = new List<Dictionary<int, int>>();
            List<string> labelOrgs = new List<string>();
            Dictionary<string, int> labelIndex = new Dictionary<string, int>();
            Dictionary<int, string> indexLabel = new Dictionary<int, string>();

            int labelCount = 0;

            fields = tfp.ReadFields();
            int classIndex = 2;
            
            string path = outputFolder + @"\" + baseName + "_data.txt";

            StreamWriter swData = new StreamWriter(outputFolder + @"\" + baseName + "_data.txt");
            StreamWriter swLabelsInt = new StreamWriter(outputFolder + @"\" + baseName + "_labels_int.txt");
            StreamWriter swLabelsText = new StreamWriter(outputFolder + @"\" + baseName + "_labels.txt");

            Console.WriteLine("classIndex: " + classIndex);


            while (!tfp.EndOfData)
            {
                fields = tfp.ReadFields();

                //Process label
                string labelOrg = fields[classIndex];
                labelOrgs.Add(labelOrg);
                string label = labelOrg.Replace("TEST:", "");
                int temp = 0;
               // Console.WriteLine("Label: " + label);

                if (!labelIndex.TryGetValue(label, out temp))
                {
                    labelIndex[label] = labelCount;
                    indexLabel[labelCount] = label;
                    ++labelCount;
                }
                swLabelsText.WriteLine(label);
                swLabelsInt.WriteLine("" + labelIndex[label]);

                //Process terms
                int termCounter = 0;
                string line = "";

                for (int i = 3; i < fields.Length; ++i) {
                    if (fields[i] != "0") {
                        line += " " + (i - 3) + ":" + fields[i];
                        ++termCounter;
                    }
                }
                line = termCounter + line;
                swData.WriteLine(line);
            }
            tfp.Close();
            swData.Flush();
            swLabelsInt.Flush();
            swLabelsText.Flush();
            swData.Close();
            swLabelsInt.Close();
            swLabelsText.Close();

            Console.WriteLine("Total topics: " + labelIndex.Count);
            Console.ReadLine();
        }

        static void executeMode3(string inputFile, string outputFolder, string baseName, string imputLabels)
        {
            string[] delimiters = { "," };
            string[] fields;
            TextFieldParser tfp;

            tfp = new TextFieldParser(imputLabels);
            tfp.HasFieldsEnclosedInQuotes = true;
            tfp.Delimiters = delimiters;

            Dictionary<string, int> labelIndex = new Dictionary<string, int>();

            int tmp;

            while (!tfp.EndOfData)
            {
                fields = tfp.ReadFields();
                if (!labelIndex.TryGetValue(fields[0], out tmp)) {
                    labelIndex[fields[0]] = labelIndex.Count;       
                }
            }


            tfp = new TextFieldParser(inputFile);
            tfp.HasFieldsEnclosedInQuotes = true;
            tfp.Delimiters = delimiters;

            //HashSet<string> globalTerms = new HashSet<string>();
            Dictionary<string, int> globalTerms = new Dictionary<string, int>();
            List<Dictionary<int, int>> documentTerms = new List<Dictionary<int, int>>();
            List<string> labelOrgs = new List<string>();
            Dictionary<int, string> indexLabel = new Dictionary<int, string>();

            int labelCount = 0;

            fields = tfp.ReadFields();
            int classIndex = 2;

            string path = outputFolder + @"\" + baseName + "_data.txt";

            StreamWriter swData = new StreamWriter(outputFolder + @"\" + baseName + "_data.txt");
            StreamWriter swLabelsInt = new StreamWriter(outputFolder + @"\" + baseName + "_labels_int.txt");
            StreamWriter swLabelsText = new StreamWriter(outputFolder + @"\" + baseName + "_labels.txt");

            Console.WriteLine("classIndex: " + classIndex);


            while (!tfp.EndOfData)
            {
                fields = tfp.ReadFields();

                //Process label
                string labelOrg = fields[classIndex];
                labelOrgs.Add(labelOrg);
                string label = labelOrg.Replace("TEST:", "");
                int temp = 0;
                // Console.WriteLine("Label: " + label);

                if (!labelIndex.TryGetValue(label, out temp))
                {
                    labelIndex[label] = labelIndex.Count;
                    indexLabel[labelCount] = label;
                }
                swLabelsText.WriteLine(label);
                swLabelsInt.WriteLine("" + labelIndex[label]);

                //Process terms
                int termCounter = 0;
                string line = "";

                for (int i = 3; i < fields.Length; ++i)
                {
                    if (fields[i] != "0")
                    {
                        line += " " + (i - 3) + ":" + fields[i];
                        ++termCounter;
                    }
                }
                line = termCounter + line;
                swData.WriteLine(line);
            }
            tfp.Close();
            swData.Flush();
            swLabelsInt.Flush();
            swLabelsText.Flush();
            swData.Close();
            swLabelsInt.Close();
            swLabelsText.Close();

            Console.WriteLine("Total topics: " + labelIndex.Count);
            Console.ReadLine();
        }
    }
}
