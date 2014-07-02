﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Windows.Devices.Geolocation;

namespace WPMobileNet.Service
{
    public class LocationService : BaseService
    {
        #region Strings
        private string SKLocationPermission = "LocationPermission";
        public string AskLocationPermissionTitle = "Location";
        public string AskLocationPermissionMessage = "This application needs access to your location. Are you agree?";
        #endregion

        #region Services
        private readonly MessageBoxService _messageBoxService;
        private readonly StateService _stateService;
        #endregion

        #region Properties
        private Geolocator _geolocator;
        private Geolocator Geolocator
        {
            get
            {
                if (HasPermission == true)
                {
                    if (_geolocator == null)
                    {
                        _geolocator = new Geolocator();
                        //_geolocator.DesiredAccuracyInMeters = DesiredAccuracyInMeters;
                        //_geolocator.MovementThreshold = GeoLocatorMovementThreshold;
                        //_geolocator.PositionChanged += _geolocator_PositionChanged;
                    }
                }
                else throw new UnauthorizedAccessException();
                return _geolocator;
            }
        }
        private Geoposition _currentLocation;
        public Geoposition CurrentLocation
        {
            get { return _currentLocation; }
            set { _currentLocation = value; }
        }
        private bool? _hasPermission;
        private bool? HasPermission
        {
            get
            {
                if (_hasPermission == null) _hasPermission = _stateService.GetState<bool?>(SKLocationPermission, true);
                if (_hasPermission == null)
                {
                    var result = _messageBoxService.Show(AskLocationPermissionTitle, AskLocationPermissionMessage, MessageBoxService.MessageButtonType.YesNo);
                    if (result.Equals(MessageBoxResult.OK)) _hasPermission = true;
                    else _hasPermission = false;
                    _stateService.SetState(SKLocationPermission, _hasPermission, true);
                }
                return _hasPermission;
            }
        }
        #endregion

        #region Constructors
        public LocationService(MessageBoxService messageBoxService, StateService stateService)
        {
            this._messageBoxService = messageBoxService;
            this._stateService = stateService;
        }
        #endregion

        #region Methods
        internal async Task<Geoposition> GetCurrentLocation()
        {
            if (Geolocator.LocationStatus == PositionStatus.Disabled) throw new UnauthorizedAccessException();
            this.CurrentLocation = await Geolocator.GetGeopositionAsync(TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(5));
            return this.CurrentLocation;
        }
        #endregion
    }
}