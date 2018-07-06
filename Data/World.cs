using HamstarHelpers.Components.Errors;
using HamstarHelpers.DebugHelpers;
using HamstarHelpers.PlayerHelpers;
using HamstarHelpers.WorldHelpers;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader.IO;


namespace ResetMode.Data {
	public enum ResetModeWorldStatus {
		Normal,
		Active,
		Expired
	}



	public class ResetModeWorldData {
		public ResetModeWorldStatus WorldStatus = ResetModeWorldStatus.Normal;
		public ISet<string> WorldPlayers = new HashSet<string>();
		public bool IsExiting = false;


		////////////////
		
		internal void Load( ResetModeMod mymod, TagCompound tags ) {
			if( tags.ContainsKey( "status" ) ) {
				this.WorldStatus = (ResetModeWorldStatus)tags.GetInt( "status" );
			}

			if( tags.ContainsKey( "player_count" ) ) {
				int count = tags.GetInt( "player_count" );
				for( int i=0; i<count; i++ ) {
					this.WorldPlayers.Add( tags.GetString( "player_" + i ) );
				}
			}

			if( mymod.Config.DebugModeInfo ) {
				LogHelpers.Log( "ResetMode - ResetModeWorldData.Load uid: " + WorldHelpers.GetUniqueIdWithSeed() + ", this.WorldStatus: " + this.WorldStatus );
			}
		}


		internal TagCompound Save( ResetModeMod mymod ) {
			if( mymod.Config.DebugModeInfo ) {
				LogHelpers.Log( "ResetMode - ResetModeWorldData.Save uid: " + WorldHelpers.GetUniqueIdWithSeed() + ", this.WorldStatus: " + this.WorldStatus );
			}
			
			var tags = new TagCompound {
				{ "status", (int)this.WorldStatus },
				{ "player_count", this.WorldPlayers.Count }
			};

			int i = 0;
			foreach( string uid in this.WorldPlayers ) {
				tags.Set( "player_" + i, uid );
				i++;
			}

			return tags;
		}


		////////////////

		internal bool IsPlaying( ResetModeMod mymod, Player player ) {
			bool has_uid;
			string uid = PlayerIdentityHelpers.GetUniqueId( player, out has_uid );
			if( !has_uid ) {
				throw new HamstarException( "ResetMode - ResetModeWorldData.IsPlaying - Player has no uid." );
			}

			return this.WorldPlayers.Contains( uid );
		}


		internal void AddPlayer( ResetModeMod mymod, Player player ) {
			bool has_uid;
			string uid = PlayerIdentityHelpers.GetUniqueId( player, out has_uid );
			if( !has_uid ) {
				throw new HamstarException( "ResetMode - ResetModeWorldData.AddPlayer - Player has no uid." );
			}

			this.WorldPlayers.Add( uid );
		}
	}
}
