
namespace MichaelReukauff.Protobuf.Options
{
    using System;
    using System.Windows.Forms;

    /// <summary>
    /// The option page 1 control.
    /// </summary>
    public partial class OptionPage1Control : UserControl
    {
        /// <summary>
        /// Option page 1 control.
        /// </summary>
        public OptionPage1Control()
        {
            InitializeComponent();
        }

        /// <summary>
        /// The background option page.
        /// </summary>
        internal OptionPage1 optionPage;

        /// <summary>
        /// Initialize the controls.
        /// </summary>
        public void Initialize()
        {
            chkAutoConvert.Checked = optionPage.AutoConvert;
        }

        /// <summary>
        /// When clicked on the text, toggle the checkbox.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The MouseEventArgs.</param>
        private void lblAutoConvert_MouseClick(object sender, MouseEventArgs e)
        {
            optionPage.AutoConvert = !chkAutoConvert.Checked;
            chkAutoConvert.Checked = optionPage.AutoConvert;
        }

        /// <summary>
        /// Save the new checked state.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The EventArgs.</param>
        private void chkAutoConvert_CheckedChanged(object sender, EventArgs e)
        {
            optionPage.AutoConvert = chkAutoConvert.Checked;
        }
    }
}
