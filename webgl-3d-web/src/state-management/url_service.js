export default class URLService {
  constructor() {
    switch (process.env.NODE_ENV ?? "production") {
      case "production":
        this._3dViewerUrl = "";
        this._baseURL = "";
        this._gisAssetsUrl = "";
        this._triggerURL = "";
        this._baseURLConnectivty = "";
        this._modelFilesURL = "";
        break;
      case "development":
        this._3dViewerUrl = "";
        this._baseURL = "";
        this._gisAssetsUrl = "";
        this._triggerURL = "";
        this._baseURLConnectivty = "";
        this._modelFilesURL = "";
        break;
      default:
        this._3dViewerUrl = "";
        this._baseURL = "";
        this._gisAssetsUrl = "";
        this._triggerURL = "";
        this._baseURLConnectivty = "";
        this._modelFilesURL = "";
        break;
    }
  }

  // this._baseURL = "http://localhost:5000";
  // this._baseURLAssets = "http://localhost:5000/assets";
  // this._baseURLConnectivty =
  //   "http://localhost:5001/3D_GET_ASSET_IN_VIEWPORT.json";

  get viewer3dURL(){
    return this._3dViewerUrl;
  }

  get baseURL() {
    return this._baseURL;
  }

  get assetsGISURL() {
    return this._gisAssetsUrl;
  }

  get baseURLConnectivty() {
    return this._baseURLConnectivty;
  }

  get triggerURL() {
    return this._triggerURL;
  }

  get modelFileURL(){
    return this._modelFilesURL;
  }
}
