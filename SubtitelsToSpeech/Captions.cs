using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubtitelsToSpeech
{
    internal class Captions
    {
        private Subtitle[] captions;
        public Captions(string fileDirection) 
        {
            StreamReader srtFile = new StreamReader(fileDirection);
            captions = new Subtitle[CountSubtitles(fileDirection)];
            string line = "";
            for (int i = 0; i < this.captions.Length; i++)
            {
                string textSubtitle = "";
                while (!line.Contains("-->"))
                    line = srtFile.ReadLine();
                TimeOnly timeStart = new TimeOnly(int.Parse(line.Substring(0, 2)), int.Parse(line.Substring(3, 2)), int.Parse(line.Substring(6, 2)), int.Parse(line.Substring(9, 3)));
                TimeOnly timeEnd = new TimeOnly(int.Parse(line.Substring(17, 2)), int.Parse(line.Substring(20, 2)), int.Parse(line.Substring(23, 2)), int.Parse(line.Substring(26, 3)));
                line = srtFile.ReadLine();
                while (line != "")
                {
                    textSubtitle += line;
                    line = srtFile.ReadLine();
                }
                this.captions[i] = new Subtitle(timeStart, timeEnd, textSubtitle);
            }
            srtFile.Close();
        }
        private static int CountSubtitles(string fileDirection)
        {
            StreamReader srtFile = new StreamReader(fileDirection);
            string line = srtFile.ReadLine(); ;
            int result = 0, countSubtitles = 0;
            while (line != null)
            {
                if (int.TryParse(line, out result))
                    countSubtitles++;
                line = srtFile.ReadLine();
            }
            return countSubtitles;
            srtFile.Close();
        }

        public int GetAmount()
        {
            return this.captions.Length;
        }
        public Subtitle GetSubtitle(int index) 
        {
            return this.captions[index];
        }

        public override string ToString()
        {
            string str = "";
            for (int i = 0; i < this.captions.Length; i++)
            {
                str += captions[i].ToString() + "\n";
            }
            return str;
        }
    }
}
