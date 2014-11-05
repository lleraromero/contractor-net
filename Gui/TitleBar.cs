using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Contractor.Gui
{
	public enum BackColorStyle
	{
		Solid,
		Gradient
	}

	public partial class TitleBar : UserControl
	{
		private Color _DarkBackColor;
		private Color _LightBackColor;
		private BackColorStyle _BackColorStyle;
		private bool _ShowBottomBorder;

		public TitleBar()
		{
			InitializeComponent();

			this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);
			this.SetStyle(ControlStyles.SupportsTransparentBackColor, false);
		}

		[Browsable(true)]
		[Bindable(true)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		[EditorBrowsable(EditorBrowsableState.Always)]
		[Category("Action")]
		public event EventHandler Close;

		[Browsable(true)]
		[Bindable(true)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		[EditorBrowsable(EditorBrowsableState.Always)]
		public override string Text
		{
			get { return labelTitle.Text; }
			set { labelTitle.Text = value; }
		}

		[Browsable(true)]
		[Bindable(true)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		[EditorBrowsable(EditorBrowsableState.Always)]
		[Category("Appearance")]
		public Color DarkBackColor
		{
			get { return _DarkBackColor; }
			set { _DarkBackColor = value; this.Invalidate(); }
		}

		[Browsable(true)]
		[Bindable(true)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		[EditorBrowsable(EditorBrowsableState.Always)]
		[Category("Appearance")]
		public Color LightBackColor
		{
			get { return _LightBackColor; }
			set { _LightBackColor = value; this.Invalidate(); }
		}

		[Browsable(true)]
		[Bindable(true)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		[EditorBrowsable(EditorBrowsableState.Always)]
		[Category("Appearance")]
		public BackColorStyle BackColorStyle
		{
			get { return _BackColorStyle; }
			set { _BackColorStyle = value; this.Invalidate(); }
		}

		[Browsable(true)]
		[Bindable(true)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		[EditorBrowsable(EditorBrowsableState.Always)]
		[Category("Appearance")]
		public bool ShowBottomBorder
		{
			get { return _ShowBottomBorder; }
			set { _ShowBottomBorder = value; this.Invalidate(); }
		}

		[Browsable(true)]
		[Bindable(true)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		[EditorBrowsable(EditorBrowsableState.Always)]
		[Category("Behavior")]
		public bool ShowCloseButton
		{
			get { return buttonClose.Visible; }
			set { buttonClose.Visible = value; }
		}

		private void TitleBar_Paint(object sender, PaintEventArgs e)
		{
			if (this.BackColorStyle == BackColorStyle.Gradient)
			{
				using (var brush = new LinearGradientBrush(this.ClientRectangle, this.DarkBackColor, this.LightBackColor, LinearGradientMode.Vertical))
					e.Graphics.FillRectangle(brush, this.ClientRectangle);
			}

			if (this.ShowBottomBorder)
			{
				var b = this.ClientRectangle.Bottom - 1;
				var r = this.ClientRectangle.Right - 1;

				e.Graphics.DrawLine(SystemPens.WindowFrame, 0, b, r, b);
			}
		}

		private void buttonClose_Click(object sender, EventArgs e)
		{
			if (this.Close != null)
				this.Close(this, EventArgs.Empty);
		}
	}
}
