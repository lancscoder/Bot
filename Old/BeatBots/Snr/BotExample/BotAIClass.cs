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
        private const string Rock = "ROCK";
        private const string Paper = "PAPER";
        private const string Scissor = "SCISSOR";
        private const string Dynamite = "DYNAMITE";
        private const string Waterbomb = "WATERBOMB";

        private static string _opponentName;
        private static string _lastOpponentsMove;
        private static int _pointstoWin;
        private static int _remainingRounds;
        private static int _dynamite;
        private static int _opponentsRemainingDynamite;

        private static List<string> _opponentMoves = new List<string>();
        private static List<string> _ourMoves = new List<string>();

        private static Random _random = new Random();
        


        /* Method called when start instruction is received
         *
         * POST http://<your_bot_url>/start
         *
         */
        internal static void SetStartValues(string opponentName, int pointstoWin, int maxRounds, int dynamite)
        {
            _opponentName = opponentName;
            _pointstoWin = pointstoWin;
            _remainingRounds = maxRounds;
            _dynamite = dynamite;
            _opponentsRemainingDynamite = dynamite;
            _opponentMoves = new List<string>();
            _ourMoves = new List<string>();
        }

        /* Method called when move instruction is received instructing opponents move
         *
         * POST http://<your_bot_url>/move
         *
         */
        public static void SetLastOpponentsMove(string lastOpponentsMove)
        {
            lastOpponentsMove = lastOpponentsMove.ToUpper();
            
            _lastOpponentsMove = lastOpponentsMove;
            _opponentMoves.Add(lastOpponentsMove);

            if (lastOpponentsMove == Dynamite)
                _opponentsRemainingDynamite--;
        }


        /* Method called when move instruction is received requesting your move
         *
         * GET http://<your_bot_url>/move
         *
         */
        internal static string GetMove()
        {
            var move = GenerateMove();

            if (move == Dynamite)
                _dynamite--;

            _ourMoves.Add(move);
            _remainingRounds--;

            return move;
        }

        internal static string GenerateMove()
        {
            var optimalMove = GetOptimalMove();

            if (!String.IsNullOrEmpty(optimalMove))
                return optimalMove;
            
            var availablemoves = new List<string> { Rock, Paper, Scissor };
            if (_dynamite > 0)
            {
                availablemoves.Add(Dynamite);
                if (_remainingRounds / _dynamite <= 2)
                    availablemoves.Add(Dynamite);
                if (_remainingRounds / _dynamite <= 3)
                    availablemoves.Add(Dynamite);
                if (_remainingRounds / _dynamite <= 4)
                    availablemoves.Add(Dynamite);
            }


            int rnd = _random.Next(availablemoves.Count);
            var move = availablemoves[rnd];

            return move;
        }

        internal static string GetOptimalMove()
        {
            if (_ourMoves.Count < 5)
                return null; //no optimal strategy to identify yet

            // opp does same move
            var last4MovesTheSame = _opponentMoves.TakeLast(4).Distinct().Count() == 1;
            if (last4MovesTheSame)
            {
                var lastMove = _opponentMoves.Last();
                
                return GetWinningMove(lastMove);
            }

            var mostlyTheLastMove = _opponentMoves.TakeLast(20).Count(m => m == _opponentMoves.Last()) > 13;
            if (mostlyTheLastMove)
            {
                var lastMove = _opponentMoves.Last();

                return GetWinningMove(lastMove);
            }

            var mostlyTheSecondLastMove = _opponentMoves.TakeLast(20).Count(m => m == _opponentMoves.TakeLast(2).First()) > 13;
            if (mostlyTheSecondLastMove)
            {
                var secondLastMove = _opponentMoves.TakeLast(2).First();

                return GetWinningMove(secondLastMove);
            }

            return null;
        }

        internal static string GetWinningMove(string move)
        {
            switch (move)
            {
                case Rock:
                    return Paper;
                case Paper:
                    return Scissor;
                case Scissor:
                    return Rock;
                case Dynamite:
                    return _opponentsRemainingDynamite > 0 ? Waterbomb : null;
                default:
                    return null;
            }
        }
    }

    public static class MiscExtensions
    {
        // Ex: collection.TakeLast(5);
        public static IEnumerable<T> TakeLast<T>(this IEnumerable<T> source, int N)
        {
            return source.Skip(Math.Max(0, source.Count() - N));
        }
    }
}
