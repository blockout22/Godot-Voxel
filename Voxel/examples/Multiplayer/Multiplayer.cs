using Godot;
using System;

public partial class Multiplayer : Node
{

    [Export]
    PackedScene Player;


    public override void _Ready(){
        CreateServer(1234);
    }

    private void CreateServer(int port)
    {
        ENetMultiplayerPeer peer = new ENetMultiplayerPeer();
        Error results = peer.CreateServer(port);
        GD.Print("Server Status: " + results.ToString());
        if(results != Error.Ok){
            JoinServer();
            return;
        }
        Multiplayer.MultiplayerPeer = peer;
        Multiplayer.PeerConnected += playerConnected;
        Multiplayer.PeerDisconnected += playerDisconnected;

        AddPlayer(Multiplayer.GetUniqueId());
    }

    private void playerDisconnected(long id)
    {
        GD.Print("A Player has disconnected ID [ " + id + "]");
    }


    private void playerConnected(long id)
    {
        GD.Print("A Player has connected ID [ " + id + "]");
        AddPlayer(id);
    }

    public void JoinServer(){
        ENetMultiplayerPeer peer = new ENetMultiplayerPeer();
        Error results = peer.CreateClient("localhost", 1234);
        GD.Print("Client Status: " + results.ToString());
        Multiplayer.MultiplayerPeer = peer;
    }

    public void AddPlayer(long id){
        var player = Player.Instantiate();
        player.Name = "" + id;
        AddChild(player);
    }

}
