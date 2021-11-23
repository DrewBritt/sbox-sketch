using Sandbox.UI;
using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sketch
{
	public partial class HUD : HudEntity<RootPanel>
	{
		public HUD()
		{
			if ( IsClient )
			{
				RootPanel.SetTemplate( "/UI/HUD.html" );
			}
		}
	}
}
