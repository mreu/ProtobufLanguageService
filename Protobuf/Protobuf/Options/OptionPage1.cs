// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OptionPage1.cs" company="Michael Reukauff, Germany">
//   Copyright © 2016 Michael Reukauff, Germany. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace MichaelReukauff.Protobuf.Options
{
    using System.ComponentModel;
    using System.Windows.Forms;

    using Microsoft.VisualStudio.Shell;

    /// <summary>
    /// The option page1 class.
    /// </summary>
    [Category("Protobuf")]
    [DisplayName("Protobuf")]
    [Description("Protobuf Settings")]
    public class OptionPage1 : DialogPage
    {
        /// <summary>
        /// Gets or sets a value indicating whether
        /// </summary>
        public bool AutoConvert { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="OptionPage1"/> class.
        /// </summary>
        public OptionPage1()
        {
            AutoConvert = false;
        }

        #region Overrides of DialogPage
        /// <summary>
        /// Gets the window that is used as the user interface of the dialog page.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Windows.Forms.IWin32Window"/> that provides the handle to the window that acts as the user interface for the dialog page.
        /// </returns>
        protected override IWin32Window Window
        {
            get
            {
                var page = new OptionPage1Control { OptionPage = this };
                page.Initialize();
                return page;
            }
        }
        #endregion Overrides of DialogPage
    }
}
