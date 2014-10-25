using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using RestSharp;
using Microsoft.CSharp;

public class test
{
	public static void Main () {
		var pl = new Playlyfe(
			client_id: "Zjc0MWU0N2MtODkzNS00ZWNmLWEwNmYtY2M1MGMxNGQ1YmQ4",
			client_secret: "YzllYTE5NDQtNDMwMC00YTdkLWFiM2MtNTg0Y2ZkOThjYTZkMGIyNWVlNDAtNGJiMC0xMWU0LWI2NGEtYjlmMmFkYTdjOTI3",
		    type: "client",
			store: null,
			load: null
		);

		var player_id = new Dictionary<string, string> (){ { "player_id", "student1" } };
		try {
			pl.get(
				route: "/unkown",
				query: player_id
			);
		}
		catch(PlaylyfeException ex) {
			Console.WriteLine (ex.Name);
			Console.WriteLine (ex.Message);
		}
		var players = pl.get(
			route: "/players",
			query: player_id
		);
		Console.WriteLine(players["data"][0]["id"]);

		var player = pl.get(
			route: "/player",
			query: player_id,
			raw: true
		);
		Console.WriteLine(player.GetType());
		Console.WriteLine(player);

		pl.get (
			route: "/definitions/processes",
			query: player_id
		);
		pl.get (
			route:  "/definitions/teams",
			query:  player_id
		);

		var processes = pl.get (
			route:  "/processes",
			query:  player_id
		);
		Console.WriteLine (processes["total"]);

		pl.get (
			route:  "/teams",  

			query:  player_id
		);

		var new_process = pl.post (
		 	route: "/definitions/processes/module1",
		    query: player_id,
	
			body: null
		);

		var patched_process = pl.patch (
		 	route: "/processes/" + new_process ["id"],
		 	query: player_id,
		 	body: new { name = "patched_process", access = "PUBLIC"}
		);
		Console.WriteLine (patched_process["id"]);

		var deleted_process = pl.delete(
		 	route: "/processes/"+new_process["id"],
		 	query: player_id
		);
		Console.WriteLine (deleted_process["message"]);
		Environment.Exit (0);
	}
}

