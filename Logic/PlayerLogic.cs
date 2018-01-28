using Microsoft.Xna.Framework;
using Rewards;
using System.IO;
using Terraria;
using Terraria.ModLoader.IO;


namespace ResetMode.Logic {
	partial class PlayerLogic {
		public string ActiveForWorldUid { get; private set; }
		public bool IsPrompting { get; private set; }
		public bool IsValidating { get; private set; }
		public bool IsValid { get; private set; }

		private bool HasShownWelcome = false;


		////////////////

		public PlayerLogic() {
			this.ActiveForWorldUid = "";
			this.IsPrompting = false;
			this.IsValidating = false;
			this.IsValid = false;
		}

		public void Load( Player player, TagCompound tags ) {
			if( tags.ContainsKey( "active_world_uid" ) ) {
				this.ActiveForWorldUid = tags.GetString( "active_world_uid" );
			}
		}

		public TagCompound Save( ResetModeMod mymod, Player player ) {
			mymod.Logic.SavePlayerSnapshot( mymod, player );

			return new TagCompound {
				{ "active_world_uid", this.ActiveForWorldUid }
			};
		}

		////////////////

		public void NetSend( ResetModeMod mymod, Player player, BinaryWriter writer ) {
			writer.Write( this.ActiveForWorldUid );
			writer.Write( RewardsAPI.GetPoints( player ) );
			mymod.Logic.NetSendPlayerData( player, writer );
		}

		public void NetReceive( ResetModeMod mymod, Player player, BinaryReader reader ) {
			this.ActiveForWorldUid = reader.ReadString();
			mymod.Logic.NetReceivePlayerData( player, reader );
		}


		////////////////
		
		public void Update( ResetModeMod mymod, Player player ) {
			var myworld = mymod.GetModWorld<ResetModeWorld>();
			
			if( !this.HasShownWelcome ) {
				this.HasShownWelcome = true;
				this.Welcome( mymod, player );
			}

			if( myworld.Logic.WorldStatus == ResetModeStatus.Active ) {
				this.ValidatePlayer( mymod, player );
			}

			if( this.IsPrompting ) {
				player.noItems = true;
				player.noBuilding = true;
				player.stoned = true;
				player.immune = true;
			}
		}


		////////////////

		public void Welcome( ResetModeMod mymod, Player player ) {
			var myworld = mymod.GetModWorld<ResetModeWorld>();

			switch( myworld.Logic.WorldStatus ) {
			case ResetModeStatus.Normal:
				if( Main.netMode == 0 ) {
					Main.NewText( "Type /resetmodestart to start Reset Mode. Type /help for a list of other available commands.", Color.LightGray );
				} else {
					Main.NewText( "Type resetmodestart in the server console to start Reset Mode.", Color.LightGray );
				}
				break;

			case ResetModeStatus.Active:
				Main.NewText( "Welcome to Reset Mode. After the timer, you and this world reset. Only your Progress Points (PP) are kept for the next.", Color.Cyan );
				break;
			}
		}
	}
}
