using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BotApi.Extensions;
using BotApi.Models;

namespace BotApi.Controllers
{

    [Route("api/bot")]
    public class BotController : Controller
    {
        private static string _opponentName;
        private static string _lastOpponentsMove;
        private static int _pointstoWin;
        private static int _remainingRounds;
        private static int _dynamite;
        private static int _opponentsRemainingDynamite;

        private static List<string> _opponentMoves = new List<string>();
        private static List<string> _ourMoves = new List<string>();

        private static Random _random = new Random();


        [HttpGet("start")]
        public Task<string> Start(int dynamiteCount, int pointstoWin, int maxRounds, string opponentName)
        {
            _opponentName = opponentName;
            _pointstoWin = pointstoWin;
            _remainingRounds = maxRounds;
            _dynamite = dynamiteCount;
            _opponentsRemainingDynamite = dynamiteCount;
            _opponentMoves = new List<string>();
            _ourMoves = new List<string>();

            return Task.FromResult("Blah 5");
        }

        [HttpGet("move")]
        public Task<string> Move()
        {
            var move = GenerateMove();

            if (move == Moves.Dynamite)
            {
                _dynamite--;
            }

            _ourMoves.Add(move);
            _remainingRounds--;

            return Task.FromResult(move);
        }

        [HttpPost("move")]
        public Task<string> MovePost()
        {
            // TODO : Test
            var lastOpponentsMove = new StreamReader(this.Request.Body).ReadToEnd();

            lastOpponentsMove = lastOpponentsMove.ToUpper();

            _lastOpponentsMove = lastOpponentsMove;
            _opponentMoves.Add(lastOpponentsMove);

            if (lastOpponentsMove == Moves.Dynamite)
            {
                _opponentsRemainingDynamite--;
            }

            return Task.FromResult("Move");
        }

        internal static string GenerateMove()
        {
            var optimalMove = GetOptimalMove();

            if (!String.IsNullOrEmpty(optimalMove))
            {
                return optimalMove;
            }

            var availablemoves = new List<string> { Moves.Rock, Moves.Paper, Moves.Scissor };
            if (_dynamite > 0)
            {
                availablemoves.Add(Moves.Dynamite);
                if (_remainingRounds / _dynamite <= 2)
                    availablemoves.Add(Moves.Dynamite);
                if (_remainingRounds / _dynamite <= 3)
                    availablemoves.Add(Moves.Dynamite);
                if (_remainingRounds / _dynamite <= 4)
                    availablemoves.Add(Moves.Dynamite);
            }

            int rnd = _random.Next(availablemoves.Count);
            var move = availablemoves[rnd];

            return move;
        }

        internal static string GetOptimalMove()
        {
            if (_ourMoves.Count < 5)
            {
                return null; //no optimal strategy to identify yet
            }

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
                case Moves.Rock:
                    return Moves.Paper;
                case Moves.Paper:
                    return Moves.Scissor;
                case Moves.Scissor:
                    return Moves.Rock;
                case Moves.Dynamite:
                    return _opponentsRemainingDynamite > 0 ? Moves.Waterbomb : null;
                default:
                    return null;
            }
        }
    }
}