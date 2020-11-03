import { observable, action, runInAction } from "mobx";

import { apiService } from "./api_service.js";
import { FlyToInterpolator, LinearInterpolator } from "react-map-gl";
import MAP_STYLE from "../map-style-basic-v8.json";
import WebMercatorViewport from "viewport-mercator-project";

export default class DataStore {
  @observable modelViewerState = true;
  
  @observable useStaticAssets = true;

  //Set Boolen to true when changing to API calls
  @observable useDynamicAssets = false;

  @observable errorState = false;

  @observable assets: any = [];
  @observable assetsLoadingState = true;

  @observable dataPanelContent = "Please select a model to view data";

  @observable mapStyle = MAP_STYLE;
  @observable connectivityData = null;

  @observable subNetConectivity: any = [];
  @observable stagedConnectivityData: any = [];
  @observable electricDevicesLayer: any = [];
  @observable dateFilter =
    new Date().getFullYear() +
    "-" +
    (new Date().getMonth() + 1) +
    "-" +
    (new Date().getDate() + 1);

  @observable statusFilter = "All";
  @observable textFilter = "";
  @observable viewportCoordinates = [
    [-88.17231208338957,41.78164438640926],
    [-88.17231208338957,41.784532458674306],
    [-88.16407233729582,41.784532458674306],
    [-88.16407233729582,41.78164438640926]
  ];

  @observable mapViewPort = {
    width: "fit",
    height: "100vh",
    latitude: 0,
    longitude: 0,
    zoom: 2,
    pitch: 0,
    bearing: 0,
  };

  @action initiateMap(){
    const newViewport = {
      width: "fit",
      height: "100vh",
      latitude: 41.7833,
      longitude: -88.1678,
      zoom: 17.8,
      pitch: 0,
      bearing: 0,
    };
    this.mapViewPort = newViewport;
    this.getModelList();
    this.getConnectivityLayer(this.viewportCoordinates); // DO NOT REMOVE
  }

  @action refreshFilters(){
    this.dataPanelContent = "Please select a model to view data";
    this.statusFilter = "All";
    this.dateFilter = 
    new Date().getFullYear() +
    "-" +
    (new Date().getMonth() + 1) +
    "-" +
    (new Date().getDate() + 1);
    this.textFilter = "";
  }

  @action toggleStreetViewMode() {
    if (this.mapViewPort.pitch === 0) {
      const newViewport = {
        width: "fit",
        height: "100vh",
        longitude: this.mapViewPort.longitude,
        latitude: this.mapViewPort.latitude,
        zoom: this.mapViewPort.zoom,
        transitionInterpolator: new LinearInterpolator(["pitch"]),
        transitionDuration: 1000,
        pitch: 60,
        bearing: this.mapViewPort.bearing,
      };
      this.mapViewPort = newViewport;
    } else {
      const newViewport = {
        width: "fit",
        height: "100vh",
        longitude: this.mapViewPort.longitude,
        latitude: this.mapViewPort.latitude,
        zoom: this.mapViewPort.zoom,
        transitionInterpolator: new LinearInterpolator(["pitch"]),
        transitionDuration: 1000,
        pitch: 0,
        bearing: this.mapViewPort.bearing,
      };
      this.mapViewPort = newViewport;
    }
  }

  @action setDataPanelModel(model: any) {
    this.dataPanelContent = model;
  }

  @action updateSearchDate(newDate: any) {
    this.dateFilter =
      newDate.getFullYear() +
      "-" +
      (newDate.getMonth() + 1) +
      "-" +
      newDate.getDate();
  }

  @action updateSearchStatus(newStatus: any) {
    this.statusFilter = newStatus;
  }

  @action updateSearchText(newText: any) {
    this.textFilter = newText;
  }

  //Change Map view port
  @action updateMapViewport(newViewport: any) {
    this.mapViewPort = newViewport;
    var viewport = new WebMercatorViewport(newViewport);
    const vwportState = [] as any;
    vwportState.push(
      viewport.unproject([0, viewport.height]),
      viewport.unproject([0, 0]),
      viewport.unproject([viewport.width, 0]),
      viewport.unproject([viewport.width, viewport.height])
    );
    this.viewportCoordinates = vwportState;
  }

  //Get list of models
  @action async getModelList() {
    let newAssets: any[] = [];
    this.assets = [];
    this.assetsLoadingState = true;

    try {
      if (this.useDynamicAssets) {
        const dynamicAssets = await apiService.getAssetsListAsync(
          this.viewportCoordinates,
          this.statusFilter,
          this.textFilter,
          this.dateFilter
        );
        if (dynamicAssets != null && dynamicAssets != []){
          newAssets = newAssets.concat(dynamicAssets);
        }
      }

      if (this.useStaticAssets) {
        const staticAssets = await apiService.getAssetsListAsyncStatic();
        newAssets = newAssets.concat(staticAssets);
      }

      this.assets = newAssets.filter(function (el : any) {
        return 'geometry' in el 
      });

      this.assetsLoadingState = false;
    } catch {
      this.assetsLoadingState = false;
      this.errorState = true;
    }
    this.refreshFilters();
  }

  @action async refeshConnectivityLayer() {
    this.getConnectivityLayer(this.viewportCoordinates);
  }

  //Get list of connectivity layers
  @action async getConnectivityLayer(viewData: Array<Array<number>>) {
    const response: Object = await apiService.getConnectivtyDataAsync(viewData);
    const stagedData: Object = await apiService.getStagedConnectivtyDataAsync();
    runInAction(() => {
      this.formatConnectivityData(response);
      this.formatStagedConnectivityData(stagedData);
    });
  }

