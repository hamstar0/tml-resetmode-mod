using HamstarHelpers.Helpers.DebugHelpers;
using HamstarHelpers.Helpers.MiscHelpers;
using HamstarHelpers.Helpers.PlayerHelpers;
using HamstarHelpers.Helpers.TmlHelpers;
using HamstarHelpers.Services.Timers;
using ResetMode.Data;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;


namespace ResetMode.Logic {
	partial class SessionLogic {
		public static string DataFileNameOnly => "Session";
		public static string RelativePath => "Reset Mode Sessions";



		////////////////

		public ResetModeSessionData Data { get; private set; }
		internal ResetModeSessionData DataOnLoad;

		public bool IsExiting = false;
		public bool IsWorldInPlay = false;

		private Func<bool> OnTickGet;



		////////////////

		internal SessionLogic() {
			this.Data = new ResetModeSessionData();

			this.OnTickGet = Timers.MainOnTickGet();
			Main.OnTick += SessionLogic._Update;
		}

		~SessionLogic() {
			Main.OnTick -= SessionLogic._Update;
		}


		////////////////

		public void Load() {
			var mymod = ResetModeMod.Instance;
			if( Main.netMode == 1 ) {
				LogHelpers.Warn( "Clients cannot load config from file" );
				return;
			}

			bool success;

			var data = DataFileHelpers.LoadJson<ResetModeSessionData>( mymod, SessionLogic.DataFileNameOnly, out success );
			if( success ) {
				// Very specific failsafe:
				if( data.IsRunning && !data.AwaitingNextWorld && data.CurrentSessionedWorldId == "" && data.AllPlayedWorlds.Count == 0 ) {
					data.IsRunning = false;
				}

				this.DataOnLoad = data.Clone();
				this.Data = data;
			}

			if( mymod.Config.DebugModeInfo ) {
				LogHelpers.Alert( "Success? "+success+": "+this.Data.ToString() );
			}
		}

		public void Save() {
			var mymod = ResetModeMod.Instance;
			if( Main.netMode == 1 ) {
				LogHelpers.Warn( "Clients cannot save config to file" );
				return;
			}
			DataFileHelpers.SaveAsJson<ResetModeSessionData>( mymod, SessionLogic.DataFileNameOnly, this.Data );
		}
		
		////////////////

		internal void SetData( ResetModeSessionData data ) {
			this.Data = data;
		}


		////////////////

		private static void _Update() {
			var mymod = ResetModeMod.Instance;
			if( mymod == null ) { return; }

			if( mymod.Session.OnTickGet() ) {
				if( LoadHelpers.IsWorldSafelyBeingPlayed() && mymod.Session.IsWorldInPlay ) {
					mymod.Session.Update();
				}
			}
		}


		////////////////

		public void LogRewardsPPSpending( Player player, float pp ) {
			string pid = PlayerIdentityHelpers.GetProperUniqueId( player );

			if( this.Data.PlayerPPSpendings.ContainsKey( pid ) ) {
				this.Data.PlayerPPSpendings[pid] += pp;
			} else {
				this.Data.PlayerPPSpendings[pid] = pp;
			}
		}

		
		public void RunModCalls() {
			var mymod = ResetModeMod.Instance;
			Mod mod;

			foreach( KeyValuePair<string, string[]> kv in mymod.Config.OnWorldEngagedCalls ) {
				string modName = kv.Key;
				if( string.IsNullOrEmpty( modName ) ) {
					LogHelpers.Warn( "Invalid mod name for API call " );
					continue;
				}

				try {
					mod = ModLoader.GetMod( modName );
					if( mod == null ) { throw new Exception(); }
				} catch {
					LogHelpers.Warn( "Missing or invalid mod \"" + modName + "\" for API call" );
					continue;
				}

				int len = kv.Value.Length;
				object[] dest = new object[len];

				Array.Copy( kv.Value, dest, len );

				try {
					mod.Call( dest );
					LogHelpers.Alert( "Calling " + kv.Key + " command \"" + string.Join( " ", dest ) + '"' );
				} catch( Exception e ) {
					LogHelpers.Warn( "World load " + kv.Key + " command error - " + e.ToString() );
				}
			}
		}
	}
}
