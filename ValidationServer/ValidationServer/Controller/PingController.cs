using System;
using System.Web.Http;

namespace ValidationServer.Controller
{
    public class PingController : ApiController
    {
        private string[] _quotes
        {
            get
            {
                return new string[]
                {
                    "Let our advance worrying become advance thinking and planning.",
                    "In preparing for battle I have always found that plans are useless, but planning is indispensable.",
                    "If you don’t know where you are going, you’ll end up someplace else.",
                    "Give me six hours to chop down a tree and I will spend the first four sharpening the axe.",
                    "You can’t depend on your eyes when your imagination is out of focus.",
                    "Lack of direction, not lack of time, is the problem. We all have twenty-four hour days.",
                    "Amateurs sit and wait for inspiration, the rest of us just get up and go to work.",
                    "Efficiency is doing things right; effectiveness is doing the right things."
                };
            }
        }

        public string Get()
        {
            return this._quotes[new Random().Next(0, this._quotes.Length)];
        }
    }
}