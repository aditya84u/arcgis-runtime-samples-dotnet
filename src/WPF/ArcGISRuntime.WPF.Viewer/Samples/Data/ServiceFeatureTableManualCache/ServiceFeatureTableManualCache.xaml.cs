// Copyright 2016 Esri.
//
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at: http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an 
// "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the License for the specific 
// language governing permissions and limitations under the License.

using Esri.ArcGISRuntime;
using Esri.ArcGISRuntime.Data;
using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Mapping;
using System;
using System.Windows;

namespace ArcGISRuntime.WPF.Samples.ServiceFeatureTableManualCache
{
    [ArcGISRuntime.Samples.Shared.Attributes.Sample(
        name: "Service feature table (manual cache)",
        category: "Data",
        description: "Display a feature layer from a service using the **manual cache** feature request mode.",
        instructions: "Run the sample and pan and zoom around the map. Observe the features loaded from the table.",
        tags: new[] { "cache", "feature request mode", "performance" })]
    public partial class ServiceFeatureTableManualCache
    {

        private ServiceFeatureTable _incidentsFeatureTable;

        public ServiceFeatureTableManualCache()
        {
            InitializeComponent();

            // Create the UI, setup the control references and execute initialization 
            Initialize();
        }

        private void Initialize()
        {
            // Create new Map with basemap
            Map myMap = new Map(BasemapStyle.ArcGISTopographic);

            // Create and set initial map location
            MapPoint initialLocation = new MapPoint(
                -13630484, 4545415, SpatialReferences.WebMercator);
            myMap.InitialViewpoint = new Viewpoint(initialLocation, 500000);

            // Create uri to the used feature service
            Uri serviceUri = new Uri(
               "https://services2.arcgis.com/ZQgQTuoyBrtmoGdP/arcgis/rest/services/SF_311_Incidents/FeatureServer/0");

            // Create feature table for the incident feature service
            _incidentsFeatureTable = new ServiceFeatureTable(serviceUri)
            {

                // Define the request mode
                FeatureRequestMode = FeatureRequestMode.ManualCache
            };

            // When feature table is loaded, populate data
            _incidentsFeatureTable.LoadStatusChanged += OnLoadedPopulateData;

            // Create FeatureLayer that uses the created table
            FeatureLayer incidentsFeatureLayer = new FeatureLayer(_incidentsFeatureTable);

            // Add created layer to the map
            myMap.OperationalLayers.Add(incidentsFeatureLayer);

            // Assign the map to the MapView
            MyMapView.Map = myMap;
        }

        private async void OnLoadedPopulateData(object sender, LoadStatusEventArgs e)
        {
            // If layer isn't loaded, do nothing
            if (e.Status != LoadStatus.Loaded)
                return;

            // Create new query object that contains parameters to query specific request types
            QueryParameters queryParameters = new QueryParameters()
            {
                WhereClause = "req_Type = 'Tree Maintenance or Damage'"
            };

            // Create list of the fields that are returned from the service
            string[] outputFields = { "*" };

            try
            {
                // Populate feature table with the data based on query
                await _incidentsFeatureTable.PopulateFromServiceAsync(queryParameters, true, outputFields);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error");
            }
        }

    }
}
