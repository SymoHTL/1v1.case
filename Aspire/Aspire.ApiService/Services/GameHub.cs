﻿using Microsoft.AspNetCore.SignalR;
using Shared.Entities;

namespace Aspire.ApiService.Services;

public class GameHub(ILogger<GameHub> logger, IPlayerRepository playerRepository, HttpClient csgoClient) : Hub<IGameClient> {
    public async Task JoinGame(string gameId, string playerName) {
        logger.LogInformation("Player {ConnectionId} joined game {GameId}", Context.ConnectionId, gameId);
        var p = await playerRepository.AddPlayer(new Player(Context.ConnectionId, gameId, playerName));
        await Clients.Caller.SetLocalPlayer(p);
        await Groups.AddToGroupAsync(Context.ConnectionId, gameId);
        await Clients.Group(gameId).PlayerJoined(p);
    }
    
    public async Task SetBet(float bet) {
        var player = await playerRepository.GetPlayer(Context.ConnectionId);
        if (player == null) return;
        player.Bet = bet;
        await playerRepository.UpdatePlayer(player);
        await Clients.Group(player.GameId).ReceiveBetChange(Context.ConnectionId, bet);
    }
    
    public async Task SetCase(string caseId) {
        var player = await playerRepository.GetPlayer(Context.ConnectionId);
        if (player == null) return;
        player.CaseId = caseId;
        await playerRepository.UpdatePlayer(player);
        await Clients.Group(player.GameId).ReceiveCaseChange(Context.ConnectionId, caseId);
    }
    
    public async Task SetReady(bool ready) {
        var player = await playerRepository.GetPlayer(Context.ConnectionId);
        if (player == null) return;
        player.Ready = player.CaseId != null && player.Bet != 0 && ready;
        await playerRepository.UpdatePlayer(player);
        await Clients.Group(player.GameId).ReceiveReadyChange(Context.ConnectionId, player.Ready);
        if (!player.Ready) return;
        await CheckAllReady(player.GameId);
    }

    private async Task CheckAllReady(string groupId) {
        var players = await playerRepository.GetPlayers(groupId);
        if (players.Any(p => !p.Ready)) return;
        var results = await SimulateCases(players);
        logger.LogInformation("Round Finished: {Results}", results);
        await Clients.Group(groupId).StartRound(results);
        foreach (var player in players) player.Ready = false;
        await playerRepository.UpdatePlayers(players);
        await Clients.Group(groupId).ReceiveReadyReset(players.Select(p => p.ConnId).ToList(), false);
    }

    private async Task<Dictionary<string, string>> SimulateCases(List<Player> players) {
        var cases = await csgoClient.GetFromJsonAsync<List<Case>>("crates.json");
        if (cases is null) return new Dictionary<string, string>();
        cases = cases.Where(c => c.Name.Contains("case", StringComparison.OrdinalIgnoreCase)).ToList();
        
        var results = new Dictionary<string, string>();
        foreach (var player in players) {
            var @case = cases.FirstOrDefault(c => c.Id == player.CaseId);
            if (@case is null) continue;
            results.Add(player.ConnId, @case.SimulateOpening());
        }
        
        return results;
    }

    public override async Task OnDisconnectedAsync(Exception? exception) {
        await base.OnDisconnectedAsync(exception);

        logger.LogInformation("Client disconnected: {ConnectionId}", Context.ConnectionId);

        var player = await playerRepository.GetPlayer(Context.ConnectionId);
        if (player == null) return;
        await playerRepository.RemovePlayer(Context.ConnectionId);
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, player.GameId);
        await Clients.Group(player.GameId).PlayerLeft(Context.ConnectionId);
    }
}