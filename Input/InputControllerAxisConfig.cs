using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TotBase
{
	[CreateAssetMenu(fileName = "InputControllerAxisConfig", menuName = "TotBase/InputControllerAxisConfig")]
	public class InputControllerAxisConfig : ScriptableObject
	{
		public AxisDefinition[] axis;
	}
}
