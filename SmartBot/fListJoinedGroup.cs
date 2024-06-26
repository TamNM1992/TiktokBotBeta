using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SmartBot
{
	public partial class fListJoinedGroup : Form
	{
		FbAction _fb { get; set; }
		public fListJoinedGroup()
		{
		}
		public fListJoinedGroup(FbAction fb)
		{
			InitializeComponent();
			checkedListBox1.DisplayMember = "Text";
			checkedListBox1.ValueMember = "Value";
			var lst = fb.ListGroup("").Result;
			foreach (var item in lst)
			{
				checkedListBox1.Items.Add(new { Text = item.Key, Value = item.Value }, false);
			}
			_fb = fb;
		}

		private void button1_Click(object sender, EventArgs e)
		{
			foreach (var itemChecked in checkedListBox1.CheckedItems)
			{
				var text = itemChecked.ToString();
				var match = Regex.Match(text, "Value = (.+)}");
				if (match.Success)
				{
					var link = match.Groups[1].Value.ToString();
					_fb.JoinedGroups.Add(link);
				}

			}

			this.Close();
		}

		private void RunHanhDong()
		{

		}

		private void fListJoinedGroup_Load(object sender, EventArgs e)
		{

		}
	}
}
