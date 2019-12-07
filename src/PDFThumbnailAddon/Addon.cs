using SwissAcademic.Citavi.Controls.Wpf;
using SwissAcademic.Citavi.Shell;
using System;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Media.Imaging;
using System.Linq;

namespace PDFThumbnail
{
    public class Addon : CitaviAddOn<MainForm>
    {
        #region Methods

        public override void OnHostingFormLoaded(MainForm mainForm)
        {
            AddTabPageToSideBar(mainForm);

            Observe(mainForm, true);

            base.OnHostingFormLoaded(mainForm);
        }

        void Observe(MainForm mainForm, bool observe)
        {
            if (observe)
            {
                mainForm.FormClosed += MainForm_FormClosed;
                
                var viewer = mainForm.PreviewControl.GetPdfViewControl();
                viewer.DocumentChanged += Viewer_DocumentChanged;
                viewer.DocumentClosing += Viewer_DocumentClosing;
            }
            else
            {
                mainForm.FormClosed -= MainForm_FormClosed;

                var viewer = mainForm.PreviewControl.GetPdfViewControl();
                viewer.DocumentChanged -= Viewer_DocumentChanged;
                viewer.DocumentClosing -= Viewer_DocumentClosing;
            }
        }

        void AddTabPageToSideBar(MainForm mainForm)
        {
            if (mainForm.PreviewControl.GetPdfViewControl().GetSideBar() is System.Windows.Controls.TabControl tabControl)
            {
             
                var bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.UriSource = new Uri("/PDFThumbnailAddon;component/Resources/thumbs.png", UriKind.RelativeOrAbsolute);
                bitmapImage.EndInit();

                var thumbnailControl = new ThumbnailsControl(mainForm.GetViewer());

                var tabPage = new TabItem
                {
                    Tag="Addon",
                    Header = new Image { Height = 16, Source = bitmapImage },
                    Content = thumbnailControl
                };

                tabControl.Items.Add(tabPage);

                thumbnailControl.Refresh();
            }
        }

        #endregion

        #region EventHandlers

        private void Viewer_DocumentChanged(object sender, EventArgs e)
        {
            if (sender is PdfViewControl viewer && viewer.GetSideBar() is System.Windows.Controls.TabControl tabControl)
            {
                var tabitem = tabControl.Items.Cast<TabItem>().FirstOrDefault(item => item.Tag != null && item.Tag.ToString().Equals("Addon", StringComparison.OrdinalIgnoreCase));
                if (tabitem != null && tabitem.Content is ThumbnailsControl thumbnailsControl)
                    thumbnailsControl.Refresh();
            }

            
        }

        private void Viewer_DocumentClosing(object sender, DocumentClosingArgs args)
        {
            if (sender is PdfViewControl viewer && viewer.GetSideBar() is System.Windows.Controls.TabControl tabControl)
            {
                var tabitem = tabControl.Items.Cast<TabItem>().FirstOrDefault(item => item.Tag != null && item.Tag.ToString().Equals("Addon", StringComparison.OrdinalIgnoreCase));
                if (tabitem != null && tabitem.Content is ThumbnailsControl thumbnailsControl)
                    thumbnailsControl.Clear();
            }
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (sender is MainForm mainForm)
            {
                Observe(mainForm, false);
            }
        }

        #endregion
    }
}