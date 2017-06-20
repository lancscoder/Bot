using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BotApi.Extensions;
using BotApi.Helpers;
using BotApi.Models;
using BotApi.Repository;

namespace BotApi.Controllers
{
    [Route("api/bot")]
    public class BotController : Controller
    {
        private static string _opponentName;
        private static string _lastOpponentsMove;
        private static int _pointstoWin;
        private static int _remainingRounds;

        private static int _originalDynamite;
        private static int _ourRemainingDynamite;
        private static int _opponentsRemainingDynamite;

        private static List<string> _opponentMoves = new List<string>();
        private static List<string> _ourMoves = new List<string>();

        private static Random _random = new Random();

        private readonly IRepository<LoggingItem> _repository;

        public BotController(IRepository<LoggingItem> repository)
        {
            _repository = repository;
        }

        [HttpGet("start")]
        public Task<string> Start(int dynamiteCount, int pointstoWin, int maxRounds, string opponentName)
        {
            // TODO : Log here....
            if (!String.IsNullOrWhiteSpace(_opponentName))
            {
                // Start of a new game so log the other stuff
                // TODO  Makei if fire n forget....
                var loggingItem = new LoggingItem()
                {
                    Time = DateTime.Now,
                    Content = "Fooooo " + DateTime.Now.ToString()
                };

                var document = _repository.CreateDcumentAsync(loggingItem).Result;
            }

            _opponentName = opponentName;
            _pointstoWin = pointstoWin;
            _remainingRounds = maxRounds;

            _originalDynamite = dynamiteCount;
            _ourRemainingDynamite = dynamiteCount;
            _opponentsRemainingDynamite = dynamiteCount;

            _opponentMoves = new List<string>();
            _ourMoves = new List<string>();

            return Task.FromResult("Start");
        }

        [HttpGet("move")]
        public Task<string> Move()
        {
            var move = MoveHelper.GenerateMove(_ourMoves, _opponentMoves, _ourRemainingDynamite, _opponentsRemainingDynamite, _remainingRounds, _random);

            if (move == Moves.Dynamite)
            {
                _ourRemainingDynamite--;
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
    }
}