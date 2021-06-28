// originale code: https://github.com/sunilpottumuttu/FiddlerGenerateHttpClientCode
namespace HttpClientCode
{
    using Fiddler;
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Net;
    using System.Web;
    using System.Windows.Forms;

    public class BootStrap: IHandleExecAction, IFiddlerExtension
    {

        private MenuItem __menuItem;
        private MenuItem __menuItemGenerate;
        private MenuItem __menuItemNoHeader;
        private Session __selectedSession;
        private FrmGenerateHttpClientCode __frm;
        private string MESSAGEBOXTEXT = "HttpClientCode";

        public BootStrap()
		{
		}


        public void OnLoad()
        {
            this.__menuItem = new MenuItem("Http Client Code");
            this.__menuItemGenerate = new MenuItem("Generate code");
            this.__menuItemNoHeader = new MenuItem("Add headers");
            this.__menuItemNoHeader.Checked = false;
            this.__menuItem.MenuItems.Add(this.__menuItemGenerate);
            this.__menuItem.MenuItems.Add(this.__menuItemNoHeader);
            FiddlerApplication.UI.lvSessions.ContextMenu.MenuItems.Add(this.__menuItem);
            this.__menuItemGenerate.Click += new EventHandler(__menuItemGenerate_Click);
            this.__menuItemNoHeader.Click += new EventHandler(__menuItemNoHeader_Click);
        }

        string CanHandle()
        {
            if (this.__selectedSession.isTunnel)
            {
                return "This Not a HTTP/HTTPS Request..Please Choose HTTP/HTTPS Request";
            }
            if (this.__selectedSession.isFTP)
            {
                return "This is a FTP Request...Please Choose HTTP/HTTPS Request";
            }
            return string.Empty;
        }

        void __menuItemNoHeader_Click(object sender, EventArgs e)
        {
            ((MenuItem)sender).Checked = !((MenuItem)sender).Checked;
        }

        void __menuItemGenerate_Click(object sender, EventArgs e)
        {
            if (FiddlerApplication.UI.lvSessions.SelectedItems.Count == 1)
            {
                this.__selectedSession = FiddlerApplication.UI.GetFirstSelectedSession();

                var result = this.CanHandle();

                if (result == string.Empty)//Check whether this can handle or not
                {
                    string text = this.GenerateHttpClientCode();

                    this.__frm = new FrmGenerateHttpClientCode();
                    this.__frm.SetText(text);
                    this.__frm.ShowDialog();
                }
                else
                {
                    MessageBox.Show(result, this.MESSAGEBOXTEXT, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            else
            {
                MessageBox.Show("Select Only One Session", this.MESSAGEBOXTEXT, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return ;
            }
        }


        public string GenerateHttpClientCode()
        {
            var template = new GenerateCode();
            template.Session = new Dictionary<string, object>();
            
            
            template.Session.Add("uri", (this.__selectedSession.isHTTPS ? "https" : "http") + "://" + this.__selectedSession.host + this.__selectedSession.PathAndQuery);
            //template.Session.Add("host", this.__selectedSession.host);
            template.Session.Add("httpmethod", this.__selectedSession.RequestMethod);
            
            


            #region Add HttpHeaders

            var headers = new Dictionary<string, string>();

            if (this.__menuItemNoHeader.Checked)
            {
                foreach (var item in this.__selectedSession.oRequest.headers)
                {
                    headers.Add(item.Name, item.Value);
                }
            }
            #endregion
                
            template.Session.Add("headers", headers);

            
            string queryString = null;
            //string path = this.__selectedSession.PathAndQuery;
            if (this.__selectedSession.RequestMethod.ToUpperInvariant() == WebRequestMethods.Http.Post)
            {
                queryString = this.__selectedSession.GetRequestBodyAsString();
            }

            //template.Session.Add("uri", path);

            var bodies = new Dictionary<string, string>();

            #region Add bodies
            if (!string.IsNullOrWhiteSpace(queryString))
            {
                NameValueCollection x = HttpUtility.ParseQueryString(queryString);
                
                try
                {

                    foreach (var item in x.AllKeys)
                    {
                        if (string.IsNullOrWhiteSpace(x[item]))
                        {
                            continue;
                        }

                        bodies.Add(item, x[item]);
                    }
                    


                }
                catch
                {
                    bodies.Clear();
                }
            }

            #endregion

            template.Session.Add("bodies", bodies);


            template.Initialize();
            var generatedCode = template.TransformText();
            return generatedCode;
        }


        public bool OnExecAction(string sCommand)
        {
            return true;
        }

		public void OnBeforeUnload()
		{
            if (this.__frm != null)
            { this.__frm.Dispose(); }
		}
     
    }
}