  @action async formatConnectivityData(data: Object) {
    const subnetLines: any = [];
    const electricDevices: any = [];
    // eslint-disable-next-line
    Object.entries(data).map((x: any) => {
      switch (x[0]) {
        case "100": //electric devices
        // eslint-disable-next-line
          x[1].features.map((eleDevices: any) => {
            const lay = [eleDevices.geometry["x"], eleDevices.geometry["y"]];
            electricDevices.push([lay]);
          });
          this.electricDevicesLayer = electricDevices || null;
          break;
        case "105": //electric assemblies
          break;
        case "110": //electric junction
          break;
        case "115": //electric lines
          break;
        case "903": // electric sublines
        case "120":
          // eslint-disable-next-line
          x[1].features.map((subnet: any) => {
            // eslint-disable-next-line
            subnet.geometry.paths.map((individualPath: any) => {
              const lay = individualPath.reduce((pV: any, cV: any) => {
                return [...pV, [cV[0], cV[1], 8.6]];
              }, []);
              subnetLines.push([lay]);
            });
          });
          this.subNetConectivity = subnetLines || null;
          break;
        case "900": // structure junction
          break;
        case "905": // structure line
          break;
        case "910": // structure boundary
          break;
      }
    });
  }

  @action async formatStagedConnectivityData(data: Object) {
    const subnetStagedLines: any = [];
    // eslint-disable-next-line
    Object.entries(data).map((x: any) => {
      switch (x[0]) {
        case "0": // points
          break;
        case "1": //lines
        // eslint-disable-next-line
          x[1].features.map((subnet: any) => {
            // eslint-disable-next-line
            subnet.geometry.paths.map((individualPath: any) => {
              const lay = individualPath.reduce((pV: any, cV: any) => {
                return [...pV, [cV[0], cV[1], 8.6]];
              }, []);
              subnetStagedLines.push([lay]);
            });
          });
          this.stagedConnectivityData = subnetStagedLines || null;
          break;
        case "2": //electric junction
          break;
      }
    });
  }

  //Model selected in menu
  //Updates viewport and renders models near selected model
  @action async setSelectedModel(model: any) {
    const newViewport = {
      width: "fit",
      height: "100vh",
      longitude: model.geometry.X,
      latitude: model.geometry.Y,
      zoom: 18,
      transitionInterpolator: new FlyToInterpolator({ speed: 3 }),
      transitionDuration: "auto",
      pitch: 0,
      bearing: 0,
    };
    this.mapViewPort = newViewport;
    this.setDataPanelModel(model);
  }

  @action async setMapStyle(map: any) {
    this.mapStyle = map;
  }

  @action async setConnectivityModel(data: any) {
    /**
     * we need to convert the double linked list from the API response to
     * a slightly more complext object. This data format takes a line first apporach.
     * Null nodes can be discarded and from <-> to relationships must be specifically
     * defined
     *
     * Data format:
     * [
     *   {
     *     from: {
     *       coordinates: [-122.269029, 37.80787]
     *     },
     *     to: {
     *       coordinates: [-122.271604, 37.803664]
     *   },
     * ]
     *
     */
    // eslint-disable-next-line
    let finalArray: any = [];
    const filteredData = data.filter(
      (x: any) =>
        x.from_id != null && x.to_id != null && x.is_connected === true
    );
    // eslint-disable-next-line
    filteredData.map((obj: any) => {
      const derivedObjectFrom = {
        from: {
          coordinates: [
            data.find((element: any) => element.id === obj.from_id).longitude,
            data.find((element: any) => element.id === obj.from_id).latitude,
          ],
        },
        to: {
          coordinates: [
            data.find((element: any) => element.id === obj.id).longitude,
            data.find((element: any) => element.id === obj.id).latitude,
          ],
        },
      };
      const derivedObjectTo = {
        from: {
          coordinates: [
            data.find((element: any) => element.id === obj.id).longitude,
            data.find((element: any) => element.id === obj.id).latitude,
          ],
        },
        to: {
          coordinates: [
            data.find((element: any) => element.id === obj.to_id).longitude,
            data.find((element: any) => element.id === obj.to_id).latitude,
          ],
        },
      };
      finalArray.push(derivedObjectTo);
      finalArray.push(derivedObjectFrom);
    });
    this.connectivityData = finalArray;
  }

  //   @action async fetchCoordinatesConnectivityMock() {
  //     /**
  //      * Data format: pathLayer
  //      * [
  //      *   {
  //      *     path: [[-122.4, 37.7], [-122.5, 37.8], [-122.6, 37.85]],
  //      *     name: 'Richmond - Millbrae',
  //      *     color: [255, 0, 0]
  //      *   },
  //      *   ...
  //      * ]
  //      */
  //     const connectivityLayers: any = [];
  //     mockConnectivityDataSubNet6.map((convertedLayers: any) => {
  //       const lay = convertedLayers.geometries.reduce((pV: any, cV: any) => {
  //         const x = [cV.x, cV.y];
  //         return [...pV, x];
  //       }, []);
  //       connectivityLayers.push([lay]);
  //     });
  //     mockConnectivityDataSubNet7.map((convertedLayers: any) => {
  //       const lay = convertedLayers.geometries.reduce((pV: any, cV: any) => {
  //         const x = [cV.x, cV.y];
  //         return [...pV, x];
  //       }, []);
  //       connectivityLayers.push([lay]);
  //     });
  //     this.subNetConectivity = connectivityLayers || null;
  // }
}
