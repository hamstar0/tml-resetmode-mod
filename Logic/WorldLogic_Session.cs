using HamstarHelpers.DebugHelpers;
using HamstarHelpers.MiscHelpers;
using HamstarHelpers.WorldHelpers;
using ResetMode.Data;
using System;
using System.Collections.Generic;
using Terraria.ModLoader;
using TimeLimit;


namespace ResetMode.Logic {
	partial class WorldLogic {
		public static bool IsWorldUidInSession( ResetModeMod mymod, string world_uid ) {
			return mymod.Session.AllPlayedWorlds.Contains( world_uid );
		}


		////////////////

		public void EngageWorldForCurrentSession( ResetModeMod mymod ) {
			string world_id = WorldHelpers.GetUniqueId();   //Main.ActiveWorldFileData.UniqueId.ToString();

			if( mymod.Config.DebugModeInfo ) {
				LogHelpers.Log( "WorldLogic.EngageWorldForCurrentSession " + world_id );
			}

			if( !mymod.Session.AwaitingNextWorld ) {
				TimeLimitAPI.TimerStart( "reset", mymod.Config.SecondsUntilResetInitially, false );
			} else {
				TimeLimitAPI.TimerStart( "reset", mymod.Config.SecondsUntilResetSubsequently, false );
			}

			mymod.Session.AddActiveWorld( world_id );

			DataFileHelpers.SaveAsJson<ResetModeSessionData>( mymod, ResetModeSessionData.DataFileNameOnly, mymod.Session );

			this.WorldStatus = ResetModeStatus.Active;

			this.RunWorldCalls( mymod );

			PlayerLogic.ValidateAll( mymod );
		}


		public void EndCurrentSession( ResetModeMod mymod ) {
			string world_id = WorldHelpers.GetUniqueId();   //Main.ActiveWorldFileData.UniqueId.ToString();

			if( mymod.Config.DebugModeInfo ) {
				LogHelpers.Log( "WorldLogic.EndCurrentSession " + world_id );
			}

			mymod.Session.ClearSessionData();

			DataFileHelpers.SaveAsJson<ResetModeSessionData>( mymod, ResetModeSessionData.DataFileNameOnly, mymod.Session );

			this.WorldPlayers.Clear();
			this.WorldStatus = ResetModeStatus.Normal;

			TimeLimitAPI.TimerStop( "reset" );
		}


		////////////////

		public void ExpireWorldForCurrentSession( ResetModeMod mymod ) {
			if( mymod.Config.DebugModeInfo ) {
				LogHelpers.Log( "WorldLogic.CloseWorldForCurrentSession" );
			}

			mymod.Session.AwaitingNextWorld = true;

			DataFileHelpers.SaveAsJson<ResetModeSessionData>( mymod, ResetModeSessionData.DataFileNameOnly, mymod.Session );

			this.WorldStatus = ResetModeStatus.Expired;

			this.GoodExit( mymod );
		}


		////////////////

		public void RunWorldCalls( ResetModeMod mymod ) {
			Mod mod;

			foreach( KeyValuePair<string, string[]> kv in mymod.Config.OnWorldEngagedCalls ) {
				string mod_name = kv.Key;
				if( string.IsNullOrEmpty( mod_name ) ) {
					LogHelpers.Log( "Invalid mod name" );
					continue;
				}

				try {
					mod = ModLoader.GetMod( mod_name );
					if( mod == null ) { throw new Exception(); }
				} catch {
					LogHelpers.Log( "Missing or invalid mod \"" + mod_name + '"' );
					continue;
				}

				int len = kv.Value.Length;
				object[] dest = new object[len];

				Array.Copy( kv.Value, dest, len );

				try {
					mod.Call( dest );
					LogHelpers.Log( "Calling " + kv.Key + " command \"" + string.Join( " ", dest ) + '"' );
				} catch( Exception e ) {
					LogHelpers.Log( "World load " + kv.Key + " command error - " + e.ToString() );
				}
			}
		}
	}
}
