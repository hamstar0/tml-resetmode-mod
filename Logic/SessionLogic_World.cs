using HamstarHelpers.DebugHelpers;
using HamstarHelpers.TmlHelpers;
using HamstarHelpers.WorldHelpers;
using ResetMode.Data;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.IO;
using Terraria.ModLoader;
using TimeLimit;


namespace ResetMode.Logic {
	partial class SessionLogic {
		public static void ClearAllWorlds() {
			TmlLoadHelpers.AddPostModLoadPromise( () => {
				ResetModeMod mymod = ResetModeMod.Instance;

				try {
					Main.LoadWorlds();

					while( Main.WorldList.Count > 0 ) {
						WorldFileData world_data = Main.WorldList[0];
						WorldFileHelpers.EraseWorld( world_data, false );
					}

					mymod.Session.Data.AwaitingNextWorld = true;

					mymod.Session.Save( mymod );
				} catch( Exception e ) {
					LogHelpers.Log( e.ToString() );
				}
			} );
		}


		public static bool IsWorldUidInSession( ResetModeMod mymod, string world_uid ) {
			return mymod.Session.Data.AllPlayedWorlds.Contains( world_uid );
		}


		////////////////

		public void AddWorldToSession( ResetModeMod mymod ) {
			string world_id = WorldHelpers.GetUniqueIdWithSeed();
			var myworld = mymod.GetModWorld<ResetModeWorld>();

			if( mymod.Config.DebugModeInfo ) {
				LogHelpers.Log( "ResetMode - SessionLogic.EngageForCurrentSession " + world_id );
			}

			if( !this.Data.AwaitingNextWorld ) {
				TimeLimitAPI.TimerStart( "reset", mymod.Config.SecondsUntilResetInitially, false );
			} else {
				TimeLimitAPI.TimerStart( "reset", mymod.Config.SecondsUntilResetSubsequently, false );
			}

			this.Data.AllPlayedWorlds.Add( world_id );
			this.Data.AwaitingNextWorld = false;
			this.Data.IsRunning = true;

			this.Save( mymod );

			myworld.Data.WorldStatus = ResetModeStatus.Active;

			this.RunWorldCalls( mymod );
		}


		public void ExpireWorldInSession( ResetModeMod mymod ) {
			var myworld = mymod.GetModWorld<ResetModeWorld>();

			if( mymod.Config.DebugModeInfo ) {
				LogHelpers.Log( "ResetMode - SessionLogic.ExpireForCurrentSession" );
			}

			mymod.Session.Data.AwaitingNextWorld = true;

			mymod.Session.Save( mymod );

			myworld.Data.WorldStatus = ResetModeStatus.Expired;

			this.GoodExit( mymod );
		}


		public void ResetWorldForSession( ResetModeMod mymod ) {
			var myworld = mymod.GetModWorld<ResetModeWorld>();

			myworld.Data.WorldPlayers.Clear();
			myworld.Data.WorldStatus = ResetModeStatus.Normal;

			TimeLimitAPI.TimerStop( "reset" );
		}


		////////////////

		public void RunWorldCalls( ResetModeMod mymod ) {
			Mod mod;

			foreach( KeyValuePair<string, string[]> kv in mymod.Config.OnWorldEngagedCalls ) {
				string mod_name = kv.Key;
				if( string.IsNullOrEmpty( mod_name ) ) {
					LogHelpers.Log( "Reset Mode - Invalid mod name for API call " );
					continue;
				}

				try {
					mod = ModLoader.GetMod( mod_name );
					if( mod == null ) { throw new Exception(); }
				} catch {
					LogHelpers.Log( "Reset Mode - Missing or invalid mod \"" + mod_name + "\" for API call" );
					continue;
				}

				int len = kv.Value.Length;
				object[] dest = new object[len];

				Array.Copy( kv.Value, dest, len );

				try {
					mod.Call( dest );
					LogHelpers.Log( "Reset Mode - Calling " + kv.Key + " command \"" + string.Join( " ", dest ) + '"' );
				} catch( Exception e ) {
					LogHelpers.Log( "Reset Mode - World load " + kv.Key + " command error - " + e.ToString() );
				}
			}
		}
	}
}
