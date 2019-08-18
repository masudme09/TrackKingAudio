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
            middleContentGridForChannels.Visibility = Visibility.Hidden;
            middleContentGrid.Visibility = Visibility.Visible; //Middle grid is visible now
            SolidColorBrush btnPressedBrush = new SolidColorBrush(Color.FromArgb(221, 221, 221, 100));
            btnTrack.Foreground = btnPressedBrush;
            lblMiddle.Content = "Track Admin";
            lblTypeTitle.Content = "Tracks";
            viewTrackListToWindowFromTracks(0, endIndex:(Tracks.Count<=10?Tracks.Count-1:9));
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
            string trackName;
            string category;
            string product;
            string brand;

            //string audiolocation = audioBrowseDialog();
            if (audiolocation=="")
            {
                System.Windows.Forms.MessageBox.Show("Select audio file");
            }
            else
            {
                Track track = new Track();

                track.id = Tracks.Count + 1;
                track.Name= trackName = txtName.Text;
                track.Category= category = txtCatagory.Text;
                track.Product= product = txtProduct.Text;
                track.Brand= brand = txtBrand.Text;
               
                //if the current name track already exists then track will not be added to tracks
                bool trackExists = Tracks.Any(tr => tr.Name == track.Name);
                if (trackExists == false)
                {
                    Tracks.Add(track);

                    //View it on UI
                    int startIndex = 0;
                    int endIndex = 0;
                    if (Tracks.Count <= 10)
                    {
                        endIndex = Tracks.Count-1;
                    }
                    else
                    {
                        endIndex = 9;
                    }
                    viewTrackListToWindowFromTracks(startIndex, endIndex);
                }

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

                        //To update Track list node
                        Track existingTrack = Tracks.Find(x => x.Name == trackName);
                        existingTrack.Category = category;
                        existingTrack.Product = product;
                        existingTrack.Brand = brand;
                        existingTrack.AudioLoc = audioLoc;

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
                intializingItemField();
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
            if (startIndex <=Tracks.Count)
            {

                UIElementCollection trackListVisualUI = WrapItems.Children;
                foreach (System.Windows.Controls.Label lbl in trackListVisualUI)
                {
                    lbl.Content = "";
                }


                int count = 1;
                for (int i = startIndex; i <=endIndex; i++)
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
        /// <summary>
        /// This method will set the default value to track setup controls
        /// </summary>
        public void intializingItemField()
        {
            txtName.Text = "Track Name";
            txtCatagory.Text = "Category";
            txtBrand.Text = "Brand";
            txtProduct.Text = "Product";
            audiolocation = "";
        }

        /// <summary>
        /// This handler will be used for all label item controls
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LblItem_1_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                SolidColorBrush btnPressedBrush = new SolidColorBrush(Color.FromArgb(255, 0, 0, 255));

                System.Windows.Controls.Label srcLabel = e.Source as System.Windows.Controls.Label; //to know which label call this handler
                SolidColorBrush btnUpBrush = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));
                UIElementCollection trackListVisualUI = WrapItems.Children;

                foreach (System.Windows.Controls.Label lbl in trackListVisualUI)//Setting all the items to default brush
                {

                    lbl.Foreground = btnUpBrush;


                }
                srcLabel.Foreground = btnPressedBrush;

                //Getting info about source label track from xml and set that as setup content
                string srcLabelTrackName = srcLabel.Content.ToString();
                Track tr = Tracks.Find(x => x.Name.Equals(srcLabelTrackName));

                //Setting the track property to text boxes
                txtName.Text = tr.Name;
                txtBrand.Text = tr.Brand;
                txtCatagory.Text = tr.Category;
                txtProduct.Text = tr.Product;
                audiolocation = tr.AudioLoc;
            }
            catch(Exception error)
            {
                //Write error log here
            }
        }

        private void BtnRemove_Click(object sender, RoutedEventArgs e)
        {
            try
                {
                string trackName = txtName.Text;

                //Now get it form track list and xml and remove it
                Track tr = Tracks.Find(x => x.Name.Equals(trackName));
                Tracks.Remove(tr);//removing from list

                //getting installation directory to save xml
                string installationPath = AppDomain.CurrentDomain.BaseDirectory;
                if (File.Exists(installationPath + "\\TrackXml\\tracInfo.xml"))
                {
                    XmlNode rmTrack = null;
                    XmlDocument doc = new XmlDocument();
                    doc.Load(installationPath + "\\TrackXml\\tracInfo.xml");
                    XmlNode rootNode = doc.FirstChild.NextSibling;

                    XmlNodeList xmlTracks = rootNode.ChildNodes;

                    foreach (XmlNode nd in xmlTracks)
                    {
                        XmlAttributeCollection ndAtbCol = nd.Attributes;

                        foreach (XmlAttribute atb in ndAtbCol)
                        {
                            if (atb.Name == "Name" && atb.Value == trackName)
                            {
                                rmTrack = nd;
                                break;
                            }
                        }

                        if (rmTrack != null)
                        {
                            break;
                        }

                    }

                    rootNode.RemoveChild(rmTrack);

                    doc.Save(installationPath + "\\TrackXml\\tracInfo.xml");
                }

                //Now need to update UI
                int lastIndex = Tracks.Count < 10 ? Tracks.Count : 10;
                viewTrackListToWindowFromTracks(0, lastIndex);

                //Setting setup window to default
                intializingItemField();
            }
            catch(Exception err)
            {
                //Log write here
            }
        }

        private void BtnChannels_Click(object sender, RoutedEventArgs e)
        {
            middleContentGrid.Visibility = Visibility.Hidden;
            middleContentGridForChannels.Visibility = Visibility.Visible;
        }

        private void BtnTrackPrevious_Click(object sender, RoutedEventArgs e)
        {
            //Find first track showing
            //First track index must be greater than 9
            //First track can not be empty
            //Previous=firsttrackCurrent-1 to firsttrackCurrent-10
            string firstTrackShowing = lblItem_1.Content.ToString();

            if(firstTrackShowing!="")
            {
                //getting know the index of last track on track list
                int indexOfFirstTrack = Tracks.FindIndex(x => x.Name == firstTrackShowing);
                int startingIndexPrev;
                int endIndexPrev;

                if(indexOfFirstTrack>9)
                {
                    startingIndexPrev = indexOfFirstTrack - 10;
                    endIndexPrev = indexOfFirstTrack - 1;
                    viewTrackListToWindowFromTracks(startingIndexPrev, endIndexPrev);
                }
                else
                {
                    System.Windows.Forms.MessageBox.Show("No more previous track");
                }
            }


        }

        private void BtnTrackNext_Click(object sender, RoutedEventArgs e)
        {
            //if we have tracks other than what now showning on lblitem_10 then this button will work
            //We have to findout what is on lblItem_10 and findout index of that fro Tracks (list)
            //lblItem10 may be empty. if so then we have disable next button

            string lastTrackShowing = lblItem_10.Content.ToString();
            if(lastTrackShowing!="")
            {
                //getting know the index of last track on track list
                int indexOfLastTrack = Tracks.FindIndex(x=> x.Name==lastTrackShowing);
                int startingIndexNext;
                int endIndexNext;

                //Now check there are more tracks or not
                if (Tracks.Count>indexOfLastTrack+1)
                {
                    startingIndexNext = indexOfLastTrack + 1;
                    endIndexNext = startingIndexNext + 9;
                    viewTrackListToWindowFromTracks(startingIndexNext, endIndexNext);
                }
                else
                {
                    System.Windows.Forms.MessageBox.Show("No more tracks to show");
                }

            }
            else
            {
                System.Windows.Forms.MessageBox.Show("No more tracks to show");
            }
        }
    }
}
