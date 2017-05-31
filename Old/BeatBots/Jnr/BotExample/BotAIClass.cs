using System;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using System.Threading;



namespace BotExample
{
    internal static class BotAIClass
    {
        private static string _opponentName;
        private static string _lastOpponentsMove;
        private static int _pointstoWin;
        private static int _maxRounds;
        private static int _dynamite;


        /* Method called when start instruction is received
         *
         * POST http://<your_bot_url>/start
         *
         */ 
        internal static void SetStartValues(string opponentName, int pointstoWin, int maxRounds, int dynamite)
        {
            _opponentName = opponentName;
            _pointstoWin = pointstoWin;
            _maxRounds = maxRounds;
            _dynamite = dynamite;
        }

        /* Method called when move instruction is received instructing opponents move
         *
         * POST http://<your_bot_url>/move
         *
         */ 
        public static void SetLastOpponentsMove(string lastOpponentsMove)
        {           
            _lastOpponentsMove = lastOpponentsMove;
        }


        /* Method called when move instruction is received requesting your move
         *
         * GET http://<your_bot_url>/move
         *
         */ 
        internal static string GetMove()
        {
            return GetRandomResponse();
        }

        internal static string GetRandomResponse()
        {
           
            Random random = new System.Random(Environment.TickCount);            
            int rnd = random.Next(5);
            switch (rnd)
            {
                case 0:
                    {
                        return "ROCK";
                    }
                case 1:
                    {
                        return "SCISSORS";
                    }
                case 2:
                    {
                        return "PAPER";
                    }
                case 3:
                {
                    return "WATERBOMB";
                }
                default:
                    {
                        _dynamite--;
                        return "DYNAMITE";
                    }
            }
        }       
    }
}
