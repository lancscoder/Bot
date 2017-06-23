using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BotApi.Extensions;
using BotApi.Helpers;
using BotApi.Models;
using BotApi.Repository;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace BotApi.Controllers
{
    [Route("api/bot")]
    [Route("/")]
    public class BotController : Controller
    {
        private static string _opponentName;
        private static string _lastOpponentsMove;
        private static int _pointstoWin;


        private static int _numberOfRounds;
        private static int _remainingRounds;

        private static int _originalDynamite;
        private static int _ourRemainingDynamite;
        private static int _opponentsRemainingDynamite;

        private static List<string> _opponentMoves = new List<string>();
        private static List<string> _ourMoves = new List<string>();

        private static Random _random = new Random();

        private readonly IRepository<LoggingItem> _repository;
        private readonly ILogger<BotController> _logger;

        public BotController(IRepository<LoggingItem> repository, ILogger<BotController> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        [HttpPost("start")]
        public Task<string> PostStart()
        {
            int dynamiteCount = 0;
            int pointsToWin = 0;
            int maxRounds = 0;
            string opponentName = "";

            try
            {
                var options = new StreamReader(this.Request.Body).ReadToEnd();

                foreach (var option in options.Split(new[] { '&' }))
                {
                    var parameters = option.Split(new[] { '=' });

                    if (parameters.Length == 2)
                    {
                        switch (parameters[0].ToLower())
                        {
                            case "dynamitecount":
                                int.TryParse(parameters[1], out dynamiteCount);
                                break;
                            case "pointstowin":
                                int.TryParse(parameters[1], out pointsToWin);
                                break;
                            case "maxrounds":
                                int.TryParse(parameters[1], out maxRounds);
                                break;
                            case "opponentname":
                                opponentName = parameters[1];
                                break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }

            try
            {
                _logger.LogInformation("POST START: Dynamite Count: {dynamiteCount}, Pints to Win; {pointsToWin}, Max Rounds: {maxRounds}, Opponent Name: {opponentName}.", dynamiteCount, pointsToWin, maxRounds, opponentName);

                if (!String.IsNullOrWhiteSpace(_opponentName))
                {
                    // Start of a new game so log the other stuff
                    // TODO  Makei if fire n forget....
                    var loggingItem = new LoggingItem()
                    {
                        Time = DateTime.Now,
                        OpponentName = _opponentName,
                        DynamiteCount = _originalDynamite,
                        PointsToWin = _pointstoWin,
                        MaxRounds = _numberOfRounds,
                        OurMoves = _ourMoves.ToArray(),
                        OpponentMoves = _opponentMoves.ToArray(),
                        Moves = _ourMoves.Select((m, i) => new LoggingMove()
                        {
                            Round = i + 1,
                            OurMove = m,
                            TheirMove = (_opponentMoves.Count() - 1) < i ? "N/A" : _opponentMoves[i]
                        }).ToArray()
                    };

                    var document = _repository.CreateDcumentAsync(loggingItem).Result;
                }
            }
            catch (Exception ex)
            {
                // Try logging if fails dont care.
            }

            _opponentName = opponentName;
            _pointstoWin = pointsToWin;

            _numberOfRounds = maxRounds;
            _remainingRounds = maxRounds;

            _originalDynamite = dynamiteCount;
            _ourRemainingDynamite = dynamiteCount;
            _opponentsRemainingDynamite = dynamiteCount;

            _opponentMoves = new List<string>();
            _ourMoves = new List<string>();

            return Task.FromResult(HttpContext.Request.Path.Value);
        }

        [HttpGet("move")]
        public Task<string> GetMove()
        {
            _logger.LogInformation("GET MOVE");

            var move = MoveHelper.GenerateMove(_ourMoves, _opponentMoves, _ourRemainingDynamite, _opponentsRemainingDynamite, _remainingRounds, _random);

            if (move == Moves.Dynamite)
            {
                _ourRemainingDynamite--;
            }

            _ourMoves.Add(move);
            _remainingRounds--;

            _logger.LogInformation("Our Move: {move}", move);

            return Task.FromResult(move);
        }

        [HttpPost("move")]
        public Task<string> PostMove()
        {
            var lastOpponentsMove = new StreamReader(this.Request.Body).ReadToEnd();

            _logger.LogInformation("POST MOVE: {lastOpponentsMove}", lastOpponentsMove);

            lastOpponentsMove = lastOpponentsMove.ToUpper();

            _lastOpponentsMove = lastOpponentsMove;
            _opponentMoves.Add(lastOpponentsMove);

            if (lastOpponentsMove == Moves.Dynamite)
            {
                _opponentsRemainingDynamite--;
            }

            _logger.LogInformation("Their move: {lastOpponentsMove}", lastOpponentsMove);

            return Task.FromResult(HttpContext.Request.Path.Value);
        }
    }
}