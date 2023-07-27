using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubtitelsToSpeech
{
    internal class Subtitle
    {
        private TimeOnly timeStart;
        private TimeOnly timeEnd;
        private string text;
        public Subtitle(TimeOnly timeStart, TimeOnly timeEnd, string text) 
        {
            this.timeStart = new TimeOnly(timeStart.Hour, timeStart.Minute, timeStart.Second, timeStart.Millisecond);
            this.timeEnd = new TimeOnly(timeEnd.Hour, timeEnd.Minute, timeEnd.Second, timeEnd.Millisecond);
            this.text = text;
        }

        public int StartInMilliSeconds()
        {
            TimeOnly Duration = this.timeStart;
            return Duration.Hour * 3600000 + Duration.Minute * 60000 + Duration.Second * 1000 + Duration.Millisecond;
        }

        public int EndInMilliSeconds()
        {
            TimeOnly Duration = this.timeEnd;
            return Duration.Hour * 3600000 + Duration.Minute * 60000 + Duration.Second * 1000 + Duration.Millisecond;
        }

        public int DurationMilliSeconds()
        {
            return EndInMilliSeconds() - StartInMilliSeconds();
        }

        public TimeOnly GetTimeStart()
        {
            return this.timeStart;
        }

        public TimeOnly GetTimeEnd()
        {
            return this.timeEnd;
        }

        public string GetText()
        {
            return this.text;
        }

        public override string ToString()
        {
            return "Start: " + this.timeStart.ToTimeSpan() + "\nEnd: " + this.timeEnd.ToTimeSpan() + "\nText: " + this.text;
        }
    }
}
