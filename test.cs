using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using RestSharp;
using Microsoft.CSharp;

public class test
{
	public static void Main () {
		Playlyfe.init (
			client_id: "Zjc0MWU0N2MtODkzNS00ZWNmLWEwNmYtY2M1MGMxNGQ1YmQ4",
			client_secret: "YzllYTE5NDQtNDMwMC00YTdkLWFiM2MtNTg0Y2ZkOThjYTZkMGIyNWVlNDAtNGJiMC0xMWU0LWI2NGEtYjlmMmFkYTdjOTI3",
		    type: "client",
			store: null,
			load: null
		);

		try {
			Playlyfe.get(
				route: "/unkown",
				query: new Dictionary<string, string>(){ {"player_id", "student1"}}
			);
		}
		catch(PlaylyfeException ex) {
			Console.WriteLine (ex.Name);
			Console.WriteLine (ex.Message);
		}
		var players = Playlyfe.get(
			route: "/players",
			query: new Dictionary<string, string>(){{"player_id", "student1"}}
		);
		Console.WriteLine(players["data"][0]["id"]);

		var player = Playlyfe.get(
			route: "/player",
			query: new Dictionary<string, string>(){{"player_id", "student1"}},
			raw: true
		);
		Console.WriteLine(player.GetType());
		Console.WriteLine(player);

		Playlyfe.get (
			route: "/definitions/processes", 
			query:  new Dictionary<string, string> (){ { "player_id", "student1" } }
		);
		Playlyfe.get (
			route:  "/definitions/teams", 
			query:  new Dictionary<string, string> (){ { "player_id", "student1" } }
		);

		var processes = Playlyfe.get (
			route:  "/processes",
			query:  new Dictionary<string, string> (){ { "player_id", "student1" } }
		);
		Console.WriteLine (processes["total"]);

		Playlyfe.get (
			route:  "/teams",
			query:  new Dictionary<string, string> (){ { "player_id", "student1" } }
		);

		var new_process = Playlyfe.post (
		 	route: "/definitions/processes/module1",
		    query: new Dictionary<string, string> () { {"player_id", "student1"} },
		 	body: null
		);

		var patched_process = Playlyfe.patch (
		 	route: "/processes/" + new_process ["id"],
		 	query: new Dictionary<string, string> () { {"player_id", "student1"} },
		 	body: new { name = "patched_process", access = "PUBLIC"}
		);
		Console.WriteLine (patched_process["id"]);

		var deleted_process = Playlyfe.delete(
		 	route: "/processes/"+new_process["id"],
		 	query: new Dictionary<string, string> () { {"player_id", "student1"} }
		);
		Console.WriteLine (deleted_process["message"]);
		Environment.Exit (0);
	}
}

