using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Navigation;
using WpfGraphHopper.Api;

namespace WpfGraphHopper
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private DateTime m_timerStart = DateTime.Now;
        List<Tuple<string, string>> m_latLngs = new List<Tuple<string, string>>();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void StartTimer()
        {
            m_timerStart = DateTime.Now;
        }

        private string StopTimerAndGetTime()
        {
            string elapsedTime = "";
            TimeSpan ts = DateTime.Now - m_timerStart;
            elapsedTime = ts.TotalSeconds + " s";
            return elapsedTime;
        }

        private void HyperlinkRequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
        }

        private void ShowAtMap(List<Tuple<string, string>> latLngs)
        {
            string page = @"
<html>
<body>
  <div id=""mapdiv""></div>
  <script src=""http://www.openlayers.org/api/OpenLayers.js"" ></script>
 
   <script>
     var zoom = 0;
     var lonLat;
     var options = {
        controls: [
        new OpenLayers.Control.Navigation(),
        new OpenLayers.Control.PanZoomBar(),
        new OpenLayers.Control.Attribution()
        ]
     };

     var map = new OpenLayers.Map(""mapdiv"", options);
     map.addLayer(new OpenLayers.Layer.OSM());

            var markers = new OpenLayers.Layer.Markers(""Markers"");
            var bounds = new OpenLayers.Bounds();
            var epsg4326 = new OpenLayers.Projection(""EPSG:4326""); //WGS 1984 projection
            var projectTo = map.getProjectionObject(); //The map projection (Spherical Mercator)
            ";

            foreach (Tuple<string, string> latLng in latLngs)
            {
                page += @"
lonLat = new OpenLayers.LonLat("+ latLng.Item2 + ", " + latLng.Item1 + ").transform(epsg4326, projectTo);";

                page += @"
markers.addMarker(new OpenLayers.Marker(lonLat));
bounds.extend(lonLat);
";
            }

            page += @"
     map.addLayer(markers);     
     map.zoomToExtent(bounds);
  </script>
</body>
</html>
";
            Browser.NavigateToString(page);
        }

        private void ShowMatrix(List<List<Tuple<double, double>>> distTimes)
        {
            string page = @"
<html>
<body>
  <table>";
            foreach (List<Tuple<double, double>> row in distTimes)
            {
                page += @"<tr>";
                foreach (Tuple<double, double> distTime in row)
                {
                    page += @"<td>" + distTime.Item1 + " m / " + distTime.Item2 + " s</td>";
                }

                page += @"</tr>";
            }

            page += @"
      </table>
</body>
</html>
";
            Browser.NavigateToString(page);
        }

        private void ButtonGeocode_Click(object sender, RoutedEventArgs e)
        {
            GraphHopper graphHopper = new GraphHopper(TextBoxKey.Text);
            string text = "";

            m_latLngs.Clear();
            Mouse.OverrideCursor = Cursors.Wait;

            LabelAddress1.Content = "";
            LabelAddress1.Tag = null;
            LabelAddress2.Content = "";
            LabelAddress2.Tag = null;
            LabelAddress3.Content = "";
            LabelAddress3.Tag = null;
            LabelAddress4.Content = "";
            LabelAddress4.Tag = null;
            TextboxResponse.Text = "";

            if (!string.IsNullOrEmpty(TextBoxAddress1.Text))
            {
                StartTimer();
                HitsResponse hits = graphHopper.Geolocate(TextBoxAddress1.Text);
                text = StopTimerAndGetTime();
                text += " - " + hits.Hits.Count() + " hits";
                LabelAddress1.Content = text;

                if (hits.Hits.Count() > 0)
                {
                    // use the first
                    Tuple<string, string> t = new Tuple<string, string>(hits.Hits[0].Point.Lat, hits.Hits[0].Point.Lng);
                    LabelAddress1.Tag = t;
                    m_latLngs.Add(t);
                }

                TextboxResponse.Text += graphHopper.LastResponse;
            }

            if (!string.IsNullOrEmpty(TextBoxAddress2.Text))
            {
                StartTimer();
                HitsResponse hits = graphHopper.Geolocate(TextBoxAddress2.Text);
                text = StopTimerAndGetTime();
                text += " - " + hits.Hits.Count() + " hits";
                LabelAddress2.Content = text;

                if (hits.Hits.Count() > 0)
                {
                    // use the first
                    Tuple<string, string> t = new Tuple<string, string>(hits.Hits[0].Point.Lat, hits.Hits[0].Point.Lng);
                    LabelAddress2.Tag = t;
                    m_latLngs.Add(t);
                }

                TextboxResponse.Text += graphHopper.LastResponse;
            }

            if (!string.IsNullOrEmpty(TextBoxAddress3.Text))
            {
                StartTimer();
                HitsResponse hits = graphHopper.Geolocate(TextBoxAddress3.Text);
                text = StopTimerAndGetTime();
                text += " - " + hits.Hits.Count() + " hits";
                LabelAddress3.Content = text;

                if (hits.Hits.Count() > 0)
                {
                    // use the first
                    Tuple<string, string> t = new Tuple<string, string>(hits.Hits[0].Point.Lat, hits.Hits[0].Point.Lng);
                    LabelAddress3.Tag = t;
                    m_latLngs.Add(t);
                }

                TextboxResponse.Text += graphHopper.LastResponse;
            }

            if (!string.IsNullOrEmpty(TextBoxAddress4.Text))
            {
                StartTimer();
                HitsResponse hits = graphHopper.Geolocate(TextBoxAddress4.Text);
                text = StopTimerAndGetTime();
                text += " - " + hits.Hits.Count() + " hits";
                LabelAddress4.Content = text;

                if (hits.Hits.Count() > 0)
                {
                    // use the first
                    Tuple<string, string> t = new Tuple<string, string>(hits.Hits[0].Point.Lat, hits.Hits[0].Point.Lng);
                    LabelAddress4.Tag = t;
                    m_latLngs.Add(t);
                }

                TextboxResponse.Text += graphHopper.LastResponse;
            }

            if(m_latLngs.Count > 0)
            {
                ShowAtMap(m_latLngs);
            }
            Mouse.OverrideCursor = null;


            if (m_latLngs.Count > 1)
            {
                ButtonMatrix.IsEnabled = true;
                ButtonRoute.IsEnabled = true;
            }
            else
            {
                ButtonMatrix.IsEnabled = false;
                ButtonRoute.IsEnabled = false;
            }
        }

        private void ButtonMatrix_Click(object sender, RoutedEventArgs e)
        {
            GraphHopper graphHopper = new GraphHopper(TextBoxKey.Text);

            Mouse.OverrideCursor = Cursors.Wait;
            List<List<Tuple<double, double>>> distTimes = graphHopper.GetDistanceTimeMatrix(m_latLngs);
            Mouse.OverrideCursor = null;

            TextboxResponse.Text = graphHopper.LastResponse;
            ShowMatrix(distTimes);
        }

        private void ButtonRoute_Click(object sender, RoutedEventArgs e)
        {
            string sOrder = "";
            GraphHopper graphHopper = new GraphHopper(TextBoxKey.Text);

            Mouse.OverrideCursor = Cursors.Wait;
            graphHopper.TimeoutSec = 10;
            List<int> optimizedOrder = graphHopper.OptimizeRoute(m_latLngs, m_latLngs[0]);
            Mouse.OverrideCursor = null;

            TextboxResponse.Text = graphHopper.LastResponse;

            foreach (int id in optimizedOrder)
            {
                sOrder += id + " ";
            }

            LabelRoute.Content = sOrder;
        }
    }
}
