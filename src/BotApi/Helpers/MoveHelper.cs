using System;
using System.Collections.Generic;
using System.Linq;
using BotApi.Models;

namespace BotApi.Helpers
{
    public class MoveHelper
    {
        public static string GenerateMove(List<string> ourMoves, List<string> opponentMoves, int dynamite, int opponentsRemainingDynamite, int remainingRounds, Random random)
        {
            var optimalMove = GetOptimalMove(ourMoves, opponentMoves, opponentsRemainingDynamite);

            if (!String.IsNullOrEmpty(optimalMove))
            {
                return optimalMove;
            }

            var availablemoves = new List<string> { Moves.Rock, Moves.Paper, Moves.Scissor };
            if (dynamite > 0)
            {
                availablemoves.Add(Moves.Dynamite);
                if (remainingRounds / dynamite <= 2)
                    availablemoves.Add(Moves.Dynamite);
                if (remainingRounds / dynamite <= 3)
                    availablemoves.Add(Moves.Dynamite);
                if (remainingRounds / dynamite <= 4)
                    availablemoves.Add(Moves.Dynamite);
            }

            int rnd = random.Next(availablemoves.Count);
            var move = availablemoves[rnd];

            return move;
        }

        internal static string GetOptimalMove(List<string> ourMoves, List<string> opponentMoves, int opponentsRemainingDynamite)
        {
            if (ourMoves.Count < 5)
            {
                return null; //no optimal strategy to identify yet
            }

            // opp does same move
            var last4MovesTheSame = opponentMoves.TakeLast(4).Distinct().Count() == 1;
            if (last4MovesTheSame)
            {
                var lastMove = opponentMoves.Last();

                return GetWinningMove(lastMove, opponentsRemainingDynamite);
            }

            var mostlyTheLastMove = opponentMoves.TakeLast(20).Count(m => m == opponentMoves.Last()) > 13;
            if (mostlyTheLastMove)
            {
                var lastMove = opponentMoves.Last();

                return GetWinningMove(lastMove, opponentsRemainingDynamite);
            }

            var mostlyTheSecondLastMove = opponentMoves.TakeLast(20).Count(m => m == opponentMoves.TakeLast(2).First()) > 13;
            if (mostlyTheSecondLastMove)
            {
                var secondLastMove = opponentMoves.TakeLast(2).First();

                return GetWinningMove(secondLastMove, opponentsRemainingDynamite);
            }

            return null;
        }

        internal static string GetWinningMove(string move, int opponentsRemainingDynamite)
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
                    return opponentsRemainingDynamite > 0 ? Moves.Waterbomb : null;
                default:
                    return null;
            }
        }
    }
}