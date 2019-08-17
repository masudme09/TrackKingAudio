using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;
using System.Xml.Linq;

namespace TrackKingAudio
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static string audiolocation = "";
        private bool loaded = false;//this variable is to check trackxml is loaded once or not
        public List<Track> Tracks = new List<Track>();//it will have all the list of tracks on xml and newly added

        public MainWindow()
        {

            InitializeComponent();          
            //Loading tracks from xml
            if (loaded == false)
            {
                //getting installation directory to save xml
                string installationPath = AppDomain.CurrentDomain.BaseDirectory;

                readTrackInfoFromXml(installationPath + "\\TrackXml\\tracInfo.xml");
            }

        }

        private void TrackButton_Click(object sender, RoutedEventArgs e)
        {
           
            SolidColorBrush btnPressedBrush = new SolidColorBrush(Color.FromArgb(221, 221, 221, 100));
            btnTrack.Foreground = btnPressedBrush;
            lblMiddle.Content = "Track Admin";
            lblTypeTitle.Content = "Tracks";
            viewTrackListToWindowFromTracks(0, 1);
        }

        private void TxtCatagory_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        /// <summary>
        /// This is the event handler for click event of track adding
        /// Track info will be added on a xml file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnAddUpdate_Click(object sender, RoutedEventArgs e)
        {
            string trackName = txtName.Text;
            string category = txtCatagory.Text;
            string product = txtProduct.Text;
            string brand = txtBrand.Text;
            //string audiolocation = audioBrowseDialog();
            if(audiolocation=="")
            {
                System.Windows.Forms.MessageBox.Show("Select audio file");
            }else
            {
                createAddUpdateXml(trackName, category, product, brand, audiolocation);
            }
            
        }

        private void BtnBrowseTrack_Click(object sender, RoutedEventArgs e)
        {
            audiolocation = audioBrowseDialog();
        }
        /// <summary>
        /// This method will create xmldocument and add or update track data
        /// </summary>
        /// <param name="trackName"></param>
        /// <param name="category"></param>
        /// <param name="product"></param>
        /// <param name="brand"></param>
        /// <param name="audioLoc"></param>
        
        public void createAddUpdateXml(string trackName, string category, string product, string brand, string audioLoc)
        {
            //getting installation directory to save xml
            string installationPath = AppDomain.CurrentDomain.BaseDirectory;
            if(!Directory.Exists(installationPath + "\\TrackXml"))
            {
                Directory.CreateDirectory(installationPath + "\\TrackXml");
            }
           
            //Create xml document for writing
            if(!File.Exists(installationPath + "\\TrackXml\\tracInfo.xml"))
            {
                XDocument doc = new XDocument(
                new XDeclaration("1.0", "UTF-8", string.Empty),
                new XElement("TrackInfo",
                    new XElement("Track",
                        new XAttribute("id", 1),
                        new XAttribute("Name", trackName),
                        new XElement("Category", category),
                        new XElement("Product", product),
                        new XElement("Brand", brand),
                        new XElement("AudioLocation", audioLoc)
                        )
                 
                        ));


                doc.Save(installationPath + "\\TrackXml\\tracInfo.xml");
            }
            else
            {
                XmlDocument doc = new XmlDocument();

                doc.Load(installationPath + "\\TrackXml\\tracInfo.xml");

                XmlNode rootNode = doc.FirstChild.NextSibling;
                //Total track saved
                int trackCount = rootNode.ChildNodes.Count;

                
                string PathString;
                try
                {
                    PathString = "//TrackInfo/Track[@Name='" + trackName + "']";
                    XmlNode ExistingNode = doc.DocumentElement.SelectSingleNode(PathString);
                    
                    if (ExistingNode != null)//Checking existing nodes are there or not
                    {
                        foreach (XmlNode nd in ExistingNode.ChildNodes)
                        {
                            if (nd.Name == "Category")
                            {
                                nd.InnerText = category;
                            }
                            else if (nd.Name == "Product")
                            {
                                nd.InnerText = product;
                            }
                            else if (nd.Name == "Brand")
                            {
                                nd.InnerText = brand;
                            }
                            else if (nd.Name == "AudioLocation")
                            {
                                nd.InnerText = audioLoc;
                            }
                        }
                    }
                    else //If no existing node, then need to create one
                    {
                        XmlElement track = doc.CreateElement("Track");
                        track.SetAttribute("id", (trackCount + 1).ToString());
                        track.SetAttribute("Name", trackName);
                      
                        XmlElement cata = doc.CreateElement("Category");
                        cata.InnerText = category;
                        track.AppendChild(cata);

                        XmlElement prod = doc.CreateElement("Product");
                        prod.InnerText = product;
                        track.AppendChild(prod);

                        XmlElement brnd = doc.CreateElement("Brand");
                        brnd.InnerText = brand;
                        track.AppendChild(brnd);

                        XmlElement audLoc = doc.CreateElement("AudioLocation");
                        audLoc.InnerText = audioLoc;
                        track.AppendChild(audLoc);

                        rootNode.AppendChild(track);
                    }
                }
                catch (Exception e)
                {
                    PathString = "";
                }
                doc.Save(installationPath + "\\TrackXml\\tracInfo.xml");
                System.Windows.Forms.MessageBox.Show("Added/Updated");
            }


        }

        public string audioBrowseDialog()
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog
            {
                InitialDirectory = @"D:\",
                Title = "Browse Audio(.wav) Files",

                CheckFileExists = true,
                CheckPathExists = true,

                DefaultExt = "wav",
                Filter = "Audio files (*.wav)|*.wav",
                FilterIndex = 2,
                RestoreDirectory = true,

                ReadOnlyChecked = true,
                ShowReadOnly = true
            };

            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                return openFileDialog1.FileName;
            }
            else
                return "";
        }

        /// <summary>
        /// This method will read xml and generate list of track with all the attributes
        /// </summary>
        /// <param name="xmlPath"></param>
        public void readTrackInfoFromXml(string xmlPath)
        {
            XmlDocument doc = new XmlDocument();
            //checking file exists or not
            if (File.Exists(xmlPath))
            {
                doc.Load(xmlPath);
                loaded = true;//Setting this value to ensure that tracks are loaded from xml once

                XmlNode rootNode = doc.FirstChild.NextSibling;//Trackinfo node

                XmlNodeList xmlTrackNodes = doc.GetElementsByTagName("Track");
                
                foreach(XmlNode nd in xmlTrackNodes)
                {
                    XmlAttributeCollection trackAtbs = nd.Attributes;
                    Track track = new Track();

                    foreach(XmlAttribute atb in trackAtbs) //Getting tracks id and Name
                    {
                        if(atb.Name=="id")
                        {
                            track.id= Convert.ToInt32(atb.Value);
                        }
                        else if(atb.Name=="Name")
                        {
                            track.Name= atb.Value;
                        }
                    }

                    XmlNodeList trackChildNodes = nd.ChildNodes;

                    foreach(XmlNode trChNd in trackChildNodes)//Getting Category, Product, Brand & AudioLocation
                    {
                        if(trChNd.Name== "Category")
                        {
                            track.Category = trChNd.InnerText;
                        }
                        else if(trChNd.Name == "Product")
                        {
                            track.Product = trChNd.InnerText;
                        }
                        else if (trChNd.Name == "Brand")
                        {
                            track.Brand= trChNd.InnerText;
                        }
                        else if (trChNd.Name == "AudioLocation")
                        {
                            track.AudioLoc= trChNd.InnerText;
                        }
                    }

                    //Adding to traclist
                    Tracks.Add(track);
                }



            }
            
        }

        /// <summary>
        /// This method will insert tracks to the UI form TrackList=Tracks
        /// </summary>
        /// <param name="startIndex"></param>
        /// <param name="endIndex"></param>
        public void viewTrackListToWindowFromTracks(int startIndex, int endIndex)
        {
            //if startindex is smaller than track count then clear all track level first
            if (startIndex < Tracks.Count)
            {

                UIElementCollection trackListVisualUI = WrapItems.Children;
                foreach (System.Windows.Controls.Label lbl in trackListVisualUI)
                {
                    lbl.Content = "";
                }


                int count = 1;
                for (int i = startIndex; i <= endIndex; i++)
                {
                    if (i < Tracks.Count)
                    {
                        //Finding by Name
                        System.Windows.Controls.Label lbl = this.FindName("lblItem_"+count) as System.Windows.Controls.Label;
                        lbl.Content = Tracks[i].Name;
                        lbl.Visibility = Visibility.Visible;
                        count++;
                    }
                    
                }
            }
        }
    }
}
