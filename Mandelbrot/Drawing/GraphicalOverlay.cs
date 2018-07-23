using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Drawing
{
    public partial class GraphicalOverlay : Component
    {
        public event EventHandler<PaintEventArgs> Paint;
        private Form _form;

        public GraphicalOverlay()
        {
            InitializeComponent();
        }

        public GraphicalOverlay(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Form Owner
        {
            get => _form;
            set 
            {
                // The owner form can only be set once.
                if (_form != null)
                    throw new InvalidOperationException();

                // Save the form for future reference.
                _form = value ?? throw new ArgumentNullException();

                // Handle the form's Resize event.
                _form.Resize += Form_Resize;

                // Handle the Paint event for each of the controls in the form's hierarchy.
                ConnectPaintEventHandlers(_form);
            }
        }

        private void Form_Resize(object sender, EventArgs e)
        {
            _form.Invalidate(true);
        }

        private void ConnectPaintEventHandlers(Control control)
        {
            // Connect the paint event handler for this control.
            // Remove the existing handler first (if one exists) and replace it.
            control.Paint -= Control_Paint;
            control.Paint += Control_Paint;

            control.ControlAdded -= Control_ControlAdded;
            control.ControlAdded += Control_ControlAdded;

            // Recurse the hierarchy.
            foreach (Control child in control.Controls)
                ConnectPaintEventHandlers(child);
        }

        private void Control_ControlAdded(object sender, ControlEventArgs e)
        {
            // Connect the paint event handler for the new control.
            ConnectPaintEventHandlers(e.Control);
        }

        private void Control_Paint(object sender, PaintEventArgs e)
        {
            // As each control on the form is repainted, this handler is called.

            var control = sender as Control;
            Point location;

            // Determine the location of the control's client area relative to the form's client area.
            if (control == _form)
                // The form's client area is already form-relative.
                location = control.Location;
            else
            {
                // The control may be in a hierarchy, so convert to screen coordinates and then back to form coordinates.
                location = _form.PointToClient(control.Parent.PointToScreen(control.Location));

                // If the control has a border shift the location of the control's client area.
                location += new Size((control.Width - control.ClientSize.Width) / 2, (control.Height - control.ClientSize.Height) / 2);
            }

            // Translate the location so that we can use form-relative coordinates to draw on the control.
            if (control != _form)
                e.Graphics.TranslateTransform(-location.X, -location.Y);

            // Fire a paint event.
            OnPaint(sender, e);
        }

        private void OnPaint(object sender, PaintEventArgs e)
        {
            // Fire a paint event.
            // The paint event will be handled in Form1.graphicalOverlay1_Paint().

            Paint?.Invoke(sender, e);
        }
    }
}