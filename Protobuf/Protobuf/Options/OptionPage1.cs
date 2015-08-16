
namespace MichaelReukauff.Protobuf.Options
{
    using System.ComponentModel;
    using System.Windows.Forms;

    using Microsoft.VisualStudio.Shell;

    [Category("Protobuf")]
    [DisplayName("Protobuf")]
    [Description("Protobuf Settings")]
    class OptionPage1 : DialogPage
    {
        public bool AutoConvert { get; set; }

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
                var page = new OptionPage1Control { optionPage = this };
                page.Initialize();
                return page;
            }
        }
        #endregion Overrides of DialogPage
    }
}
