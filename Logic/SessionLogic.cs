﻿using HamstarHelpers.DebugHelpers;
using HamstarHelpers.MiscHelpers;
using HamstarHelpers.PlayerHelpers;
using HamstarHelpers.TmlHelpers;
using ResetMode.Data;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;


namespace ResetMode.Logic {
	partial class SessionLogic {
		public static string DataFileNameOnly { get { return "Session"; } }
		public static string RelativePath { get { return "Reset Mode Sessions"; } }



		////////////////

		public ResetModeSessionData Data { get; private set; }

		public bool IsExiting = false;
		public bool IsWorldInPlay = false;



		////////////////

		internal SessionLogic() {
			this.Data = new ResetModeSessionData();
			
			Main.OnTick += SessionLogic._Update;
		}

		~SessionLogic() {
			Main.OnTick -= SessionLogic._Update;
		}


		////////////////

		public void Load( ResetModeMod mymod ) {
			bool success;

			var data = DataFileHelpers.LoadJson<ResetModeSessionData>( mymod, SessionLogic.DataFileNameOnly, out success );
			if( success ) {
				// Very specific failsafe:
				if( data.IsRunning && !data.AwaitingNextWorld && data.CurrentSessionedWorldId == "" && data.AllPlayedWorlds.Count == 0 ) {
					data.IsRunning = false;
				}

				this.Data = data;
			}

			if( mymod.Config.DebugModeInfo ) {
				LogHelpers.Log( "ResetMode.Logic.SessionLogic.Load - Success? "+success+": "+this.Data.ToString() );
			}
		}

		public void Save( ResetModeMod mymod ) {
			DataFileHelpers.SaveAsJson<ResetModeSessionData>( mymod, SessionLogic.DataFileNameOnly, this.Data );
		}
		
		////////////////

		internal void SetData( ResetModeMod mymod, ResetModeSessionData data ) {
			this.Data = data;
		}


		////////////////

		private static void _Update() {
			var mymod = ResetModeMod.Instance;
			if( mymod == null ) { return; }
			
			if( LoadHelpers.IsWorldSafelyBeingPlayed() && mymod.Session.IsWorldInPlay ) {
				mymod.Session.UpdateSession();
			}
		}


		////////////////

		public void LogRewardsPPSpending( Player player, float pp ) {
			bool success;
			string pid = PlayerIdentityHelpers.GetUniqueId( player, out success );
			if( !success ) {
				LogHelpers.Log( "ResetMode.Logic.SessionLogic.LogRewardsPPSpending - Invalid player UID for " + player.name );
				return;
			}

			if( this.Data.PlayerPPSpendings.ContainsKey( pid ) ) {
				this.Data.PlayerPPSpendings[pid] += pp;
			} else {
				this.Data.PlayerPPSpendings[pid] = pp;
			}
		}

		
		public void RunModCalls( ResetModeMod mymod ) {
			Mod mod;

			foreach( KeyValuePair<string, string[]> kv in mymod.Config.OnWorldEngagedCalls ) {
				string mod_name = kv.Key;
				if( string.IsNullOrEmpty( mod_name ) ) {
					LogHelpers.Log( "ResetMode.Logic.SessionLogic.RunModCalls - Invalid mod name for API call " );
					continue;
				}

				try {
					mod = ModLoader.GetMod( mod_name );
					if( mod == null ) { throw new Exception(); }
				} catch {
					LogHelpers.Log( "ResetMode.Logic.SessionLogic.RunModCalls - Missing or invalid mod \"" + mod_name + "\" for API call" );
					continue;
				}

				int len = kv.Value.Length;
				object[] dest = new object[len];

				Array.Copy( kv.Value, dest, len );

				try {
					mod.Call( dest );
					LogHelpers.Log( "ResetMode.Logic.SessionLogic.RunModCalls - Calling " + kv.Key + " command \"" + string.Join( " ", dest ) + '"' );
				} catch( Exception e ) {
					LogHelpers.Log( "ResetMode.Logic.SessionLogic.RunModCalls - World load " + kv.Key + " command error - " + e.ToString() );
				}
			}
		}
	}
}
