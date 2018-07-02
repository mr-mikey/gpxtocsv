using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace gpxtotxt
{
    public class Funkcje
    {
        List<XmlDocument> docList = new List<XmlDocument>();
        int colsWPT = 0;
        int rowsWPT = 1;
        int nrColWPT = 1;
        int colsTRK = 0;
        int rowsTRK = 1;
        int nrColTRK = 0;
        double progressBar = 0;
        double progress = 0;
        public string StripTagsCharArray(string source)
        {
            char[] array = new char[source.Length];
            int arrayIndex = 0;
            bool inside = false;

            for (int i = 0; i < source.Length; i++)
            {
                char let = source[i];
                if (let == '<')
                {
                    inside = true;
                    continue;
                }
                if (let == '>')
                {
                    inside = false;
                    continue;
                }
                if (!inside)
                {
                    array[arrayIndex] = let;
                    arrayIndex++;
                }
            }
            return new string(array, 0, arrayIndex);
        }
        public void CzytajPliki(String path)
        {
            try
            {
                Console.WriteLine("Loading files:");
                for (int i = 0; i < Directory.GetFiles(path, "*.gpx").Length; i++)
                {
                    XmlDocument doc = new XmlDocument();
                    doc.Load(Directory.GetFiles(path, "*.gpx")[i]);
                    docList.Add(doc);
                    Console.WriteLine(Directory.GetFiles(path, "*.gpx")[i]);
                    Thread.Sleep(500);
                }
                Console.WriteLine("Proceeding files:");
            }
            catch (Exception e)
            {
                Console.WriteLine("{0} Caught exception.", e);
                Thread.Sleep(10000);
            }
        }
        public void SprawdzPliki()
        {
            for (int j = 0; j < docList.Count; j++)
            {
                if (docList[j].GetElementsByTagName("wpt").Count > 0)
                {
                    rowsWPT += docList[j].GetElementsByTagName("wpt").Count;
                    //colsWPT = docList[j].GetElementsByTagName("wpt")[0].ChildNodes.Count + 5;
                    colsWPT = 10;
                }
                if (docList[j].GetElementsByTagName("trkpt").Count > 0)
                {
                    rowsTRK += docList[j].GetElementsByTagName("trkpt").Count;
                    // colsTRK = docList[j].GetElementsByTagName("trkpt")[0].ChildNodes.Count + 5; //+2 => lat and lon + name which is elsewhere
                    colsTRK = 10;
                }
            }
            progressBar = rowsWPT + rowsTRK;
        }
        public void PrzetworzPliki()
        {
            String[][] arrayWPT = new String[rowsWPT][];
            String[][] arrayTRK = new String[rowsTRK][];
            for (int i = 0; i < arrayWPT.Length; i++)
            {
                arrayWPT[i] = new string[colsWPT];
            }
            for (int i = 0; i < arrayTRK.Length; i++)
            {
                arrayTRK[i] = new string[colsTRK];
            }
            // wypisz nazwy do tablic
            for (int h = 0; h < colsWPT; h++) 
            {
                switch (h)
                {
                    case 0: //LP
                        {
                            arrayWPT[0][h] = "Lp;";
                           
                            break;
                        }
                    case 1: //name
                        {
                            arrayWPT[0][h] = "Nazwa;";
                           
                            break;
                        }
                    case 2: //lat
                        {
                            arrayWPT[0][h] = "Szerokość;";
                           
                            break;
                        }
                    case 3: //long
                        {
                            arrayWPT[0][h] = "Długość;";
                           
                            break;
                        }
                    case 4: //ele
                        {
                            arrayWPT[0][h] = "Elewacja;";
                           
                            break;
                        }
                    case 5: //pdop
                        {
                            arrayWPT[0][h] = "Błąd;";
                           
                            break;
                        }
                    case 6: //data
                        {
                            arrayWPT[0][h] = "Data;";
                           
                            break;
                        }
                    case 7: //godzina
                        {
                            arrayWPT[0][h] = "Godzina;";
                           
                            break;
                        }
                    case 8: //link
                        {
                            arrayWPT[0][h] = "NazwaZdjecia;";
                           
                            break;
                        }
                    case 9: //uwagi
                        {
                            arrayWPT[0][h] = "Uwagi;";
                           
                            break;
                        }
                    default:
                        {
                            break;
                        }
                }
            }
            //Przetwarzanie plikow po kolei
            for (int j = 0; j < docList.Count; j++)
            {

                if (docList[j].GetElementsByTagName("wpt").Count > 0) 
                {
                    for (int z = nrColWPT; z <= docList[j].GetElementsByTagName("wpt").Count-1+nrColWPT; z++)
                    {
                        PasekStanu();
                        for (int h = 0; h < colsWPT; h++)
                        {
                            string wptDane;
                            switch (h)
                            {
                                case 0: //LP
                                    {
                                        arrayWPT[z][h] = z.ToString() + ";";
                                        break;
                                    }
                                case 1: //name
                                    {
                                        for(int i = 0; i< docList[j].GetElementsByTagName("wpt")[z - nrColWPT].ChildNodes.Count; i++)
                                        {
                                             
                                            if (docList[j].GetElementsByTagName("wpt")[z - nrColWPT].ChildNodes[i].Name == "name")
                                            {
                                                wptDane = docList[j].GetElementsByTagName("wpt")[z - nrColWPT].ChildNodes[i].InnerText;
                                                arrayWPT[z][h] = wptDane + ";";
                                            }
                                        }
                                        

                                        break;
                                    }
                                case 2: //lat
                                    {
                                        arrayWPT[z][h] = docList[j].GetElementsByTagName("wpt")[z - nrColWPT].Attributes[0].InnerText + ";";

                                        break;
                                    }
                                case 3: //long
                                    {
                                        arrayWPT[z][h] = docList[j].GetElementsByTagName("wpt")[z - nrColWPT].Attributes[1].InnerText + ";";

                                        break;
                                    }
                                case 4: //ele
                                    {
                                        for (int i = 0; i < docList[j].GetElementsByTagName("wpt")[z - nrColWPT].ChildNodes.Count; i++)
                                        {
                                            
                                            if (docList[j].GetElementsByTagName("wpt")[z - nrColWPT].ChildNodes[i].Name == "ele")
                                            {
                                                wptDane = docList[j].GetElementsByTagName("wpt")[z - nrColWPT].ChildNodes[i].InnerText;
                                                arrayWPT[z][h] = wptDane + ";";
                                            }
                                        }

                                        break;
                                    }
                                case 5: //pdop
                                    {
                                        for (int i = 0; i < docList[j].GetElementsByTagName("wpt")[z - nrColWPT].ChildNodes.Count; i++)
                                        {
                                            
                                            if (docList[j].GetElementsByTagName("wpt")[z - nrColWPT].ChildNodes[i].Name == "pdop")
                                            {
                                                wptDane = docList[j].GetElementsByTagName("wpt")[z - nrColWPT].ChildNodes[i].InnerText;
                                                arrayWPT[z][h] = wptDane + ";";
                                            }
                                        }

                                        break;
                                    }
                                case 6: //data
                                    {
                                        for (int i = 0; i < docList[j].GetElementsByTagName("wpt")[z - nrColWPT].ChildNodes.Count; i++)
                                        {
                                           
                                            if (docList[j].GetElementsByTagName("wpt")[z - nrColWPT].ChildNodes[i].Name == "time")
                                            {
                                                wptDane = docList[j].GetElementsByTagName("wpt")[z - nrColWPT].ChildNodes[i].InnerText;
                                                arrayWPT[z][h] = wptDane.Substring(0, wptDane.IndexOf("T"))+ ";";
                                            }
                                        }

                                        break;
                                    }
                                case 7: //godzina
                                    {
                                        for (int i = 0; i < docList[j].GetElementsByTagName("wpt")[z - nrColWPT].ChildNodes.Count; i++)
                                        {
                                           
                                            if (docList[j].GetElementsByTagName("wpt")[z - nrColWPT].ChildNodes[i].Name == "time")
                                            {
                                                wptDane = docList[j].GetElementsByTagName("wpt")[z - nrColWPT].ChildNodes[i].InnerText;
                                                arrayWPT[z][h] = wptDane.Substring(wptDane.IndexOf("T")+1, wptDane.IndexOf("Z")- wptDane.IndexOf("T")-1)+";";
                                            }
                                        }

                                        break;
                                    }
                                case 8: //link
                                    {
                                        for (int i = 0; i < docList[j].GetElementsByTagName("wpt")[z - nrColWPT].ChildNodes.Count; i++)
                                        {
                                            
                                            if (docList[j].GetElementsByTagName("wpt")[z - nrColWPT].ChildNodes[i].Name == "link")
                                            {
                                                wptDane = docList[j].GetElementsByTagName("wpt")[z - nrColWPT].ChildNodes[i].Attributes[0].Value;
                                                wptDane = wptDane.Substring(wptDane.IndexOf("/") + 1,wptDane.Length-2);
                                                wptDane = wptDane.Substring(wptDane.IndexOf("/") + 1, wptDane.Length - wptDane.IndexOf("/")-1)+";";
                                                arrayWPT[z][h] = wptDane;
                                            }
                                        }

                                        break;
                                    }
                                case 9: //uwagi
                                    {
                                        for (int i = 0; i < docList[j].GetElementsByTagName("wpt")[z - nrColWPT].ChildNodes.Count; i++)
                                        {
                                            wptDane = docList[j].GetElementsByTagName("wpt")[z - nrColWPT].ChildNodes[i].InnerText;
                                            if (docList[j].GetElementsByTagName("wpt")[z - nrColWPT].ChildNodes[i].Name == "desc")
                                            {
                                                //arrayWPT[z][h] = wptDane + ";";
                                            }
                                        }

                                        break;
                                    }
                            }
                        }
                        /*if (docList[j].GetElementsByTagName("wpt")[z-1].ChildNodes[g] == null && docList[j].GetElementsByTagName("wpt")[z-1].ChildNodes[3].Name == "desc")
                            {
                                char[] liczby = new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
                                String ss = StripTagsCharArray(docList[j].GetElementsByTagName("wpt")[0].ChildNodes[3].InnerText).Trim();
                                string[] tablica = new string[10];
                                tablica = ss.Split('\n');
                                for (int i = 0; i < docList[j].GetElementsByTagName("wpt")[z-1].Attributes.Count; i++)
                                {
                                    arrayWPT[z][g - 1 + i] = docList[j].GetElementsByTagName("wpt")[z-1].Attributes[i].Value + ";";
                                }
                                for (int i = 0; i < tablica.Length; i++)
                                {
                                    arrayWPT[z][g + 1 + i] = tablica[i].Substring(tablica[i].IndexOfAny(liczby)) + ";";
                                }
                                break;
                            }
                            if (g >= 3 && g < 7)
                            {
                                if(docList[j].GetElementsByTagName("wpt")[z - 1].ChildNodes[g + 1].Name == "link")
                                {
                                    arrayWPT[z][g] = docList[j].GetElementsByTagName("wpt")[z - 1].ChildNodes[g + 1].Attributes[0].Value + ";";
                                }
                                else
                                {
                                    arrayWPT[z][g] = docList[j].GetElementsByTagName("wpt")[z - 1].ChildNodes[g+1].InnerText + ";";
                                }

                            }
                            else if( g<7)
                            {
                                arrayWPT[z][g] = docList[j].GetElementsByTagName("wpt")[z - 1].ChildNodes[g].InnerText + ";";
                            }*/
                    }
                    nrColWPT = docList[j].GetElementsByTagName("wpt").Count + nrColWPT;

                }

                if (docList[j].GetElementsByTagName("trkpt").Count > 0) // to do
                {
                    for (int z = nrColTRK; z <= docList[j].GetElementsByTagName("trkpt").Count - 1 + nrColTRK; z++)
                    {
                        PasekStanu();
                        for (int h = 0; h < colsWPT; h++)
                        {
                            string trkDane;
                            switch (h)
                            {
                                case 0: //LP
                                    {

                                        arrayTRK[z][h] = z.ToString() + ";";
                                        break;
                                    }
                                case 1: //name
                                    {
                                        for (int i = 0; i < docList[j].GetElementsByTagName("trkpt")[0].ChildNodes.Count; i++)
                                        {
                                           
                                            if (docList[j].GetElementsByTagName("trk")[0].ChildNodes[i].Name == "name")
                                            {
                                                trkDane = docList[j].GetElementsByTagName("trk")[0].ChildNodes[i].InnerText;
                                                arrayTRK[z][h] = trkDane + ";";
                                            }
                                        }


                                        break;
                                    }
                                case 2: //lat
                                    {
                                        arrayTRK[z][h] = docList[j].GetElementsByTagName("trkpt")[z - nrColTRK].Attributes[0].InnerText + ";";

                                        break;
                                    }
                                case 3: //long
                                    {
                                        arrayTRK[z][h] = docList[j].GetElementsByTagName("trkpt")[z - nrColTRK].Attributes[1].InnerText + ";";

                                        break;
                                    }
                                case 4: //ele
                                    {
                                        for (int i = 0; i < docList[j].GetElementsByTagName("trkpt")[z - nrColTRK].ChildNodes.Count; i++)
                                        {
                                            
                                            if (docList[j].GetElementsByTagName("trkpt")[z - nrColTRK].ChildNodes[i].Name == "ele")
                                            {
                                                trkDane = docList[j].GetElementsByTagName("trkpt")[z - nrColTRK].ChildNodes[i].InnerText;
                                                arrayTRK[z][h] = trkDane + ";";
                                            }
                                        }

                                        break;
                                    }
                                case 5: //pdop
                                    {
                                        for (int i = 0; i < docList[j].GetElementsByTagName("trkpt")[z - nrColTRK].ChildNodes.Count; i++)
                                        {
                                            
                                            if (docList[j].GetElementsByTagName("trkpt")[z - nrColTRK].ChildNodes[i].Name == "pdop")
                                            {
                                                trkDane = docList[j].GetElementsByTagName("trkpt")[z - nrColTRK].ChildNodes[i].InnerText;
                                                arrayTRK[z][h] = trkDane + ";";
                                            }
                                        }

                                        break;
                                    }
                                case 6: //data
                                    {
                                        for (int i = 0; i < docList[j].GetElementsByTagName("trkpt")[z - nrColTRK].ChildNodes.Count; i++)
                                        {
                                            
                                            if (docList[j].GetElementsByTagName("trkpt")[z - nrColTRK].ChildNodes[i].Name == "time")
                                            {
                                                trkDane = docList[j].GetElementsByTagName("trkpt")[z - nrColTRK].ChildNodes[i].InnerText;
                                                arrayTRK[z][h] = trkDane.Substring(0, trkDane.IndexOf("T")) + ";";
                                            }
                                        }

                                        break;
                                    }
                                case 7: //godzina
                                    {
                                        for (int i = 0; i < docList[j].GetElementsByTagName("trkpt")[z - nrColTRK].ChildNodes.Count; i++)
                                        {
                                            
                                            if (docList[j].GetElementsByTagName("trkpt")[z - nrColTRK].ChildNodes[i].Name == "time")
                                            {
                                                trkDane = docList[j].GetElementsByTagName("trkpt")[z - nrColTRK].ChildNodes[i].InnerText;
                                                arrayTRK[z][h] = trkDane.Substring(trkDane.IndexOf("T") + 1, trkDane.IndexOf("Z") - trkDane.IndexOf("T") - 1) + ";";
                                            }
                                        }

                                        break;
                                    }
                                default: 
                                    {
                                        break;
                                    }
                            }
                        }
                    }
                    nrColTRK = docList[j].GetElementsByTagName("trkpt").Count + nrColTRK;
                }

               
            }
            StreamWriter file = new StreamWriter("data.csv");
            //my2darray  is my 2d array created.
            for (int i = 0; i < rowsWPT; i++)
            {
                for (int j = 0; j < colsWPT; j++)
                {
                    file.Write(arrayWPT[i][j]);
                }
                file.Write("\n"); // go to next line
            }
            for (int i = 0; i < rowsTRK; i++)
            {
                for (int j = 0; j < colsTRK; j++)
                {
                    file.Write(arrayTRK[i][j]);
                }
                file.Write("\n"); // go to next line
            }
            file.Close();
        }
        public void PasekStanu()
        {
            progress++;
            if( progress<= progressBar)
            {
                if (Math.Round(progress / progressBar * 100) % 1 == 0)
                {
                    Console.Clear();
                    Console.WriteLine("Ładowanie:");
                    Console.WriteLine(Math.Round(progress / progressBar * 100) + "%");
                }
            }
            
        }
    }
    class Program
    {
        
        static void Main(string[] args)
        {
            Funkcje funkcje = new Funkcje();
            Console.WriteLine("Enter the path to your gpx files:");
            Console.WriteLine(@"Example: d:\folder\files");
            funkcje.CzytajPliki(Console.ReadLine());
            funkcje.SprawdzPliki();
            funkcje.PrzetworzPliki();


        }

    }
        
    
}
