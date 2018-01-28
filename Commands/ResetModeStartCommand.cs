using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;


namespace ResetMode.Commands {
	class ResetModeStartCommand : ModCommand {
		public override CommandType Type {
			get {
				if( Main.netMode == 0 ) {
					return CommandType.World;
				}
				return CommandType.Console;
			}
		}
		public override string Command { get { return "resetmodestart"; } }
		public override string Usage { get { return "/resetmodestart"; } }
		public override string Description { get { return "Begins reset mode."; } }
		

		////////////////

		public override void Action( CommandCaller caller, string input, string[] args ) {
			var mymod = (ResetModeMod)this.mod;
			var myworld = mymod.GetModWorld<ResetModeWorld>();

			if( mymod.Logic.IsSessionStarted(mymod) ) {
				caller.Reply( "Reset mode is already in session.", Color.Red );
				return;
			}

			mymod.Logic.StartSession( mymod );
			myworld.Logic.EngageWorldForCurrentSession( mymod );

			caller.Reply( "Reset mode begun! This will continue until for each new world /resetmodeend is called.", Color.YellowGreen );
		}
	}
}
