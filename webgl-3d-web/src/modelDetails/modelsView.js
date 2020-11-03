import React, { Component } from "react";
import { inject, observer } from "mobx-react";
import ModelsListElement from "./modelsListElement";
import Materialize from "materialize-css";
import Loader from '../Components/Loader';



class ModelsView extends Component {

  componentDidMount() {
    var context = this;

    const today = new Date()
    const tomorrow = new Date(today)
    tomorrow.setDate(tomorrow.getDate() + 1)

    var dateElems = document.querySelectorAll(".datepicker");
    Materialize.Datepicker.init(dateElems, {
      format: "yyyy-mm-dd",
      defaultDate: tomorrow,
      setDefaultDate: true,
      container: "body",
      autoClose: true,
      selectMonths: true,
      selectYears: 200,
      onSelect: function(date) {
        context.props.store.updateSearchDate(date);
      },
    });


    var selectElems = document.querySelectorAll('select');
    Materialize.FormSelect.init(selectElems);
  }

  updateInputValue = (evt) =>{
    this.props.store.updateSearchText(evt.target.value);
  }

  modelListView = () => {
    if (this.props.store.assets !== []){
      return (
        <div className="assets-list-view">
          {
            this.props.store.assets.map((model,i) => (
              <ModelsListElement model={model} keyValue={i} key={`${model.Equipement}-${i}`}/>
            ))
          }
        </div>
      );
    }
  };



  loadingState = () => {
    return <Loader />
  }

  updateStatus = (evt) => {
    this.props.store.updateSearchStatus(evt.target[evt.target.value].text.substring(8));
  }

  searchAsset = () => {
    if (this.props.store.mapViewPort.zoom < 12) {
      Materialize.toast({
        html: "Please zoom in more to load assets",
        displayLength: 2000,
        classes: "rounded",
      });
    }
      this.props.store.getModelList();
      this.props.store.refeshConnectivityLayer();

  }

  withDataState = () => {
    return (
      <div className="modelsView">

        <div className="black menu-header center center-align valign-wrapper ">
          <h4 className="white-text">EPNG AM POC</h4>
        </div>

        <div className="white">

          <div  className="row padding-5px search-container">
            <div className="card valign-wrapper search-box">
              <div className="input-field col l9 s9 m9">
                <input className="black-text" id="model_filter" placeholder="Search Asset" type="text" onChange={this.updateInputValue}/>
              </div>
              <div className="col l3 s3 m3">
                <button 
                  style={{border: "none", backgroundColor: "transparent" }}
                  onClick={this.searchAsset}>
                  <i className="material-icons black-text">search</i>
                </button>
              </div>
            </div>
          </div>


          <div className="left-align search-filters-list">

            <div className="input-field chip light-blue darken-3 white-text filter-chip">
              <div className="filter-chip-content ">
                <select id="status_selector" onChange={this.updateStatus}>
                  <option defaultValue value="0">Status: All</option>
                  <option value="1">Status: New</option>
                  <option value="2">Status: Approved</option>
                </select>
              </div>
            </div>

            <div className="chip input-field light-blue darken-3 white-text filter-chip">
              <div className="filter-chip-content">
                <input id="date" type="text" className="datepicker white-text"/>
              </div>
            </div>

          </div>
        </div>


        {this.modelListView()}
      </div>
    );
  };

  render() {
    const { assetsLoadingState } = this.props.store;
    if (assetsLoadingState === false) {
      return this.withDataState(this.props.store);
    } else {
      return this.loadingState();
    }
  }
}

export default inject("store")(observer(ModelsView));
