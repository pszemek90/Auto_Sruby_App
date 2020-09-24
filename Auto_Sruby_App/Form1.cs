using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Tekla.Structures.Geometry3d;
using Tekla.Structures.Model;
using Tekla.Structures.Model.UI;

namespace Auto_Sruby_App
{
	public partial class Form1 : Form
	{
		Model model;
		public Form1()
		{
			InitializeComponent();
			model = new Model();
		}

		private void Form1_Load(object sender, EventArgs e)
		{
		}

		private void connectParts()
		{
			List<BoltGroup> boltGroups = new List<BoltGroup>();

			Picker picker = new Picker();

			Tekla.Structures.Model.UI.ModelObjectSelector selector = new Tekla.Structures.Model.UI.ModelObjectSelector();
			ModelObjectEnumerator enumerator = selector.GetSelectedObjects();

			foreach (ModelObject modelObject in enumerator)
			{
				BoltGroup boltGroup = (BoltGroup)modelObject;
				boltGroups.Add(boltGroup);
			}

			foreach (BoltGroup group in boltGroups)
			{
				int partsBoltedBefore;
				int partsBoltedAfter;
				do
				{
					Solid solid = group.GetSolid();
					partsBoltedBefore = group.GetOtherPartsToBolt().Count;
					Double boltLength = 0.0;
					Point maximumPoint = solid.MaximumPoint;
					Point minimumPoint = solid.MinimumPoint;
					Tekla.Structures.Model.ModelObjectSelector groupSelector = model.GetModelObjectSelector();
					ModelObjectEnumerator collidingObjects = groupSelector.GetObjectsByBoundingBox(minimumPoint, maximumPoint);
					while (collidingObjects.MoveNext())
					{
						Part part = collidingObjects.Current as Part;
						if (part != null)
						{
							group.AddOtherPartToBolt(part);
						}
					}
					group.GetReportProperty("LENGTH", ref boltLength);
					if (group.BoltStandard == "4017-8.8" && boltLength % 10 != 0)
					{
						group.BoltStandard = "4014-8.8";
					}
					group.Modify();
					partsBoltedAfter = group.GetOtherPartsToBolt().Count;
					group.CutLength = boltLength + 20;
				} while (partsBoltedBefore != partsBoltedAfter);
			}
			model.CommitChanges();
		}

		private void button1_Click(object sender, EventArgs e)
		{
			connectParts();
		}
	}
}
