/*         INFINITY CODE         */
/*   https://infinity-code.com   */

using System;
using UnityEngine;
using UnityEngine.UI;

namespace InfinityCode.OnlineMapsDemos
{
    /// <summary>
    /// Example is how to use a combination of data from Google Places API on bubble popup.
    /// </summary>
    [AddComponentMenu("Infinity Code/Online Maps/Demos/UIBubblePopup")]
    public class UIBubblePopup : MonoBehaviour
    {
        /// <summary>
        /// Root canvas
        /// </summary>
        public Canvas canvas;

        /// <summary>
        /// Bubble popup
        /// </summary>
        public GameObject bubble;
        public GameObject user;

        /// <summary>
        /// VenueListView component for displaying venue data
        /// </summary>
        public VenueListView venueListView;
        public Texture2D userTexture;
        /// <summary>
        /// Array of VenueModel data
        /// </summary>
        public VenueModel[] venues;
        [SerializeField] private ButtonView _closeMap;
        /// <summary>
        /// Action to invoke when a venue is selected
        /// </summary>
        public Action<VenueModel> OnVenueSelected;

        /// <summary>
        /// Reference to active marker
        /// </summary>
        private OnlineMapsMarker targetMarker;
        private OnlineMapsMarker userMarker;
        /// <summary>
        /// This method is called by clicking on the map
        /// </summary>
        public void OnMapClick()
        {
            // Remove active marker reference
            targetMarker = null;

            // Hide the popup
            bubble.SetActive(false);
        }

        /// <summary>
        /// This method is called by clicking on the marker
        /// </summary>
        /// <param name="marker">The marker on which clicked</param>
        private void OnMarkerClick(OnlineMapsMarkerBase marker)
        {
            Logger.Log("MARKER", "MARKER");
            // Set active marker reference
            targetMarker = marker as OnlineMapsMarker;

            // Get a result item from instance of the marker
            VenueModel venue = marker["data"] as VenueModel;
            if (venue == null) return;

            // Show the popup
            bubble.SetActive(true);

            // Initialize VenueListView with the venue data
            venueListView.Init(venue);

            // Invoke the OnVenueSelected action with the selected venue
            OnVenueSelected?.Invoke(venue);

        }
        public void CreatePoint(GeoPoint point)
        {
            if (point == null) return;

            if (userMarker != null)
            {
                userMarker.SetPosition(point.Longitude, point.Latitude);
            }
            else
            {
                userMarker = OnlineMapsMarkerManager.CreateItem(point.Longitude, point.Latitude, userTexture);
                userMarker["data"] = null;
                userMarker.OnClick += (marker) =>
                {

                };
            }
        }
        /// <summary>
        /// Start is called on the frame when a script is enabled just before any of the Update methods are called the first time.
        /// </summary>
        private void Start()
        {
            UIContainer.SubscribeToView(_closeMap, (object _) => OnMapClick());
            /* Subscribe to events of the map 

            OnlineMapsControlBase.instance.OnMapClick += OnMapClick;




            if (venues != null)
            {
                foreach (VenueModel venue in venues)
                {
                    OnlineMapsMarker marker = OnlineMapsMarkerManager.CreateItem(venue.Location.Longitude, venue.Location.Latitude);
                    marker["data"] = venue;
                    marker.OnClick += OnMarkerClick;
                }
            }

            // Initially hide popup
            bubble.SetActive(false);*/
            UpdateMarkers(true);
        }

    public void UpdateMarkers(bool preserveUserMarker = true)
        {
            OnlineMapsMarker savedUserMarker = preserveUserMarker ? userMarker : null;

            OnlineMapsMarkerManager.RemoveAllItems();

            if (savedUserMarker != null)
            {
                OnlineMapsMarkerManager.AddItem(savedUserMarker);
            }

            if (venues != null)
            {
                foreach (VenueModel venue in venues)
                {
                    if (venue?.Location != null)
                    {
                        OnlineMapsMarker marker = OnlineMapsMarkerManager.CreateItem(venue.Location.Longitude, venue.Location.Latitude);
                        marker["data"] = venue;
                        marker.OnClick += OnMarkerClick;
                    }
                }
            }

            bubble.SetActive(false);
        }
    }
}