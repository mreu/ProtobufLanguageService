// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OptionPage1Control.cs" company="Michael Reukauff, Germany">
//   Copyright © 2016 Michael Reukauff, Germany. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

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
        /// Initializes a new instance of the <see cref="OptionPage1Control"/> class.
        /// </summary>
        public OptionPage1Control()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Gets or sets the background option page.
        /// </summary>
        public OptionPage1 OptionPage { get; set; }

        /// <summary>
        /// Initialize the controls.
        /// </summary>
        public void Initialize()
        {
            chkAutoConvert.Checked = OptionPage.AutoConvert;
        }

        /// <summary>
        /// When clicked on the text, toggle the checkbox.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The MouseEventArgs.</param>
        private void lblAutoConvert_MouseClick(object sender, MouseEventArgs e)
        {
            OptionPage.AutoConvert = !chkAutoConvert.Checked;
            chkAutoConvert.Checked = OptionPage.AutoConvert;
        }

        /// <summary>
        /// Save the new checked state.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The EventArgs.</param>
        private void chkAutoConvert_CheckedChanged(object sender, EventArgs e)
        {
            OptionPage.AutoConvert = chkAutoConvert.Checked;
        }
    }
}
