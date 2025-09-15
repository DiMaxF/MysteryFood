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

        /// <summary>
        /// VenueListView component for displaying venue data
        /// </summary>
        public VenueListView venueListView;

        /// <summary>
        /// Array of VenueModel data
        /// </summary>
        public VenueModel[] venues;

        /// <summary>
        /// Action to invoke when a venue is selected
        /// </summary>
        public Action<VenueModel> OnVenueSelected;

        /// <summary>
        /// Reference to active marker
        /// </summary>
        private OnlineMapsMarker targetMarker;

        /// <summary>
        /// This method is called by clicking on the map
        /// </summary>
        private void OnMapClick()
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

            // Update popup position
            UpdateBubblePosition();
        }

        /// <summary>
        /// Start is called on the frame when a script is enabled just before any of the Update methods are called the first time.
        /// </summary>
        private void Start()
        {
            // Subscribe to events of the map 
            OnlineMaps.instance.OnChangePosition += UpdateBubblePosition;
            OnlineMaps.instance.OnChangeZoom += UpdateBubblePosition;
            OnlineMapsControlBase.instance.OnMapClick += OnMapClick;

            if (OnlineMapsControlBaseDynamicMesh.instance != null)
            {
                OnlineMapsControlBaseDynamicMesh.instance.OnMeshUpdated += UpdateBubblePosition;
            }

            if (OnlineMapsCameraOrbit.instance != null)
            {
                OnlineMapsCameraOrbit.instance.OnCameraControl += UpdateBubblePosition;
            }

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
            bubble.SetActive(false);
        }

        /// <summary>
        /// Updates the popup position
        /// </summary>
        private void UpdateBubblePosition()
        {
            /*if (targetMarker == null) return;

            // Hide the popup if the marker is outside the map view
            if (!targetMarker.inMapView)
            {
                if (bubble.activeSelf) bubble.SetActive(false);
            }
            else if (!bubble.activeSelf)
            {
                bubble.SetActive(true);
            }

            // Convert the coordinates of the marker to the screen position
            Vector2 screenPosition = OnlineMapsControlBase.instance.GetScreenPosition(targetMarker.position);

            // Add marker height
            screenPosition.y += targetMarker.height;

            // Get a local position inside the canvas
            Vector2 point;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, screenPosition, null, out point);

            // Set local position of the popup
            (bubble.transform as RectTransform).localPosition = point;*/
        }
    }
}