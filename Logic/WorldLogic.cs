using HamstarHelpers.DebugHelpers;
using ResetMode.NetProtocol;
using System;
using System.IO;
using Terraria.ModLoader.IO;
using TimeLimit;


namespace ResetMode.Logic {
	public enum ResetModeStatus {
		Normal,
		Active,
		Expired
	}



	class WorldLogic {
		internal ResetModeStatus WorldStatus = ResetModeStatus.Normal;


		////////////////
		
		internal void Load( ResetModeMod mymod, TagCompound tags ) {
			if( tags.ContainsKey("status") ) {
				this.WorldStatus = (ResetModeStatus)tags.GetInt( "status" );
			}

			if( mymod.Config.AwaitingNextWorld ) {
				this.Start( mymod );
			}
		}

		internal TagCompound Save() {
			return new TagCompound { { "status", (int)this.WorldStatus } };
		}

		////////////////

		public void NetReceive( BinaryReader reader ) {
			try {
				this.WorldStatus = (ResetModeStatus)reader.ReadInt32();
			} catch( Exception e ) { LogHelpers.Log( e.Message ); }
		}

		public void NetSend( BinaryWriter writer ) {
			try {
				writer.Write( (int)this.WorldStatus );
			} catch( Exception e ) { LogHelpers.Log( e.Message ); }
		}



		////////////////

		public void Start( ResetModeMod mymod ) {
			int time = mymod.Config.AwaitingNextWorld ? mymod.Config.SecondsUntilResetSubsequently : mymod.Config.SecondsUntilResetInitially;
			
			mymod.Config.AwaitingNextWorld = false;
			mymod.JsonConfig.SaveFile();

			TimeLimitAPI.TimerStart( "reset", time, false );

			this.WorldStatus = ResetModeStatus.Active;
			ServerPackets.SendWorldStatus( mymod );
		}


		public void ResetToNextWorld( ResetModeMod mymod ) {
			this.WorldStatus = ResetModeStatus.Expired;
			ServerPackets.SendWorldStatus( mymod );

			mymod.Config.AwaitingNextWorld = true;
			mymod.JsonConfig.SaveFile();

			TimeLimitAPI.TimerStart( "exit", 5, false );
		}
	}
}
